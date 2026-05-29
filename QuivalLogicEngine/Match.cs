using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.States;
using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

public class Match
{
    private List<Player> Players { get; set; }
    private List<ICardIntent> CardIntents { get; set; }
    public List<ICardIntent> SuccessfulIntents { get; set; }
    private BoardState BoardState { get; set; }
    private List<EventMessage> EventMessages { get; set; }
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

    //NOTE: We start at 1 so we can keep '0' as an unassigned card id
    private int CardIdTotal = 1;

    private List<Card> MatchCards;
    private int MaxRounds = 5;

    private bool OnePlayerMode = false;

    public Match()
    {
        Players = new();
        CardIntents = new();
        SuccessfulIntents = new();
        BoardState = new();
        TurnCount = 1;
        RoundCount = 1;
        MatchCards = new();
        EventMessages = new();

        //TEMP for testing
        CardParser.ParseSomeCards("C:\\Users\\lavelle.t\\Projects\\Personal\\Quival\\cards.json");
    }

    public bool PlayerHasSetCard(int playerId)
    {
        return Players[playerId].CardToPlay != null;
    }

    public ClientGameState GetGameState(int playerId)
    {
        PlayerState ps = new()
        {
            Id = Players[playerId].Id,
            HealthPoints = Players[playerId].HealthPoints,
            ManaPoints = Players[playerId].Mana,
            Hand = Players[playerId].Hand,
            Deck = Players[playerId].Deck,
            CardToPlay = Players[playerId].CardToPlay,
            BlockingCreature = Players[playerId].BlockingCreature
        };

        ClientGameState state = new()
        {
            PlayerState = ps,
            BoardState = BoardState,
            CardIntents = CardIntents,
            TurnCount= TurnCount,
            RoundCount= RoundCount,
            GameEvents = EventMessages
        };

        Player opponent = GetOpponent(playerId);
        state.OpponentId = opponent.Id;

        if (OnePlayerMode)
        {
            state.OpponentCardCount = 0;
            state.OpponentHealthPoints = 20;
        }
        else
        {
            state.OpponentCardCount = opponent.Hand.Count();
            state.OpponentHealthPoints = opponent.HealthPoints;
            state.OpponentManaPoints = opponent.Mana;
            state.OpponentBlockCard = opponent.BlockingCreature;
        }

        return state;
    }

    private Player GetOpponent(int playerId) 
    {
        if (OnePlayerMode)
            return Players[playerId];

        int opponentId = playerId == 0 ? 1 : 0;
        return Players[opponentId];
    }

    public void SetPlayer(int id, List<Card> deck)
    {
        SetCardIds(deck);
        MatchCards.AddRange(deck);

        Player player = new Player(id, deck)
        {
            Mana = 1
        };

        Players.Add(player);
    }

    public void SetCardToPlay(int playerId, int cardId)
    {
        var card = GetCardFromId(cardId);
        if (card != null)
        {
            if (card.Cost <= Players[playerId].Mana)
            {
                Players[playerId].CardToPlay = card;
                Players[playerId].Hand.Remove(card);
            }
            else
            {
                //TODO: send some kind of message back to client about not having enough mana?
            }
        }
    }

    public void SetCardToAttack(int playerId, int cardId)
    {
        //TODO: maybe check this ID exists on the board first
        var card = GetCardFromId(cardId);
        if (card != null)
        {
            Players[playerId].CardToPlay = new AttackCard(playerId, cardId);
        }
    }

    public void SetCardToBlock(int playerId, int cardId)
    {
        var card = GetCardFromId(cardId);
        if (card != null)
        {
            Players[playerId].CardToPlay = new BlockCard(playerId, cardId);
        }
    }

    public void SetBlankCard(int playerId)
    {
        Players[playerId].CardToPlay = new BlankCard(playerId);
    }

    public bool BothCardsToPlayAreSet()
    {
        if (OnePlayerMode)
            return true;

        foreach (var player in Players)
        {
            if (player.CardToPlay == null)
                return false;
        }

        return true;
    }

    public List<Card> GetPlayerHand(int id)
    {
        return Players[id].Hand;
    }

    public void SetCardIds(List<Card> deck)
    {
        foreach (var card in deck)
        {
            card.Id = CardIdTotal++;
            Console.WriteLine($"[EVENT] Card {card.Name} assigned Id {card.Id}");
        }
    }

    private Card? GetCardFromId(int cardId)
    {
        return MatchCards.SingleOrDefault(c => c.Id == cardId);
    }

    //TODO: this probably doesn't need to return the round
    public int ProcessCards()
    {
        EventMessages.Clear();
        CardIntents.Clear();

        //TODO: have this calculated in a loop
        if (Players[0].CardToPlay is BlankCard &&
            Players[1].CardToPlay is BlankCard)
        {
            EventMessage(new BothPlayersOutOfMovesEvent());
            Players[0].CardToPlay = null;
            Players[1].CardToPlay = null;
            NextTurn();
            return 1;
        }

        EventMessage(new NewRound(RoundCount));

        foreach (var player in Players)
        {
            if (player.CardToPlay != null)
            {
                player.Mana -= player.CardToPlay.Cost;

                var intents = player.CardToPlay.GetIntents();

                foreach (var intent in intents)
                    intent.PlayerId = player.Id;

                CardIntents.AddRange(intents);

                player.CardToPlay = null;
            }
        }

        List<Summon> Summons = new();
        List<Block> Blocks = new();
        List<Attack> Attacks = new();
        List<DamageMultiply> DamageMultiplies = new();

        Summons.AddRange(CardIntents.OfType<Summon>().ToList());
        Blocks.AddRange(CardIntents.OfType<Block>().ToList());
        Attacks.AddRange(CardIntents.OfType<Attack>().ToList());

        //Summon card
        foreach (var summon in Summons)
        {
            if (BoardState.CreatureSlotFree(summon.PlayerId))
            {
                var card = GetCardFromId(summon.CardId);
                if (card != null && card is CreatureCard creature) 
                {
                    BoardState.SummonCreature(summon.PlayerId, creature);
                    SuccessfulIntents.Add(summon);
                    EventMessage(new SummonEvent(summon.PlayerId, creature.Id, creature.Name!));

                    creature.HasActed = true;
                    creature.CurrentHealth = creature.Health;
                }
            }
            else
            {
                Console.WriteLine($"[ERROR]: Not enough room on {summon.PlayerId} board for {summon.CardId}");
            }
        }

        //Move to blockzone
        foreach (var block in Blocks)
        {
            var cardToBlock = GetCardFromId(block.CardId);

            if (cardToBlock != null && cardToBlock is CreatureCard cc)
            {
                CreatureCard? oldBlocker = Players[block.PlayerId].BlockingCreature!;
                Players[block.PlayerId].BlockingCreature = cc;

                if (oldBlocker == null)
                {
                    BoardState.SummonedCreatures[block.PlayerId].Remove(cc);
                    EventMessage(new MoveToBlockZoneEvent(block.PlayerId, cc.Id, cc.Name!));
                }
                else
                {
                    int index = BoardState.SummonedCreatures[block.PlayerId].IndexOf(cc);
                    BoardState.SummonedCreatures[block.PlayerId][index] = oldBlocker;
                    EventMessage(new BlockSwapEvent(block.PlayerId, cc.Id, cc.Name!, oldBlocker.Id, oldBlocker.Name!));
                }

                cc.HasActed = true;
            }
        }

        //Attack
        foreach (var attack in Attacks)
        {
            var card = GetCardFromId(attack.CardId);

            if (card != null && card is CreatureCard attackingCreature)
            {
                EventMessage(new AttackEvent(attack.PlayerId, card.Id, card.Name!));

                Player otherPlayer = GetOpponent(attack.PlayerId);

                if (otherPlayer.BlockingCreature != null)
                {
                    var blockingCreature = otherPlayer.BlockingCreature;
                    Console.WriteLine($"{blockingCreature.Name} blocks them");

                    blockingCreature.CurrentHealth -= attackingCreature.Attack;
                    attackingCreature.CurrentHealth -= blockingCreature.Attack;

                    if (blockingCreature.CurrentHealth <= 0)
                    {
                        SuccessfulIntents.Add(new CreatureDeath() { PlayerId = otherPlayer.Id, CardId = blockingCreature!.Id });
                        EventMessage(new CreatureDeathEvent(blockingCreature.Id, blockingCreature.Name!));
                        otherPlayer.BlockingCreature = null;
                    }

                    if (attackingCreature.CurrentHealth <= 0)
                    {
                        SuccessfulIntents.Add(new CreatureDeath() { PlayerId = attack.PlayerId, CardId = attackingCreature.Id });
                        EventMessage(new CreatureDeathEvent(attackingCreature.Id, attackingCreature.Name!));
                        BoardState.SummonedCreatures[attack.PlayerId].Remove(attackingCreature);
                    }
                }
                else
                {
                    otherPlayer.HealthPoints -= attackingCreature.Attack;
                    SuccessfulIntents.Add(new DamagePlayer(otherPlayer.Id, attackingCreature.Attack));
                }

                attackingCreature.HasActed = true;
            }
        }

        RoundCount++;

        //TODO: I think that we should do the calculating which players can and can't move next turn here
        //currently it's all being done client side which is stupid

        if (RoundCount > MaxRounds)
        {
            NextTurn();
        }

        return RoundCount;
    }

    private void NextTurn()
    {
        TurnCount++;
        RoundCount = 1;
        EventMessage(new NewTurn(TurnCount, RoundCount)); //TODO: do we need this?

        foreach (var card in MatchCards)
        {
            if (card is CreatureCard cc)
                cc.HasActed = false;
        }

        foreach (var player in Players)
        {
            player.Mana += BoardState.GetCurrentMana();
            player.DrawCard(1);
        }

        BoardState.IncreaseManaClock();
    }

    private void EventMessage(EventMessage message)
    {
        Console.WriteLine(message.GetString());
        EventMessages.Add(message);
    }
}

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
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

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
            Hand = Players[playerId].Hand,
            Deck = Players[playerId].Deck,
            CardToPlay = Players[playerId].CardToPlay
        };


        ClientGameState state = new()
        {
            PlayerState = ps,
            BoardState = BoardState,
            CardIntents = CardIntents,
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

        Players.Add(new Player(id, deck));
    }

    public void SetCardToPlay(int playerId, int cardId)
    {
        var card = GetCardFromId(cardId);
        if (card != null)
        {
            Players[playerId].CardToPlay = card;
            Players[playerId].Hand.Remove(card);
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

    public int ProcessCards()
    {
        CardIntents.Clear();

        Console.WriteLine($"[EVENT]: Round {RoundCount}");

        foreach (var player in Players)
        {
            if (player.CardToPlay != null)
            {
                var intents = player.CardToPlay.GetIntents();

                foreach (var intent in intents)
                    intent.PlayerId = player.Id;

                CardIntents.AddRange(intents);
            }
        }

        List<Summon> Summons = new();
        List<Block> Blocks = new();
        List<Attack> Attacks = new();
        List<DamageMultiply> DamageMultiplies = new();

        Summons.AddRange(CardIntents.OfType<Summon>().ToList());
        Blocks.AddRange(CardIntents.OfType<Block>().ToList());
        Attacks.AddRange(CardIntents.OfType<Attack>().ToList());

        foreach (var block in Blocks)
        {
            var blockingCard = GetCardFromId(block.CardId);

            if (blockingCard != null && blockingCard is CreatureCard cc)
            {
                //move card from board to block zone
                Players[block.PlayerId].BlockingCreature = cc;
                BoardState.SummonedCreatures[block.PlayerId].Remove(cc);
            }
        }

        foreach (var summon in Summons)
        {
            if (BoardState.CreatureSlotFree(summon.PlayerId))
            {
                var card = GetCardFromId(summon.CardId);
                if (card != null && card is CreatureCard creature) 
                {
                    BoardState.SummonCreature(summon.PlayerId, creature);
                    SuccessfulIntents.Add(summon);
                    Console.WriteLine($"[EVENT]: player {summon.PlayerId} summoned {creature.Id}");
                }
            }
            else
            {
                Console.WriteLine($"[ERROR]: Not enough room on {summon.PlayerId} board for {summon.CardId}");
            }
        }

        foreach (var attack in Attacks)
        {
            var card = GetCardFromId(attack.CardId);

            if (card != null && card is CreatureCard attackingCreature)
            {
                Player otherPlayer = GetOpponent(attack.PlayerId);

                if (otherPlayer.BlockingCreature != null)
                {
                    var blockingCreature = otherPlayer.BlockingCreature; //for easier reading
                    Console.WriteLine($"[EVENT]: ({blockingCreature.Id}){blockingCreature.Name} blocks ({attackingCreature.Id}){attackingCreature.Name}");

                    blockingCreature.Health -= attackingCreature.Attack;
                    attackingCreature.Health -= blockingCreature.Attack;

                    if (blockingCreature.Health <= 0)
                    {
                        blockingCreature = null;
                        SuccessfulIntents.Add(new CreatureDeath() { PlayerId = otherPlayer.Id, CardId = blockingCreature!.Id });
                        Console.WriteLine($"[EVENT]: ({blockingCreature.Id}){blockingCreature.Name} died");
                    }

                    if (attackingCreature.Health <= 0)
                    {
                        BoardState.SummonedCreatures[attack.PlayerId].Remove(attackingCreature);
                        SuccessfulIntents.Add(new CreatureDeath() { PlayerId = attack.PlayerId, CardId = attackingCreature.Id });
                        Console.WriteLine($"[EVENT]: ({attackingCreature.Id}){attackingCreature.Name} died");
                    }
                }
                else
                {
                    otherPlayer.HealthPoints -= attackingCreature.Attack;
                    SuccessfulIntents.Add(new DamagePlayer(otherPlayer.Id, attackingCreature.Attack));
                    Console.WriteLine($"[EVENT]: Player {attack.PlayerId}'s creature {card.Id} attacks player {otherPlayer.Id} for {attackingCreature.Attack}");
                    Console.WriteLine($"[EVENT]: Player {otherPlayer} has {Players[otherPlayer.Id].HealthPoints} health");
                }
            }
        }

        foreach (var player in Players)
            player.CardToPlay = null;

        RoundCount++;

        if (RoundCount > MaxRounds)
        {
            TurnCount++;
            Console.WriteLine($"[EVENT]: Starting Turn {TurnCount}");
            //StartRoundStuff
            //Draw Card
            //ResetBlockers
            //Maybe reset creature health?
        }

        return RoundCount;
    }
}

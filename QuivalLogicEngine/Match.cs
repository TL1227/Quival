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

    private bool OnePlayerMode = true;

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
            OpponentCardCount = GetOpponentCardCount(playerId),
            CardIntents = CardIntents
        };

        return state;
    }

    private int GetOpponentCardCount(int playerId)
    {
        foreach (var player in Players)
        {
            if (player.Id == playerId)
                continue;

            return Players[player.Id].Hand.Count();
        }

        return 0;
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
            card.Id = CardIdTotal++;
    }

    private Card? GetCardFromId(int cardId)
    {
        return MatchCards.SingleOrDefault(c => c.Id == cardId) ?? null;
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

        //handle the intents
        foreach (var summon in Summons)
        {
            if (BoardState.CreatureSlotFree(summon.PlayerId))
            {
                var card = GetCardFromId(summon.CardId);
                if (card != null && card is CreatureCard creature) 
                {
                    BoardState.SummonCreature(summon.PlayerId, creature);
                    SuccessfulIntents.Add(summon);
                    Console.WriteLine($"[EVENT]: player {summon.PlayerId} summoned {creature.Name}");
                }
            }
            else
            {
                Console.WriteLine($"[ERROR]: Not enough room on {summon.PlayerId} board for {summon.CardId}");
            }
        }

        foreach (var attack in Attacks)
        {
            var card = (CreatureCard)GetCardFromId(attack.CardId);
            int otherPlayer = attack.PlayerId == 1 ? 0 : 1;

            Players[otherPlayer].HealthPoints -= card.Attack;
            SuccessfulIntents.Add(new DamagePlayer(otherPlayer, card.Attack));
            Console.WriteLine($"[EVENT]: Player {attack.PlayerId}'s creature {card.Id} attacks player {otherPlayer} for {card.Attack}");
            Console.WriteLine($"[EVENT]: Player {otherPlayer} has {Players[otherPlayer].HealthPoints} health");
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

    private void AssignBlocks(ICardIntent[]? blocks)
    {
        if (blocks == null || blocks.Length == 0) return;

        foreach (var block in blocks)
        {
            if (block is Block b)
            {
                //NOTE: currently this might change depending on rules and if we want to be 
                //able to stack blocking creatures and whatnot
                /*
                if (!BoardState.PlayerHasBlockingCreatures(b.PlayerId))
                {
                    BoardState.SetBlocker(b.PlayerId, b.CardId);
                }
                */
            }
        }
    }
}

using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine;

enum Phases
{
    Draw,
    Summon,
    Combat,
    SecondSummon,
    Last
}

public class Match
{
    private Player[] Players { get; set; }
    private List<ICardIntent>[] CardIntents { get; set; }
    public List<ICardIntent> SuccessfulIntents { get; set; }
    private BoardState BoardState { get; set; }
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

    private int CardIdTotal = 1;

    private List<Card> MatchCards;
    private int MaxRounds = 5;

    public Match()
    {
        Players = new Player[2];
        CardIntents = new List<ICardIntent>[2]{ new(), new() } ;
        SuccessfulIntents = new();
        BoardState = new();
        TurnCount = 1;
        RoundCount = 1;
        MatchCards = new();
    }

    public void SetPlayer(int id, List<Card> deck)
    {
        if (id > Players.Length - 1) 
            return;

        SetCardIds(deck);
        MatchCards.AddRange(deck);

        Players[id] = new Player(deck);

    }

    public void SetCardToPlay(int playerId, int cardId)
    {
        var card = GetCardFromId(cardId);
        if (card != null)
        {
            Players[playerId].CardToPlay = card;
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
        Console.WriteLine($"[EVENT]: Round {RoundCount}");

        for (int p = 0; p < 2; p++)
        {
            if (Players[p].CardToPlay != null)
            {
                var intents = Players[p].CardToPlay.GetIntents();

                foreach (var intent in intents)
                    intent.PlayerId = p;

                CardIntents[p].AddRange(intents);
            }
        }

        List<Summon> Summons = new();
        List<Block> Blocks = new();
        List<Attack> Attacks = new();
        List<DamageMultiply> DamageMultiplies = new();

        foreach (var intent in CardIntents)
        {
            Summons.AddRange(intent.OfType<Summon>().ToList());
            Blocks.AddRange(intent.OfType<Block>().ToList());
            Attacks.AddRange(intent.OfType<Attack>().ToList());
        }

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

        foreach (var cardintent in CardIntents)
            cardintent.Clear();

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

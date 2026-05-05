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
    private BoardState BoardState { get; set; }
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

    private int CardIdTotal = 1;

    private List<ICard> MatchCards;

    public Match()
    {
        Players = new Player[2];
        CardIntents = new List<ICardIntent>[2]{ new(), new() } ;
        BoardState = new();
        TurnCount = 1;
        RoundCount = 1;
        MatchCards = new();
    }

    public void SetPlayer(int id, List<ICard> deck)
    {
        if (id > Players.Length - 1) 
            return;

        SetCardIds(deck);
        MatchCards.AddRange(deck);

        Players[id] = new Player(deck);

    }

    public void SetCardToPlay(int playerId, int cardId)
    {
        Players[playerId].CardToPlay = cardId;
    }

    public bool BothCardsToPlayAreSet()
    {
        foreach (var player in Players)
        {
            if (player.CardToPlay == 0)
                return false;
        }

        return true;
    }

    public List<ICard> GetPlayerHand(int id)
    {
        return Players[id].Hand;
    }

    public void SetCardIds(List<ICard> deck)
    {
        foreach (var card in deck)
            card.Id = CardIdTotal++;
    }

    private ICard? GetCardFromId(int cardId)
    {
        return MatchCards.SingleOrDefault(c => c.Id == cardId) ?? null;
    }

    public void ProcessCards()
    {
        Console.WriteLine($"[EVENT]: Round {RoundCount}");

        for (int p = 0; p < 2; p++)
        {
            if (Players[p].CardToPlay == 0) 
                continue;

            ICard? card = GetCardFromId(Players[p].CardToPlay);

            if (card != null)
            {
                var intents = card.GetIntents();

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
                BoardState.SummonCreature(summon.PlayerId, summon.CardId);
            }
            else
            {
                Console.WriteLine($"[ERROR]: Not enough room on {summon.PlayerId} board for {summon.CardId}");
            }
        }

        foreach (var cardintent in CardIntents)
            cardintent.Clear();

        foreach (var player in Players)
            player.CardToPlay = 0;

        RoundCount++;
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

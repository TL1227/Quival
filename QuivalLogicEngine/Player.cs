using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Turns;

namespace QuivalLogicEngine;

public class Player
{
    public int Id { get; set; }
    public int HealthPoints { get; set; }
    public int Mana { get; set; }
    public List<Card> Hand { get; set; }
    public List<Card> Deck { get; set; }
    public int StartingHandSize { get; set; } = 4;
    public Card? CardToPlay { get; set; }
    public QuivalTurn? SubmittedTurn { get; set; }
    public List<TargetSelection> TargetSelections { get; set; } = new();
    public bool MakingSelections { get; set; } = false;

    public CreatureCard? BlockingCreature { get; set; }

    public Player(int id, List<Card> deck)
    {
        Id = id;
        HealthPoints = 20;
        Mana = 0;
        Deck = new(deck);
        Hand = new();

        ShuffleDeck();
        GetStartingHand();
    }

    public void RemoveCardFromHand(int cardId)
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            if (Hand[i].Id == cardId)
            {
                Hand.RemoveAt(i);
            }
        }
    }

    public void DrawCard(int number)
    {
        for (int i = 0; i < number; i++) 
        {
            if (Deck.Count <= 0)
            {
                //TODO: figure out what happens if we run out of cards
            }
            else if(Hand.Count < 7) //TODO: this is just temporary till we program in the discarding at end of turn stuff
            {
                Hand.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }
    }

    private void ShuffleDeck()
    {
        List<Card> shuffleList = new();
        Random rnd = new();
        while (Deck.Count() > 0)
        {
            int num = rnd.Next(0, Deck.Count());
            shuffleList.Add(Deck[num]);
            Deck.RemoveAt(num);
        }

        Deck = new(shuffleList);
    }

    private void GetStartingHand()
    {
        for (int i = 0; i < StartingHandSize; i++)
        {
            Hand.Add(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    public void ResetBlockingCreatureActions()
    {
        if (BlockingCreature != null)
        {
            BlockingCreature.HasActed = false;
        }
    }
}

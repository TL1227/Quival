using QuivalLogicEngine.Cards;
using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

public class Player
{
    public int Id { get; set; }
    public int HealthPoints { get; set; }
    public int Mana { get; set; }
    public List<Card> Hand { get; set; }
    public List<Card> Deck { get; set; }

    public Card? CardToPlay { get; set; }

    public CreatureCard? BlockingCreature { get; set; }

    public Player(int id, List<Card> deck)
    {
        Id = id;
        HealthPoints = 20;
        Mana = 1;
        Deck = new(deck);
        Hand = new();

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

    private void GetStartingHand()
    {
        for (int i = 0; i < 7; i++)
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

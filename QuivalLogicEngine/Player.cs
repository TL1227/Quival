using QuivalLogicEngine.Cards;
using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

internal class Player
{
    public int HealthPoints { get; set; }
    public List<Card> Hand { get; set; }
    public List<Card> Deck { get; set; }

    public Card? CardToPlay { get; set; }

    public Player(List<Card> deck)
    {
        HealthPoints = 20;
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

}

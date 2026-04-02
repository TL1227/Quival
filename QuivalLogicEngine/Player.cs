namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }

    public Player()
    {
        HealthPoints = 20;
        Deck = new(); //todo: get this somehow 
        Hand = GetStartingHand(Deck);
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

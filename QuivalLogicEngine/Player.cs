namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }
    public SpellStream SpellStream { get; }

    public Player()
    {
        HealthPoints = 20;
        Deck = new(); //todo: get this somehow 
        Hand = GetStartingHand(Deck);
        SpellStream = new();
    }

    public bool SpellStreamSet()
    {
        return SpellStream.ContainsCards();
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

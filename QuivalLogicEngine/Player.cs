namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }
    private Queue<Card> SpellStream { get; set; }

    public Player()
    {
        HealthPoints = 20;
        Deck = new(); //todo: get this somehow 
        Hand = GetStartingHand(Deck);
        SpellStream = new();
    }

    public void SetSpellStream(Queue<Card> spellStream)
    {
        SpellStream = spellStream;
    }

    public bool SpellStreamSet()
    {
        return SpellStream.Count > 0;
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }
    public SpellStream SpellStream { get; set; }

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

    public void SetSpellStream(SpellStream spellStream)
    {
        SpellStream = spellStream;
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

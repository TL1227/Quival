using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }
    private SpellStream Stream { get; set; }

    public Player()
    {
        HealthPoints = 20;
        Deck = new(); //todo: get this somehow 
        Hand = GetStartingHand(Deck);
        Stream = new();
    }

    public void SetSpellStream(SpellStream spellStream)
    {
        Stream = spellStream;
    }

    public bool SpellStreamSet()
    {
        return Stream. > 0;
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

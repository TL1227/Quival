using System.Runtime.CompilerServices;

namespace QuivalLogicEngine;

internal class Player
{
    private int HealthPoints { get; set; }
    private List<Card> Hand { get; set; }
    private List<Card> Deck { get; set; }
<<<<<<< HEAD
    public SpellStream SpellStream { get; }
=======
    private SpellStream Stream { get; set; }
>>>>>>> a0127bc342dc66b9a4cdda07ba28fa54093a61be

    public Player()
    {
        HealthPoints = 20;
        Deck = new(); //todo: get this somehow 
        Hand = GetStartingHand(Deck);
        Stream = new();
    }

<<<<<<< HEAD
    public bool SpellStreamSet()
    {
        return SpellStream.ContainsCards();
=======
    public void SetSpellStream(SpellStream spellStream)
    {
        Stream = spellStream;
    }

    public bool SpellStreamSet()
    {
        return Stream. > 0;
>>>>>>> a0127bc342dc66b9a4cdda07ba28fa54093a61be
    }

    private List<Card> GetStartingHand(List<Card> deck)
    {
        return new();
    }
}

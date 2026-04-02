namespace QuivalLogicEngine;

public enum CardType
{
    Creature,
    Spell,
    SlowSpell,
    QuickSpell,
}

public class Card
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public CardType Type { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }

    public Card(CardType type, int attack, int defence)
    {
        Type = type;
        Attack = attack;
        Defence = defence;
    }
}

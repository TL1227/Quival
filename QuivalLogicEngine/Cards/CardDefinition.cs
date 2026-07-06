namespace QuivalLogicEngine.Cards;

public enum CardType
{
    Spell,
    Creature,
}

public class CardDefinition
{
    public string UniqueId { get; set; }
    public CardType CardType { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? FlavourText { get; set; }
    public int Cost { get; set; }
    public int Attack { get; set; }
    public int Health { get; set; }
    public List<Trigger> Triggers { get; set; } = new();
    public List<Ability> PassiveAbilities { get; set; } = new();
}

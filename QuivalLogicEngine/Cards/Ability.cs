using QuivalLogicEngine.Cards.Effects;

namespace QuivalLogicEngine.Cards;

public enum Conditional
{
    None,
    Round1,
    Round2,
    Round3,
    Round4,
    Round5,
    PlayerCreatureDiedThisTurn,
    OpponentCreatureDiedThisTurn,
    AnyCreatureDiedThisTurn
}

//NOTE: if we want to have an ability trigger multiple times for "Each" conditional met then we need to create a seperate ability.
public enum ConditionalType
{
    And,
    Or,
}

public class Ability
{
    public int Id { get; set; }
    public Target Target { get; set; }

    public Effect Effect { get; set; }
    public Value Value { get; set; }
    public List<Conditional> Conditionals { get; set; } = new();
    public ConditionalType ConditionalType { get; set; }

    public Effect? BonusEffect { get; set; }
    public Value? BonusValue { get; set; }
    public List<Conditional>? BonusConditionals { get; set; } = new();
    public ConditionalType BonusConditionalType { get; set; }
}

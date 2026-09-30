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
    public ConditionalType? BonusConditionalType { get; set; }

    public string GetConditionalText()
    {
        string conditionalText = "";

        foreach (var conditional in Conditionals)
        {
            string text = "if";

            switch (conditional)
            {
                case Conditional.Round1:
                    text = "it's round 1";
                    break;
                case Conditional.Round2:
                    text = "it's round 2";
                    break;
                case Conditional.Round3:
                    text = "it's round 3";
                    break;
                case Conditional.Round4:
                    text = "it's round 4";
                    break;
                case Conditional.Round5:
                    text = "it's round 5";
                    break;
                case Conditional.PlayerCreatureDiedThisTurn:
                    text = "a player creature died this turn";
                    break;
                case Conditional.OpponentCreatureDiedThisTurn:
                    text = "an opponent's creature died this turn";
                    break;
                case Conditional.AnyCreatureDiedThisTurn:
                    text = "a creature died this turn";
                    break;
                case Conditional.None:
                    break;
            }

            conditionalText += $" {text} _";
        }

        switch (ConditionalType)
        {
            case ConditionalType.And:
                conditionalText = conditionalText.Replace("_", "and");
                break;
            case ConditionalType.Or:
                conditionalText = conditionalText.Replace("_", "or");
                break;
        }

        return conditionalText;
    }
}



using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine.Turns;

/*
public enum TurnType
{
    Attack,
    Cast,
    EndTurn,
    MoveToBlock,
}
*/

public class QuivalTurn
{
    public TriggerType Trigger { get; set;}
    public int CardToPlayId { get; set; }
    public Dictionary<Effect, List<int>> SelectedCardIds { get; set; }

    public QuivalTurn()
    {
        SelectedCardIds = new();
    }
}

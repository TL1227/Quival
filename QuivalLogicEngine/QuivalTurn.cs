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
    public Trigger Trigger { get; set;}
    public int CardToPlayId { get; set; }
    public Dictionary<Intent, List<int>> SelectedCardIds { get; set; }

    public QuivalTurn()
    {
        SelectedCardIds = new();
    }
}

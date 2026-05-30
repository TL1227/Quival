namespace QuivalLogicEngine.Turns;

public enum TurnType
{
    Attack,
    Cast,
    EndTurn,
    MoveToBlock,
}

public class QuivalTurn
{
    public TurnType TurnType { get; set;}
    public int CardToPlayId { get; set; }
    public List<int> SelectedCardIds { get; set; }

    public QuivalTurn()
    {
        SelectedCardIds = new();
    }
}

using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine.Turns;

public enum TurnType
{
    Cast,
    Attack,
    MoveToBlockZone,
    EndTurn
}

public class QuivalTurn
{
    public TurnType TurnType { get; set;}
    public int CardToPlayId { get; set; }
    public Dictionary<Effect, List<int>> SelectedCardIds { get; set; }

    public QuivalTurn()
    {
        SelectedCardIds = new();
    }
}

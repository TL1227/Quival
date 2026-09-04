using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "targettype")]
[JsonDerivedType(typeof(SelectionTarget), 0)]
[JsonDerivedType(typeof(SelfTarget), 1)]
[JsonDerivedType(typeof(PlayerTarget), 2)]
[JsonDerivedType(typeof(OpponentTarget), 3)]
public abstract class Target 
{
    public abstract string DisplayName { get; set; }
}

public class SelfTarget : Target 
{
    public override string DisplayName { get; set; } = "Self";
}

public class PlayerTarget : Target
{
    public override string DisplayName { get; set; } = "Player";
}

public class OpponentTarget : Target
{
    public override string DisplayName { get; set; } = "Opponent";
}

public enum TargetPool
{
    Creatures,
    Controllers,
    Damagables
}

public class SelectionTarget : Target
{
    public override string DisplayName { get; set; } = "Select Target";
    public TargetPool TargetPool { get; set; }
    public Side Side { get; set; }
    public bool CanTargetSelf { get; set; }
    public int NumberToPick { get; set; }

    //TODO:This needs to actually be implemented in our selector
    public bool CanPickUpTo { get; set; }

    public List<int> GetTargetPool(int playerId, Match match)
    {
        List<Card> Alltargets = new();

        List<Card> targets = new();
        switch (TargetPool)
        {
            case TargetPool.Creatures:
                targets.AddRange(match.GetAllCreaturesOnBoard());
                break;
            case TargetPool.Controllers:
                targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                break;
            case TargetPool.Damagables:
                targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                targets.AddRange(match.GetAllCreaturesOnBoard());
                break;
        }

        targets = GetCardsOnSide(targets, playerId, Side);

        Alltargets.AddRange(targets); 

        if (!CanTargetSelf)
        {
            Alltargets = Alltargets.Where(x => x.Id != playerId).ToList();
        }

        return Alltargets.Select(x => x.Id).ToList();
    }

    private static List<Card> GetCardsOnSide(List<Card> cards, int playerId, Side side)
    {
        switch (side)
        {
            case Side.Opponent:
                return cards.Where(t => t.PlayerId == Match.GetOpponentId(playerId)).ToList();
            case Side.Player:
                return cards.Where(t => t.PlayerId == playerId).ToList();
            case Side.Any:
            default:
                return cards;
        }
    }
}

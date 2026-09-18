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
    public abstract TargetPool GetTargetPool();

    public abstract string GetTargetDescription(bool isCreatureCard);
}

public class SelfTarget : Target 
{
    public override string DisplayName { get; set; } = "Self";
    public override TargetPool GetTargetPool() => TargetPool.Creatures;

    public override string GetTargetDescription(bool isCreatureCard)
    {
        return "itself";
    }
}

public class PlayerTarget : Target
{
    public override string DisplayName { get; set; } = "Player";
    public override TargetPool GetTargetPool() => TargetPool.Controllers;
    public override string GetTargetDescription(bool isCreatureCard)
    {
        if (isCreatureCard)
            return "it's controller";
        else
            return "yourself";
    }
}

public class OpponentTarget : Target
{
    public override string DisplayName { get; set; } = "Opponent";
    public override TargetPool GetTargetPool() => TargetPool.Controllers;
    public override string GetTargetDescription(bool isCreatureCard)
    {
        return "your opponent";
    }
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

    public override TargetPool GetTargetPool() => TargetPool;

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

    public override string GetTargetDescription(bool isCreatureCard)
    {
        string targetPool = "";
        switch (TargetPool)
        {
            case TargetPool.Creatures:
                targetPool = " creature(s) ";
                switch (Side)
                {
                    case Side.Any:
                        targetPool = " creature(s) ";
                        break;
                    case Side.Opponent:
                        targetPool = " opponent's creature(s) ";
                        break;
                    case Side.Player:
                        targetPool = " creature(s) you control ";
                        break;
                    default:
                        break;
                }
                break;

            case TargetPool.Damagables:
                switch (Side)
                {
                    case Side.Any:
                        targetPool = " damagable target(s) ";
                        break;
                    case Side.Opponent:
                        targetPool = " opponent's damagable target(s) ";
                        break;
                    case Side.Player:
                        targetPool = " damagable target(s) of yours";
                        break;
                    default:
                        break;
                }
                break;

            case TargetPool.Controllers:
                targetPool = " player(s)" ;
                break;

            default:
                break;
        }

        if (NumberToPick > 1)
            targetPool = targetPool.Replace("(", "").Replace(")", "");
        else
            targetPool = targetPool.Replace("(s)", "");


        string text = "";
        if (NumberToPick == 1)
        {
            if (Side == Side.Opponent)
                text = $"an{targetPool}";
            if (Side == Side.Any)
                text = $"any{targetPool}";
            else
                text = $"a{targetPool}";
        }
        else
        {
            text = $"{NumberToPick}{targetPool}";
        }

        return text;
    }

    public string GetTargetNoun()
    {
        return "hello";
    }
}

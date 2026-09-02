using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "targettype")]
    [JsonDerivedType(typeof(SelectionTarget), 0)]
    [JsonDerivedType(typeof(SelfTarget), 1)]
    [JsonDerivedType(typeof(PlayerTarget), 2)]
    [JsonDerivedType(typeof(OpponentTarget), 3)]
    public abstract class Target { }

    public interface ISelectionTarget 
    { 
        public Side Side { get; set; }
    }

    public interface ICanTargetSelf
    {
        public bool CanTargetSelf { get; set; }
    }

    public class SelfTarget : Target { }

    public class PlayerTarget : Target
    {
        public int GetTargetId(Card card) => card.PlayerId;
    }

    public class OpponentTarget : Target
    {
        public int GetTargetId(Card card) => (card.PlayerId == 0) ? 1 : 0;
    }

    public class CreatureTarget : Target, ISelectionTarget, ICanTargetSelf 
    {
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
    }

    public class ControllerTarget : Target, ISelectionTarget
    {
        public Side Side { get; set; }
    }

    public class DamageableTarget : Target, ISelectionTarget, ICanTargetSelf 
    {
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
    }

    public class SelectionTarget : Target
    {
        //NOTE: This should probably be a single ISelectionTarget rather than a list
        public List<ISelectionTarget> TargetsPool { get; set; } = new();
        public int NumberToPick {  get; set; }

        public List<int> GetTargetPool(int playerId, Match match)
        {
            List<Card> Alltargets = new();

            foreach (var tp in TargetsPool)
            {
                List<Card> targets = new();
                switch (tp)
                {
                    case CreatureTarget:
                        targets.AddRange(match.GetAllCreaturesOnBoard());
                        break;
                    case ControllerTarget:
                        targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                        break;
                    case DamageableTarget:
                        targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                        targets.AddRange(match.GetAllCreaturesOnBoard());
                        break;
                }

                targets = GetCardsOnSide(targets, playerId, tp.Side);

                Alltargets.AddRange(targets);

                if (tp is ICanTargetSelf selfTarget && !selfTarget.CanTargetSelf)
                {
                    Alltargets = Alltargets.Where(x => x.Id != playerId).ToList();
                }
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
}

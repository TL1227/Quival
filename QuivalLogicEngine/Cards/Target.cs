using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "targettype")]
    [JsonDerivedType(typeof(SelectionTarget), 0)]
    [JsonDerivedType(typeof(SelfTarget), 1)]
    [JsonDerivedType(typeof(PlayerTarget), 2)]
    [JsonDerivedType(typeof(OpponentTarget), 3)]
    public abstract class Target { }

    public class SelfTarget : Target { }

    public class PlayerTarget : Target
    {
        public int GetTargetId(Card card) => card.PlayerId;
    }

    public class OpponentTarget : Target
    {
        public int GetTargetId(Card card) => (card.PlayerId == 0) ? 1 : 0;
    }

    public class TargetPoolItem
    {
        public TargetPool TargetPoolType;
        public Side Side;
    }

    public enum TargetPool
    {
        Creature,
        Direct,
    }

    public class SelectionTarget : Target
    {
        public List<TargetPoolItem> TargetsPool { get; set; }
        //public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
        public int NumberToPick {  get; set; }

        public List<int> GetTargetPool(Card self, Match match)
        {
            List<Card> Alltargets = new();

            foreach (var tp in TargetsPool)
            {
                List<Card> targets = new();
                switch (tp.TargetPoolType)
                {
                    case TargetPool.Creature:
                        targets.AddRange(match.GetAllCreaturesOnBoard());
                        break;
                    case TargetPool.Direct:
                        targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                        break;
                }

                switch (tp.Side)
                {
                    case Side.Opponent:
                        targets = targets.Where(t => t.PlayerId == Match.GetOpponentId(self.PlayerId)).ToList();
                        break;
                    case Side.Player:
                        targets = targets.Where(t => t.PlayerId == self.PlayerId).ToList();
                        break;
                    default:
                    case Side.Any:
                        break;
                }

                Alltargets.AddRange(targets);
            }

            if (!CanTargetSelf)
                Alltargets = Alltargets.Where(x => x.Id != self.Id).ToList();

            return Alltargets.Select(x => x.Id).ToList();
        }
    }
}

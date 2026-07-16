using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "targettype")]
    [JsonDerivedType(typeof(SelectionTarget), 0)]
    [JsonDerivedType(typeof(SelfTarget), 1)]
    public abstract class Target { }

    public class SelfTarget : Target
    {
    }

    public enum DirectTargetType
    {
        Player,
        Opponent
    }

    public class PlayerTarget : Target
    {
        public TargetPool TargetPool { get; set; } = TargetPool.Direct;
        public int GetTargetId(Card card) => card.PlayerId;
    }

    public class OpponentTarget : Target
    {
        public TargetPool TargetPool { get; set; } = TargetPool.Direct;
        public int GetTargetId(Card card) => (card.PlayerId == 0) ? 1 : 0;
    }

    public enum TargetPool
    {
        Creature,
        Direct,
    }

    public class SelectionTarget : Target
    {
        public List<TargetPool> TargetsPool { get; set; }
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
        public int NumberToPick {  get; set; }

        public List<int> GetTargetPool(Card self, Match match)
        {
            List<Card> targets = new();

            foreach (var tp in TargetsPool)
            {
                switch (tp)
                {
                    case TargetPool.Creature:
                        targets.AddRange(match.GetAllCreaturesOnBoard());
                        break;
                    case TargetPool.Direct:
                        targets.AddRange(match.MatchCards.OfType<PlayerCard>().ToList());
                        break;
                }
            }

            if (!CanTargetSelf)
                targets = targets.Where(x => x.Id != self.Id).ToList();

            return Side switch
            {
                Side.Opponent => targets.Where(t => t.PlayerId == Match.GetOpponentId(self.PlayerId)).Select(x => x.Id).ToList(),
                Side.Player => targets.Where(t => t.PlayerId == self.PlayerId).Select(x => x.Id).ToList(),
                _ => targets.Select(x => x.Id).ToList(),
            };
        }
    }
}

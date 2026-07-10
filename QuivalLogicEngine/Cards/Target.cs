using System.Text.Json.Serialization;
namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "targettype")]
    [JsonDerivedType(typeof(AutoTarget), 0)]
    [JsonDerivedType(typeof(SelectionTarget), 1)]
    public abstract class Target { }

    public enum AutoTargetType
    {
        Self,
        Player,
        Opponent
    }

    public class AutoTarget : Target
    {
        public AutoTargetType AutoTargetType { get; set; }

        public int GetTargetId(Card card)
        {
            return AutoTargetType switch
            {
                AutoTargetType.Self => card.Id,
                AutoTargetType.Player => card.PlayerId,
                AutoTargetType.Opponent => (card.PlayerId == 0) ? 1 : 0,
                _ => throw new Exception($"AutoTargetType Not Found On Card Id {card.Id}"),
            };
        }
    }

    public enum SelectionTargetType
    {
        Creature,
        Damagable,
        Direct //Directly select opponent or player
    }

    public class SelectionTarget : Target
    {
        public SelectionTargetType SelectionTargetType { get; set; }
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
        public int NumberToPick {  get; set; }

        public List<int> GetTargetPool(Card self, Match match)
        {
            List<Card> targets = new();
            var creatureCards = match.GetAllCreaturesOnBoard();
            var playerCards = match.MatchCards.OfType<PlayerCard>().ToList();

            switch (SelectionTargetType)
            {
                case SelectionTargetType.Creature:
                    targets.AddRange(creatureCards);
                    break;
                case SelectionTargetType.Damagable:
                    {
                        targets.AddRange(creatureCards);
                        targets.AddRange(playerCards);
                    }
                    break;
                case SelectionTargetType.Direct:
                    targets.AddRange(playerCards);
                    break;
                default:
                    break;
            }

            if (!CanTargetSelf)
                targets = targets.Where(x => x.Id != self.Id).ToList();

            switch (Side)
            {
                case Side.Any:
                default:
                    {
                        return targets.Select(x => x.Id).ToList();
                    }
                case Side.Opponent:
                    {
                        return targets.Where(t => t.PlayerId == Match.GetOpponentId(self.PlayerId))
                            .Select(x => x.Id)
                            .ToList();
                    }
                case Side.Player:
                    {
                        return targets.Where(t => t.PlayerId == self.PlayerId)
                            .Select(x => x.Id)
                            .ToList();
                    }
            }
        }
    }
}

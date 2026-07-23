using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "effect")]
    [JsonDerivedType(typeof(HealEffect), 0)]
    [JsonDerivedType(typeof(DirectDamageEffect), 1)]
    [JsonDerivedType(typeof(ReviveEffect ), 2)]
    [JsonDerivedType(typeof(AttackBuffRoundEffect), 3)]
    [JsonDerivedType(typeof(AttackBuffEffect), 4)]
    [JsonDerivedType(typeof(AttackDebuffEffect), 5)]
    [JsonDerivedType(typeof(DrawCardEffect), 6)]
    public abstract class Effect()
    {
        public List<TargetPool> ValidTargets { get; set; }

        public abstract string TargetString { get; set; }

        public bool IsValidSelectionTarget(SelectionTarget target)
        {
            foreach (var targetpool in target.TargetsPool)
            {
                if (!ValidTargets.Contains(targetpool))
                    return false;
            }
            
            return true;
        }

        public string GetTargetString()
        {
            return TargetString;
        }
    }

    public class HealEffect : Effect 
    {
        public override string TargetString { get; set; } = "Heal";

        public HealEffect()
        {
            ValidTargets = 
            [
                TargetPool.Creature,
                TargetPool.Direct,
            ];
        }
    }

    public class DirectDamageEffect : Effect 
    {
        public override string TargetString { get; set; } = "Damage";

        public DirectDamageEffect()
        {
            ValidTargets =
            [
                TargetPool.Creature,
                TargetPool.Direct,
            ];
        }
    }

    public class ReviveEffect : Effect 
    {
        public override string TargetString { get; set; } = "Revive";

        public ReviveEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackBuffRoundEffect : Effect 
    {
        public override string TargetString { get; set; } = "Buff";

        public AttackBuffRoundEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackBuffEffect : Effect 
    {
        public override string TargetString { get; set; } = "Buff";

        public AttackBuffEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackDebuffEffect : Effect 
    {
        public override string TargetString { get; set; } = "Debuff";

        public AttackDebuffEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class DrawCardEffect : Effect 
    {
        public override string TargetString { get; set; } = "Draw";

        public DrawCardEffect()
        {
            ValidTargets = [TargetPool.Direct];
        }
    }
}

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
        public TargetPool ValidTargetPool { get; set; }

        public abstract string TargetString { get; set; }

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
            ValidTargetPool = TargetPool.Damagables;
        }
    }

    public class DirectDamageEffect : Effect 
    {
        public override string TargetString { get; set; } = "Damage";

        public DirectDamageEffect()
        {
            ValidTargetPool = TargetPool.Damagables;
        }
    }

    public class ReviveEffect : Effect 
    {
        public override string TargetString { get; set; } = "Revive";

        public ReviveEffect()
        {
            ValidTargetPool =  TargetPool.Creatures;
        }
    }

    public class AttackBuffRoundEffect : Effect 
    {
        public override string TargetString { get; set; } = "Buff";

        public AttackBuffRoundEffect()
        {
            ValidTargetPool =  TargetPool.Creatures;
        }
    }

    public class AttackBuffEffect : Effect 
    {
        public override string TargetString { get; set; } = "Buff";

        public AttackBuffEffect()
        {
            ValidTargetPool =  TargetPool.Creatures;
        }
    }

    public class AttackDebuffEffect : Effect 
    {
        public override string TargetString { get; set; } = "Debuff";

        public AttackDebuffEffect()
        {
            ValidTargetPool =  TargetPool.Creatures;
        }
    }

    public class DrawCardEffect : Effect 
    {
        public override string TargetString { get; set; } = "Draw";

        public DrawCardEffect()
        {
            ValidTargetPool =  TargetPool.Controllers;
        }
    }
}

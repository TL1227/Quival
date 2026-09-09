using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards.Effects;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "effect")]
[JsonDerivedType(typeof(Heal), 0)]
[JsonDerivedType(typeof(DirectDamage), 1)]
[JsonDerivedType(typeof(Revive), 2)]
[JsonDerivedType(typeof(AttackBuffRound), 3)]
[JsonDerivedType(typeof(AttackBuff), 4)]
[JsonDerivedType(typeof(AttackDebuff), 5)]
[JsonDerivedType(typeof(DrawCard), 6)]
public abstract class Effect()
{
    public TargetPool ValidTargetPool { get; set; }

    public abstract string EffectString { get; set; }

    public string GetTargetString()
    {
        return EffectString;
    }
}

public class Heal: Effect 
{
    public override string EffectString { get; set; } = "Heal";

    public Heal()
    {
        ValidTargetPool = TargetPool.Damagables;
    }
}

public class DirectDamage: Effect 
{
    public override string EffectString { get; set; } = "Damage";

    public DirectDamage()
    {
        ValidTargetPool = TargetPool.Damagables;
    }
}

public class Revive: Effect 
{
    public override string EffectString { get; set; } = "Revive";

    public Revive()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }
}

public class AttackBuffRound: Effect 
{
    public override string EffectString { get; set; } = "Buff";

    public AttackBuffRound()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }
}

public class AttackBuff: Effect 
{
    public override string EffectString { get; set; } = "Buff";

    public AttackBuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }
}

public class AttackDebuff: Effect 
{
    public override string EffectString { get; set; } = "Debuff";

    public AttackDebuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }
}

public class DrawCard: Effect 
{
    public override string EffectString { get; set; } = "Draw";

    public DrawCard()
    {
        ValidTargetPool =  TargetPool.Controllers;
    }
}

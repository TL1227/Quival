using QuivalLogicEngine.CardDescription;
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

    public abstract string GetEffectCardDescription();

}

public class Heal: Effect 
{
    public override string EffectString { get; set; } = "Heal";

    public Heal()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription()
    {
        return $"heal {CardDescriptionKeys.EffectValue} point(s) of health to {CardDescriptionKeys.Target}";
    }
}

public class DirectDamage: Effect 
{
    public override string EffectString { get; set; } = "Damage";

    public DirectDamage()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription()
    {
        return $"deal {CardDescriptionKeys.EffectValue} point(s) of direct damage to {CardDescriptionKeys.Target}";
    }
}

public class Revive: Effect 
{
    public override string EffectString { get; set; } = "Revive";

    public Revive()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription()
    {
        return $"revive {CardDescriptionKeys.EffectValue} creature";
    }
}

public class AttackBuffRound: Effect 
{
    public override string EffectString { get; set; } = "Attack Buff Round";

    public AttackBuffRound()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription()
    {
        return $"buff the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue} till end of round";
    }
}

public class AttackBuff: Effect 
{
    public override string EffectString { get; set; } = "Attack Buff";

    public AttackBuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription()
    {
        return $"buff the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue}";
    }
}

public class AttackDebuff: Effect 
{
    public override string EffectString { get; set; } = "Attack Debuff";

    public AttackDebuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription()
    {
        return $"defbuff the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue} point(s)";
    }
}

public class DrawCard: Effect 
{
    public override string EffectString { get; set; } = "Draw";

    public DrawCard()
    {
        ValidTargetPool =  TargetPool.Controllers;
    }

    public override string GetEffectCardDescription()
    {
        return $"Draw {CardDescriptionKeys.EffectValue} card(s)";
    }
}

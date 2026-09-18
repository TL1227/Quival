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

    public abstract string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget);

}

public class Heal: Effect 
{
    public override string EffectString { get; set; } = "Heal";

    public Heal()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it heals {CardDescriptionKeys.EffectValue} point(s) of health to {CardDescriptionKeys.Target}";
        else if (isSelfTarget)
            return $"it heals itself by {CardDescriptionKeys.EffectValue} point(s)";
        else
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

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it deals {CardDescriptionKeys.EffectValue} damage to {CardDescriptionKeys.Target}";
        else if (isSelfTarget)
            return $"it damages itself by {CardDescriptionKeys.EffectValue}";
        else
            return $"deal {CardDescriptionKeys.EffectValue} damage to {CardDescriptionKeys.Target}";
    }
}

//TODO: actually implement this in game
public class Revive: Effect 
{
    public override string EffectString { get; set; } = "Revive";

    public Revive()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it revives {CardDescriptionKeys.EffectValue} creature";
        else if (isSelfTarget)
            return $"it revives itself by {CardDescriptionKeys.EffectValue}";
        else
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

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it buffs the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue} till end of round";
        else if (isSelfTarget)
            return $"it buffs it's attack by {CardDescriptionKeys.EffectValue}";
        else
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

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it buffs the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue}";
        else
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

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        if (isCreatureCard)
            return $"it defbuffs the attack of {CardDescriptionKeys.Target} by {CardDescriptionKeys.EffectValue} point(s)";
        else
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

    public override string GetEffectCardDescription(bool isCreatureCard, bool isSelfTarget)
    {
        return $"Draw {CardDescriptionKeys.EffectValue} card(s)";
    }
}

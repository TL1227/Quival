using System.Text.Json.Serialization;
using Key = QuivalLogicEngine.CardDescription.CardDescriptionKeys;

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

    public abstract string GetEffectCardDescription(Target target, Trigger trigger);

}

public class Heal: Effect 
{
    public override string EffectString { get; set; } = "Heal";

    public Heal()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription(Target target, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"heal {Key.EffectValue} point(s) of life {Key.CountValue} to {selectionTarget.GetSelectionTargetDescription()}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"heal {Key.CardName} by {Key.EffectValue} point(s)";
                }
                else
                {
                    text = $"it heals itself by {Key.EffectValue} point(s)";
                }
                break;
            case PlayerTarget:
                    text = $"you gain {Key.EffectValue} point(s) of life";
                break;
            case OpponentTarget:
                text = $"each opponent gains {Key.EffectValue} point(s) of life";
                break;
        }

        return text;
    }
}

public class DirectDamage: Effect 
{
    public override string EffectString { get; set; } = "Damage";

    public DirectDamage()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"deal {Key.EffectValue} damage {Key.CountValue} to {selectionTarget.GetSelectionTargetDescription()}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"deal {Key.EffectValue} damage to {Key.CardName}";
                }
                else
                {
                    text = $"it deals {Key.EffectValue} damage to itself";
                }
                break;
            case PlayerTarget:
                text = $"you take {Key.EffectValue} damage";
                break;
            case OpponentTarget:
                text = $"deal {Key.EffectValue} damage to each oppononent";
                break;
        }

        return text;
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

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        return "Revive effect text not yet completed";

        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"deal {Key.EffectValue} damage to {Key.Target}";
                break;
            case SelfTarget:
                text = $"it deals {Key.EffectValue} damage to itself";
                break;
            case PlayerTarget:
                text = $"deal {Key.EffectValue} damage to it's controller ";
                break;
            case OpponentTarget:
                text = $"deal {Key.EffectValue} damage to each oppononent";
                break;
        }

        return text;
    }
}

public class AttackBuffRound: Effect 
{
    public override string EffectString { get; set; } = "Attack Buff Round";

    public AttackBuffRound()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"increase {selectionTarget.GetSelectionTargetDescription()} by {Key.EffectValue} {Key.CountValue} until the end of the round";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"increase the attack of {Key.CardName} by {Key.EffectValue} until the end of the round";
                }
                else
                {
                    text = $"increase it's own attack by {Key.CardName} until the end of the round";
                }
                break;
        }

        return text;
    }
}

public class AttackBuff: Effect 
{
    public override string EffectString { get; set; } = "Attack Buff";

    public AttackBuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"increase the attack of {selectionTarget.GetSelectionTargetDescription()} by {Key.EffectValue} {Key.CountValue}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"increase the attack of {Key.CardName} by {Key.EffectValue}";
                }
                else
                {
                    text = $"increase it's own attack by {Key.CardName}";
                }
                break;
        }

        return text;
    }
}

public class AttackDebuff: Effect 
{
    public override string EffectString { get; set; } = "Attack Debuff";

    public AttackDebuff()
    {
        ValidTargetPool =  TargetPool.Creatures;
    }

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"decrease the attack of {selectionTarget.GetSelectionTargetDescription()} by {Key.EffectValue} {Key.CountValue}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"decrease the attack of {Key.CardName} by {Key.EffectValue}";
                }
                else
                {
                    text = $"decrease it's own attack by {Key.CardName}";
                }
                break;
        }

        return text;
    }
}

public class DrawCard: Effect 
{
    public override string EffectString { get; set; } = "Draw";

    public DrawCard()
    {
        ValidTargetPool =  TargetPool.Controllers;
    }

    public override string GetEffectCardDescription(Target target,  Trigger trigger)
    {
        return $"Draw {Key.EffectValue} card(s) {Key.CountValue}";
    }
}

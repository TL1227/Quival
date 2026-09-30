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

    public abstract string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger);

}

public class Heal: Effect 
{
    public override string EffectString { get; set; } = "Heal";

    public Heal()
    {
        ValidTargetPool = TargetPool.Damagables;
    }

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"heal {CardDescriptionKeys.EffectValue} point(s) of life to {selectionTarget.GetSelectionTargetDescription()}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"heal {CardDescriptionKeys.CardName} by {CardDescriptionKeys.EffectValue} point(s)";
                }
                else
                {
                    text = $"it heals itself by {CardDescriptionKeys.EffectValue} point(s)";
                }
                break;
            case PlayerTarget:
                    text = $"you gain {CardDescriptionKeys.EffectValue} point(s) of life";
                break;
            case OpponentTarget:
                text = $"each opponent gains {CardDescriptionKeys.EffectValue} point(s) of life";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"deal {CardDescriptionKeys.EffectValue} damage to {selectionTarget.GetSelectionTargetDescription()}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"deal {CardDescriptionKeys.EffectValue} damage to {CardDescriptionKeys.CardName}";
                }
                else
                {
                    text = $"it deals {CardDescriptionKeys.EffectValue} damage to itself";
                }
                break;
            case PlayerTarget:
                text = $"you take {CardDescriptionKeys.EffectValue} damage";
                break;
            case OpponentTarget:
                text = $"deal {CardDescriptionKeys.EffectValue} damage to each oppononent";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        return "Revive effect text not yet completed";

        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"deal {CardDescriptionKeys.EffectValue} damage to {CardDescriptionKeys.Target}";
                break;
            case SelfTarget:
                text = $"it deals {CardDescriptionKeys.EffectValue} damage to itself";
                break;
            case PlayerTarget:
                text = $"deal {CardDescriptionKeys.EffectValue} damage to it's controller ";
                break;
            case OpponentTarget:
                text = $"deal {CardDescriptionKeys.EffectValue} damage to each oppononent";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"increase {selectionTarget.GetSelectionTargetDescription()} by {CardDescriptionKeys.EffectValue} until the end of the round";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"increase the attack of {CardDescriptionKeys.CardName} by {CardDescriptionKeys.EffectValue} until the end of the round";
                }
                else
                {
                    text = $"increase it's own attack by {CardDescriptionKeys.CardName} until the end of the round";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"increase the attack of {selectionTarget.GetSelectionTargetDescription()} by {CardDescriptionKeys.EffectValue}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"increase the attack of {CardDescriptionKeys.CardName} by {CardDescriptionKeys.EffectValue}";
                }
                else
                {
                    text = $"increase it's own attack by {CardDescriptionKeys.CardName}";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        string text = "";

        switch (target)
        {
            case SelectionTarget selectionTarget:
                text = $"decrease the attack of {selectionTarget.GetSelectionTargetDescription()} by {CardDescriptionKeys.EffectValue}";
                break;
            case SelfTarget:
                if (trigger is PhaseTrigger || trigger is ListeningTrigger)
                {
                    text = $"decrease the attack of {CardDescriptionKeys.CardName} by {CardDescriptionKeys.EffectValue}";
                }
                else
                {
                    text = $"decrease it's own attack by {CardDescriptionKeys.CardName}";
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

    public override string GetEffectCardDescription(Target target, CardType cardType, Trigger trigger)
    {
        return $"Draw {CardDescriptionKeys.EffectValue} card(s)";
    }
}

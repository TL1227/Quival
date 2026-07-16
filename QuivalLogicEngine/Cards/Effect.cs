
    /*
    public enum Effect
    {
        //target Creature
        AttackBuffRound,
        AttackBuff,
        AttackDebuff,
        CreateAttackUpBadge,
        CreateDamageAbsorbBadge,
        RestoreAction,

        //target player only
        DrawCard,
        Discard
    }
    */
namespace QuivalLogicEngine.Cards
{
    public abstract class Effect()
    {
        public required List<TargetPool> ValidTargets { get; set; }

        public bool IsValidSelectionTarget(SelectionTarget target)
        {
            foreach (var targetpool in target.TargetsPool)
            {
                if (!ValidTargets.Contains(targetpool))
                    return false;
            }
            
            return true;
        }
    }

    public class HealEffect : Effect 
    {
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
        public ReviveEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackBuffRoundEffect : Effect 
    {
        public AttackBuffRoundEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackBuffEffect : Effect 
    {
        public AttackBuffEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class AttackDebuffEffect : Effect 
    {
        public AttackDebuffEffect()
        {
            ValidTargets = [TargetPool.Creature];
        }
    }

    public class DrawCardEffect : Effect {
        public DrawCardEffect()
        {
            ValidTargets = [TargetPool.Direct];
        }
    }

}

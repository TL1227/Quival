using System.Net.NetworkInformation;

namespace QuivalLogicEngine.Cards
{
    public enum TriggerType
    {
        //These also double up as TurnTypes so maybe they should be seperate in that way?
        None,
        Attack,
        Cast,
        PlayerActivate, //This is what we'll use to say that an ability can be triggered by the player as an action
        MoveToBlockZone,
        BlockSwap,

        //passive triggers - maybe these need to be some kind of subtype?
        EndTurn,
        EndRound,
        CreatureCast,
        CreatureDeath
    }

    public abstract class EffectTest()
    {
        public required List<Target> ValidTargets { get; set; }

        public bool IsValidTarget(Target target)
        {
            if (target is SelectionTarget st)
            {
                var test = ValidTargets.SingleOrDefault(x => x is SelectionTarget y && y.SelectionTargetType == st.SelectionTargetType );
                return test != null;
            }
            else
            {
                return ValidTargets.Contains(target);
            }
        }
    }

    public class HealEffect : EffectTest 
    {
        public HealEffect()
        {
            ValidTargets = new()
            {
                new DirectTarget(),
                new SelfTarget(),
                new SelectionTarget(){ SelectionTargetType = SelectionTargetType.Damagable },
                new SelectionTarget(){ SelectionTargetType = SelectionTargetType.Direct }
            };
        }
    }

    public class ReviveEffect : EffectTest 
    {
        public ReviveEffect()
        {
            ValidTargets = new()
            {
                new SelectionTarget(){ SelectionTargetType = SelectionTargetType.Creature }
            };
        }
    }

    public class DirectDamageEffect : EffectTest 
    {
        public DirectDamageEffect()
        {
            ValidTargets = new()
            {
                new DirectTarget(),
                new SelfTarget(),
                new SelectionTarget(){ SelectionTargetType = SelectionTargetType.Damagable },
                new SelectionTarget(){ SelectionTargetType = SelectionTargetType.Direct }
            };
        }
    }


    public enum Effect
    {
        //target None
        None,

        //target Creature
        AttackBuffRound,
        AttackBuff,
        AttackDebuff,
        CreateAttackUpBadge,
        CreateDamageAbsorbBadge,
        RestoreAction,

        //target Creature or Player
        Heal, 
        DirectDamage,

        //target player only
        DrawCard,
    }

    public enum Conditional
    {
        None,
        Round1,
        Round2,
        Round3,
        Round4,
        Round5,
    }

    public enum TargetType
    {
        //You don't need to pick a target
        None,
        Self, //as in the card the ability belongs to
        Player,
        Opponent,

        //Needs Targets Selecting
        Any,
        Creature,
        Damageable,

        //Change this probably
        UseFirst, //use this when you want to just use the target from the first ability in the trigger
    }

    public enum Side
    {
        Any,
        Opponent,
        Player,
    }

    public enum ChoiceType
    {
        And, //NOTE: And is the default
        Or,
        PickNumber, //TODO: should 'Or' just be 'PickNumber 1'?
        PickUpTo
    }

    public enum ValueFrom
    {
        None,
        CreaturesOnTheBoard,
        CardsInHand,
    }

    public class Trigger //NOTE: This should probably just be called Trigger and the enum be called TriggerType
    {
        public TriggerType TriggerType { get; set; }
        public List<Ability> Abilities { get; set; } = new();
        public ChoiceType ChoiceType { get; set; }
        public int ChoiceNumber { get; set; }
        public Side Side { get; set; }

        public Trigger()
        {
            int count = 0;
            foreach (var ability in Abilities)
            {
                ability.Id = count++;
            }
        }
    }

    public class Ability
    {
        public int Id { get; set; } 
        public Target Target { get; set; }

        public Effect Effect { get; set; }
        public Value Value { get; set; }
        public List<Conditional> Conditionals { get; set; } = new();

        public Effect? BonusEffect { get; set; }
        public Value? BonusValue { get; set; }
        public List<Conditional>? BonusConditionals { get; set; } = new();


        public EffectTest EffectTest { get; set; }
    }

    public class TargetSelection
    {
        public List<int> TargetsToPickFrom { get; set; } = new();
        public List<int> SelectedTargets { get; set; } = new();
        public TriggerType Trigger { get; set; }
        public int CardId { get; set; }

        //NOTE: These are all in Ability. Should we just grab a copy of it?
        public int NumberToPick {  get; set; }
        public int AbilityId { get; set; }
        public Effect Effect { get; set; }
    }
}

namespace QuivalLogicEngine.Cards
{
    public enum TriggerType
    {
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

    public enum Effect
    {
        None,
        AttackBuffRound,
        AttackBuff,
        AttackDebuff,
        AttackUpToken,
        DamageAbsorbToken,
        DirectDamage,
        Heal,
        DrawCard,
        RestoreAction, //so the target can perform another action this turn
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
        None, //You don't need to pick a target
        Any,
        Self, //as in the card the ability belongs to
        Creature,
        Player,
        Opponent,
        Damageable,
        UseFirst, //use this when you want to just use the target from the first ability in the trigger
    }

    public enum Side
    {
        None,
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
        public Effect Effect { get; set; }

        //NOTE: maybe these 3 should be squashed into some kind of Target class 
        public TargetType TargetType { get; set; }
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; } = true;

        public int NumberOfTargetSelectionsNeeded { get; set; } = 0;
        public int Value { get; set; }
        public ValueFrom ValueFrom { get; set; }
        public List<Conditional> Conditionals { get; set; } = new();
    }

    public class TargetSelection
    {
        public List<int> TargetsToPickFrom { get; set; } = new();
        public List<int> SelectedTargets { get; set; } = new();
        public TriggerType Trigger { get; set; }
        public int CardId { get; set; }

        //NOTE: These are all in Ability. Should we just grab a copy of it?
        public int NumberToPick {  get; set; }
        public TargetType TargetType { get; set; }
        public int AbilityId { get; set; }
        public Effect Effect { get; set; }
    }
}

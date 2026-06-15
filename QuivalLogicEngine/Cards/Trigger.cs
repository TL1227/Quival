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
        EndTurn
    }

    public enum Effect
    {
        None,
        AttackBuff,
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
        Damageable
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

    public class Trigger //NOTE: This should probably just be called Trigger and the enum be called TriggerType
    {
        public TriggerType TriggerType { get; set; }
        public List<Ability> Abilities { get; set; } = new();
        public ChoiceType ChoiceType { get; set; }
        public int ChoiceNumber { get; set; }
    }

    public class Ability
    {
        public Effect Effect { get; set; }
        public TargetType TargetType { get; set; }
        public bool CanTargetSelf { get; set; } = true;
        public int NumberOfTargets { get; set; }
        public Side Side { get; set; }
        public int Value { get; set; }
        public List<Conditional> Conditionals { get; set; } = new();
    }

    public class TargetSelection
    {
        public List<int> TargetsToPickFrom { get; set; } = new();
        public int NumberToPick {  get; set; }
        public List<int> SelectedTargets { get; set; } = new();
        public TargetType TargetType { get; set; }
        public TriggerType Trigger { get; set; }
        public Ability CardAction { get; set; } = new();
    }
}

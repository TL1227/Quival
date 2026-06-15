namespace QuivalLogicEngine.Cards
{
    public enum Trigger
    {
        None,
        Attack,
        Cast,
        PlayerActivate, //This is what we'll use to say that an ability can be triggered by the player as an action
        MoveToBlockZone,
        BlockSwap
    }

    public enum Intent
    {
        None,
        AttackBuff,
        AttackUpToken,
        DamageAbsorbToken,
        DirectDamage,
        Heal,
        DrawCard,
        RushDown, //This is a keyword and might do better in it's own "Keywords" field
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

    public class Ability //NOTE: This should probably just be called Trigger and the enum be called TriggerType
    {
        public Trigger Trigger { get; set; }
        public List<CardAction> Actions { get; set; } = new();
        public ChoiceType ChoiceType { get; set; }
        public int ChoiceNumber { get; set; }
    }

    public class CardAction //NOTE: Then this would be called Ability which is closer to what it is
    {
        public Intent Intent { get; set; } //NOTE: intent could then be Effect, which again is more descriptive
        public TargetType TargetType { get; set; }
        public bool CanTargetSelf { get; set; } = true;
        public int NumberOfTargets { get; set; }
        public Side Side { get; set; }
        public int Value { get; set; }
        public List<Conditional> Conditionals { get; set; } = new();
    }

    public class TargetSelection
    {
        public List<Card> TargetsToPickFrom { get; set; } = new();
        public int NumberToPick {  get; set; }
        public List<Card> SelectedTargets { get; set; } = new();
    }
}

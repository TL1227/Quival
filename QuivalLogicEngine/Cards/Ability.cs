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
        DamageAbsorbToken,
        DirectDamage,
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
        None,
        Self, //as in the card the ability belongs to
        Damageable, //so creatures, players. This should futureproof if we have some other card type that gets given health somehow
        CanAct, //so something that can take an action, currently this is creatures
        Player,
        Creature
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

    public class Ability
    {
        public Trigger Trigger { get; set; }
        public List<CardAction> Actions { get; set; } = new();
        public ChoiceType ChoiceType { get; set; }
        public int ChoiceNumber { get; set; }
    }

    public class CardAction
    {
        public Intent Intent { get; set; }
        public TargetType TargetType { get; set; }
        public int NumberOfTargets { get; set; }
        public Side Side { get; set; }
        public int Value { get; set; }
        public List<Conditional> Conditionals { get; set; } = new();
    }
}

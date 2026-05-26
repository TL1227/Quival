namespace QuivalLogicEngine.Cards
{
    public enum Trigger
    {
        Attack,
        Cast,
        Activate //This is what we'll use to say that an ability can be triggered by the player as an action
    }

    public enum Intent
    {
        AttackBuff,
        DamageAbsorbToken,
        DirectDamage,
        DrawCard,
        RushDown, //This is a keyword and might do better in it's own "Keywords" field
        RestoreAction, //so the target can perform another action this turn
    }

    public enum Conditional
    {
        Round1,
        Round2,
        Round3,
        Round4,
        Round5,
    }

    public enum Target
    {
        Self, //as in the card the ability belongs to
        Damageable, //so creatures, players. This should futureproof if we have some other card type that gets given health somehow
        CanAct, //so something that can take an action, currently this is creatures
        Player,
        Creature
    }

    public enum Side
    {
        Any,
        Opponent,
        Player,
    }

    public class Ability
    {
        public List<Trigger> Triggers { get; set; }
        public List<Intent> Intents { get; set; }
        public List<Target> Targets { get; set; }
        public List<Side> Sides { get; set; }
        public List<int> Values { get; set; }
        public List<Conditional> Conditionals { get; set; }
    }
}

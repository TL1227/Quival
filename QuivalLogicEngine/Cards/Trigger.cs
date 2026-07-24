using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "trigger")]
    [JsonDerivedType(typeof(CastTrigger), 0)]
    [JsonDerivedType(typeof(SelfTrigger), 1)]
    [JsonDerivedType(typeof(ListeningTrigger), 2)]
    public abstract class Trigger //NOTE: This should probably just be called Trigger and the enum be called TriggerType
    {
        //public TriggerType TriggerType { get; set; }
        public List<Ability> Abilities { get; set; } = new();
        public ChoiceType ChoiceType { get; set; }
        public int ChoiceNumber { get; set; }
        public abstract bool SameAs(Trigger otherTrigger);

        public Trigger()
        {
            int count = 0;
            foreach (var ability in Abilities)
            {
                ability.Id = count++;
            }
        }
    }

    public class CastTrigger : Trigger
    {
        public override bool SameAs(Trigger otherTrigger)
        {
            return GetType() == otherTrigger.GetType();
        }
    }

    public enum SelfTriggerType
    {
        Attack,
        PlayerActivate, //This is what we'll use to say that an ability can be triggered by the player as an action
        MoveToBlockZone,
        BlockSwap,
        TakeDamage,
        Dies
    }

    public class SelfTrigger : Trigger
    {
        public required SelfTriggerType SelfTriggerType { get; set; }

        public override bool SameAs(Trigger otherTrigger)
        {
            return otherTrigger is SelfTrigger st &&
                st.SelfTriggerType == SelfTriggerType;
        }
    }

    public enum ListeningTriggerType
    {
        CreatureCast,
        CreatureDies,
        CreatureAttacks,
        CreatureTakesDamage,
        CreatureMovesToBlockZone,

        SpellCast,

        DrawCard,
        DiscardCard
    }

    public class ListeningTrigger : Trigger
    {
        public required ListeningTriggerType ListeningTriggerType  { get; set; }
        public Side Side { get; set; }
        public bool CanTargetSelf { get; set; }
        public override bool SameAs(Trigger otherTrigger)
        {
            return otherTrigger is ListeningTrigger st &&
                st.ListeningTriggerType == ListeningTriggerType;
        }
    }

    public enum PhaseTriggerType
    {
        EndTurn,
        EndRound,
    }

    public class PhaseTrigger : Trigger
    {
        public required PhaseTriggerType PhaseTriggerType { get; set; }
        public override bool SameAs(Trigger otherTrigger)
        {
            return otherTrigger is PhaseTrigger st &&
                st.PhaseTriggerType == PhaseTriggerType;
        }
    }

    public enum Conditional
    {
        Round1,
        Round2,
        Round3,
        Round4,
        Round5,
        PlayerCreatureDiedThisTurn,
        OpponentCreatureDiedThisTurn,
        AnyCreatureDiedThisTurn
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
    }

    public class TargetSelection
    {
        public List<int> TargetsToPickFrom { get; set; } = new();
        public List<int> SelectedTargets { get; set; } = new();
        public int CardId { get; set; }

        //NOTE: These are all in Ability. Should we just grab a copy of it?
        public int NumberToPick {  get; set; }
        public int AbilityId { get; set; }
        public Effect Effect { get; set; }
    }
}

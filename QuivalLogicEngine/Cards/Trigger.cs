using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

using QuivalLogicEngine.Cards.Effects;

namespace QuivalLogicEngine.Cards;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "trigger")]
[JsonDerivedType(typeof(CastTrigger), 0)]
[JsonDerivedType(typeof(SelfTrigger), 1)]
[JsonDerivedType(typeof(ListeningTrigger), 2)]
[JsonDerivedType(typeof(PhaseTrigger), 3)]
public abstract class Trigger //NOTE: This should probably just be called Trigger and the enum be called TriggerType
{
    public abstract string Name { get; set; }
    public List<Ability> Abilities { get; set; } = new();
    public ChoiceType ChoiceType { get; set; }
    public int ChoiceNumber { get; set; }
    public abstract bool SameAs(Trigger otherTrigger);
    public abstract string[]? GetEnums();
    public abstract string GetTriggerDescription();

    public static string CardNameVariable { get; set; } = "[cardname]";

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
    public override string Name { get; set; } = "Cast";
    public override bool SameAs(Trigger otherTrigger)
    {
        return GetType() == otherTrigger.GetType();
    }

    public override string[]? GetEnums() => ["None"];

    public override string GetTriggerDescription()
    {
        return $"When {CardNameVariable} is cast,";
    }
}

public enum SelfTriggerType
{
    Attack,
    MoveToBlockZone,
    BlockSwap,
    TakeDamage,
    Dies
}

public class SelfTrigger : Trigger
{
    public SelfTriggerType SelfTriggerType { get; set; }
    public override string Name { get; set; } = "Self";
    public override bool SameAs(Trigger otherTrigger)
    {
        return otherTrigger is SelfTrigger st &&
            st.SelfTriggerType == SelfTriggerType;
    }

    public override string[]? GetEnums()
    {
        return Enum.GetNames<SelfTriggerType>();
    }

    public override string GetTriggerDescription()
    {
        switch (SelfTriggerType)
        {
            case SelfTriggerType.Attack:
                return $"{CardNameVariable} attacks";
            case SelfTriggerType.MoveToBlockZone:
                return $"{CardNameVariable} moves to the block zone";
            case SelfTriggerType.BlockSwap:
                return $"{CardNameVariable} block swaps with another creature";
            case SelfTriggerType.TakeDamage:
                return $"{CardNameVariable} takes damage";
            case SelfTriggerType.Dies:
                return $"{CardNameVariable} dies";
        }

        return $"No description text for {SelfTriggerType}";
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
    public ListeningTriggerType ListeningTriggerType { get; set; }
    public override string Name { get; set; } = "Listening";
    public Side Side { get; set; }
    public override bool SameAs(Trigger otherTrigger)
    {
        return otherTrigger is ListeningTrigger st &&
            st.ListeningTriggerType == ListeningTriggerType;
    }

    public override string[]? GetEnums()
    {
        return Enum.GetNames<ListeningTriggerType>();
    }

    public override string GetTriggerDescription()
    {
        switch (ListeningTriggerType)
        {
            case ListeningTriggerType.CreatureDies:
            case ListeningTriggerType.CreatureAttacks:
            case ListeningTriggerType.CreatureTakesDamage:
            case ListeningTriggerType.CreatureMovesToBlockZone:
            {
                return GetListeningToCreatureActionDescription();
            }

            case ListeningTriggerType.DiscardCard:
            case ListeningTriggerType.SpellCast:
            case ListeningTriggerType.CreatureCast:
            case ListeningTriggerType.DrawCard:
            {
                return GetListeningToPlayerActionDescription();
            }
        }

        throw new NotImplementedException("Can't get listeningtrigger description");
    }

    private string GetListeningToCreatureActionDescription()
    {
        string triggerText = "";

        switch (ListeningTriggerType)
        {
            case ListeningTriggerType.CreatureDies:
                triggerText = "dies";
                break;

            case ListeningTriggerType.CreatureAttacks:
                triggerText = "attacks";
                break;

            case ListeningTriggerType.CreatureTakesDamage:
                triggerText = "takes damage";
                break;

            case ListeningTriggerType.CreatureMovesToBlockZone:
                triggerText = "enters the block zone";
                break;
        }

        switch (Side)
        {
            case Side.Any:
                return $"a creature {triggerText}";
            case Side.Opponent:
                return $"a creature your opponent controls {triggerText}";
            case Side.Player:
                return $"a creature you control {triggerText}";
        }

        throw new NotImplementedException("can't find listeningtriggertype description");
    }

    private string GetListeningToPlayerActionDescription()
    {
        string triggerText = "";
        string triggerTextFirstPerson = "";

        switch (ListeningTriggerType)
        {
            case ListeningTriggerType.SpellCast:
                triggerText = "casts a spell";
                triggerTextFirstPerson = "cast a spell";
                break;
            case ListeningTriggerType.CreatureCast:
                triggerText = "summons a creature";
                triggerTextFirstPerson = "summon a creature";
                break;

            case ListeningTriggerType.DrawCard:
                triggerText = "draws a card";
                triggerTextFirstPerson = "draw a card";
                break;
            case ListeningTriggerType.DiscardCard:
                triggerText = "discards a card";
                triggerTextFirstPerson = "discard a card";
                break;
        }

        switch (Side)
        {
            case Side.Any:
                return $"a player {triggerText}";
            case Side.Opponent:
                return $"your opponent {triggerText}";
            case Side.Player:
                return $"you {triggerTextFirstPerson}";
        }

        throw new NotImplementedException("can't find listeningtriggertype description");
    }
}

public enum PhaseTriggerType
{
    EndTurn,
    EndRound,
}

public class PhaseTrigger : Trigger
{
    public PhaseTriggerType PhaseTriggerType { get; set; }
    public override string Name { get; set; } = "Phase";
    public override bool SameAs(Trigger otherTrigger)
    {
        return otherTrigger is PhaseTrigger st &&
            st.PhaseTriggerType == PhaseTriggerType;
    }

    public override string[]? GetEnums()
    {
        return Enum.GetNames<PhaseTriggerType>();
    }

    public override string GetTriggerDescription()
    {
        if (PhaseTriggerType == PhaseTriggerType.EndTurn)
            return "the turn ends";
        if (PhaseTriggerType == PhaseTriggerType.EndRound)
            return "the round ends";

        throw new NotImplementedException("Can't get phasetrigger description");
    }
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

public class TargetSelection
{
    public List<int> TargetsToPickFrom { get; set; } = new();
    public List<int> SelectedTargets { get; set; } = new();
    public int CardId { get; set; }

    //NOTE: These are all in Ability. Should we just grab a copy of it?
    public int NumberToPick { get; set; }
    public int AbilityId { get; set; }
    public Effect Effect { get; set; }
}

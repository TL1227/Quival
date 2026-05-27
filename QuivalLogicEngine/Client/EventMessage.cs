using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Client
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(AttackEvent), "attackevent")]
    [JsonDerivedType(typeof(SummonEvent), "summonevent")]
    [JsonDerivedType(typeof(CreatureDeathEvent), "creaturedeathevent")]
    [JsonDerivedType(typeof(MoveToBlockZoneEvent), "movetoblockzoneenevent")]
    [JsonDerivedType(typeof(BlockSwapEvent), "blockswapevent")]
    [JsonDerivedType(typeof(BothPlayersOutOfMovesEvent), "BothPlayersOutOfMovesEvent")]
    [JsonDerivedType(typeof(NewRound), "NewRound")]
    [JsonDerivedType(typeof(NewTurn), "NewTurn")]
    public abstract class EventMessage
    {
        public abstract string GetString();
    }

    public class AttackEvent : EventMessage
    {
        public int PlayerId { get; set; }
        public int CreatureId { get; set; }
        public string  CreatureName { get; set; }

        public AttackEvent(int playerId, int creatureId, string creatureName)
        {
            PlayerId = playerId;
            CreatureId = creatureId;
            CreatureName = creatureName;
        }

        public override string GetString() 
        {
            return $"Player {PlayerId} attacks with {CreatureName}";
        }
    }
    public class SummonEvent : EventMessage
    {
        public int PlayerId { get; set; }
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }

        public SummonEvent(int playerId, int creatureId, string creatureName)
        {
            PlayerId = playerId;
            CreatureId = creatureId;
            CreatureName = creatureName;
        }

        public override string GetString() 
        {
            return $"Summoned {CreatureName}";
        }
    }

    public class CreatureDeathEvent : EventMessage
    {
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }

        public CreatureDeathEvent(int creatureId, string creatureName)
        {
            CreatureId = creatureId;
            CreatureName = creatureName;
        }

        public override string GetString() 
        {
            return $"{CreatureName} died!";
        }
    }

    public class MoveToBlockZoneEvent : EventMessage
    {
        public int PlayerId { get; set; }
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }

        public MoveToBlockZoneEvent(int playerId, int creatureId, string creatureName)
        {
            PlayerId = playerId;
            CreatureId = creatureId;
            CreatureName = creatureName;
        }

        public override string GetString() 
        {
            return $"{CreatureName} moved to the block zone";
        }
    }
    
    public class BlockSwapEvent : EventMessage
    {
        public int PlayerId { get; set; }
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }
        public int OldCreatureId { get; set; }
        public string OldCreatureName { get; set; }

        public BlockSwapEvent(int playerId, int creatureId, string creatureName, int oldCreatureId, string oldCreatureName)
        {
            PlayerId = playerId;
            CreatureId = creatureId;
            CreatureName = creatureName;
            OldCreatureId = oldCreatureId;
            OldCreatureName = oldCreatureName;
        }

        public override string GetString() 
        {
            return $"{CreatureName} replaced {OldCreatureName} in the block zone";
        }
    }
    public class BothPlayersOutOfMovesEvent : EventMessage
    {
        public override string GetString()
        {
            return $"Both players are out of moves!";
        }
    }

    public class NewRound : EventMessage
    {
        public int Round { get; set; }

        public NewRound(int round)
        {
            Round = round;
        }

        public override string GetString()
        {
            return $"Round {Round}";
        }
    }
    public class NewTurn : EventMessage
    {
        public int Turn { get; set; }
        public int Round { get; set; }

        public NewTurn(int turn, int round)
        {
            Turn = turn;
            Round = round;
        }

        public override string GetString()
        {
            return $"Turn {Turn} Round {Round}";
        }
    }
}

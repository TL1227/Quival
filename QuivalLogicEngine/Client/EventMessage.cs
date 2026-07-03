using QuivalLogicEngine.Cards;
using System.Text;
using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Client
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(AttackEvent), "attackevent")]
    [JsonDerivedType(typeof(SummonEvent), "summonevent")]
    [JsonDerivedType(typeof(CastEvent), "castevent")]
    [JsonDerivedType(typeof(CreatureDeathEvent), "creaturedeathevent")]
    [JsonDerivedType(typeof(MoveToBlockZoneEvent), "movetoblockzoneenevent")]
    [JsonDerivedType(typeof(BlockSwapEvent), "blockswapevent")]
    [JsonDerivedType(typeof(BothPlayersOutOfMovesEvent), "BothPlayersOutOfMovesEvent")]
    [JsonDerivedType(typeof(NewRound), "NewRound")]
    [JsonDerivedType(typeof(NewTurn), "NewTurn")]
    [JsonDerivedType(typeof(CardActionEvent), "CardActionEvent")]
    public abstract class EventMessage
    {
        public int PlayerId { get; set; }
        public abstract string GetString();

        public List<CardActionEvent> CardActionEvents { get; set; } = new();
    }

    public class AttackEvent : EventMessage
    {
        public int CreatureId { get; set; }
        public string  CreatureName { get; set; }
        public int BlockingCreatureId { get; set; }

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


    public class CastEvent : EventMessage
    {
        public SpellCard CastCard { get; set; }

        public CastEvent(int playerId, SpellCard castCard)
        {
            PlayerId = playerId;
            CastCard = castCard;
        }

        public override string GetString() 
        {
            return $"Cast {CastCard.Name}";
        }
    }

    public class MoveToBlockZoneEvent : EventMessage
    {
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


    public class CardActionEvent : EventMessage
    {
        public Card CardActionSource { get; set; }
        public Effect Effect { get; set; }
        public int Value { get; set; }
        public List<int> TargetsCardIds { get; set; } = new();

        public override string GetString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var target in TargetsCardIds)
            {
                switch (Effect)
                {
                    case Effect.None: 
                        break;
                    case Effect.AttackBuffRound:
                        sb.AppendLine($"Card {target} gets attack buff of {Value}"); 
                        break;
                    case Effect.DamageAbsorbToken:
                        break;
                    case Effect.DirectDamage:
                        sb.AppendLine($"Card {target} takes {Value} damage!"); 
                        break;
                    case Effect.DrawCard:
                        break;
                    case Effect.RestoreAction:
                        break;
                    default:
                        break;
                }
            }

            return (sb.Length > 0) ? sb.ToString() : "CardActionUnknown";
        }
    }

    //Events without player ID
    public class CreatureDeathEvent : EventMessage
    {
        public int CreatureId { get; set; }
        public string CreatureName { get; set; }

        public CreatureDeathEvent(int creatureId, string creatureName)
        {
            PlayerId = -1;
            CreatureId = creatureId;
            CreatureName = creatureName;
        }

        public override string GetString() 
        {
            return $"{CreatureName} died!";
        }
    }

    public class BothPlayersOutOfMovesEvent : EventMessage
    {
        public BothPlayersOutOfMovesEvent()
        {
            PlayerId = -1;
        }

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
            PlayerId = -1;
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
            PlayerId = -1;
        }

        public override string GetString()
        {
            return $"Turn {Turn} Round {Round}";
        }
    }
}

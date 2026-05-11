using System.Text.Json.Serialization;

namespace QuivalLogicEngine
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(Attack), "attack")]
    [JsonDerivedType(typeof(Summon), "summon")]
    [JsonDerivedType(typeof(Block), "block")]
    [JsonDerivedType(typeof(DamageMultiply), "damagemultiply")]
    [JsonDerivedType(typeof(DamagePlayer), "damageplayer")]
    public interface ICardIntent 
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
    }

    public class Attack : ICardIntent
    {
        public int PlayerId { get; set; } 
        public int CardId { get; set; }
        public Attack(int playerId, int cardId)
        {
            PlayerId = playerId;
            CardId = cardId;
        }
    }

    public class Block : ICardIntent
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
    }

    public class Summon : ICardIntent
    {
        public int CardId { get; set; } 
        public int PlayerId { get; set; } 

        public Summon() {}
        public Summon(int id) { CardId = id; }
    }

    public class DamageMultiply : ICardIntent //TODO: This could be a more generic MultiplyDamage intent
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
        public int Ammount { get; set; }
    }

    public class DamagePlayer : ICardIntent
    {
        public int CardId { get; set; } 
        public int PlayerId { get; set; } 
        public int Damage { get; set; }

        public DamagePlayer(int playerId, int damage)
        { 
            PlayerId = playerId; 
            Damage = damage;
        }
    }
}

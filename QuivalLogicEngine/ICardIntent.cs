using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public interface ICardIntent 
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
    }

    public class Attack : ICardIntent
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
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

        public Summon(int id) { CardId = id; }
    }

    public class DamageMultiply : ICardIntent //TODO: This could be a more generic MultiplyDamage intent
    {
        public int CardId { get; set; }
        public int PlayerId { get; set; } 
        public int Ammount { get; set; }
    }
}

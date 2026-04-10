using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public interface ICardIntent 
    {
        public int CardId { get; }
    }

    public record Attack(int CardId) : ICardIntent;
    public record Block(int CardId, int PlayerId, int SlotNumber) : ICardIntent;
    public record Summon(int CardId, int PlayerId, int SlotNumber) : ICardIntent;
    public record DoubleUp(int CardId, int SlotNumber) : ICardIntent;
}

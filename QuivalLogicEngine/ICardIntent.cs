using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public interface ICardIntent 
    {
        int PlayerId { get; }
        int SlotNumber { get; }
    }

    public record Attack(int PlayerId, int SlotNumber, int Target, string somethingElse) : ICardIntent;
    public record Summon(int PlayerId, int SlotNumber, int cardId) : ICardIntent;
}

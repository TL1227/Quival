using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public interface ICardIntent 
    {
        int PlayerId { get; set; }
        int SlotNumber { get; set; }
    }

    public record Attack(int PlayerId, int SlotNumber, int Target, string somethingElse) : ICardIntent;
    public record Summon(int PlayerId, int SlotNumber, int cardId) : ICardIntent;
}

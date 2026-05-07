using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    internal class BoardState
    {
        public List<int>[] CreatureIds { get; set; }
        public List<int>[] BlockingCreatureIds { get; set; }
        public List<List<CreatureCard>> SummonedCreatures { get; set; }

        public BoardState() 
        {
            CreatureIds = new List<int>[2];
            BlockingCreatureIds = new List<int>[2];
            SummonedCreatures = [ new List<CreatureCard>(5), new List<CreatureCard>(5) ];
        }

        public bool PlayerHasBlockingCreatures(int id)
        {
            return BlockingCreatureIds[id].Count > 0;
        }

        public void SetBlocker(int playerId, int cardId)
        {
            BlockingCreatureIds[playerId].Add(cardId);
        }

        public bool CreatureSlotFree(int playerId)
        {
            return SummonedCreatures[playerId].Count < 5;
        }

        public void SummonCreature(int playerId, CreatureCard creature)
        {
            SummonedCreatures[playerId].Add((creature));
            Console.WriteLine($"[EVENT]: player {playerId} summoned {creature.Name}");
        }
    }
}

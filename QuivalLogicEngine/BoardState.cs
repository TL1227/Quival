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

        public void SummonCreature(int playerId, int cardId)
        {
            //TODO: This needs to find card in players hand and move it to the battlefield
            SummonedCreatures[playerId].Add(new CreatureCard(cardId, 2, 4));
            Console.WriteLine($"[EVENT]: player {playerId} summoned {cardId}");
        }
    }
}

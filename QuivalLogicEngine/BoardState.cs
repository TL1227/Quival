using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine
{
    public class BoardState
    {
        public List<int>[] CreatureIds { get; set; }
        public List<int>[] BlockingCreatureIds { get; set; }
        public List<List<CreatureCard>> SummonedCreatures { get; set; }

        //public int[] ManaClock = [2, 3, 4, 5];
        public int[] ManaClock = [5, 5, 5, 5];
        public int ManaClockIndex = 0;

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
        }

        public int GetCurrentMana()
        {
            return ManaClock[ManaClockIndex];
        }

        public void IncreaseManaClock()
        {
            if (++ManaClockIndex >= ManaClock.Length)
                ManaClockIndex = ManaClock.Length - 1;
        }

        public void ResetSummonedCreaturesActions()
        {
            foreach (var creatures in SummonedCreatures)
                foreach(var creature in creatures)
                    creature.HasActed = false;
        }
    }
}

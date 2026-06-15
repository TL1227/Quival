using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine
{
    public class BoardState
    {
        public List<List<CreatureCard>> SummonedCreatures { get; set; }

        //public int[] ManaClock = [2, 3, 4, 5];
        public int[] ManaClock = [5, 5, 5, 5];
        public int ManaClockIndex = 0;

        public BoardState() 
        {
            SummonedCreatures = [ new List<CreatureCard>(5), new List<CreatureCard>(5) ];
        }

        public bool CreatureSlotFree(int playerId)
        {
            return SummonedCreatures[playerId].Count < 5;
        }

        public void SummonCreature(int playerId, CreatureCard creature)
        {
            SummonedCreatures[playerId].Add((creature));
        }

        public CreatureCard? GetSummonedCreature(int cardId)
        {
            foreach (var sc in SummonedCreatures)
                foreach (var s in sc)
                    if (s.Id == cardId)
                        return s;

            return null;
        }

        public List<CreatureCard> GetAllSummonedCreatures()
        {
            List<CreatureCard> list = new();
            list.AddRange(SummonedCreatures[0]);
            list.AddRange(SummonedCreatures[1]);

            return list;
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
    }
}

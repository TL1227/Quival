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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public class SpellStream
    {
        private Card? QuickSlotCard { get; set; }
        private Queue<Card> Stream { get; set; }
        private Card? SlowSlotCard { get; set; }

        public SpellStream()
        {
            Stream = new();
        }

        public int GetStreamCount()
        {
            return Stream.Count();
        }

    }
}

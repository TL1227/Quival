using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public enum MessageType
    {
        Null, //I'm using this when we need to pass around a "null" message
        OpeningHand,
        SpellStream
    }

    public class Message
    {
        public MessageType Type { get; set; }
        public List<ICard>? Cards { get; set; }
        public List<int>? CardIds { get; set; }
    }
}

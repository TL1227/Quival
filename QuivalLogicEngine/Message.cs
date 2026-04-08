using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public enum MessageType
    {
        Empty, //I'm using this when we need to pass around a "null" message
        OpeningHand,
        SpellStream
    }

    public class Message
    {
        public MessageType Type { get; set; }
        public List<Card>? Cards { get; set; }
        public SpellStream? SpellStream { get; set; }
    }
}

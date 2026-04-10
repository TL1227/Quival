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
        public List<Card>? Cards { get; set; }
<<<<<<< HEAD
=======
        public SpellStream? SpellStream { get; set; }
>>>>>>> a0127bc342dc66b9a4cdda07ba28fa54093a61be
    }
}

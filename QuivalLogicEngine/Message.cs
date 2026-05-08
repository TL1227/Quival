using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine.Messages
{
    public class Message
    {
        public string Type { get; set; }
        public int Version { get; } = 1;
        public Guid ClientGuid { get; set; }
        public Guid ServerGuid { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class Connect : Message
    {
        public Connect(Guid guid)
        {
            Type = "Connect";
            ClientGuid = guid;
        }
    }

    public class AcceptConnection : Message
    {
        public AcceptConnection(Guid guid)
        {
            Type = "AcceptConnection";
            ServerGuid = guid;
        }
    }

    public class HandUpdate : Message
    {
        List<ICard> Cards { get; set; }

        public HandUpdate(List<ICard> cards)
        {
            Type = "HandUpdate";
            Cards = cards;
        }
    }
}

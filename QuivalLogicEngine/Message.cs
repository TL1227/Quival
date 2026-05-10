using QuivalLogicEngine.Cards;
using System.Text.Json;

namespace QuivalLogicEngine.Messages
{
    public class Message
    {
        public string Type { get; set; }
        public int Version { get; } = 1;
        public Guid ClientGuid { get; set; }
        public Guid ServerGuid { get; set; }
        public DateTime TimeStamp { get; set; }

        public static Message? GetMessageFromJson(string json)
        {
            var parsed = JsonDocument.Parse(json);
            var type = parsed.RootElement.GetProperty("type").GetString();

            Message? message = type switch
            {
                "Connect" => JsonSerializer.Deserialize<ConnectionRequest>(parsed),
                "AcceptConnection" => JsonSerializer.Deserialize<AcceptConnection>(parsed),
                "HandUpdate" => JsonSerializer.Deserialize<HandUpdate>(parsed),
                _ => null
            };

            return message;
        }
    }

    public class ConnectionRequest : Message
    {
        public ConnectionRequest(Guid guid)
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
        public List<Card> Cards { get; set; }

        public HandUpdate(List<Card> cards)
        {
            Type = "HandUpdate";
            Cards = cards;
        }
    }

    public class PlayCard : Message
    {
        public Card CardToPlay { get; set; }

        public PlayCard(Card cardToPlay)
        {
            CardToPlay = cardToPlay;
        }
    }
}

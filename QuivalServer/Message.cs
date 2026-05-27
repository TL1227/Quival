using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using System.Text.Json;

namespace QuivalServer;

public abstract class Message
{
    public int Version { get; } = 1;
    public DateTime TimeStamp { get; } = DateTime.UtcNow;
    public abstract string Type { get; }
    public Guid ClientGuid { get; set; }
    public Guid ServerGuid { get; set; }

    public static Message? GetMessageFromJson(string json)
    {
        var parsed = JsonDocument.Parse(json);
        var type = parsed.RootElement.GetProperty("Type").GetString();

        try
        {
            Message? message = type switch
            {
                "Connect" => JsonSerializer.Deserialize<ConnectionRequest>(json),
                "HandUpdate" => JsonSerializer.Deserialize<HandUpdate>(json),
                "PlayCard" => JsonSerializer.Deserialize<PlayCard>(json),
                "PlayAttack" => JsonSerializer.Deserialize<PlayAttack>(json),
                "PlayBlock" => JsonSerializer.Deserialize<PlayBlock>(json),
                "PlayBlank" => JsonSerializer.Deserialize<PlayBlank>(json),
                "GameStateUpdate" => JsonSerializer.Deserialize<GameStateUpdate>(json),
                _ => null
            };

            return message;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }
}

public class ConnectionRequest : Message
{
    public override string Type => "Connect";
    public ConnectionRequest() { }
    public ConnectionRequest(Guid guid)
    {
        ClientGuid = guid;
    }
}

public class HandUpdate : Message
{
    public override string Type => "HandUpdate";
    public List<Card> Cards { get; set; }
    public HandUpdate() { }
    public HandUpdate(List<Card> cards)
    {
        Cards = cards;
    }
}

public class PlayCard : Message
{
    public override string Type => "PlayCard";
    public int CardId { get; set; }
    public PlayCard(int cardId)
    {
        CardId = cardId;
    }
}

public class PlayAttack : Message
{
    public override string Type => "PlayAttack";
    public int CardId { get; set; }
    public PlayAttack(int cardId)
    {
        CardId = cardId;
    }
}

public class PlayBlock : Message
{
    public override string Type => "PlayBlock";
    public int CardId { get; set; }
    public PlayBlock(int cardId)
    {
        CardId = cardId;
    }
}

public class PlayBlank : Message
{
    public override string Type => "PlayBlank";
}

public class GameStateUpdate : Message
{
    public override string Type => "GameStateUpdate";
    public required ClientGameState GameState { get; set; }
}

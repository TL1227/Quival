using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Turns;
using System.Text.Json;

namespace QuivalLogicEngine.Messages;

public abstract class Message
{
    public int Version { get; } = 1;
    public DateTime TimeStamp { get; } = DateTime.UtcNow;
    public abstract string Type { get; }
    public Guid ClientGuid { get; set; }
    public Guid ServerGuid { get; set; }
    public string ToJson() => JsonSerializer.Serialize(this, GetType());

    public static Message? GetMessageFromJson(string json)
    {
        var parsed = JsonDocument.Parse(json);
        var type = parsed.RootElement.GetProperty("Type").GetString();

        try
        {
            Message? message = type switch
            {
                "Connect" => JsonSerializer.Deserialize<ConnectionRequest>(json),
                "ConnectedToRoom" => JsonSerializer.Deserialize<ConnectedToRoom>(json),
                "HandUpdate" => JsonSerializer.Deserialize<HandUpdate>(json),
                "PlayCard" => JsonSerializer.Deserialize<PlayCard>(json),
                "PlayAttack" => JsonSerializer.Deserialize<PlayAttack>(json),
                "PlayBlock" => JsonSerializer.Deserialize<PlayBlock>(json),
                "PlayBlank" => JsonSerializer.Deserialize<PlayBlank>(json),
                "GameStateUpdate" => JsonSerializer.Deserialize<GameStateUpdate>(json),
                "SubmitTurn" => JsonSerializer.Deserialize<SubmitTurn>(json),
                "MakeSelections" => JsonSerializer.Deserialize<MakeSelections>(json),
                "CreateRoomRequest" => JsonSerializer.Deserialize<CreateRoomRequest>(json),
                "JoinRoomRequest" => JsonSerializer.Deserialize<JoinRoomRequest>(json),
                "JoinRoomResponse" => JsonSerializer.Deserialize<JoinRoomResponse>(json),
                "StartMatchRequest" => JsonSerializer.Deserialize<StartMatchRequest>(json),
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
    public List<string> DeckUniqueIds { get; set; } = new();
    public ConnectionRequest() { }
    public ConnectionRequest(Guid guid)
    {
        ClientGuid = guid;
    }
}

public class ConnectedToRoom : Message
{
    public override string Type => "ConnectedToRoom";
    public string RoomName { get; set; }
    public int RoomId { get; set; }
    public int PlayerId { get; set; }
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

public class SubmitTurn : Message
{
    public override string Type => "SubmitTurn";
    public required QuivalTurn Turn { get; set; }
}

public class MakeSelections : Message
{
    public override string Type => "MakeSelections";
    public required List<TargetSelection> TargetSelections { get; set; }
}

public class CreateRoomRequest : Message
{
    public override string Type => "CreateRoomRequest";
    public required string RoomName { get; set; }
    public bool Success { get; set; } = false;
}

public class JoinRoomRequest : Message
{
    public override string Type => "JoinRoomRequest";
    public List<string> CardIds { get; set; } = new();
    public bool JoinRandom { get; set; } = false;
}

public class JoinRoomResponse : Message
{
    public override string Type => "JoinRoomResponse";
    public bool Success { get; set; }
}

public class StartMatchRequest : Message
{
    public override string Type => "StartMatchRequest";
}

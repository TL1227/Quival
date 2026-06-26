using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QuivalServer;

internal class PlayerClient
{
    internal required TcpClient Client;
    internal Guid Guid;
    internal int Id;
    internal List<Card> Deck;
    internal required StreamReader Reader;
    internal required StreamWriter Writer;
    internal Room? CurrentRoom;
}

internal class Program
{
    static internal Version CurrentVersion { get; set; } = new Version(0, 1, 0);
    static internal int PortNumber = 5005;
    static internal int RoomCount = 0;
    static internal List<Room> Rooms = new();
    static internal List<PlayerClient> Players = new();

    static internal IDataAccessor DataAccessor { get; set; }

    static internal Guid ServerGuid;

    static internal bool UseDebugCards = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Quival Server Version {CurrentVersion}");

        ServerGuid = Guid.NewGuid();

        TcpListener listener = new(IPAddress.Any, PortNumber);
        listener.Start();
        Console.WriteLine($"Server running on machine {Environment.MachineName}");
        Console.WriteLine($"Server listening on port {PortNumber}");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
        }
    }


    static void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            StreamReader streamReader = new(stream, Encoding.UTF8);
            StreamWriter streamWriter = new(stream, Encoding.UTF8) { AutoFlush = true };

            string? initialMessage = streamReader.ReadLine();

            if (initialMessage == null)
                return;

            var doc = JsonDocument.Parse(initialMessage);

            if (doc == null)
                return;

            var type = doc.RootElement.GetProperty("Type").GetString();

            if (type != "Connect")
                return;

            ConnectionRequest connectionRequest = JsonSerializer.Deserialize<ConnectionRequest>(doc)!;

            PlayerClient playerClient = new()
            {
                Client = client,
                Guid = connectionRequest.ClientGuid,
                Reader = streamReader,
                Writer = streamWriter
            };

            Players.Add(playerClient);

            playerClient.Writer.WriteLine(connectionRequest.ToJson());

            string? message;
            while ((message = playerClient.Reader.ReadLine()) != null)
            {
                var parsedMessage = Message.GetMessageFromJson(message);

                if (parsedMessage != null)
                {
                    HandleMessage(parsedMessage, playerClient);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            client.Close();
            //Console.WriteLine($"Closed client connection: {client}");
        }
    }

    public static JoinRoomResponse JoinRandomRoom(PlayerClient playerClient, List<string> cardIds)
    {
        List<Card> deck = new();
        DataAccessor = new JsonDataAccess();
        foreach (var card in cardIds)
        {
            Card? c = DataAccessor.GetCard(card);
            if (c != null)
            {
                deck.Add(c);
            }
            else
            {
                //TODO: send a message back to client saying the deck is invalid
                return new JoinRoomResponse() { Success = false };
            }
        }

        if (!ValidateDeck(deck))
        {
            //TODO: send a message back to client saying the deck is invalid
            return new JoinRoomResponse() { Success = false };
        }

        //find a free room
        foreach (var room in Rooms)
        {
            bool success = room.AddPlayer(playerClient, deck);
            if (success)
            {
                playerClient.CurrentRoom = room;
                return new JoinRoomResponse() { Success = true };
            }
        }

        var newRoom = new Room() { Id = RoomCount++ };
        newRoom.Name = $"Room {newRoom.Id}";
        Rooms.Add(newRoom);
        newRoom.AddPlayer(playerClient, deck);
        playerClient.CurrentRoom = newRoom;

        return new JoinRoomResponse() { Success = true };
    }

    public static void SendRoomJoinResponse(PlayerClient playerClient)
    {
    }

    public static void HandleMessage(Message message, PlayerClient player)
    {
        if (player.CurrentRoom != null)
        {
            player.CurrentRoom.HandleMessage(message, player);
        }
        else
        {
            switch (message)
            {
                case CreateRoomRequest request:
                    //TODO: handle this one day
                    break;
                case JoinRoomRequest request:
                    if (request.JoinRandom == true)
                    {
                        var response = JoinRandomRoom(player, request.CardIds);
                        player.Writer.WriteLine(response.ToJson());
                    }
                    else
                    {
                        JoinRoomResponse response = new();
                        response.Success = false;
                        player.Writer.WriteLine(response.ToJson());
                    }
                    break;
                default:
                    break;
            }
        }
    }

    static bool ValidateDeck(List<Card> deck)
    {
        bool hasMoreThanFourCopies = deck
            .GroupBy(s => s.SetCode)
            .Any(g => g.Count() > 4);

        return hasMoreThanFourCopies;
    }
}

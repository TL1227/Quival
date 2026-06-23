using QuivalLogicEngine;
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
}

internal class Program
{
    internal static Version CurrentVersion { get; set; } = new Version(0, 1, 0);
    internal static int PortNumber = 5005;
    internal static int RoomCount = 0;
    internal static List<Room> Rooms = new();

    internal static IDataAccessor DataAccessor { get; set; }

    internal static Guid ServerGuid;

    internal static bool UseDebugCards = false;

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

            string? connectMessage = streamReader.ReadLine();

            if (connectMessage == null)
                return;

            var doc = JsonDocument.Parse(connectMessage);

            if (doc == null)
                return;

            var type = doc.RootElement.GetProperty("Type").GetString();

            if (type != "Connect")
                return;

            ConnectionRequest initialMessage = JsonSerializer.Deserialize<ConnectionRequest>(doc)!;


            PlayerClient playerClient = new()
            {
                Client = client,
                Guid = initialMessage.ClientGuid,
                Reader = streamReader,
                Writer = streamWriter
            };

            DataAccessor = new JsonDataAccess();
            List<Card> deck = new();
            foreach (var card in initialMessage.DeckUniqueIds)
            {
                Card? c = DataAccessor.GetCard(card);
                if (c != null)
                {
                    deck.Add(c);
                }
                else
                {
                    //TODO: send a message back to client saying the deck is invalid
                }
            }

            if (!ValidateDeck(deck))
            {
                //TODO: send a message back to client saying the deck is invalid
            }

            //create first room if none found
            if (Rooms.Count <= 0)
            {
                var newRoom = new Room() { Id = RoomCount++ };
                newRoom.Name = $"Room {newRoom.Id}";
                Rooms.Add(newRoom);
                newRoom.AddPlayer(playerClient, deck);
            }
            else 
            {
                //find a free room
                foreach (var room in Rooms)
                {
                    bool success = room.AddPlayer(playerClient, deck);
                    if (success) break;
                }

                //if no room found
                var newRoom = new Room() { Id = RoomCount++ };
                newRoom.Name = $"Room {newRoom.Id}";
                Rooms.Add(newRoom);
                newRoom.AddPlayer(playerClient, deck);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            client.Close();
            //Console.WriteLine($"Closed client connection: {client}");
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

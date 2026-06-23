using QuivalLogicEngine;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    internal static List<Card>[] Decks;
    internal static PlayerClient[] Clients;
    internal static int ClientCount = 0;
    internal static int RoomCount = 0;
    internal static List<Room> Rooms = new();

    internal static IDataAccessor DataAccessor { get; set; }

    internal static Guid ServerGuid;

    internal static Match Match;

    internal static bool UseDebugCards = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Quival Server Version {CurrentVersion}");

        ServerGuid = Guid.NewGuid();

        TcpListener listener = new(IPAddress.Any, PortNumber);
        listener.Start();
        Console.WriteLine($"Server running on machine {Environment.MachineName}");
        Console.WriteLine($"Server listening on port {PortNumber}");

        Match = new Match();
        Clients = new PlayerClient[2];

        Decks = [ new List<Card>(), new List<Card>() ];
        //InitDecks();

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
            if (Clients[0] != null && Clients[1] != null)
            {
                Console.WriteLine($"Someome tried to connect but the room's full :(");
                return;
            }

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
                Id = ClientCount,
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

            /*
            if (Clients[ClientCount] == null)
            {
                Match.SetPlayer(playerClient.Id, Decks[playerClient.Id]);

                //send connection message back with server guid populated
                initialMessage.ServerGuid = ServerGuid;
                string jsonMessage = JsonSerializer.Serialize(initialMessage);
                streamWriter.WriteLine(jsonMessage);

                Clients[ClientCount] = playerClient;
                Console.WriteLine($"Accepting Player {Clients[0].Id + 1}");
                ClientCount++;

                if (Clients[0] != null && Clients[1] != null)
                {
                    foreach (var cli in Clients)
                    {
                        if (client == null)
                            continue;

                        GameStateUpdate update = new()
                        {
                            GameState = Match.GetGameState(cli.Id)
                        };

                        string? gs = JsonSerializer.Serialize(update, update.GetType());
                        cli.Writer.WriteLineAsync(gs);
                    }
                }

                string? message;
                while ((message = streamReader.ReadLine()) != null)
                {
                    var parsedMessage = Message.GetMessageFromJson(message);

                    if (parsedMessage != null)
                    {
                        HandleMessageStatic(parsedMessage, playerClient.Id, streamWriter, );
                    }
                }
            }
            */

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            client.Close();
            //Console.WriteLine($"Closed client connection: {client}");
        }
    }

    static void InitDecks()
    {
        JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new JsonStringEnumConverter());

        if (UseDebugCards)
        {
            var json = File.ReadAllText("..\\..\\..\\..\\QuivalCardsDebug.json");
            Card card = JsonSerializer.Deserialize<Card>(json, options)!;

            for (int d = 0; d < 2; d++)
                for (int i = 0; i < 20; i++)
                {
                    if (card is CreatureCard cc)
                    {
                        var j = JsonSerializer.Serialize(cc);
                        Decks[d].Add(JsonSerializer.Deserialize<CreatureCard>(j)!);
                    }
                    else if (card is SpellCard sc)
                    {
                        var j = JsonSerializer.Serialize(sc);
                        Decks[d].Add(JsonSerializer.Deserialize<SpellCard>(j)!);
                    }
                }
        }
        else
        {
            var json = File.ReadAllText("..\\..\\..\\..\\QuivalCards.json");
            if (json == null)
            {
                Console.WriteLine("Can't find QuivalCards.json!");
                return;
            }

            Set currentSet = JsonSerializer.Deserialize<Set>(json, options)!;

            //add 4 copies of each card for each deck
            for (int d = 0; d < 2; d++)
                foreach (var card in currentSet.Cards)
                    for (int i = 0; i < 4; i++)
                    {
                        if (card is CreatureCard cc)
                        {
                            var j = JsonSerializer.Serialize(cc);
                            Decks[d].Add(JsonSerializer.Deserialize<CreatureCard>(j, options)!);
                        }
                        else if (card is SpellCard sc)
                        {
                            var j = JsonSerializer.Serialize(sc);
                            Decks[d].Add(JsonSerializer.Deserialize<SpellCard>(j, options)!);
                        }
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

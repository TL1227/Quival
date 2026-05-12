using QuivalLogicEngine;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;
using QuivalLogicEngine.Client;

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

    //internal static int PLAYER_1 = 0;
    internal static int PLAYER_2 = 1;

    internal static List<Card> TheDeck;

    internal static TcpClient? PlayerTwo;
    internal static Guid PlayerTwoGuid;
    internal static List<Card> TheDeck2;

    internal static List<Card>[] Decks;
    internal static PlayerClient[] Clients;
    internal static int ClientCount = 0;

    internal static Guid ServerGuid;

    internal static Match Match;

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Quival Server Version {CurrentVersion}");

        ServerGuid = Guid.NewGuid();

        TcpListener listener = new(IPAddress.Any, PortNumber);
        listener.Start();
        Console.WriteLine($"Server listening on port {PortNumber}");

        Match = new Match();
        Clients = new PlayerClient[2];

        Decks = new List<Card>[2];

        Decks[0] = [
            new CreatureCard(0, 1, 1, 3),
            new CreatureCard(0, 1, 1, 3),
            new CreatureCard(0, 2, 2, 3),
            new CreatureCard(0, 2, 3, 3),
            new CreatureCard(0, 3, 1, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 4, 2, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 2, 4, 3)
        ];

        Decks[1] = [
            new CreatureCard(0, 1, 1, 3),
            new CreatureCard(0, 1, 1, 3),
            new CreatureCard(0, 2, 2, 3),
            new CreatureCard(0, 2, 3, 3),
            new CreatureCard(0, 3, 1, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 4, 2, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 2, 4, 3),
            new CreatureCard(0, 2, 4, 3)
        ];

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

            if (Clients[ClientCount] == null)
            {

                //send connection message back with server guid populated
                initialMessage.ServerGuid = ServerGuid;
                string jsonMessage = JsonSerializer.Serialize(initialMessage);
                streamWriter.WriteLine(jsonMessage);

                Match.SetPlayer(playerClient.Id, Decks[playerClient.Id]);
                var hand = Match.GetPlayerHand(playerClient.Id);
                HandUpdate handupdate = new(hand);

                jsonMessage = JsonSerializer.Serialize(handupdate);

                streamWriter.WriteLine(jsonMessage);

                Clients[ClientCount] = playerClient;
                Console.WriteLine($"Accepting Player {Clients[0].Id + 1}");
                ClientCount++;
            }

            string? message;
            while ((message = streamReader.ReadLine()) != null)
            {
                var parsedMessage = Message.GetMessageFromJson(message);

                if (parsedMessage != null)
                {
                    HandleMessage(parsedMessage, playerClient.Id, streamWriter);
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

    static void HandleMessage(Message message, int playerId, StreamWriter writer)
    {
        Console.WriteLine($"Message from player {playerId}");

        switch (message)
        {
            case PlayCard:
                {
                    PlayCard playCard = (PlayCard)message;
                    Match.SetCardToPlay(playerId, playCard.CardId);

                    if (Match.BothCardsToPlayAreSet())
                    {
                        ProcessCards();
                    }
                }
                break;
            case PlayAttack:
                {
                    PlayAttack attack = (PlayAttack)message;
                    Match.SetCardToAttack(playerId, attack.CardId);

                    if (Match.BothCardsToPlayAreSet())
                    {
                        ProcessCards();
                    }
                }
                break;
            default:
                break;
        }
    }

    static void ProcessCards()
    {
        Match.ProcessCards();

        foreach (var client in Clients)
        {
            if (client == null)
                continue;

            GameStateUpdate update = new()
            {
                GameState = Match.GetGameState(client.Id)
            };

            string? gs = JsonSerializer.Serialize(update, update.GetType());
            client.Writer.WriteLineAsync(gs);
        }
    }
}

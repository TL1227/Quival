using QuivalLogicEngine;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QuivalServer;

internal class Program
{
    internal static Version CurrentVersion { get; set; } = new Version(0, 1, 0);
    internal static int PortNumber = 5005;

    internal static TcpClient? PlayerOne;
    internal static Guid PlayerOneGuid;
    internal static int PLAYER_1 = 0;
    internal static List<Card> TheDeck;

    internal static TcpClient? PlayerTwo;
    internal static Guid PlayerTwoGuid;
    internal static int PLAYER_2 = 1;
    internal static List<Card> TheDeck2;

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

        TheDeck =
        [
            new CreatureCard(0, 1, 1),
            new CreatureCard(0, 1, 1),
            new CreatureCard(0, 2, 2),
            new CreatureCard(0, 2, 3),
            new CreatureCard(0, 3, 1),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 4, 2),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 2, 4)
        ];

        TheDeck2 =
        [
            new CreatureCard(0, 1, 1),
            new CreatureCard(0, 1, 1),
            new CreatureCard(0, 2, 2),
            new CreatureCard(0, 2, 3),
            new CreatureCard(0, 3, 1),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 4, 2),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 2, 4),
            new CreatureCard(0, 2, 4)
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
            NetworkStream steam = client.GetStream();
            StreamReader streamReader = new(steam, Encoding.UTF8);
            StreamWriter streamWriter = new(steam, Encoding.UTF8) { AutoFlush = true };

            string? connectMessage = streamReader.ReadLine();

            var doc = JsonDocument.Parse(connectMessage);
            var type = doc.RootElement.GetProperty("type").GetString();

            if (type != "Connect")
                return;

            if (doc == null)
                return;

            ConnectionRequest initialMessage = JsonSerializer.Deserialize<ConnectionRequest>(doc)!;

            int playerId = -1;
            if (connectMessage != null)
            {
                if (PlayerOne == null)
                {
                    PlayerOne = client;
                    playerId = PLAYER_1;
                    PlayerOneGuid = initialMessage.ClientGuid;

                    AcceptConnection acceptConnection = new(ServerGuid);
                    string jsonMessage = JsonSerializer.Serialize(acceptConnection);
                    streamWriter.WriteLine(jsonMessage);
                    Console.WriteLine("Accepting Player 1");

                    Match.SetPlayer(PLAYER_1, TheDeck);
                    var hand = Match.GetPlayerHand(PLAYER_1);
                    HandUpdate handupdate = new(hand);

                    jsonMessage = JsonSerializer.Serialize(handupdate);

                    streamWriter.WriteLine(jsonMessage);
                }
                else if (PlayerTwo == null)
                {
                    PlayerTwo = client;
                    playerId = PLAYER_2;
                    PlayerTwoGuid = initialMessage.ClientGuid;

                    AcceptConnection acceptConnection = new(ServerGuid);
                    string jsonMessage = JsonSerializer.Serialize(acceptConnection);
                    streamWriter.WriteLine(jsonMessage);
                    Console.WriteLine("Accepting Player 2");

                    Match.SetPlayer(PLAYER_2, TheDeck2);
                    var hand = Match.GetPlayerHand(PLAYER_2);
                    HandUpdate handupdate = new(hand);

                    jsonMessage = JsonSerializer.Serialize(handupdate);

                    streamWriter.WriteLine(jsonMessage);
                }
                else
                {
                    Console.WriteLine($"Someome tried to connect but the room's full :(");
                }

                string? message;
                while ((message = streamReader.ReadLine()) != null)
                {
                    var parsedMessage = Message.GetMessageFromJson(message);

                    if (parsedMessage != null)
                    {
                        HandleMessage(parsedMessage, playerId);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            client.Close();
            Console.WriteLine($"Closed client connection: {client}");
        }
    }

    static void HandleMessage(Message message, int playerId)
    {
        Console.WriteLine($"Message from player {playerId}");

            switch (message)
            {
                case AcceptConnection:
                    {
                    }
                    break;
                case ConnectionRequest:
                    {
                    }
                    break;
                case HandUpdate:
                    {
                        //HandUpdate handUpdate = (HandUpdate)message;
                        //Window.UpdateHand(handUpdate.Cards);
                    }
                    break;
                default:
                    break;
            }
        }
    }

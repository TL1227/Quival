using System.Net;
using System.Net.Sockets;
using System.Text;
using QuivalLogicEngine;

using System.Text.Json;

namespace QuivalServer;

internal class Program
{
    internal static Version CurrentVersion { get; set; } = new Version(0, 1, 0);
    internal static int PortNumber = 5005;
    internal static TcpClient? PlayerOne;
    internal static int Player1ID = 1;
    internal static TcpClient? PlayerTwo;
    internal static int Player2ID = 2;
    internal static Match Match;

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Quival Server Version {CurrentVersion}");

        TcpListener listener = new(IPAddress.Any, PortNumber);
        listener.Start();
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
            NetworkStream steam = client.GetStream();
            StreamReader streamReader = new(steam, Encoding.UTF8);
            StreamWriter streamWriter = new(steam, Encoding.UTF8) { AutoFlush = true };

            string? connectMessage = streamReader.ReadLine();
            int playerId = -1;
            if (connectMessage != null)
            {
                if (PlayerOne == null)
                {
                    PlayerOne = client;
                    playerId = 0;
                    Console.WriteLine($"Welcome player 1!");
                    streamWriter.WriteLine($"Welcome player 1!");

                    Message openingHandMessage = new();
                    openingHandMessage.Type = MessageType.OpeningHand;

                    openingHandMessage.Cards = new List<Card>()
                    {
                        //hardcode for now
                        new Card(CardType.Creature, 1, 1),
                        new Card(CardType.Creature, 1, 2),
                        new Card(CardType.Creature, 1, 2),
                        new Card(CardType.Creature, 2, 1),
                        new Card(CardType.Creature, 3, 1),
                        new Card(CardType.Creature, 3, 2),
                        new Card(CardType.Creature, 4, 4),
                    };

                    string jsonMessage = JsonSerializer.Serialize(openingHandMessage);

                    streamWriter.WriteLine(jsonMessage);
                }
                else if (PlayerTwo == null)
                {
                    PlayerTwo = client;
                    playerId = 1;
                    Console.WriteLine($"Welcome player 2!");

                    streamWriter.WriteLine($"Welcome player 2!");

                    Match = new();

                    Message openingHandMessage = new();
                    openingHandMessage.Type = MessageType.OpeningHand;

                    openingHandMessage.Cards = new List<Card>()
                    {
                        //hardcode for now
                        new Card(CardType.Creature, 1, 1),
                        new Card(CardType.Creature, 1, 2),
                        new Card(CardType.Creature, 1, 2),
                        new Card(CardType.Creature, 2, 1),
                        new Card(CardType.Creature, 3, 1),
                        new Card(CardType.Creature, 3, 2),
                        new Card(CardType.Creature, 4, 4),
                    };

                    string jsonMessage = JsonSerializer.Serialize(openingHandMessage);

                    streamWriter.WriteLine(jsonMessage);
                }
                else
                {
                    Console.WriteLine($"Someome tried to connect but the room's full :(");
                }

                string? message;
                while ((message = streamReader.ReadLine()) != null)
                {
                    Message? result = JsonSerializer.Deserialize<Message>(message);
                    if (result != null)
                    {
                        HandleMessage(result, playerId);
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

        switch (message.Type)
        {
            case MessageType.OpeningHand:
                {
                    //Shouldn't get this type of message probably
                }
                break;
            case MessageType.SpellStream:
                {
                    if (message.SpellStream != null)
                    {
                        if (playerId == 0 || playerId == 1)
                        {
                            Match.SetSpellStream(message.SpellStream, playerId);

                            if (Match.BothStreamsAreSet())
                            {
                                Match.ProcessSpellStreams();
                            }
                        }
                        else
                        {
                            //handle playerId error
                        }
                    }
                }
                break;
            default:
            case MessageType.Empty:
                break;
        }
    }
}

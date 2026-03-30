using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QuivalServer
{
    internal class Program
    {
        internal static Version CurrentVersion { get; set; }
        internal static int PortNumber = 5005;
        internal static TcpClient? PlayerOne;
        internal static TcpClient? PlayerTwo;

        static async Task Main(string[] args)
        {
            CurrentVersion = new Version(0,1,0);
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
                StreamWriter streamWriter = new(steam, Encoding.UTF8);

                string? connectMessage = streamReader.ReadLine();

                if (connectMessage != null)
                {
                    if (PlayerOne == null)
                    {
                        PlayerOne = client;
                        Console.WriteLine($"Welcome player 1!");
                    }
                    else if (PlayerTwo == null)
                    {
                        PlayerTwo = client;
                        Console.WriteLine($"Welcome player 2!");
                    }
                    else
                    {
                        Console.WriteLine($"Someome tried to connect but the room's full :(");
                    }

                    Console.WriteLine($"New client connected with: {connectMessage}");

                    string? message;
                    while ((message = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine($"{message}");
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
    }
}

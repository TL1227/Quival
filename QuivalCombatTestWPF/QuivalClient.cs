using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Windows;

using System.Text.Json;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;

namespace QuivalCombatTestWPF
{
    class QuivalClient
    {
        private StreamWriter? Writer;
        private StreamReader? Reader;
        private MainWindow Window;
        private Guid Clientguid;
        private Guid Serverguid;

        public QuivalClient(MainWindow window)
        {
            Window = window;
            Clientguid = Guid.NewGuid();
        }

        public async Task<bool> ConnectToServer()
        {
            try
            {
                TcpClient Client = new TcpClient(Environment.MachineName, 5005);
                var Stream = Client.GetStream();
                Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
                Reader = new StreamReader(Stream, Encoding.UTF8);

                ConnectionRequest connectionRequest = new(Clientguid);

                string req = JsonSerializer.Serialize<ConnectionRequest>(connectionRequest);

                Writer.WriteLine(req);

                string? response = Reader.ReadLine();

                if (response == null)
                    return false;

                var message = Message.GetMessageFromJson(response);

                if (response != null && message is AcceptConnection accept)
                {
                    Serverguid = accept.ServerGuid;
                    _ = RecieveMessages();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendString(string message)
        {
            if (Writer == null) 
                return;

            Writer.WriteLine(message);
        }

        public void SendMessage(Message message)
        {
            if (Writer == null || message == null) 
                return;

            string? toSend = MessageToJson(message);

            Writer.WriteLine(toSend);
        }

        public async Task RecieveMessages() 
        {
            if (Reader == null)
                return;

            string? message;
            while ((message = await Reader.ReadLineAsync()) != null)
            {
                Message? recieved = Message.GetMessageFromJson(message);

                if (recieved != null)
                {
                    await HandleMessage(recieved);
                }
            }
        }

        private async Task HandleMessage(Message message)
        {
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
                        HandUpdate handUpdate = (HandUpdate)message;
                        Window.UpdateHand(handUpdate.Cards);
                    }
                    break;
                default:
                    break;
            }
        }

        //TODO: maybe this goes in a Mapper class
        private List<BoardCard> CardToBoardCard(List<Card> cards)
        {
            List<BoardCard> result = new();

            try
            {
                foreach (Card card in cards)
                {
                    if (card is CreatureCard creature)
                    {
                        BoardCard bc = new(0, creature.Attack, creature.Health);
                        result.Add(bc);
                    }
                    else
                    {
                        BoardCard bc = new(0, 0, 0);
                        result.Add(bc);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        private static string? MessageToJson(Message message)
        {
            if (message != null)
            {
                try
                {
                    return JsonSerializer.Serialize(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return null;
        }

        public void SubmitCard(Card card)
        {

        }
    }
}

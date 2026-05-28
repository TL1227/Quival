using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using QuivalServer;

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

                if (response != null && message is ConnectionRequest accept)
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

        public async Task SendMessageAsync(Message message)
        {
            if (Writer == null || message == null) 
                return;

            string? toSend = MessageToJson(message);

            await Writer.WriteLineAsync(toSend);

            Window.MessageSent();
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
                case GameStateUpdate:
                    {
                        GameStateUpdate gameState = (GameStateUpdate)message;
                        Window.UpdateGameState(gameState.GameState);
                    }
                    break;
                default:
                    break;
            }
        }

        private static string? MessageToJson(Message message)
        {
            if (message != null)
            {
                try
                {
                    string str = message.GetType().ToString();
                    return JsonSerializer.Serialize(message, message.GetType());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return null;
        }

        public void PlayCard(int cardId)
        {
            PlayCard playCard = new(cardId);
            _ = SendMessageAsync(playCard);
        }

        public void PlayAttack(int cardId)
        {
            PlayAttack attackCard = new(cardId);
            _ = SendMessageAsync(attackCard);
        }

        public void PlayBlock(int cardId)
        {
            PlayBlock blockCard = new(cardId);
            _ = SendMessageAsync(blockCard);
        }
        public void PlayBlank()
        {
            PlayBlank blankCard = new();
            _ = SendMessageAsync(blankCard);
        }
    }
}

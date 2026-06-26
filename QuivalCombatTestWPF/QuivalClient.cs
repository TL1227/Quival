using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using QuivalLogicEngine.Messages;
using QuivalLogicEngine.Turns;
using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    public class QuivalClient
    {
        private StreamWriter? Writer;
        private StreamReader? Reader;
        private Guid Clientguid;
        private Guid Serverguid;
        private MainWindow MainWindow;

        public QuivalClient(MainWindow window)
        {
            MainWindow = window;
            Clientguid = Guid.NewGuid();
            ConnectToServer();
        }

        public async Task<bool> ConnectToServer()
        {
            var machineName = Environment.MachineName;

            var machineNameFile = $"{AppContext.BaseDirectory}\\..\\MachineName.txt";
            if (File.Exists(machineNameFile))
            {
                machineName = File.ReadAllText(machineNameFile);
            }

            try
            {
                TcpClient Client = new TcpClient(machineName, 5005);
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
                case HandUpdate handUpdate:
                    {
                        if (MainWindow.ViewContent.Content is MatchView matchView)
                            matchView?.UpdateHand(handUpdate.Cards);
                    }
                    break;
                case GameStateUpdate gameState:
                    {
                        if (MainWindow.ViewContent.Content is MatchView matchView)
                            matchView.UpdateGameState(gameState.GameState);
                    }
                    break;
                case MakeSelections makeSelections:
                    {
                        if (MainWindow.ViewContent.Content is MatchView matchView)
                            matchView.MakeSelections(makeSelections);
                    }
                    break;
                case JoinRoomResponse joinRoomResponse:
                    {
                        if (MainWindow.ViewContent.Content is MainMenu mainMenu)
                            mainMenu.ProcessJoinRoomResponce(joinRoomResponse);
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

        public void SubmitTurn(QuivalTurn turn)
        {
            SubmitTurn submitTurn = new(){Turn = turn};
            _ = SendMessageAsync(submitTurn);
        }

        public void SubmitSelection(List<TargetSelection> targetSelection)
        {
            MakeSelections ms = new(){TargetSelections = targetSelection};
            _ = SendMessageAsync(ms);
        }

        public static bool SubmitRoomCreationRequest(string name)
        {
            //TODO: this all needs changing 

            /*
            try
            {
                TcpClient Client = new TcpClient(Environment.MachineName, 5005);
                var Stream = Client.GetStream();
                StreamWriter writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
                StreamReader reader = new StreamReader(Stream, Encoding.UTF8);

                CreateRoomRequest request = new()
                {
                    RoomName = name
                };

                string requestJson = JsonSerializer.Serialize(request);
                writer.WriteLine(requestJson);
                string? result = reader.ReadLine();

                if (result != null)
                {
                    Message? recieved = Message.GetMessageFromJson(result);
                    if (recieved is CreateRoomRequest response)
                    {
                        return response.Success;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            */

            return false;
        }
    }
}

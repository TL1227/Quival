using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using QuivalLogicEngine.Messages;
using QuivalLogicEngine.Turns;
using QuivalLogicEngine.Cards;
using System.Configuration;
using System.Diagnostics;

namespace QuivalCombatTestWPF
{
    class QuivalClient
    {
        private StreamWriter? Writer;
        private StreamReader? Reader;
        private MatchView MatchView;
        private Guid Clientguid;
        private Guid Serverguid;

        public QuivalClient(MatchView matchView)
        {
            MatchView = matchView;
            Clientguid = Guid.NewGuid();
        }

        public async Task<bool> ConnectToServer(List<string> deckIds)
        {
            try
            {
                TcpClient Client = new TcpClient(Environment.MachineName, 5005);
                var Stream = Client.GetStream();
                Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
                Reader = new StreamReader(Stream, Encoding.UTF8);

                ConnectionRequest connectionRequest = new(Clientguid);
                connectionRequest.DeckUniqueIds = deckIds;

                string req = JsonSerializer.Serialize<ConnectionRequest>(connectionRequest);

                Writer.WriteLine(req);

                string? response = Reader.ReadLine();

                if (response == null)
                    return false;

                var message = Message.GetMessageFromJson(response);

                if (response != null && message is ConnectedToRoom accept)
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

            MatchView.MessageSent();
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
                case HandUpdate:
                    {
                        HandUpdate handUpdate = (HandUpdate)message;
                        MatchView.UpdateHand(handUpdate.Cards);
                    }
                    break;
                case GameStateUpdate:
                    {
                        GameStateUpdate gameState = (GameStateUpdate)message;
                        MatchView.UpdateGameState(gameState.GameState);
                    }
                    break;
                case MakeSelections:
                    {
                        MakeSelections makeSelections = (MakeSelections)message;
                        MatchView.MakeSelections(makeSelections);
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

                writer.WriteLine(request);
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

            return false;
        }
    }
}

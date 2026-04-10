using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows;

using System.Text.Json;
using QuivalLogicEngine;
using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    class QuivalClient
    {
        private StreamWriter? Writer;
        private StreamReader? Reader;
        private MainWindow Window;

        public QuivalClient(MainWindow window)
        {
            Window = window;
        }

        public async Task<bool> ConnectToServer()
        {
            try
            {
                TcpClient Client = new TcpClient(Environment.MachineName, 5005);
                var Stream = Client.GetStream();
                Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
                Reader = new StreamReader(Stream, Encoding.UTF8);

                Writer.WriteLine("This is a connection test!");
                string? response = Reader.ReadLine();

                if (response != null)
                {
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
            if (Writer == null) 
                return;

            string toSend = MessageToJson(message);

            Writer.WriteLine(toSend);
        }

        public async Task RecieveMessages() 
        {
            if (Reader == null)
                return;

            string? message;
            while ((message =  await Reader.ReadLineAsync()) != null)
            {
                await HandleMessage(GetMessage(message.ToString()));
            }
        }

        private async Task HandleMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.OpeningHand:
                    {
                        if (message.Cards != null)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() => 
                            {
                                var hand = CardToBoardCard(message.Cards);
                                Window.UpdateHand(hand);
                            });
                        }
                    }
                    break;
                case MessageType.Null: 
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
                    BoardCard bc = new(card.Attack, card.Defence);
                    result.Add(bc);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        //TODO: put into some sort of translator class
        private static Message GetMessage(string message)
        {
            if (message != null)
            {
                try
                {
                    var value = JsonSerializer.Deserialize<Message>(message) ?? new Message() { Type = MessageType.Null };
                    return value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //return JsonSerializer.Deserialize<Message>(message) ?? new Message() { Type = MessageType.Empty };
            }

            return new Message() { Type = MessageType.Null };
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
    }
}

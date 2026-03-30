using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace QuivalCombatTestWPF
{
    class QuivalClient
    {
        private StreamWriter Writer;
        private StreamReader Reader;

        public bool ConnectToServer()
        {
            try
            {
                TcpClient Client = new TcpClient(Environment.MachineName, 5005);
                var Stream = Client.GetStream();
                Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
                Reader = new StreamReader(Stream, Encoding.UTF8);

                Writer.WriteLine("This is a connection test!");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void WriteMessage(string message)
        {
            Writer.WriteLine(message);
        }
    }
}

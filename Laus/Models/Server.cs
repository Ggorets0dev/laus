using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Net.Http;

namespace Laus.Models
{
    internal class Server
    {
        private TcpListener _listener;

        public Server(IPAddress ipAddress, int port = 8888)
        {
            _listener = new TcpListener(ipAddress, port);
        }

        public async Task ListenAsync()
        {
            _listener.Start();

            while (true) 
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Task.Run(async () => await ProcessClientAsync(tcpClient));
            }
        }

        async Task ProcessClientAsync(TcpClient tcpClient)
        {
            // Console.WriteLine("Connected");

            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            int bytesRead;

            do
            {
                bytesRead = stream.ReadByte();
                response.Add((byte)bytesRead);
            }
            while ((char)bytesRead != Message.Termination);

            //Console.WriteLine("Length is " + Encoding.UTF8.GetString(response.ToArray()).Length);
            //Console.WriteLine(Encoding.UTF8.GetString(response.ToArray()));

            string readMessage = Encoding.UTF8.GetString(response.ToArray());

            var message = new Message(readMessage);

            // Console.WriteLine("Data is - " + message.Data);  

            if (message.CommandCode == TcpCommandCodes.CheckUser)
            {
                string alias = Config.Get().Alias;
                var approveMessage = new Message(TcpCommandCodes.ApproveUser, alias);
                var bytesMessage = Encoding.UTF8.GetBytes(approveMessage.ToString());

                Array.Reverse(bytesMessage);

                await stream.WriteAsync(bytesMessage, 0, bytesMessage.Length);
            }

            response.Clear();
            tcpClient.Close();
        }

        ~Server()
        {
            _listener.Stop();
        }
    }
}

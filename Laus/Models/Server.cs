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
    public class Server
    {
        private TcpListener _listener;

        public Server(IPAddress ipAddress, int port)
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
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            int bytesRead;

            while ((bytesRead = stream.ReadByte()) != '\n')
                response.Add((byte)bytesRead);

            var message = new Message(response);

            if (message.CommandCode == (int)TcpCommandCodes.CheckUser)
            {
                byte[] intBytes = BitConverter.GetBytes((int)TcpCommandCodes.ApproveUser);
                Array.Reverse(intBytes);
                await stream.WriteAsync(intBytes, 0, intBytes.Length);
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

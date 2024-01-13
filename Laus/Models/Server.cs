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
using WinHardwareSpecs;

namespace Laus.Models
{
    /// <summary>
    /// Сервер для обработки запросов P2P сети
    /// </summary>
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
            var stream = tcpClient.GetStream();
            var response = new List<byte>();
            int bytesRead;

            do
            {
                bytesRead = stream.ReadByte();
                response.Add((byte)bytesRead);
            }
            while ((char)bytesRead != Message.Termination);

            string readMessage = Encoding.UTF8.GetString(response.ToArray());
            var message = new Message(readMessage);

            if (message.CommandCode == LanCommandCodes.CheckUser)
            {
                string alias = Config.Get().Alias;
                var bytesMessage = new Message(LanCommandCodes.ApproveUser, alias).ToBytes();

                Array.Reverse(bytesMessage);

                await stream.WriteAsync(bytesMessage, 0, bytesMessage.Length);
            }

            else if (message.CommandCode == LanCommandCodes.RequestSpecs)
            {
                var specs = SpecMonitor.GetSpecification();
                string specsJson = specs.ToJson();

                var bytesMessage =  new Message(LanCommandCodes.ResponseSpecs, specsJson).ToBytes();

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

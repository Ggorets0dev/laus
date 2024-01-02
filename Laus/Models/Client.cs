using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace Laus.Models
{
    internal class Client: IDisposable
    {
        private bool disposed = false;

        private Ping _ping = new Ping();
        private TcpClient _client;

        public Client() => _client = new TcpClient();
        public Client(string ipAddress, int port) => _client = new TcpClient(ipAddress, port);

        public bool Ping(string pingAddress, ushort msTimeout = 100)
        {
            try
            {
                var pingReply = _ping.Send(pingAddress, msTimeout);

                if (pingReply == null || pingReply.Status != IPStatus.Success)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                // -
            }

            _client.Close();
            _client.Dispose();
            _ping.Dispose();

            disposed = true;
        }

        ~Client() => Dispose(false);
    }
}

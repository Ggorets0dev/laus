﻿using System;
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

        private TcpClient _client;
        private string _ipAddress;
        private int _port;

        public Client()
        {
            _ipAddress = "127.0.0.1";
            _port = 8888;
            _client = new TcpClient(_ipAddress, _port);
        }
        public Client(string ipAddress, int port = 8888)
        {
            _client = new TcpClient(ipAddress, port);
            _ipAddress = ipAddress;
            _port = port;
        }

        public (bool isApproved, string alias) CheckUser()
        {
            var stream = _client.GetStream();

            var checkMessage = new Message(TcpCommandCodes.CheckUser);
            var checkBytes = Encoding.UTF8.GetBytes(checkMessage.ToString());
            stream.Write(checkBytes, 0, checkBytes.Length);

            var readBuffer = new byte[Message.MaxSize];
            int bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);

            stream.Close();

            string cleanMessage = Message.ProccessRawBytes(readBuffer);
            var readMessage = new Message(cleanMessage);

            return (readMessage.CommandCode == TcpCommandCodes.ApproveUser, readMessage.Data);
        }

        static public bool Ping(string ipAddress, ushort msTimeout = 100)
        {
            try
            {
                var pingReply = new Ping().Send(ipAddress, msTimeout);

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

            if (_client != null)
            {
                _client.Close();
                _client.Dispose();
            }

            disposed = true;
        }

        ~Client() => Dispose(false);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus.Models
{
    internal class Message
    {
        static private char _separator = '@';

        public TcpCommandCodes CommandCode { get; set; }
        public string Data { get; set; }

        public Message(List<byte> bytesRead) 
        {
            string msg = Encoding.UTF8.GetString(bytesRead.ToArray());

            CommandCode = (TcpCommandCodes)int.Parse(msg.Substring(0, msg.IndexOf(_separator)));
            Data = msg.Substring(msg.IndexOf(_separator) + 1, msg.Length - 1);
        }
    }
}

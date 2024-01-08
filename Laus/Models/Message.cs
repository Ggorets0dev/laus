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
        static private char _termination = ';'; 

        static public char Termination => _termination;

        public TcpCommandCodes CommandCode { get; set; }
        public string Data { get; set; }

        public Message(byte[] bytesRead)
        {
            string msg = Encoding.UTF8.GetString(bytesRead);

            CommandCode = (TcpCommandCodes)int.Parse(msg.Substring(0, msg.IndexOf(_separator)));
            Data = msg.Substring(msg.IndexOf(_separator) + 1, msg.Length - 3);
        }

        public Message(TcpCommandCodes commandCode, string data)
        {
            CommandCode = commandCode;
            Data = data;
        }

        public override string ToString() => $"{(int)CommandCode}{_separator}{Data}{_termination}";
    }
}

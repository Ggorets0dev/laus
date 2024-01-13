using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Laus.Models
{
    /// <summary>
    /// Сообщение между клиентом и сервером
    /// </summary>
    internal class Message
    {
        static private readonly ushort _maxSize = 2048;
        
        static private readonly char _separator = '@';
        static private readonly char _termination = ';'; 

        static public char Termination => _termination;
        static public ushort MaxSize => _maxSize;

        public LanCommandCodes CommandCode { get; set; }
        public string Data { get; set; }

        public Message(string msg)
        {
            CommandCode = (LanCommandCodes)int.Parse(msg.Substring(0, msg.IndexOf(_separator)));
            Data = msg.Substring(msg.IndexOf(_separator) + 1, msg.Length - 3);
        }

        public Message(LanCommandCodes commandCode, string data = "NULL")
        {
            CommandCode = commandCode;
            Data = data;
        }

        static public string ProccessRawBytes(byte[] bytesRead)
        {
            Array.Reverse(bytesRead);

            string rawString = Encoding.UTF8.GetString(bytesRead);
            string cleanString = new string(rawString.Where(symb => (int)symb != 0).ToArray());

            return cleanString;
        }

        public override string ToString() => $"{(int)CommandCode}{_separator}{Data}{_termination}";

        public byte[] ToBytes() => Encoding.UTF8.GetBytes(ToString());
    }
}

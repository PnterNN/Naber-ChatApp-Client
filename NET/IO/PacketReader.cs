using System.Net.Sockets;
using System.Text;
using System.IO;
using System;

namespace JavaProject___Server.NET.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);

            var msg = Encoding.UTF8.GetString(msgBuffer);
            return msg;
        }

        public byte[] ReadAudioMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            var opa = _ns.Read(msgBuffer, 0, length);
            byte[] copiedBuffer = new byte[opa];
            Array.Copy(msgBuffer, 0, copiedBuffer, 0, opa);
            return copiedBuffer;
        }

    }
}

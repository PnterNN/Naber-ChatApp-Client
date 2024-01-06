using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaProject___Server.NET.IO
{
    public class PacketBuilder
    {
        MemoryStream _ms;
        private object locker = new object();
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            lock (locker)
            {
                _ms.WriteByte(opcode);
            }
        }
        public void WriteMessage(string msg)
        {

            lock (locker)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
                byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);
                _ms.Write(lengthBytes, 0, lengthBytes.Length);
                _ms.Write(messageBytes, 0, messageBytes.Length);
            }
            //byte[] buff = BitConverter.GetBytes(msg.Length);
            //_ms.Write(buff, 0, buff.Length);
            //_ms.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
        }


        public byte[] GetPacketBytes()
        {
            lock (locker)
            {
                var result = _ms.ToArray();

                Task.Run(() =>
                {
                    Clear(_ms);
                });

                return result;
            }
        }
        public void Clear(MemoryStream source)
        {
            lock (locker)
            {
                byte[] buffer = source.GetBuffer();
                Array.Clear(buffer, 0, buffer.Length);
                source.Position = 0;
                source.SetLength(0);
            }
        }
        public void WriteAudioMessage(byte[] audio)
        {
            lock (locker)
            {
                int audioLength = audio.Length;
                _ms.Write(BitConverter.GetBytes(audioLength), 0, BitConverter.GetBytes(audioLength).Length);
                _ms.Write(audio, 0, audioLength);
            }
        }

    }
}

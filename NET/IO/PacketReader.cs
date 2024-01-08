using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using System.Threading;

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

        public byte[] ReadAudioMessage(int timeoutMilliseconds = 1000000) // Varsayılan olarak 10 saniye zaman aşımı
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];

            using (var cts = new CancellationTokenSource(timeoutMilliseconds))
            {
                try
                {
                    int bytesRead = 0;
                    while (bytesRead < length)
                    {
                        int remainingBytes = length - bytesRead;
                        int chunkSize = Math.Min(4096, remainingBytes);

                        int bytesReadThisRound = _ns.Read(msgBuffer, bytesRead, chunkSize);

                        if (bytesReadThisRound == 0)
                        {
                            // Veri okuma tamamlanmadan önce bağlantı koptu
                            throw new OperationCanceledException();
                        }

                        bytesRead += bytesReadThisRound;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Zaman aşımına uğradı, işlemi sonlandır veya hata mesajı gönder.
                    Console.WriteLine("Audio message reception timed out.");
                    throw; // İstediğiniz gibi hata durumunu kontrol edebilirsiniz.
                }
            }

            byte[] copiedBuffer = new byte[length];
            Array.Copy(msgBuffer, 0, copiedBuffer, 0, length);
            return copiedBuffer;
        }

    }
}

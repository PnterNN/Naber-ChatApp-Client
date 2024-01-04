using JavaProject___Server.NET.IO;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace JavaProject___Client.NET
{

    
    public class Server
    {
        //Serverdan gelen paketleri dinlemek için eventler
        public event Action UserConnectedEvent;
        public event Action UserDisconnectedEvent;
        public event Action MessageReceivedEvent;

        public event Action TweetReceivedEvent;
        public event Action GetTweetsEvent;
        public event Action DeleteTweetEvent;
        public event Action LikeEvent;

        public event Action FriendRequestEvent;
        public event Action FriendRequestCancelEvent;
        public event Action FriendRequestAcceptEvent;
        public event Action FriendRequestDeclineEvent;
        public event Action FriendRemoveEvent;
        public event Action getFriendEvent;

        public event Action GroupCreatedEvent;
        
        public event Action DeleteMessageEvent;
        public event Action MicMutedEvent;
        public event Action TooManyPacketsEvent;

        public event Action LoginCorrectEvent;
        public event Action LoginFailEvent;

        public event Action RegisterSuccessEvent;
        public event Action RegisterFailEvent;

        public string Username;
        public string UID;

        private static TcpClient _client = new TcpClient();

        public PacketReader PacketReader;
        public Server()
        {
            _client = new TcpClient();
        }

        //servere register yapmak için kullanılan fonksiyon
        public void Register(string username, string email, string password)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect("46.31.77.173", 9001);//46.31.77.173
                }
                catch
                {
                    MessageBox.Show("Sunucu bulunumadı, uygulama kapanıyor...");
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    return;
                }
                PacketReader = new PacketReader(_client.GetStream());
                ReadPackets();
            }
            var RegisterPacket = new PacketBuilder();
            RegisterPacket.WriteOpCode(0);
            RegisterPacket.WriteMessage(username);
            RegisterPacket.WriteMessage(email);
            RegisterPacket.WriteMessage(password);
            _client.Client.Send(RegisterPacket.GetPacketBytes());
        }

        //Servere giriş yapmak için kullanılan fonksiyon
        public void Login(string email, string password)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect("46.31.77.173", 9001);//46.31.77.173
                }
                catch
                {
                    MessageBox.Show("Sunucu ile iletişim kurulamıyor, daha sonra tekrar deneyin");
                }

                PacketReader = new PacketReader(_client.GetStream());
                ReadPackets();
            }
            var loginPacket = new PacketBuilder();
            loginPacket.WriteOpCode(1);
            loginPacket.WriteMessage(email);
            loginPacket.WriteMessage(password);
            _client.Client.Send(loginPacket.GetPacketBytes());
        }
        // OpCodes
        //0 - Register
        //1 - Login
        //2 - Info
        //3 - New Connection
        //4 - Disconnect
        //5 - Message - Group Message
        //6 - Group Created
        //7 - Delete Message


        //Sunucudan gelen paketleri okumak için kullanılan fonksiyon
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        // opcode
                        // 0 - register
                        // 1 - login
                        // 2 - info
                        // 3 - new connection
                        // 4 - disconnect
                        // 5 - message
                        // 6 - group created
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {
                            case 0:
                                var registerSuccess = PacketReader.ReadMessage();
                                if (registerSuccess == "True")
                                {
                                    RegisterSuccessEvent?.Invoke();
                                }
                                else
                                {
                                    RegisterFailEvent?.Invoke();
                                }
                                break;
                            case 1:
                                var loginSuccess = PacketReader.ReadMessage();
                                if (loginSuccess == "True")
                                {
                                    LoginCorrectEvent?.Invoke();
                                }
                                else
                                {
                                    LoginFailEvent?.Invoke();
                                }
                                break;
                            case 2:
                                Username = PacketReader.ReadMessage();
                                UID = PacketReader.ReadMessage();
                                break;
                            case 3:
                                UserConnectedEvent?.Invoke();
                                break;
                            case 4:
                                UserDisconnectedEvent?.Invoke();
                                break;
                            case 5:
                                MessageReceivedEvent?.Invoke();
                                break;
                            case 6:
                                GroupCreatedEvent?.Invoke();
                                break;
                            case 7:
                                DeleteMessageEvent?.Invoke();
                                break;
                            case 8:
                                MicMutedEvent?.Invoke();
                                break;
                            case 10:
                                TooManyPacketsEvent?.Invoke();
                                break;
                            case 11:
                                LikeEvent?.Invoke();
                                break;
                            case 12:
                                TweetReceivedEvent?.Invoke();
                                break;
                            case 13:
                                GetTweetsEvent?.Invoke();
                                break;
                            case 14:
                                DeleteTweetEvent?.Invoke();
                                break;
                            case 15:
                                FriendRequestEvent?.Invoke();
                                break;
                            case 16:
                                FriendRequestCancelEvent?.Invoke();
                                break;
                            case 17:
                                FriendRequestAcceptEvent?.Invoke();
                                break;
                            case 18:
                                MessageBox.Show("Arkadaşlık isteğiniz reddedildi");
                                FriendRequestDeclineEvent?.Invoke();
                                break;
                            case 19:
                                FriendRemoveEvent?.Invoke();
                                break;
                            case 20:
                                getFriendEvent?.Invoke();
                                break;
                            default:
                                Console.WriteLine("Unknown opcode: " + opcode);
                                break;
                        }
                    }
                    catch(Exception e)
                    {
                        //Eğer sunucu çökerse client kapanıyor
                        MessageBox.Show("Sunucu çöktü, uygulama kapanıyor... " + e);
                        Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    }
                }
            });
        }

        public void sendFriendRemove(string username)
        {

            var packet = new PacketBuilder();
            packet.WriteOpCode(19);
            packet.WriteMessage(username);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void sendFriendDecline(string username)
        {
            MessageBox.Show("Arkadaşlık isteğiniz gönderildi");
            var packet = new PacketBuilder();
            packet.WriteOpCode(18);
            packet.WriteMessage(username);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void sendFriendAccept(string username)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(17);
            packet.WriteMessage(username);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void sendFriendRequestCancel(string username)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(16);
            packet.WriteMessage(username);
            _client.Client.Send(packet.GetPacketBytes());
        }

        public void sendFriendRequest(string username)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(15);
            packet.WriteMessage(username);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void createGroup(string groupName, string clientIDS)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(6);
            packet.WriteMessage(groupName);
            packet.WriteMessage(clientIDS);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void SendMessage(string message, string contactUID, string firstMessage, string messageUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(5);
            packet.WriteMessage(message);
            packet.WriteMessage(contactUID);
            packet.WriteMessage(firstMessage);
            packet.WriteMessage(messageUID);
            _client.Client.Send(packet.GetPacketBytes());
        }

        public void deleteTweet(string tweetUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(14);
            packet.WriteMessage(tweetUID);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void likeTweet(string tweetUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(11);
            packet.WriteMessage(tweetUID);
            _client.Client.Send(packet.GetPacketBytes());
        }
        public void SendTweet(string message, string tweetUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(12);
            packet.WriteMessage(message);
            packet.WriteMessage(tweetUID);
            _client.Client.Send(packet.GetPacketBytes());
        }

        public void DeleteMessage(string UID, string contactUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(7);
            packet.WriteMessage(UID);
            packet.WriteMessage(contactUID);
            _client.Client.Send(packet.GetPacketBytes());
        }
    }
}

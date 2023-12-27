using ChatApp.Core;
using JavaProject___Client.MVVM.Model;
using JavaProject___Client.NET;
using JavaProject___Client.Services;
using JavaProject___Server.NET.IO;
using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class HomeViewModelUsers : Core.ViewModel
    {

        public IDataService DataService { get; set; }

        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }
        //****************************************************************
        public String Username { get; set; }

        public string UID { get; set; }

        private Server _server;
        public UserModel ProfileUser
        {
            get
            {
                return DataService.ProfileUser;
            }
            set
            {
                DataService.ProfileUser = value;
                OnPropertyChanged();
            }
        }
        private UserModel _selectedUser;
        public UserModel SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                DataService.SelectedUser = value;
                OnPropertyChanged();
            }
        }
        private bool _microphone;
        public bool Microphone
        {
            get { return _microphone; }
            set
            {
                _microphone = value;
                OnPropertyChanged();
            }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private void UserConnected()
        {
            string username = _server.PacketReader.ReadMessage();
            string uid = _server.PacketReader.ReadMessage();
            string messageCount = _server.PacketReader.ReadMessage();
            var user = new UserModel(Navigation, DataService)
            {
                Username = username,
                ImageSource = "CornflowerBlue",
                UID = uid,
                Messages = new ObservableCollection<MessageModel>()
            };
            string dataUsername = "";
            string dataUID = "";
            string dataImageSource = "";
            string dataMessage = "";
            DateTime dataTime = DateTime.Now;
            bool dataFirstMessage = true;
            string messageUID = "";
            for (int i = 0; i < int.Parse(messageCount); i++)
            {
                try
                {
                    dataUsername = _server.PacketReader.ReadMessage();
                    dataUID = _server.PacketReader.ReadMessage();
                    dataImageSource = _server.PacketReader.ReadMessage();
                    dataMessage = _server.PacketReader.ReadMessage();
                    dataTime = DateTime.Parse(_server.PacketReader.ReadMessage());
                    dataFirstMessage = bool.Parse(_server.PacketReader.ReadMessage());
                    messageUID = _server.PacketReader.ReadMessage();
                    if (dataUsername != null)
                    {
                        var ownMessage = false;
                        if(dataUsername == DataService.Username)
                        {
                            ownMessage = true;
                        }
                        user.Messages.Add(new MessageModel(Navigation, DataService)
                        {
                            Username = dataUsername,
                            ImageSource = "",
                            ownMessage = ownMessage,
                            UsernameColor = "CornflowerBlue",
                            Message = formattedString(dataMessage),
                            UID = messageUID,
                            Time = dataTime,
                            FirstMessage = dataFirstMessage
                        });
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            if (dataUsername == "")
            {
                user.Messages.Add(new MessageModel(Navigation, DataService)
                {
                    Username = "ChatApp",
                    ImageSource = "",
                    UsernameColor = "CornflowerBlue",
                    ownMessage = false,
                    Message = username + " Çevrimiçi oldu",
                    Time = DateTime.Now,
                    FirstMessage = true
                });
            }
            if (!DataService.Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => DataService.Users.Add(user));
            }
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            var username = _server.PacketReader.ReadMessage();
            var sendedUserUID = _server.PacketReader.ReadMessage();
            var messageUID = _server.PacketReader.ReadMessage();
            var user = DataService.Users.Where(x => x.UID == sendedUserUID).FirstOrDefault();
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bool FirstMessage = false;
                    if(user.Messages.Count > 0)
                    {
                        if (user.LastMessage.Username != username)
                        {
                            FirstMessage = true;
                        }
                    }
                    else
                    {
                        FirstMessage = true;
                    }
                    user.Messages.Add(new MessageModel(Navigation, DataService)
                    {
                        Username = username,
                        ImageSource = "",
                        ownMessage = false,
                        UsernameColor = "CornflowerBlue",
                        UID = messageUID,
                        Message = formattedString(msg),
                        Time = DateTime.Now,
                        FirstMessage = FirstMessage
                    });
                    if (SelectedUser != user)
                    {
                        try
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer("./Assets/notification.wav");
                            player.Play();
                        }
                        catch
                        {

                        }
                    }
                    OnPropertyChanged("LastMessageText");
                });
            }
        }

        private void createGroup()
        {
            var groupName = _server.PacketReader.ReadMessage();
            var userUIDS = _server.PacketReader.ReadMessage();
            List<string> uids = userUIDS.Split(' ').ToList();
            List<string> usernames = new List<string>();
            foreach (string uid in uids)
            {
                UserModel? userModel = DataService.Users.Where(x => x.UID == uid).FirstOrDefault();
                if (userModel != null)
                {
                    usernames.Add(userModel.Username);
                }
                else
                {
                    //kullanıcı açık olmadığı için ekleme yapılmadı
                }
            }
            var user = new UserModel(Navigation, DataService)
            {
                Username = groupName,
                ImageSource = "CornflowerBlue",
                UID = userUIDS,
                Messages = new ObservableCollection<MessageModel>()
            };
            user.Messages.Add(new MessageModel(Navigation, DataService)
            {
                Username = groupName,
                ImageSource = "",
                UsernameColor = "CornflowerBlue",
                ownMessage = false,
                Message = "Uye Listesi: " + string.Join(" ", usernames),
                Time = DateTime.Now,
                FirstMessage = true
            });
            Application.Current.Dispatcher.Invoke(() => DataService.Users.Add(user));
        }

        private void UserDisconnected()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = DataService.Users.FirstOrDefault(x => x.UID == uid);
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => DataService.Users.Remove(user));
            }
        }
        public RelayCommand SendMessageCommand { get; set; }

        public RelayCommand NavigateToHome { get; set; }

        public RelayCommand DeleteMessage { get; set; }

        private void deleteMessage()
        {
            var contactUID = _server.PacketReader.ReadMessage();
            var messageUID = _server.PacketReader.ReadMessage();
            var user = DataService.Users.Where(x => x.UID == contactUID).FirstOrDefault();
            if (user != null)
            {
                var message = user.Messages.Where(x => x.UID == messageUID).FirstOrDefault();
                if (message != null)
                {
                    Application.Current.Dispatcher.Invoke(() => user.Messages.Remove(message));
                }
            }
        }

        public RelayCommand NavigateToTweet { get; set; }
        public RelayCommand NavigateToProfile { get; set; }
        public RelayCommand NavigateToSettings { get; set; }

        public RelayCommand ApplicationExit { get; set; }

        static string formattedString(string originalString)
        {
            for (int i = 99; i < originalString.Length; i += 100)
            {
                originalString = originalString.Insert(i, "\n");
            }
            return originalString;
        }

        public HomeViewModelUsers(INavigationService navService, IDataService dataservice)
        {
            
            DataService = dataservice;
            Navigation = navService;
            _server = dataservice.server;
            Username = dataservice.Username;
            UID = dataservice.UID;
            NavigateToTweet = new RelayCommand(o =>
            {

                Navigation.NavigateTo<HomeViewModelTweet>();
            });

            ApplicationExit = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            NavigateToProfile = new RelayCommand(o =>
            {
                dataservice.ProfileUser.Username = dataservice.Username;
                foreach (var tweet in dataservice.Tweets)
                {
                    if (tweet.Username == dataservice.Username)
                    {
                        dataservice.ProfileUser.Tweets.Add(tweet);
                    }
                }
                Navigation.NavigateTo<HomeViewModelProfile>();
            });

            NavigateToSettings = new RelayCommand(o =>
            {
                Navigation.NavigateTo<HomeViewModelSettings>();
            });

            
            MessageModel message = new MessageModel(navService, dataservice);
            message.Message = "Merhaba, benim adım " + dataservice.Username + ". Sana nasıl yardımcı olabilirim?";
            message.ownMessage = true;
            message.FirstMessage = true;
            message.Username = dataservice.Username;
            message.Time = DateTime.Now;
            

            UserModel user = new UserModel(navService, dataservice);
            user.Username = "Test1";
            user.Messages = new ObservableCollection<MessageModel>();
            user.Messages.Add(message);

            Application.Current.Dispatcher.Invoke(() => DataService.Users.Add(user));

            SendMessageCommand = new RelayCommand(o =>
                {
                    if (!string.IsNullOrEmpty(Message))
                    {
                        if (_selectedUser != null)
                        {
                            Random random = new Random();
                            int messageUID = random.Next(100000000, 999999999);
                            bool FirstMessage = false;
                            if (_selectedUser.Messages.Count > 0)
                            {
                                if (_selectedUser.LastMessage.Username != dataservice.Username)
                                {
                                    FirstMessage = true;
                                }
                            }
                            else
                            {
                                FirstMessage = true;
                            }
                            SelectedUser.Messages.Add(new MessageModel(Navigation, DataService)
                            {
                                Username = dataservice.Username,
                                ImageSource = "",
                                UID = messageUID.ToString(),
                                ownMessage = true,
                                UsernameColor = "CornflowerBlue",
                                Message = formattedString(Message),
                                Time = DateTime.Now,
                                FirstMessage = FirstMessage
                            });
                            _server.SendMessage(Message, SelectedUser.UID, FirstMessage.ToString(), messageUID.ToString());
                        }
                        Message = "";
                        OnPropertyChanged("LastMessageText");
                    }
                });

                _server.UserConnectedEvent += UserConnected;
                _server.MessageReceivedEvent += MessageReceived;
                _server.UserDisconnectedEvent += UserDisconnected;
                _server.GroupCreatedEvent += createGroup;
                _server.DeleteMessageEvent += deleteMessage;
        }
    }
}


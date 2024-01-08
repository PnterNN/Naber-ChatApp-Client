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
using GroovyCodecs.G729;
using NAudio.CoreAudioApi;
using System.IO;
using System.Windows.Shapes;
using JavaProject___Client.MVVM.View;
using GroovyCodecs.WavFile;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class HomeViewModelUsers : Core.ViewModel
    {

        private static G729Encoder enkoder;
        private static G729Decoder dekoder;
        private WaveFileWriter waveWriter;
        private WasapiCapture waveIn;

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
                VoiceButtonEnabled = true;
                OnPropertyChanged();
            }
        }
        private string _voiceButtonColor;

        public string VoiceButtonColor
        {
            get
            {
                return _voiceButtonColor;
            }
            set
            {
                _voiceButtonColor = value;
                OnPropertyChanged();
            }
        }

        private string _voiceButtonContent;
        public string VoiceButtonContent
        {
            get
            {
                return _voiceButtonContent;
            }
            set
            {
                _voiceButtonContent = value;
                OnPropertyChanged();
            }
        }
        public bool VoiceButtonEnabled
        {
            get
            {
                return DataService.voiceButtonEnabled;
            }
            set
            {
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
                VoiceButtonEnabled = true;
                OnPropertyChanged();
            }
        }

        private void RegisterUserConnected()
        {
            string username = _server.PacketReader.ReadMessage();
            string uid = _server.PacketReader.ReadMessage();
            var user = new UserModel(Navigation, DataService)
            {
                Username = username,
                ImageSource = "CornflowerBlue",
                UID = uid,
                Messages = new ObservableCollection<MessageModel>(),
                Status = true
            };

            user.Messages.Add(new MessageModel(Navigation, DataService)
            {
                Username = "Naber",
                ImageSource = "",
                UsernameColor = "CornflowerBlue",
                ownMessage = false,
                Message = username + " Çevrimiçi oldu",
                Time = DateTime.Now,
                FirstMessage = true
            });

            if (!DataService.Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => DataService.Users.Add(user));
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
            user.Status = true;
            string dataUsername = "";
            string dataUID = "";
            string dataImageSource = "";
            string dataMessage = "";
            string dataVoice = "";
            DateTime dataTime = DateTime.Now;
            bool dataFirstMessage = true;
            string messageUID = "";
            int index2 = 0;
            for (int i = 0; i < int.Parse(messageCount); i++)
            {
                try { 
                    dataUsername = _server.PacketReader.ReadMessage();
                    dataUID = _server.PacketReader.ReadMessage();
                    dataImageSource = _server.PacketReader.ReadMessage();
                    dataMessage = _server.PacketReader.ReadMessage();
                    dataVoice = _server.PacketReader.ReadMessage();
                    dataTime = DateTime.Parse(_server.PacketReader.ReadMessage());
                    dataFirstMessage = bool.Parse(_server.PacketReader.ReadMessage());
                    messageUID = _server.PacketReader.ReadMessage();
                    Random random = new Random();
                    int messageUID2 = int.Parse(messageUID) - random.Next(0,9999);
                    int index = user.Messages.Count();
                    if (dataVoice != "0")
                    {
                        dekoder = new G729Decoder();
                        string[] sesdizi = dataVoice.Split(',');
                        List<byte> baytListe = new List<byte>();
                        foreach (string deger in sesdizi)
                        {
                            if (byte.TryParse(deger, out byte sonuc))
                            {
                                baytListe.Add(sonuc);
                            }
                            else
                            {
                            }
                        }
                        byte[] baytDizisi = baytListe.ToArray();
                        byte[] baytDizisi2 = dekoder.Process(baytDizisi);

                        WaveFormat waveFormat = new WaveFormat(44100, 1);
                        waveWriter = new WaveFileWriter($"cache/{messageUID2}" + ".wav", waveFormat);
                        waveWriter.Write(baytDizisi2, 0, baytDizisi2.Length);
                        waveWriter.Close();
                        _ = Task.Run(() =>
                        {
                            int _index = index;
                            Task.Delay(1000).ContinueWith(_ =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    user.Messages.Insert(_index+index2, new MessageModel(Navigation, DataService)
                                    {
                                        Username = dataUsername,
                                        ImageSource = "CornflowerBlue",
                                        SoundName = $"{messageUID2}.wav",
                                        UsernameColor = "CornflowerBlue",
                                        Time = dataTime,
                                        VoiceMessage = true,
                                        State = "Play",
                                        UID = messageUID2.ToString()
                                    });
                                });
                                index2++;
                            });
                        });
                    }
                    else
                    {
                        if (dataUsername != null)
                        {
                            var ownMessage = false;
                            if (dataUsername == DataService.Username)
                            {
                                ownMessage = true;
                            }
                            user.Messages.Insert(index,new MessageModel(Navigation, DataService)
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
                }
                catch
                {

                }
            }
            if (dataUsername == "")
            {
                user.Messages.Add(new MessageModel(Navigation, DataService)
                {
                    Username = "Naber",
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
            if(user != null)
            {
                user.Status = false;
            }
            /*
            if (user != null)
            {
                Application.Current.Dispatcher.Invoke(() => DataService.Users.Remove(user));
            }
            */
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
        public RelayCommand VoiceButtonCommand { get; set; }

        static string formattedString(string originalString)
        {
            for (int i = 99; i < originalString.Length; i += 100)
            {
                originalString = originalString.Insert(i, "\n");
            }
            return originalString;
        }


        private void csvToWav(string UID)
        {

            enkoder = new G729Encoder();
            dekoder = new G729Decoder();

            StreamReader st = new StreamReader($"cache/{UID}.csv");
            string sesdos = st.ReadLine();
            st.Close();
            string[] sesdizi = sesdos.Split(',');

            List<byte> baytListe = new List<byte>();
            foreach (string deger in sesdizi)
            {
                if (byte.TryParse(deger, out byte sonuc))
                {
                    baytListe.Add(sonuc);
                }
                else
                {
                    // Dönüşüm başarısız olduğunda yapılacak işlemi burada ayarlayabilirsiniz, örneğin bir hata mesajı yazdırabilirsiniz.
                }
            }

            byte[] baytDizisi = baytListe.ToArray();
            byte[] baytDizisi2 = dekoder.Process(baytDizisi);

            WaveFormat waveFormat = new WaveFormat(44100, 1);
            waveWriter = new WaveFileWriter($"cache/{UID}" +"-2"+ ".wav", waveFormat);
            waveWriter.Write(baytDizisi2, 0, baytDizisi2.Length);
            waveWriter.Close();
        }

        private void wavToCsv(string UID)
        {

            enkoder = new G729Encoder();
            var audioFile = new WavReader();
            audioFile.OpenFile($"cache/{UID}.wav");
            var inBuffer = audioFile.readWav();

            byte[] baytdizi2 = enkoder.Process(inBuffer);

            StreamWriter stwr = new StreamWriter($"cache/{UID}.csv");
            string sesdos = "";
            for (long sayac = 0; sayac < baytdizi2.Length; sayac++)
            {
                sesdos = sesdos + baytdizi2[sayac].ToString() + ",";
            }
            sesdos = sesdos + "B";
            stwr.Write(sesdos);
            stwr.Close();

        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (VoiceButtonContent == "Stop Voice")
            {
                waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private void byteToVoice(byte[] voice, string UID)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dekoder = new G729Decoder();
                byte[] baytDizisi2 = dekoder.Process(voice);
                WaveFormat waveFormat = new WaveFormat(44100, 1);
                using (waveWriter = new WaveFileWriter($"cache/{UID}.wav", waveFormat))
                {
                    waveWriter.Write(baytDizisi2, 0, baytDizisi2.Length);
                    waveWriter.Close();
                }
            });
        }
        private void VoiceMessageReceived()
        {
            byte[] voice = _server.PacketReader.ReadAudioMessage();
            var messageUID = _server.PacketReader.ReadMessage();
            var contactUID = _server.PacketReader.ReadMessage();
            int messageUID2 = int.Parse(messageUID) + 1;
            byteToVoice(voice, messageUID2.ToString());
            var user = DataService.Users.Where(x => x.UID == contactUID).FirstOrDefault();
            if (user != null)
            {
                _ = Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        
                        user.Messages.Add(new MessageModel(Navigation, DataService)
                        {
                            Username = user.Username,
                            ImageSource = "CornflowerBlue",
                            SoundName = $"{messageUID2}.wav",
                            Time = DateTime.Now,
                            VoiceMessage = true,
                            State = "Play",
                            UID = messageUID2.ToString()
                        });
                    });
                });
            }
        }

        public HomeViewModelUsers(INavigationService navService, IDataService dataservice)
        {
            
            DataService = dataservice;
            Navigation = navService;
            _server = dataservice.server;
            Username = dataservice.Username;
            UID = dataservice.UID;


            VoiceButtonContent = "Start Voice";
            VoiceButtonColor = "#4E72AB";

            NavigateToTweet = new RelayCommand(o =>
            {

                Navigation.NavigateTo<HomeViewModelTweet>();
            });

            ApplicationExit = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            string messageUID = "";
            VoiceButtonCommand = new RelayCommand(o =>
            {
                if (VoiceButtonContent == "Start Voice")
                {
                    VoiceButtonContent = "Stop Voice";
                    VoiceButtonColor = "#BE3144";

                    Random random = new Random();
                    messageUID = random.Next(100000000, 999999999).ToString();

                    var enumerator = new MMDeviceEnumerator();
                    var device = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ElementAt(DataService.MicrophoneDevice);
                    waveIn = new WasapiCapture(device);

                    WaveFormat waveFormat = new WaveFormat(44100, 1);
                    waveIn.WaveFormat = waveFormat;
                    waveIn.DataAvailable += OnDataAvailable;
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName("cache/"));
                    waveWriter = new WaveFileWriter($"cache/{messageUID}.wav", waveFormat);
                    waveIn.StartRecording();

                }
                else
                {
                    VoiceButtonContent = "Start Voice";
                    VoiceButtonColor = "#4E72AB";

                    waveIn.StopRecording();

                    waveWriter.Close();
                    waveWriter = null;
                    waveIn.Dispose();
                    _ = Task.Run(() => 
                    {
                        Task.Delay(1000).Wait();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_selectedUser != null)
                            {
                                _selectedUser.Messages.Add(new MessageModel(Navigation, DataService)
                                {
                                    Username = Username,
                                    ImageSource = "CornflowerBlue",
                                    UsernameColor = "CornflowerBlue",
                                    SoundName = $"{messageUID}.wav",
                                    Time = DateTime.Now,
                                    VoiceMessage = true,
                                    State = "Play",
                                    UID = messageUID
                                });
                            }
                        });
                    });

                    Task.Delay(1000).Wait();


                    enkoder = new G729Encoder();
                    var audioFile = new GroovyCodecs.WavFile.WavReader();
                    audioFile.OpenFile($"cache/{messageUID}.wav");
                    var inBuffer = audioFile.readWav();
                    byte[] audio = enkoder.Process(inBuffer);
                    _server.sendVoiceMessage(audio, messageUID, _selectedUser.UID);

                }
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
            _server.userRegisterConnectedEvent += RegisterUserConnected;
            _server.VoiceMessageReceivedEvent += VoiceMessageReceived;
        }
    }
}


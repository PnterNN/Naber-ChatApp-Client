using ChatApp.Core;
using JavaProject___Client.MVVM.Model;
using JavaProject___Client.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaProject___Client.Services
{
    public interface IDataService
    {
        public string Username { get; set; }
        public string UID { get; set; }
        public int MicrophoneDevice { get; set; }
        public int SpeakerDevice { get; set; }

        public bool voiceButtonEnabled { get; set; }

        public UserModel SelectedUser { get; set; }
        public UserModel ProfileUser { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<TweetModel> Tweets { get; set; }
        public ObservableCollection<UserModel> FriendRequests { get; set; }
        public ObservableCollection<UserModel> Friends { get; set; }
        public Server server { get; set; }
    }
    internal class DataService : IDataService
    {
        public string Username { get; set; }
        public Server server { get; set; }
        public string UID { get; set; }

        public int MicrophoneDevice { get; set; }
        public int SpeakerDevice { get; set; }

        public bool voiceButtonEnabled 
        {
            get
            {
                if (MicrophoneDevice != -1 && SelectedUser != null)
                    return true;
                else
                    return false;
            }
            set{}
        }

        public UserModel ProfileUser { get; set; }
        public UserModel SelectedUser { get; set; }

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<TweetModel> Tweets { get; set; }
        public ObservableCollection<UserModel> FriendRequests { get; set; }
        public ObservableCollection<UserModel> Friends { get; set; }
    }
}

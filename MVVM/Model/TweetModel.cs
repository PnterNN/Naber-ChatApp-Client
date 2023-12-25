using ChatApp.Core;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace JavaProject___Client.MVVM.Model
{
    public class TweetModel : ObservableObject
    {
        private IDataService dataService { get; set; } 
        private INavigationService navigationService { get; set; }
        public RelayCommand LikeButton { get; set; }

        public RelayCommand DeleteButton { get; set; }

        public RelayCommand FriendRequest { get; set; }

        private void deleteTweet(object param)
        {
            if (param != null)
            {
                dataService.server.deleteTweet(param.ToString());
            }
        }

        public TweetModel(INavigationService navService, IDataService dataservice)
        {
            dataService = dataservice;
            navigationService = navService;
            Like = new ObservableCollection<UserModel>();

            DeleteButton = new RelayCommand((param) => deleteTweet(param));

            LikeButton = new RelayCommand(o =>
            {
                UserModel user = new UserModel(navService, dataservice);
                user.UID = dataService.UID;
                bool check = false;
                foreach(UserModel likeuser in Like)
                {
                    if(likeuser.UID == user.UID)
                    {
                        check = true;
                        return;
                    }
                }
                if (check == false)
                {
                    dataservice.server.likeTweet(UID);
                }
            });

            FriendRequest = new RelayCommand(o =>
            {
                bool status = false;
                foreach (var friend in dataService.Friends)
                {
                    if (friend.Username == Username)
                    {
                        status = true;
                    }
                }
                foreach (var friend in dataService.FriendRequests)
                {
                    if (friend.Username == Username)
                    {
                        status = true;
                    }
                }
                if (status==false)
                {
                    dataservice.FriendRequests.Add(new UserModel(navigationService, dataService)
                    {
                        Username = Username,
                        UID = UID,
                        ownRequest = true
                    });
                    dataservice.server.sendFriendRequest(Username);
                }  
            });
        }
        
        public string Username { get; set; }
        public string ImageSource { get; set; }
        public string Message { get; set; }
        public bool? ownMessage { get; set; }
        public DateTime Time { get; set; }
        public string UID { get; set; }

        public ObservableCollection<UserModel> _like;
        public ObservableCollection<UserModel> Like
        {
            get { return _like; }
            set 
            { 
                _like = value; 
                OnPropertyChanged(); 
            }
        }
        
    }
}

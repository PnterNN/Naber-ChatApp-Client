using ChatApp.Core;
using JavaProject___Client.MVVM.ViewModel;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace JavaProject___Client.MVVM.Model
{
    public class UserModel : ObservableObject
    {
        private IDataService dataService { get; set; }
        private INavigationService navigationService { get; set; }

        public RelayCommand FriendAccept { get; set; }
        public RelayCommand FriendDecline { get; set; }
        public RelayCommand FriendRequest { get; set; }
        public RelayCommand FriendRemove { get; set; }
        public RelayCommand FriendRequestRemove { get; set; }
        public RelayCommand GoToProfile { get; set; }

        public RelayCommand ChatFriend { get; set; }
        public UserModel(INavigationService navService, IDataService dataservice)
        {
            dataService = dataservice;
            navigationService = navService;
            FriendAccept = new RelayCommand(o =>
            {
                dataService.server.sendFriendAccept(Username);
                foreach (var user in dataService.FriendRequests)
                {
                    if (user.Username == Username)
                    {
                        dataService.Friends.Add(user);
                        dataService.FriendRequests.Remove(user);
                        return;
                    }
                }
            });

            FriendDecline = new RelayCommand(o =>
            {
                dataService.server.sendFriendDecline(Username);
                foreach (var user in dataService.FriendRequests)
                {
                    if (user.Username == Username)
                    {
                        dataService.FriendRequests.Remove(user);
                        return;
                    }
                }
            });


            FriendRequest = new RelayCommand(o =>
            {

            });
            FriendRemove = new RelayCommand(o =>
            {
                dataService.server.sendFriendRemove(Username);
                foreach (var user in dataService.Friends)
                {
                    if (user.Username == Username)
                    {
                        dataService.Friends.Remove(user);
                        return;
                    }
                }
            });
            FriendRequestRemove = new RelayCommand(o =>
            {
                dataService.server.sendFriendRequestCancel(Username);
                foreach (var user in dataService.FriendRequests)
                {
                    if (user.Username == Username)
                    {
                        dataService.FriendRequests.Remove(user);
                        return;
                    }
                }
            });



            GoToProfile = new RelayCommand(o =>
            {
                foreach (var tweet in dataService.Tweets)
                {
                    if (tweet.Username == Username)
                    {
                        dataService.ProfileUser.Tweets.Add(tweet);
                    }
                }
                dataService.ProfileUser.Username = Username;
                dataService.ProfileUser.Status = Status;
                navigationService.NavigateTo<HomeViewModelProfile>();
            });
        }

        public string Username { get; set; }
        public string UID { get; set; }

        public bool? _status;
        public bool? Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
        public List<UserModel> Friends { get; set; }
        public bool? ownRequest { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<TweetModel> Tweets { get; set; }
        public MessageModel LastMessage => Messages.Last();
        public string LastMessageText 
        {
            get 
            {
                if (Messages.Last().Message.Length > 25)
                {
                    return Messages.Last().Message.Substring(0,25) + " ...";
                }
                else
                {
                    return Messages.Last().Message;
                }
            }
        }
    }
}

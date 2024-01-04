using ChatApp.Core;
using JavaProject___Client.Core;
using JavaProject___Client.MVVM.Model;
using JavaProject___Client.NET;
using JavaProject___Client.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class HomeViewModelTweet : Core.ViewModel
    {
        public string Username { get; set; }
        private Server _server;
        public string UID { get; set; }

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

        private string _tweet;
        public string Tweet
        {
            get { return _tweet; }
            set
            {
                _tweet = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<UserModel> FriendRequests
        {
            get
            {
                return DataService.FriendRequests;
            }
            set
            {
                DataService.FriendRequests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> Friends
        {
            get
            {
                return DataService.Friends;
            }
            set
            {
                DataService.Friends = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TweetModel> Tweets
        {
            get 
            {
                return DataService.Tweets;
            }
            set
            {
                DataService.Tweets = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand NavigateToHomeUser {get; set;}

        public RelayCommand NavigateToSettings { get; set; }

        public RelayCommand NavigateToProfile { get; set; }

        public RelayCommand SendTweet { get; set; }

        public RelayCommand ApplicationExit { get; set; }

        public RelayCommand NavigateToTest { get; set; }

        public void tweetReceivedEvent()
        {
            var username = _server.PacketReader.ReadMessage();
            var tweetmessage = _server.PacketReader.ReadMessage();
            var tweetUID = _server.PacketReader.ReadMessage();

            TweetModel tweet = new TweetModel(Navigation, DataService);
            tweet.Username = username;
            tweet.Message = tweetmessage;
            tweet.UID = tweetUID;
            tweet.Time = DateTime.Now;
            tweet.Like = new ObservableCollection<UserModel>();

            if (username == DataService.Username)
            {
                tweet.ownMessage = true;
            }
            else
            {
                tweet.ownMessage = false;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Tweets.Add(tweet);
            });
        }
        public void likeEvent()
        {
            var userUID = _server.PacketReader.ReadMessage();
            var tweetUID = _server.PacketReader.ReadMessage();
            foreach(TweetModel tweet in Tweets)
            {
                if(tweet.UID == tweetUID)
                {
                    UserModel user = new UserModel(Navigation, DataService);
                    user.UID = userUID;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        tweet.Like.Add(user);
                    });
                }
            }
        }
        public void getTweets()
        {
            var tweetsCount = _server.PacketReader.ReadMessage();
            for (int i = 0; i < int.Parse(tweetsCount); i++)
            {
                var username = _server.PacketReader.ReadMessage();
                var tweetUID = _server.PacketReader.ReadMessage();
                var tweetImage = _server.PacketReader.ReadMessage();
                var tweetmessage = _server.PacketReader.ReadMessage();
                var tweetLikes = _server.PacketReader.ReadMessage();
                DateTime tweetTime = DateTime.Parse(_server.PacketReader.ReadMessage());

                ObservableCollection<UserModel> like1 = new ObservableCollection<UserModel>();
                string[] likes = tweetLikes.Split(' ');
                foreach(string like in likes)
                {
                    if(like != "" || !string.IsNullOrEmpty(like))
                    {
                        UserModel user = new UserModel(Navigation, DataService);
                        user.UID = like;
                        like1.Add(user);
                    }
                }
                
                TweetModel tweet = new TweetModel(Navigation, DataService);
                tweet.Username = username;
                tweet.Message = tweetmessage;
                tweet.UID = tweetUID;
                tweet.Time = tweetTime;
                tweet.Like = like1;

                if (username == DataService.Username)
                {
                    tweet.ownMessage = true;
                }
                else
                {
                    tweet.ownMessage = false;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Tweets.Add(tweet);
                });
            }
        }
        public void deleteTweetEvent()
        {
            var tweetUID = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Tweets.Remove(Tweets.Where(o => o.UID == tweetUID).Single());
                try
                {
                    DataService.ProfileUser.Tweets.Remove(DataService.ProfileUser.Tweets.Where(o => o.UID == tweetUID).Single());
                }
                catch
                {

                }
                
            });
        }

        public void friendRequestEvent()
        {
            var username = _server.PacketReader.ReadMessage();
           
            UserModel user = new UserModel(Navigation, DataService);
            user.Username = username;
            user.ownRequest = false;

            var user2 = DataService.Users.Where(x => x.Username == username).FirstOrDefault();
            if(user2 != null)
            {
                if (user2.Status == true)
                {
                    user.Status = true;
                }
                else if (user2.Status == false)
                {
                    user.Status = false;
                }
                else
                {
                    user.Status = false;
                }
            }
            else
            {
                user.Status = false;
            }
            

            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequests.Add(user);
            });
        }
        public void friendRequestCancelEvent()
        {
            var username = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequests.Remove(FriendRequests.Where(o => o.Username == username).Single());
            });
        }
        private void friendRequestAcceptEvent()
        {
            var username = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Friends.Add(FriendRequests.Where(o => o.Username == username).Single());
                FriendRequests.Remove(FriendRequests.Where(o => o.Username == username).Single());
            });
        }

        private void friendRequestDeclineEvent()
        {
            var username = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequests.Remove(FriendRequests.Where(o => o.Username == username).Single());
            });
        }

        private void friendRemoveEvent()
        {
            var username = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Friends.Remove(Friends.Where(o => o.Username == username).Single());
                }
                catch
                {

                }
            });
        }

        private void getFriendEvent()
        {

            var friendsCount = _server.PacketReader.ReadMessage();
            for (int i = 0; i < int.Parse(friendsCount); i++)
            {
                var username = _server.PacketReader.ReadMessage();
                var ownRequest = bool.Parse(_server.PacketReader.ReadMessage());
                var state = bool.Parse(_server.PacketReader.ReadMessage());
                UserModel user = new UserModel(Navigation, DataService);

                var user2 = DataService.Users.Where(x => x.Username == username).FirstOrDefault();
                if (user2!=null)
                {
                    if (user2.Status == true)
                    {
                        user.Status = true;
                    }
                    else if (user2.Status == false)
                    {
                        user.Status = false;
                    }
                    else
                    {
                        user.Status = false;
                    }
                }
                else
                {
                    user.Status = false;
                }
                
                user.Username = username;
                
                if (state == true)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Friends.Add(user);
                    });
                }
                else
                {
                    if (ownRequest == true)
                    {
                        user.ownRequest = true;
                    }
                    else
                    {
                        user.ownRequest = false;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FriendRequests.Add(user);
                    });
                }
               
                
            }

        }

        public HomeViewModelTweet(INavigationService navService, IDataService dataservice)
        {
            DataService = dataservice;
            Navigation = navService;
            _server = dataservice.server;

            _server.FriendRequestEvent += friendRequestEvent;
            _server.FriendRequestCancelEvent += friendRequestCancelEvent;
            _server.FriendRequestAcceptEvent += friendRequestAcceptEvent;
            _server.FriendRequestDeclineEvent += friendRequestDeclineEvent;
            _server.FriendRemoveEvent += friendRemoveEvent;

            _server.GetTweetsEvent += getTweets;
            _server.TweetReceivedEvent += tweetReceivedEvent;
            _server.LikeEvent += likeEvent;
            _server.DeleteTweetEvent += deleteTweetEvent;

            Username = dataservice.Username;
            UID = dataservice.UID;
            dataservice.ProfileUser.Tweets = new ObservableCollection<TweetModel>();
            FriendRequests = new ObservableCollection<UserModel>();
            Friends = new ObservableCollection<UserModel>();
            Tweets = new ObservableCollection<TweetModel>();

            ApplicationExit = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            SendTweet = new RelayCommand(o =>
            {
                if (Tweet != null || Tweet == "")
                {
                    Random random = new Random();
                    int tweetUID = random.Next(100000000, 999999999);
                    dataservice.server.SendTweet(Tweet, tweetUID.ToString());
                    Tweet = "";
                }
            });

            NavigateToTest = new RelayCommand(o =>
            {
                
                Navigation.NavigateTo<TestViewModel>();
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

            NavigateToHomeUser = new RelayCommand(o =>
            {
                Navigation.NavigateTo<HomeViewModelUsers>();
            });

            _server.getFriendEvent += getFriendEvent;
        }

    }
}

using ChatApp.Core;
using JavaProject___Client.MVVM.Model;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class HomeViewModelProfile : Core.ViewModel
    {
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


        public RelayCommand NavigateToBack { get; set; }

        public HomeViewModelProfile(INavigationService navService, IDataService dataservice)
        {
            DataService = dataservice;
            Navigation = navService;

            dataservice.Username = dataservice.server.Username;
            dataservice.UID = dataservice.server.UID;

            dataservice.Users = new ObservableCollection<UserModel>();
            dataservice.Tweets = new ObservableCollection<TweetModel>();
            dataservice.Messages = new ObservableCollection<MessageModel>();
            dataservice.Friends = new ObservableCollection<UserModel>();
            dataservice.FriendRequests = new ObservableCollection<UserModel>();

            dataservice.ProfileUser = new UserModel(navService,dataservice);
            dataservice.ProfileUser.Username = dataservice.Username;
            dataservice.ProfileUser.Tweets = new ObservableCollection<TweetModel>();

            

            NavigateToBack = new RelayCommand(o =>
            {
                dataservice.ProfileUser.Tweets.Clear();
                Navigation.NavigateToBack();
            }, canExecute => true
            );
        }
    }
}

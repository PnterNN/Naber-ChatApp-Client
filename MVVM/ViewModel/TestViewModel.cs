using JavaProject___Client.MVVM.Model;
using JavaProject___Client.Services;
using JavaProject___Client.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Core;

namespace JavaProject___Client.MVVM.ViewModel
{
    class TestViewModel : JavaProject___Client.Core.ViewModel
    {
        public IDataService DataService { get; set; }
        public INavigationService Navigation { get; set; }

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

        public RelayCommand NavigateToBack { get; set; }

        public TestViewModel(INavigationService navService, IDataService dataservice)
        {
            DataService = dataservice;
            Navigation = navService;


            NavigateToBack = new RelayCommand(o =>
            {
                dataservice.ProfileUser.Tweets.Clear();
                Navigation.NavigateToBack();
            }, canExecute => true
            );
        }
    }
}

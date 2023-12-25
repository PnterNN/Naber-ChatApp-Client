using ChatApp.Core;
using JavaProject___Client.Core;
using JavaProject___Client.NET;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace JavaProject___Client.MVVM.ViewModel
{
    public class LoginViewModel : Core.ViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        private Server _server;
        public IDataService DataService { get; set; }

        private string _errorMessage { get; set; }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

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

        private bool _buttonState;
        public bool ButtonState
        {
            get { return _buttonState; }
            set
            {
                if (_buttonState != value)
                {
                    _buttonState = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoginCorrect()
        {
            ButtonState = true;
            Navigation.NavigateTo<HomeViewModelProfile>();
            Navigation.NavigateTo<HomeViewModelUsers>();
            Navigation.NavigateTo<HomeViewModelTweet>();
        }
        public void LoginFailed()
        {
            ButtonState = true;
            ErrorMessage = "Login failed";
        }

        public async void TooManyPackets()
        {
            ButtonState = false;
            ErrorMessage = "Çok fazla deneme yaptın 1 dakika bekle";
            await Task.Delay(60000);
            ButtonState = true;

        }

        public RelayCommand Login { get; set; }

        public RelayCommand NavigatoToRegister { get; set; }

        public LoginViewModel(INavigationService navService, IDataService dataservice)
        {
            ButtonState = true;
            DataService = dataservice;
            Navigation = navService;

            _server = new Server();
            DataService.server = _server;

            Login = new RelayCommand(o =>
            {
                if (Email != null && Password != null && Email != "" && Password != "")
                {
                    ButtonState = false;
                    DataService.server.Login(Email, Password);
                }
                else
                {
                    ErrorMessage = "Please fill in all fields";
                }
                
            }, canExecute => true
            );

            NavigatoToRegister = new RelayCommand(o =>
            {
                Navigation.NavigateTo<RegisterViewModel>();
            }, canExecute => true
            );

            DataService.server.LoginCorrectEvent += LoginCorrect;
            DataService.server.LoginFailEvent += LoginFailed;
            DataService.server.TooManyPacketsEvent += TooManyPackets;
        }
    }
}

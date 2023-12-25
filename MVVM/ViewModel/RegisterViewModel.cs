using ChatApp.Core;
using JavaProject___Client.NET;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class RegisterViewModel : Core.ViewModel
    {
        public IDataService DataService { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

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

        private Server _server;

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

        public void RegisterSuccess()
        {
            ErrorMessage = "Kayıt olundu, yönlendiriliyorsunuz...";
            Task.Delay(1000).ContinueWith(t =>
            {
                ButtonState = true;
                Navigation.NavigateTo<HomeViewModelProfile>();
                Navigation.NavigateTo<HomeViewModelUsers>();
                Navigation.NavigateTo<HomeViewModelTweet>();
            });
        }

        public void RegisterFailed()
        {
            ErrorMessage = "Böyle bir hesap var";
            ButtonState = true;
        }

        public async void TooManyPackets()
        {
            ButtonState = false;
            ErrorMessage = "Çok fazla deneme yaptın 1 dakika bekle";
            await Task.Delay(60000);
            ButtonState = true;
        }

        public RelayCommand RegisterToServer { get; set; }

        public RelayCommand NavigateToLogin { get; set; }
        public RegisterViewModel(INavigationService navService, IDataService dataservice)
        {
            ButtonState = true;
            DataService = dataservice;
            Navigation = navService;

            _server = DataService.server;

            Regex regex_email = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            Regex regex_password = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,15}$");   

            

            RegisterToServer = new RelayCommand(o =>
            {
                if (Username != null &&Email != null && Password != null && Username != "" && Email != "" && Password != "")
                {
                    Match match_email = regex_email.Match(Email);
                    Match match_password = regex_password.Match(Password);
                    if (match_email.Success)
                    {
                        if (match_password.Success)
                        {
                            _server.Register(Username, Email, Password);
                            ButtonState = false;
                        }
                        else
                        {
                            ErrorMessage = "Please enter a valid password";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Please enter a valid email address";
                    }
                    
                }
                else
                {
                    ErrorMessage = "Please enter user information";
                }
                
            }, canExecute => true
            );

            NavigateToLogin = new RelayCommand(o =>
            {
                Navigation.NavigateTo<LoginViewModel>();
            }, canExecute => true
            );

            _server.RegisterSuccessEvent += RegisterSuccess;
            _server.RegisterFailEvent += RegisterFailed;
            _server.TooManyPacketsEvent += TooManyPackets;


        }
    }
}

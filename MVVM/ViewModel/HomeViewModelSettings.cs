using ChatApp.Core;
using JavaProject___Client.Services;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class HomeViewModelSettings : Core.ViewModel
    {
        public int MicrophoneDevice 
        {
            get => DataService.MicrophoneDevice;
            set
            {
                DataService.MicrophoneDevice = value;
                OnPropertyChanged();
            }
        }
        public int SpeakerDevice
        {
            get => DataService.SpeakerDevice;
            set
            {
                DataService.SpeakerDevice = value;
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

        public HomeViewModelSettings(INavigationService navService, IDataService dataservice)
        {
            DataService = dataservice;
            Navigation = navService;
            NavigateToBack = new RelayCommand(o =>
            {
                Navigation.NavigateToBack();
            }, canExecute => true
            );
        }
    }
}

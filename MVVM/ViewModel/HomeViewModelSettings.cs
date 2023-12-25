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
        private string _microphoneDevice;
        public string MicrophoneDevice 
        {
            get => _microphoneDevice;
            set
            {
                _microphoneDevice = value;
                OnPropertyChanged();
            }
        }

        private string _speakerDevice;
        public string SpeakerDevice
        {
            get => _speakerDevice;
            set
            {
                _speakerDevice = value;
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
            if (DataService.MicrophoneDevice != null)
            {
                MicrophoneDevice = DataService.MicrophoneDevice;
                SpeakerDevice = DataService.SpeakerDevice;
            }
            

            NavigateToBack = new RelayCommand(o =>
            {
                DataService.MicrophoneDevice = MicrophoneDevice;
                DataService.SpeakerDevice = SpeakerDevice;
                Navigation.NavigateToBack();
            }, canExecute => true
            );
        }
    }
}

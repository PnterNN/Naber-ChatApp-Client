using ChatApp.Core;
using JavaProject___Client.Services;
using Microsoft.VisualBasic.Devices;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JavaProject___Client.MVVM.Model
{
    public class MessageModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        private WaveOut waveOut;
        private TimeSpan duration;
        private FileStream fs;
        public RelayCommand DeleteMessage { get; set; }

        public RelayCommand SoundCommand { get; set; }

        private void deleteMessage(object param)
        {
            if (param != null)
            {
                _dataService.server.DeleteMessage(param.ToString(), _dataService.SelectedUser.UID);
            }
        }
        public MessageModel(INavigationService navService, IDataService dataservice)
        {
            _dataService = dataservice;
            _navigationService = navService;
            DeleteMessage = new RelayCommand((param) => deleteMessage(param));


            _ = Task.Run(() =>
            {
                Task.Delay(100).Wait();
                if (VoiceMessage == true)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        fs = new FileStream("cache/" + UID + ".wav", FileMode.Open);
                        waveOut = new WaveOut();
                        waveOut.DeviceNumber = dataservice.SpeakerDevice;
                        WaveFileReader wavReader = new WaveFileReader(fs);

                        
                        duration = wavReader.TotalTime;
                        waveOut.Init(wavReader);
                        waveOut.PlaybackStopped += (sender, e) =>
                        {
                            fs.Position = 0;
                            fs.Seek(0, SeekOrigin.Begin);
                            SoundName = UID + ".wav";
                            State = "Play";
                        };
                    });
                }
            });

            SoundCommand = new RelayCommand(o =>
            {
                if (State == "Play")
                {
                    State = "Pause";
                    try
                    {
                        waveOut.Play();
                        _ = Task.Run(() =>
                        {
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                SoundName = waveOut.GetPositionTimeSpan().ToString(@"mm\:ss") + "/" + duration.ToString(@"mm\:ss");
                            }
                        });
                    }
                    catch
                    {

                    }
                }
                else
                {
                    State = "Play";
                    try
                    {
                        waveOut.Pause();
                    }
                    catch
                    {
                            
                    }
                }
            });
        }
        public string Username { get; set; }
        public bool? ownMessage { get; set; }
        public string ImageSource { get; set; }
        public string UsernameColor { get; set; }
        public string Message { get; set; }
        public string UID { get; set; }
        public DateTime Time { get; set; }
        public bool? FirstMessage { get; set; }
        private string _soundName;

        public string SoundName
        {
            get { return _soundName; }
            set
            {
                _soundName = value;
                OnPropertyChanged();
            }
        }
        public bool VoiceMessage { get; set; }

        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }
    }
}
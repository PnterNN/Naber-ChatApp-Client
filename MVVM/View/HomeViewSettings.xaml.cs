using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JavaProject___Client.MVVM.View
{
    /// <summary>
    /// HomeViewSettings.xaml etkileşim mantığı
    /// </summary>
    public partial class HomeViewSettings : UserControl
    {
        public HomeViewSettings()
        {
            InitializeComponent();

            audioDeviceLoad();
            
        }
        public static string[] GetOutputDevices()
        {
            var enumerator = new MMDeviceEnumerator();

            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).
                Select(endpoint => endpoint.FriendlyName).ToArray();
        }
        public static string[] GetInputDevices()
        {
            var enumerator = new MMDeviceEnumerator();

            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).
                Select(endpoint => endpoint.FriendlyName).ToArray();
        }

        private void audioDeviceLoad()
        {
            var outDevices = GetOutputDevices();
            foreach (var device in outDevices)
                SettingsSpeaker.Items.Add(device);

            var inDevices = GetInputDevices();
            foreach (var device in inDevices)
                SettingsMicrophone.Items.Add(device);
        }
    }
}

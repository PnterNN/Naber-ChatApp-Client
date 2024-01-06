using ChatApp.Core;
using JavaProject___Client.NET;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JavaProject___Client.MVVM.ViewModel
{
    internal class MainViewModel : Core.ViewModel
    {
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

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo("cache");
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {

                }
                
            }
        }
        public MainViewModel(INavigationService navService, IDataService dataservice)
        {
            DataService = dataservice;
            Navigation = navService;
            Navigation.NavigateTo<LoginViewModel>();

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }
    }
}

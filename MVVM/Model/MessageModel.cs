using ChatApp.Core;
using JavaProject___Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JavaProject___Client.MVVM.Model
{
    public class MessageModel
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        public RelayCommand DeleteMessage { get; set; }

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
        }
        public string Username { get; set; }
        public bool? ownMessage { get; set; }
        public string ImageSource { get; set; }
        public string UsernameColor { get; set; }
        public string Message { get; set; }
        public string UID { get; set; }
        public DateTime Time { get; set; }
        public bool? FirstMessage { get; set; }
    }
}

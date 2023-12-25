using ChatApp.Core;
using JavaProject___Client.Core;
using JavaProject___Client.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JavaProject___Client.Services
{
    public interface INavigationService
    {
        ViewModel CurrentView { get; }
        ViewModel LastView { get; }
        void NavigateTo<T>() where T : ViewModel;
        void NavigateToBack();
    }
    internal class NavigationService : ObservableObject, INavigationService
    {

        private readonly Func<Type, ViewModel> _viewModelFactory;
        private ViewModel _currentView;
        private ViewModel _lastView;

        public ViewModel LastView
        {
            get => _lastView;
            private set
            {
                _lastView = value;
                OnPropertyChanged();
            }
        }
        public ViewModel CurrentView
        {
            get => _currentView;
            private set 
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }
        public void NavigateToBack()
        {
            CurrentView = LastView;
        }
        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            
            ViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            if(CurrentView != null)
            {
                LastView = CurrentView;
            }
            else
            {
                LastView = viewModel;
            }
            
            CurrentView = viewModel;
        }   
    }
}

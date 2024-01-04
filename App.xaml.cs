using JavaProject___Client.Core;
using JavaProject___Client.MVVM.View;
using JavaProject___Client.MVVM.ViewModel;
using JavaProject___Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JavaProject___Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<MainViewModel>();


            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<TestViewModel>();

            services.AddSingleton<HomeViewModelSettings>();
            services.AddSingleton<HomeViewModelTweet>();
            services.AddSingleton<HomeViewModelProfile>();
            services.AddSingleton<HomeViewModelUsers>();

            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));
            _serviceProvider = services.BuildServiceProvider(); 

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
                        
        }
    }
}

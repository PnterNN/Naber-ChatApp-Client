using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
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
    /// HomeView_Tweet.xaml etkileşim mantığı
    /// </summary>
    public partial class HomeViewTweet : UserControl
    {
        public HomeViewTweet()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)tweets.Items).CollectionChanged += ListView_CollectionChanged;
        }
        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                tweets.ScrollIntoView(e.NewItems[0]);
            }
        }
    }
}

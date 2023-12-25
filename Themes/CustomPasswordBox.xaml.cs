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

namespace JavaProject___Client.Themes
{
    /// <summary>
    /// CustomPasswordBox.xaml etkileşim mantığı
    /// </summary>
    /// 

    public partial class CustomPasswordBox : UserControl
    {

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(CustomPasswordBox));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public CustomPasswordBox()
        {
            InitializeComponent();
            customPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = customPasswordBox.Password;
        }
    }
}

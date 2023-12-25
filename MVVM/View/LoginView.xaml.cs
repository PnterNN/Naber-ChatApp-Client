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
    /// LoginView.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Username")
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Username";
            }
        }

        private void TextBox_GotFocus2(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Password")
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus2(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Password";
            }
        }

        private void ForgotPassword_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deneme!");
        }

        private void SignUp_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deneme!");

        }




        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Deneme!");
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Deneme!");
        }
    }
}

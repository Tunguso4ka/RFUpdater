using System.Windows;
using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interakční logika pro LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        void LoginCheck()
        {
            if (LoginTextBox.Text == "0" && LoginPasswordBox.Password == "0")
            {
                Properties.Settings.Default.UserAuthorizited = true;
                Properties.Settings.Default.Save();
                (new MainWindow()).Show();
                ((MainWindow)Window.GetWindow(this)).Close();
            }
            else
            {

            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "";
            LoginPasswordBox.Password = "";
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text != "" && LoginPasswordBox.Password != "")
            {
                LoginCheck();
            }
        }
    }
}

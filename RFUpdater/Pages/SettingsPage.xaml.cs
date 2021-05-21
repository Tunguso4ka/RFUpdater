using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        AboutWindow AboutWindow = new AboutWindow();
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\settings.dat";
        bool LogOutPressed;
        public SettingsPage()
        {
            InitializeComponent();
            CheckAndSet();
            ChangeTheme();
        }

        void ChangeTheme()
        {
            if(Properties.Settings.Default.ThemeNum == 0)
            {
                LogOutBtn.Style = (Style)FindResource("ButtonStyleGreen");
                AboutBtn.Style = (Style)FindResource("ButtonStyleGreen");
                SaveBtn.Style = (Style)FindResource("ButtonStyleGreen");
            }
        }

        void CheckAndSet()
        {
            if (Properties.Settings.Default.UpdaterLanguage == "en")
            {
                LRB0.IsChecked = true;
            }
            else
            {
                LRB1.IsChecked = true;
            }
            if (Properties.Settings.Default.AutoUpdate == true)
            {
                CB0.IsChecked = true;
            }
            if (Properties.Settings.Default.SaveFolderPath == @"C:\Games\")
            {
                LRB2.IsChecked = true;
            }
            else
            {
                LRB3.IsChecked = true;
            }
            if (Properties.Settings.Default.ThemeNum == 0)
            {
                LRB4.IsChecked = true;
            }
            else if (Properties.Settings.Default.ThemeNum == 1)
            {
                LRB5.IsChecked = true;
            }
            else
            {
                LRB6.IsChecked = true;
            }
            if (Properties.Settings.Default.UserAuthorizited == false)
            {
                LogOutBtn.Foreground = Brushes.DarkGray;
                LogOutBtn.IsEnabled = false;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //language
            if (LRB0.IsChecked == true)
            {
                Properties.Settings.Default.UpdaterLanguage = "en";
            }
            else
            {
                Properties.Settings.Default.UpdaterLanguage = "ru";
            }

            //autoupdate
            if (CB0.IsChecked == true)
            {
                Properties.Settings.Default.AutoUpdate = true;
            }
            else
            {
                Properties.Settings.Default.AutoUpdate = false;
            }

            //folder
            if (LRB2.IsChecked == true)
            {
                Properties.Settings.Default.SaveFolderPath = @"C:\Games\";
            }
            else
            {
                Properties.Settings.Default.SaveFolderPath = @"D:\Games\";
            }

            //themes
            if (LRB4.IsChecked == true)
            {
                Properties.Settings.Default.ThemeNum = 0;
            }
            else if (LRB5.IsChecked == true)
            {
                Properties.Settings.Default.ThemeNum = 1;
            }
            else
            {
                Properties.Settings.Default.ThemeNum = 2;
            }

            //save
            Properties.Settings.Default.Save();

            //restart app
            new MainWindow().Show();
            Window.GetWindow(this).Close();
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LogOutPressed == true)
            {
                Properties.Settings.Default.UserAuthorizited = true;
                LogOutBtn.Content = "Log out";
                LogOutPressed = false;
            }
            else
            {
                Properties.Settings.Default.UserAuthorizited = false;
                LogOutBtn.Content = "Cancel";
                LogOutPressed = true;
            }
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow.ShowInTaskbar = false;
            AboutWindow.Show();
        }
    }
}
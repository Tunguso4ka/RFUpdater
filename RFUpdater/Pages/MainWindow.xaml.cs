﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Thread settingsThread = new Thread(() => { });

        Forms.NotifyIcon notifyIcon;

        string SettingsPath;
        string DownGamesPath;
        string RFUUpdateInfoUrl = @"https://drive.google.com/uc?export=download&id=1oKyTppE7V8E-Q0UF0_SXNmW3diQ0QbLJ";
        string RFUUpdateInfoPath;
        Version NewRFUVersion;

        //pages
        public SettingsPage ASettingsPage;
        public StartPage AStartPage;
        public LibraryPage ALibraryPage;
        public SearchPage ASearchPage;
        public UserPage AUserPage;
        public LoginPage ALoginPage;


        //ints

        //strings[]
        public string[] SavedGamesInfo = new string[99];

        public MainWindow()
        {
            InitializeComponent();

            CreateNotifyIcon();

            Properties.Settings.Default.AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\";
            Properties.Settings.Default.Save();

            //UpdateCheckerWindow updateCheckerWindow = new UpdateCheckerWindow();
            //updateCheckerWindow.Show();

            if (!Directory.Exists(Properties.Settings.Default.AppDataPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.AppDataPath);
            }
            if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\");
            }
            Checks();

            Frame0.Navigate(AStartPage);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void Pages()
        {
            AStartPage = new StartPage();
            ASettingsPage = new SettingsPage();
            ALibraryPage = new LibraryPage();
            ASearchPage = new SearchPage();
            AUserPage = new UserPage();
            ALoginPage = new LoginPage();
        }
        void Checks()
        {

            StringsSet();
            SettingsSearch();
            UpdatesChecking();
            BetaCheck();
            AuthorizCheck();
            ThemeSet();
            Pages();
        }

        void SettingsSearch()
        {
            if (File.Exists(DownGamesPath))
            {
                try
                {
                    BinaryReader BinaryReader = new BinaryReader(File.OpenRead(DownGamesPath));
                    int LineNum = 0;
                    while (BinaryReader.PeekChar() > -1)
                    {
                        SavedGamesInfo[LineNum] = BinaryReader.ReadString();
                        LineNum++;
                    }
                    BinaryReader.Dispose();

                    //MessageBox.Show(SavedGamesInfo[0]);

                    Properties.Settings.Default.SavedGamesIsReal = true;
                }
                catch
                {
                    Properties.Settings.Default.SavedGamesIsReal = false;
                    MessageBox.Show(SavedGamesInfo[0], "Error");
                }
            }
            else
            {
                Properties.Settings.Default.SavedGamesIsReal = false;
            }

        }
        void UpdatesChecking()
        {
            try
            {
                if (File.Exists(RFUUpdateInfoPath))
                {
                    File.Delete(RFUUpdateInfoPath);
                }
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(RFUUpdateInfoUrl, RFUUpdateInfoPath);
                using (StreamReader StreamReader = new StreamReader(RFUUpdateInfoPath))
                {
                    NewRFUVersion = new Version(StreamReader.ReadLine());
                    StreamReader.Dispose();
                }
                File.Delete(RFUUpdateInfoPath);

                switch (NewRFUVersion.CompareTo(Assembly.GetExecutingAssembly().GetName().Version))
                {
                    case 0:
                         //такая же
                        break;
                    case 1:
                        if (File.Exists(Properties.Settings.Default.AppDataPath + "RFUI.exe"))
                        {
                            Process.Start(Properties.Settings.Default.AppDataPath + "RFUI.exe");
                        }
                         //новее
                        break;
                }
            }
            catch
            {

            }
        }
        void BetaCheck()
        {
            //Properties.Settings.Default.IsBetaOn
            if(Properties.Settings.Default.IsBetaOn == true)
            {
                SearchBtn.Visibility = Visibility.Visible;
                UserBtn.Visibility = Visibility.Visible;
            }
        }
        void AuthorizCheck()
        {
            //Properties.Settings.Default.UserAuthorizited
            if (Properties.Settings.Default.UserAuthorizited == true)
            {
                UserBtn.Content = "";
                UserBtn.ToolTip = Properties.Settings.Default.UserName;
            }
        }
        void ThemeSet()
        {
            if (Properties.Settings.Default.ThemeNum == 0)
            {
                WindowBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 69, 65));
                FirstGrid.Background = new SolidColorBrush(Color.FromRgb(65, 139, 130));
                SecondGrid.Background = new SolidColorBrush(Color.FromRgb(79, 168, 158));

                MenuBtn.Style = (Style)FindResource("ButtonStyleGreen");
                SearchBtn.Style = (Style)FindResource("ButtonStyleGreen");
                LibraryBtn.Style = (Style)FindResource("ButtonStyleGreen");
                SettingsBtn.Style = (Style)FindResource("ButtonStyleGreen");
                UserBtn.Style = (Style)FindResource("ButtonStyleGreen");
                MinimBtn.Style = (Style)FindResource("ButtonStyleGreen");
                CloseBtn.Style = (Style)FindResource("ButtonStyleGreen");
            }
        }
        void StringsSet()
        {
            //Properties.Settings.Default.AppDataPath
            SettingsPath = Properties.Settings.Default.AppDataPath + "settings.dat";
            RFUUpdateInfoPath = Properties.Settings.Default.AppDataPath + "RFUV.txt";
            DownGamesPath = Properties.Settings.Default.AppDataPath + "gamesonthispc.dat";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if((string)ClickedButton.Tag == "Settings")
            {
                Frame0.Navigate(ASettingsPage);
            }
            else if ((string)ClickedButton.Tag == "Library")
            {
                Frame0.Navigate(ALibraryPage);
            }
            else if ((string)ClickedButton.Tag == "Close")
            {
                if (Properties.Settings.Default.Installing == false)
                {
                    this.WindowState = WindowState.Minimized;
                    this.ShowInTaskbar = false;
                }
                else
                {
                    this.WindowState = WindowState.Minimized;
                    MessageBox.Show("You can't close Updater while it doing its work.");
                }
            }
            else if ((string)ClickedButton.Tag == "Minimize")
            {
                this.WindowState = WindowState.Minimized;
            }
            else if ((string)ClickedButton.Tag == "Maximize")
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    ClickedButton.Content = "";
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Maximized;
                    ClickedButton.Content = "";
                    this.WindowStyle = WindowStyle.None;
                }
            }
            else if ((string)ClickedButton.Tag == "Menu")
            {
                Frame0.Navigate(AStartPage);
            }
            else if ((string)ClickedButton.Tag == "User")
            {
                if (Properties.Settings.Default.UserAuthorizited == true)
                {
                    Frame0.Navigate(AUserPage);
                }
                else
                {
                    Frame0.Navigate(ALoginPage);
                }
            }
            else if ((string)ClickedButton.Tag == "Search")
            {
                Frame0.Navigate(ASearchPage);
            }
            else if ((string)ClickedButton.Tag == "Messages")
            {
                //Frame0.Navigate(MessagesPage);
            }
        }

        private void CreateNotifyIcon()
        {

            notifyIcon = new Forms.NotifyIcon(new Container());
            ///*
            Forms.ContextMenuStrip _ContextMenuStrip = new Forms.ContextMenuStrip();

            Forms.ToolStripMenuItem _StripMenuItemAppName = new Forms.ToolStripMenuItem();

            _ContextMenuStrip.Items.AddRange(
                new Forms.ToolStripMenuItem[]
                {
                    _StripMenuItemAppName,
                    new Forms.ToolStripMenuItem("Main page", null, new EventHandler(StartPageClicked)),
                    new Forms.ToolStripMenuItem("Game library", null, new EventHandler(LibraryPageClicked)),
                    new Forms.ToolStripMenuItem("Exit", null, new EventHandler(ExitClicked))
                }
            );

            _StripMenuItemAppName.Text = "RFUpdater";
            _StripMenuItemAppName.Enabled = false;
            _StripMenuItemAppName.Image = Properties.Resources.rfulogo0525;

            notifyIcon.Icon = Properties.Resources.rfulogo0525ico;
            notifyIcon.ContextMenuStrip = _ContextMenuStrip;
            notifyIcon.Text = "RFUpdater";
            notifyIcon.Visible = true;
            //*/
            
            /*
            Forms.ContextMenu _ContextMenu = new Forms.ContextMenu();

            Forms.MenuItem _MenuItemAppName = new Forms.MenuItem();

            _ContextMenu.MenuItems.AddRange(
                new Forms.MenuItem[]
                {
                    _MenuItemAppName,
                    new Forms.MenuItem("-"),
                    new Forms.MenuItem("Main page", new EventHandler(StartPageClicked)),
                    new Forms.MenuItem("Game library", new EventHandler(LibraryPageClicked)),
                    new Forms.MenuItem("-"),
                    new Forms.MenuItem("Exit", new EventHandler(ExitClicked))
                }
            );

            _MenuItemAppName.Text = "RFUpdater";
            _MenuItemAppName.Enabled = false;

            notifyIcon.Icon = Properties.Resources.rfuball_XtN_icon;
            notifyIcon.ContextMenu = _ContextMenu;
            notifyIcon.Text = "RFUpdater";
            notifyIcon.Visible = true;

            */

            notifyIcon.MouseDown += new Forms.MouseEventHandler(NotifyIconClicked);
            
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Maximized;
                MaximizeBtn.Content = "";
                this.WindowStyle = WindowStyle.None;
            }
        }

        /* 
         public SettingsPage ASettingsPage;
        public StartPage AStartPage;
        public LibraryPage ALibraryPage;
        public SearchPage ASearchPage;
        public UserPage AUserPage;
         */

        [STAThread]
        private void NotifyIconClicked(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                Frame0.Content = AStartPage;
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }
        private void StartPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = AStartPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }
        private void LibraryPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = ALibraryPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ExitClicked(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        public void KillNotifyIcon()
        {
            notifyIcon.Dispose();
        }
    }
}

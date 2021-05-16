using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

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
            Frame0.Content = AStartPage;
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
                FirstGrid.Background = new SolidColorBrush(Color.FromRgb(6, 90, 130));
                SecondGrid.Background = new SolidColorBrush(Color.FromRgb(24, 85, 114));
            }
        }
        void StringsSet()
        {
            //Properties.Settings.Default.AppDataPath
            SettingsPath = Properties.Settings.Default.AppDataPath + "settings.dat";
            RFUUpdateInfoPath = Properties.Settings.Default.AppDataPath + "RFUV.txt";
            DownGamesPath = Properties.Settings.Default.AppDataPath + "gamesonthispc.dat";
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = ASettingsPage;
        }

        private void LibraryBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = ALibraryPage;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Installing == false)
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
                System.Windows.MessageBox.Show("You can't close Updater while he doing hes work.");
            }
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = AStartPage;
        }

        private void MinimBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.UserAuthorizited == true)
            {
                Frame0.Content = AUserPage;
            }
            else
            {
                Frame0.Content = ALoginPage;
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = ASearchPage;
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
            _StripMenuItemAppName.Image = Properties.Resources.rfulogo0516;

            notifyIcon.Icon = Properties.Resources.rfulogo0516ico;
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
    }
}

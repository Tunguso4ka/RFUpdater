using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread settingsThread = new Thread(() => { });

        NotifyIcon notifyIcon;

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
        public string[] SavedGamesVersions = new string[99];
        public string[] SavedGamesPaths = new string[99];

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
                    string Line;
                    string[] LineList;
                    int LineNum = 0;
                    while (BinaryReader.PeekChar() > -1)
                    {
                        Line = BinaryReader.ReadString();
                        LineList = Line.Split('}');
                        SavedGamesVersions[LineNum] = LineList[0];
                        SavedGamesPaths[LineNum] = LineList[1];
                        LineNum++;
                    }
                    BinaryReader.Dispose();

                    //MessageBox.Show(((MainWindow)Window.GetWindow(this)).SavedGamesVersions[0] + "}" + ((MainWindow)Window.GetWindow(this)).SavedGamesPaths[0]);

                    Properties.Settings.Default.SavedGamesIsReal = true;
                }
                catch
                {
                    Properties.Settings.Default.SavedGamesIsReal = false;
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
            notifyIcon = new NotifyIcon(new Container());
            ContextMenu context_menu = new ContextMenu();

            context_menu.MenuItems.AddRange(new MenuItem[] {new MenuItem("Main page", new EventHandler(StartPageClicked)),new MenuItem("Library", new EventHandler(LibraryPageClicked)), new MenuItem("Search", new EventHandler(SearchPageClicked)), new MenuItem("Settings", new EventHandler(SettingsPageClicked)), new MenuItem("Close", new EventHandler(ExitClicked)) });

            notifyIcon.Icon = Properties.Resources.RFUicon;
            notifyIcon.ContextMenu = context_menu;
            notifyIcon.Text = "RFU";
            notifyIcon.Visible = true;
        }

        /* 
         public SettingsPage ASettingsPage;
        public StartPage AStartPage;
        public LibraryPage ALibraryPage;
        public SearchPage ASearchPage;
        public UserPage AUserPage;
         */

        [STAThread]
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
        private void SearchPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = ASearchPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }
        private void SettingsPageClicked(object sender, EventArgs e)
        {
            Frame0.Content = ASettingsPage;
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ExitClicked(object sender, EventArgs e)
        {
            Close();
            notifyIcon.Dispose();
        }
    }
}

using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\";
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\settings.dat";
        string Language;
        string RFUUpdateInfoUrl = @"https://drive.google.com/uc?export=download&id=1oKyTppE7V8E-Q0UF0_SXNmW3diQ0QbLJ";
        string RFUUpdateInfoPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUV.txt";
        string Game0pdateInfoPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFV.txt";
        string Game0UpdateInfoUrl = @"https://drive.google.com/uc?export=download&id=1-vMmTTLHl7z3O-cAMD_ubUeW1WJyL5ID";
        string Game0Name;
        string Game0Path;
        string Game0UpdateUrl = @"https://drive.google.com/uc?export=download&confirm=no_antivirus&id=1kMrTP1cCcUwDVvQCOYi-7Qs-f9htvpm9";
        string SaveFolderPath;
        Version Game0Version;
        int GameStatus;
        Version NewRFUVersion;
        Version OldRFUVersion;
        bool AutoUpdate;
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            regKey.SetValue("RFUpdater", Assembly.GetExecutingAssembly().Location);
            regKey.Close();
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            Language = culture.ToString();
            SettingsSearch();
            UpdatesChecking();
            Frame0.Content = new StartPage(GameStatus, Language);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void SettingsSearch()
        {
            if (File.Exists(SettingsPath))
            {
                try
                {
                    BinaryReader BinaryReader = new BinaryReader(File.OpenRead(SettingsPath));
                    Language = BinaryReader.ReadString();
                    Game0Name = BinaryReader.ReadString();
                    Game0Version = new Version(BinaryReader.ReadString());
                    Game0Path = BinaryReader.ReadString();
                    GameStatus = BinaryReader.ReadInt32();
                    AutoUpdate = BinaryReader.ReadBoolean();
                    SaveFolderPath = BinaryReader.ReadString();
                    BinaryReader.Dispose();
                }
                catch
                {
                    Game0Name = "Random Fights";
                    GameStatus = 0;
                }
            }
            else
            {
                Game0Name = "Random Fights";
                Game0Path = "";
                GameStatus = 0;
            }

        }
        void UpdatesChecking()
        {
            try
            {
                if(File.Exists(RFUUpdateInfoPath))
                {
                    File.Delete(RFUUpdateInfoPath);
                }
                if (File.Exists(Game0pdateInfoPath))
                {
                    File.Delete(Game0pdateInfoPath);
                }
                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(RFUUpdateInfoUrl, RFUUpdateInfoPath);
                using (StreamReader StreamReader = new StreamReader(RFUUpdateInfoPath))
                {
                    NewRFUVersion = new Version(StreamReader.ReadLine());
                    StreamReader.Dispose();
                }
                File.Delete(RFUUpdateInfoPath);
                WebClient.DownloadFile(Game0UpdateInfoUrl, Game0pdateInfoPath);
                
                using (StreamReader StreamReader = new StreamReader(Game0pdateInfoPath))
                {
                    Game0Version = new Version(StreamReader.ReadLine());
                    Game0UpdateUrl = StreamReader.ReadLine();
                    StreamReader.Dispose();
                }
                File.Delete(Game0pdateInfoPath);
                
            }
            catch
            {
                using (StreamReader StreamReader = new StreamReader(RFUUpdateInfoPath))
                {
                    NewRFUVersion = new Version(StreamReader.ReadLine());
                    Game0UpdateInfoUrl = StreamReader.ReadLine();
                    StreamReader.Dispose();
                }

                using (StreamReader StreamReader = new StreamReader(Game0pdateInfoPath))
                {
                    Game0Version = new Version(StreamReader.ReadLine());
                    Game0UpdateUrl = StreamReader.ReadLine();
                    StreamReader.Dispose();
                }
            }
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            string gameName = Game0Name, GamePath = Game0Path, GameUpdateUrl = Game0UpdateUrl;
            Version gameVersion = Game0Version;
            Frame0.Content = new SettingsPage(gameName, gameVersion, GamePath, GameUpdateUrl, GameStatus, Language, AutoUpdate, SaveFolderPath);
        }

        private void GameBtn0_Click(object sender, RoutedEventArgs e)
        {
            string gameName = Game0Name, GamePath = Game0Path, GameUpdateUrl = Game0UpdateUrl;
            Version gameVersion = Game0Version;
            Frame0.Content = new GamePage(gameName, gameVersion, GamePath, GameUpdateUrl, GameStatus, Language, AutoUpdate, SaveFolderPath);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = new StartPage(GameStatus, Language);
        }

        private void ServersBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame0.Content = new ServersPage();
        }

        private void MinimBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DownloadingBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

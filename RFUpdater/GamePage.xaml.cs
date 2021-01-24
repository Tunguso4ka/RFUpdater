using System;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Diagnostics;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        string GameName;
        Version GameVersion;
        string FolderPath;
        string GameFolderPath;
        string GamePath;
        string ZipPath;
        string GameUpdateUri;
        int GameStatus;
        string Language;
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\settings.dat";
        bool AutoUpdate;

        public GamePage(string gameName, Version gameVersion, string gamePath, string gameUpdateUrl, int gameStatus, string language, bool autoUpdate, string gameFolderPath)
        {
            InitializeComponent();
            GameName = gameName;
            GameNameTextBlock.Text = GameName;
            GameVersion = gameVersion;
            VersionTextBlock.Text = "Version: " + GameVersion;
            GamePath = gamePath;
            GameUpdateUri = gameUpdateUrl;
            GameStatus = gameStatus;
            Language = language;
            AutoUpdate = autoUpdate;
            GameFolderPath = gameFolderPath;
            ProgressBar0.Visibility = Visibility.Hidden;
            DownSpeedTextBlock.Visibility = Visibility.Hidden;
            if (GameStatus == 0)
            {
                StatusTextBlock.Text = "Status: Not installed.";
                InstallBtn.Content = "⬇💾Install";
                DeleteBtn.Visibility = Visibility.Hidden;
            }
            else if (GameStatus == 1)
            {
                StatusTextBlock.Text = "Status: Installed.";
                InstallBtn.Content = "🎮Play";
                DeleteBtn.Visibility = Visibility.Visible;
            }
            else if (GameStatus == 2)
            {
                StatusTextBlock.Text = "Status: Update found.";
                InstallBtn.Content = "🆕Update";
                DeleteBtn.Visibility = Visibility.Visible;
            }
        }

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            Install();
        }

        async void Install()
        {
            if (GameStatus == 0)
            {
                Installing();
            }
            else if (GameStatus == 1)
            {
                if(File.Exists(GamePath))
                {
                    Process.Start(GamePath);
                }
                else
                {
                    DeleteGame();
                }
            }
            else if (GameStatus == 2)
            {

            }
        }

        void Installing()
        {
            WebClient WebClient = new WebClient();
            FolderPath = GameFolderPath + GameName;
            FolderPath = FolderPath.Replace(' ', '_');
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            ZipPath = FolderPath + @"\RandomFights.zip";
            GamePath = FolderPath + @"\" + GameVersion + @"\RandomFights.exe";

            Uri UpdateUri = new Uri(GameUpdateUri);

            WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            WebClient.DownloadFileAsync(UpdateUri, ZipPath);

            InstallBtn.Visibility = Visibility.Hidden;
            ProgressBar0.Visibility = Visibility.Visible;
            DownSpeedTextBlock.Visibility = Visibility.Visible;
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e) 
        {
            ProgressBar0.Value = e.ProgressPercentage;
            DownSpeedTextBlock.Text = "Bytes received: " + e.BytesReceived; 
        }

        private void Completed(object sender, AsyncCompletedEventArgs e) 
        { 
            if (e.Error != null) 
            { 
                MessageBox.Show(e.Error.Message); 
            } 
            else 
            {
                GameStatus = 1;
                StatusTextBlock.Text = "Status: Installed.";
                InstallBtn.Content = "🎮Play";
                InstallBtn.Visibility = Visibility.Visible;
                DeleteBtn.Visibility = Visibility.Visible;
                ProgressBar0.Visibility = Visibility.Hidden;
                DownSpeedTextBlock.Visibility = Visibility.Hidden;

                ZipFile.ExtractToDirectory(ZipPath, FolderPath);
                File.Delete(ZipPath);

                BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
                BinaryWriter.Write(Language);
                BinaryWriter.Write(GameName);
                BinaryWriter.Write(Convert.ToString(GameVersion));
                BinaryWriter.Write(GamePath);
                BinaryWriter.Write(GameStatus);
                BinaryWriter.Write(AutoUpdate);
                BinaryWriter.Write(GameFolderPath);
                BinaryWriter.Dispose();
            } 
        }


        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(GamePath))
            {
                File.Delete(GameFolderPath);
                DeleteGame();
            }
            else
            {
                DeleteGame();
            }
        }
        void DeleteGame()
        {
            GameStatus = 0;
            StatusTextBlock.Text = "Status: Not installed.";
            InstallBtn.Content = "⬇💾Install";
            DeleteBtn.Visibility = Visibility.Hidden;

            BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
            BinaryWriter.Write(Language);
            BinaryWriter.Write(GameName);
            BinaryWriter.Write(Convert.ToString(GameVersion));
            BinaryWriter.Write(GamePath);
            BinaryWriter.Write(GameStatus);
            BinaryWriter.Write(AutoUpdate);
            BinaryWriter.Write(GameFolderPath);
            BinaryWriter.Dispose();
        }
    }
}

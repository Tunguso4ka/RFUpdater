using System;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        string GameName;
        Version NewGameVersion;
        Version ThisGameVersion;
        string FolderPath;
        string GamePath;
        string ZipPath;
        string GameUpdateUri;
        string SettingsPath = Properties.Settings.Default.AppDataPath + "gamesonthispc.dat";
        int GameStatus;
        int _Tag;
        int ThisUserLikeNum = 0;
        bool GameIsComingSoon;

        ProgressBar _ProgressBar;
        TextBlock _ProgressTextBox;

        DirectoryInfo FolderPathDirectory;

        public GamePage(LibraryPage _LibraryPage)
        {
            InitializeComponent();

            //
            GameName = _LibraryPage.GameName;
            GameNameTextBlock.Text = GameName;

            NewGameVersion = _LibraryPage.newGameVersion;
            ThisGameVersion = _LibraryPage.thisGameVersion;

            GamePath = _LibraryPage.gamePath;
            GameUpdateUri = _LibraryPage.GameUpdateUrl;
            _Tag = _LibraryPage._Tag;

            //MessageBox.Show(NewGameVersion + "", "0");

            if (_LibraryPage.AGameReleaseStatus == 0)
            {
                GameReleaseStatusTextBlock.Text = "⏳\nComing soon...";
                InstallBtn.IsEnabled = false;
                GameIsComingSoon = true;
            }
            else if (_LibraryPage.AGameReleaseStatus == 1)
            {
                GameReleaseStatusTextBlock.Text = "β";
            }
            else
            {
                GameReleaseStatusTextBlock.Text = "";
            }

            if (ThisGameVersion == new Version("0.0"))
            {
                GameStatus = -2;
            }
            else
            {
                switch (NewGameVersion.CompareTo(ThisGameVersion))
                {
                    case 0:
                        GameStatus = 0; //такая же
                        break;
                    case 1:
                        GameStatus = 1; //новее
                        break;
                    case -1:
                        GameStatus = -1; //старше
                        break;
                }
            }

            //новая и текущая версия вносятся в текстблоки
            if (GameIsComingSoon == false) {
                VersionTextBlock.Text = "This version: " + ThisGameVersion;
            }
            if (!NewGameVersion.Equals(ThisGameVersion))
            {
                VersionTextBlock.Text += " New version: " + NewGameVersion;
            }

            FolderPath = Properties.Settings.Default.SaveFolderPath + GameName;
            FolderPath = FolderPath.Replace(' ', '_');
            ProgressBar0.Visibility = Visibility.Hidden;
            DownSpeedTextBlock.Visibility = Visibility.Hidden;

            if (GameStatus == -2)
            {
                StatusTextBlock.Text = "Status: Not installed.";
                InstallBtn.Content = "⬇💾Install";
                InstallBtn.ToolTip = "Install";
                DeleteBtn.Visibility = Visibility.Hidden;
            }
            else if (GameStatus == 1)
            {
                StatusTextBlock.Text = "Status: Update found.";
                InstallBtn.Content = "🆕Update";
                InstallBtn.ToolTip = "Update";
                DeleteBtn.Visibility = Visibility.Visible;
            }
            else
            {
                StatusTextBlock.Text = "Status: Installed.";
                InstallBtn.Content = "🎮Play";
                InstallBtn.ToolTip = "Play";
                DeleteBtn.Visibility = Visibility.Visible;
            }

            InfoRead();

            ChangeTheme();

            //MessageBox.Show("" + GameUpdateUri);
        }

        void ChangeTheme()
        {
            if(Properties.Settings.Default.ThemeNum == 0)
            {
                InstallBtn.Style = (Style)FindResource("ButtonStyleGreen");
                DeleteBtn.Style = (Style)FindResource("ButtonStyleGreen");
                LikeBtn.Style = (Style)FindResource("ButtonStyleGreen");
                DisLikeBtn.Style = (Style)FindResource("ButtonStyleGreen");
                GameInfoHideBtn.Style = (Style)FindResource("ButtonStyleGreen");

                GameReleaseStatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(97, 214, 200));
            }
        }

        async void InfoRead()
        {
            try
            {
                string WhatsNewPath = FolderPath + @"\WhatsNew.txt";
                if (GameStatus != 0 && File.Exists(WhatsNewPath))
                {
                    StreamReader _StreamReader = new StreamReader(WhatsNewPath);
                    GameUpdateInfoTextBlock.Text = await _StreamReader.ReadToEndAsync();
                }
            }
            catch 
            {
                GameUpdateInfoTextBlock.Text = "Sorry. No info :(";
            }
        }

        private void Buttons_Click(object sender, RoutedEventArgs e)
        {
            Button _Button = (Button)sender;
            //MessageBox.Show((string)_Button.Tag);
            if ((string)_Button.Tag == "Install")
            {
                //Game Status check
                if (GameStatus == -2)
                {
                    MessageBox.Show("Installing");
                    Installing();
                }
                else if (GameStatus == 0)
                {
                    if (File.Exists(GamePath))
                    {
                        Process.Start(GamePath);
                    }
                    else
                    {
                        DeleteGame();
                    }
                }
                else if (GameStatus == 1)
                {
                    MessageBox.Show("Updating");
                    Installing();
                }
            }
            else if ((string)_Button.Tag == "Delete")
            {
                if (Directory.Exists(FolderPath))
                {
                    Directory.Delete(FolderPath, true);
                }
                DeleteGame();
            }
            else if ((string)_Button.Tag == "Like")
            {
                //like  
                ThisUserLikeNum = 1;
                LikeBtn.Content = "";
                DisLikeBtn.Content = "";
            }
            else if ((string)_Button.Tag == "DisLike")
            {
                //dislike 
                ThisUserLikeNum = 2;
                LikeBtn.Content = "";
                DisLikeBtn.Content = "";
            }
            else if ((string)_Button.Tag == "GameInfo")
            {
                if (GameInfoStackPanel.Visibility == Visibility.Collapsed)
                {
                    GameInfoStackPanel.Visibility = Visibility.Visible;
                    GameInfoHideBtn.ToolTip = "Hide info";
                }
                else
                {
                    GameInfoStackPanel.Visibility = Visibility.Collapsed;
                    GameInfoHideBtn.ToolTip = "Show info";
                }
            }

        }

        void Installing()
        {
            _ProgressBar = ((MainWindow)Window.GetWindow(this)).MainProgressBar;
            _ProgressTextBox = ((MainWindow)Window.GetWindow(this)).ProgressTextBlock;

            Properties.Settings.Default.Installing = true;
            WebClient WebClient = new WebClient();

            if (Directory.Exists(FolderPath))
            {
                Directory.Delete(FolderPath, true);
                Directory.CreateDirectory(FolderPath);
            }
            else
            {
                Directory.CreateDirectory(FolderPath);
            }

            ZipPath = FolderPath + @"\RandomFights.zip";
            GamePath = FolderPath + @"\RandomFights.exe";

            if(File.Exists(ZipPath))
            {
                File.Delete(ZipPath);
            }

            if (File.Exists(GamePath))
            {
                File.Delete(GamePath);
            }

            Uri UpdateUri = new Uri(GameUpdateUri);
            WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            WebClient.DownloadFileAsync(UpdateUri, ZipPath);

            InstallBtn.IsEnabled = false;
            //ProgressBar0.Visibility = Visibility.Visible;
            _ProgressBar.Visibility = Visibility.Visible;
            _ProgressTextBox.Visibility = Visibility.Visible;
            DownSpeedTextBlock.Visibility = Visibility.Visible;
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //ProgressBar0.Value = e.ProgressPercentage;
            _ProgressBar.Value = e.ProgressPercentage;
            _ProgressTextBox.Text = e.ProgressPercentage + "%";
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
                try
                {
                    MessageBox.Show("Complete");
                    GameStatus = 0;
                    StatusTextBlock.Text = "Status: Installed.";
                    InstallBtn.Content = "🎮Play";

                    ZipFile.ExtractToDirectory(ZipPath, FolderPath);
                    File.Delete(ZipPath);

                    BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
                    int i = 0;
                    while (i != 99)
                    {
                        if (i == _Tag)
                        {
                            BinaryWriter.Write(NewGameVersion + "}" + GamePath);
                        }
                        else
                        {
                            string[] _SavedGamesInfo = ((MainWindow)Window.GetWindow(this)).SavedGamesInfo[i].Split('}');
                            BinaryWriter.Write(_SavedGamesInfo[0] + "}" + _SavedGamesInfo[1]); //Version}Path
                        }
                        i++;
                    }

                    if (!Directory.Exists(Properties.Settings.Default.AppDataPath + @"Games\" + GameName))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + GameName);
                        Directory.CreateDirectory(Properties.Settings.Default.AppDataPath + @"Games\" + GameName + @"\Screenshots\");
                    }

                    //MessageBox.Show(((MainWindow)Window.GetWindow(this)).SavedGamesVersions[0] + "}" + ((MainWindow)Window.GetWindow(this)).SavedGamesPaths[0]);
                    BinaryWriter.Dispose();
                }
                catch
                {
                    MessageBox.Show("Error: Can't download this app, try later.", "Error");
                }
            }
            InstallBtn.IsEnabled = true;
            DeleteBtn.Visibility = Visibility.Visible;
            //ProgressBar0.Visibility = Visibility.Hidden;
            _ProgressBar.Visibility = Visibility.Collapsed;
            _ProgressTextBox.Visibility = Visibility.Collapsed;
            DownSpeedTextBlock.Visibility = Visibility.Collapsed;

            Properties.Settings.Default.Installing = false;
            Properties.Settings.Default.Save();
        }

        void DeleteGame()
        {
            GameStatus = -2;
            StatusTextBlock.Text = "Status: Not installed.";
            InstallBtn.Content = "⬇💾Install";
            DeleteBtn.Visibility = Visibility.Hidden;

            BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
            int i = 0;
            while (i != 99)
            {
                if (i == _Tag)
                {
                    BinaryWriter.Write("" + "}" + "");
                }
                else
                {
                    string[] _SavedGamesInfo = ((MainWindow)Window.GetWindow(this)).SavedGamesInfo[i].Split('}');
                    BinaryWriter.Write(_SavedGamesInfo[0] + "}" + _SavedGamesInfo[1]); //Version}Path
                }
                i++;
            }
            BinaryWriter.Dispose();
        }
    }
}

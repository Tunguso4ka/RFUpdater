using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        WebClient webClient = new WebClient();
        public List<GameData> ListWithGameData = new List<GameData>();
        Uri GameListFileUri = new Uri("https://drive.google.com/uc?id=1QzOoLrQKW48salKmltEPDAis2Rd_GFz9", UriKind.RelativeOrAbsolute);
        string GameListFilePath;
        string[] GamesNamesList = new string[99];
        string[] GamesPathesList = new string[99];

        public GamePage RandomFightsPage;
        Uri ImageSourceUri = new Uri("https://drive.google.com/uc?id=1pbvzQhhskJR8Vi-y-rC9AJ4iI1uylJ5g", UriKind.RelativeOrAbsolute); //RFU logo - https://drive.google.com/uc?id=1vm1sKGFaGSlJWCSaqyiMdR2z62FUTewn https://drive.google.com/file/d/1vm1sKGFaGSlJWCSaqyiMdR2z62FUTewn/view?usp=sharing gamelist - https://drive.google.com/uc?id=1QzOoLrQKW48salKmltEPDAis2Rd_GFz9 https://drive.google.com/uc?id=1Ia1E7q7Hpz-zihtBMxI8jDgGk5tXEk-X
        public LibraryPage()
        {
            InitializeComponent();
            GetGameListTxt();
        }

        void GetGameListTxt()
        {
            GameListFilePath = Properties.Settings.Default.AppDataPath + "gamelist.txt";
            GoogleDiscGamesCheck();
        }

        void FolderGameCheck()
        {
            string[] Directories;
            string[] DirectoryFilesNames;
            Directories = Directory.GetDirectories(Properties.Settings.Default.AppDataPath + @"Games\");
            if(Directories.Length > 0)
            {
                for(int i = 0; i != Directories.Length - 1; i++)
                {
                    DirectoryFilesNames = Directory.GetFiles(Properties.Settings.Default.AppDataPath + @"Games\" + Directories[i]);
                    if(DirectoryFilesNames.Length > 0)
                    {
                        
                    }
                }
            }
            else
            {
                GoogleDiscGamesCheck();
            }
        }

        async void GoogleDiscGamesCheck()
        {
            try
            {
                if (File.Exists(GameListFilePath))
                {
                    File.Delete(GameListFilePath);
                }
                webClient.DownloadFile(GameListFileUri, GameListFilePath);
                using (StreamReader StreamReader = new StreamReader(GameListFilePath))
                {
                    int LineNum = 0;
                    string[] LineList;
                    string line;
                    while ((line = await StreamReader.ReadLineAsync()) != null)
                    {
                        LineList = line.Split('}');
                        GamesNamesList[LineNum] = LineList[0];
                        GamesPathesList[LineNum] = LineList[2];
                        ListWithGameData.Add(new GameData() { AGameName = LineList[0], AGameSource = new Uri(LineList[1], UriKind.RelativeOrAbsolute), BtnTag = Convert.ToString(LineNum) });
                        LineNum++;
                    }
                    StreamReader.Dispose();
                    GameItemsControl.ItemsSource = ListWithGameData;
                }
                File.Delete(GameListFilePath);
            }
            catch
            {
                MessageBox.Show("Error: check your internet connection. ", "Error");
            }
        }

        private void AGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Button PressedButton = (Button)sender;
            int Tag = Convert.ToInt32((string)PressedButton.Tag);
            Version newGameVersion;
            Version thisGameVersion;
            string GameUpdateUrl;
            string GameInfoPath = Properties.Settings.Default.AppDataPath + "RFV.txt";
            string gamePath;

            try
            {
                string[] _SavedGamesInfo = ((MainWindow)Window.GetWindow(this)).SavedGamesInfo[Tag].Split('}');

                thisGameVersion = new Version(_SavedGamesInfo[0]);
                gamePath = _SavedGamesInfo[1];
            }
            catch
            {
                thisGameVersion = new Version("0.0");
                gamePath = "";
                MessageBox.Show("Error: cant get saved games path or/and saved games versions");
            }

            webClient.DownloadFile(new Uri(GamesPathesList[Tag], UriKind.RelativeOrAbsolute), GameInfoPath);

            using (StreamReader StreamReader = new StreamReader(GameInfoPath))
            {
                newGameVersion = new Version(StreamReader.ReadLine());
                GameUpdateUrl = StreamReader.ReadLine();
                StreamReader.Dispose();

                MessageBox.Show(" 1: " + newGameVersion, GameUpdateUrl);
            }
            File.Delete(GameInfoPath);

            if (Properties.Settings.Default.SavedGamesIsReal == false)
            {
                thisGameVersion = new Version("0.0");
                gamePath = "";
                MessageBox.Show("0");
            }
            //https://filetransfer.io/data-package/hayIzLuP/download GamesPathesList[Tag]
            //MessageBox.Show("" + Tag);
            RandomFightsPage = new GamePage(GamesNamesList[Tag], newGameVersion, thisGameVersion, gamePath, GameUpdateUrl, Tag);
            //string gameName, Version newGameVersion, !Version thisGameVersion, !string gamePath, string gameUpdateUrl
            ((MainWindow)Window.GetWindow(this)).Frame0.Content = RandomFightsPage;

        }
    }

    public class GameData
    {
        public string AGameName { get; set; }
        public Uri AGameSource { get; set; }
        public string BtnTag { get; set; }
    }
}

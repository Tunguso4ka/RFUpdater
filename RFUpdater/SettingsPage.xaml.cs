using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        string GameName;
        Version GameVersion;
        string GamePath;
        string GameUpdateUrl;
        int GameStatus;
        string Language;
        string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\settings.dat";
        string SaveFolderPath;
        bool AutoUpdate;
        public SettingsPage(string gameName, Version gameVersion, string gamePath, string gameUpdateUrl, int gameStatus, string language, bool autoUpdate, string saveFolderPath)
        {
            InitializeComponent();
            GameName = gameName;
            GameVersion = gameVersion;
            GamePath = gamePath;
            GameUpdateUrl = gameUpdateUrl;
            GameStatus = gameStatus;
            Language = language;
            if(Language == "en-EN")
            {
                LRB0.IsChecked = true;
            }
            else
            {
                LRB1.IsChecked = true;
            }
            AutoUpdate = autoUpdate;
            if (AutoUpdate == true)
            {
                CB0.IsChecked = true;
            }
            SaveFolderPath = saveFolderPath;
            if (SaveFolderPath == @"C:\Games\")
            {
                LRB2.IsChecked = true;
            }
            else
            {
                LRB3.IsChecked = true;
            }
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(LRB0.IsChecked == true)
            {
                Language = "en-EN";
            }
            else
            {
                Language = "ru-RU";
            }

            if (CB0.IsChecked == true)
            {
                AutoUpdate = true;
            }
            else
            {
                AutoUpdate = false;
            }

            if (LRB2.IsChecked == true)
            {
                SaveFolderPath = @"C:\Games\";
            }
            else
            {
                SaveFolderPath = @"D:\Games\";
            }

            BinaryWriter BinaryWriter = new BinaryWriter(File.Open(SettingsPath, FileMode.Create));
            BinaryWriter.Write(Language);
            BinaryWriter.Write(GameName);
            BinaryWriter.Write(Convert.ToString(GameVersion));
            BinaryWriter.Write(GamePath);
            BinaryWriter.Write(GameStatus);
            BinaryWriter.Write(AutoUpdate);
            BinaryWriter.Write(SaveFolderPath);
            BinaryWriter.Dispose();

            new MainWindow().Show();
            Window.GetWindow(this).Close();
        }
    }
}

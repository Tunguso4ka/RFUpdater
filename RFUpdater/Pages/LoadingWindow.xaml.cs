using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RFUpdater
{
    /// <summary>
    /// Interakční logika pro LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        MediaPlayer _MediaPlayer;

        string MusicPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\Resources\streetphenomena.mp3";

        public LoadingWindow()
        {
            InitializeComponent();

            _MediaPlayer = new MediaPlayer();

            try
            {
                _MediaPlayer.Open(new Uri(MusicPath, UriKind.Relative));
                SongControl(0);
            }
            catch
            {

            }

            OpenMainWindow();
        }

        void OpenMainWindow()
        {
            MainWindow _MainWindow = new MainWindow();
            _MainWindow.Show();

            SongControl(1);
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void SongControl(int i)
        {
            if(i == 0)
            {
                _MediaPlayer.Play();
            }
            else if (i == 1)
            {
                _MediaPlayer.Stop();
            }
        }
    }
}

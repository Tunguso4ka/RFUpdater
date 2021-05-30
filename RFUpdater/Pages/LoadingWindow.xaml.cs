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
        public LoadingWindow()
        {
            InitializeComponent();
            PlaySong();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void PlaySong()
        {
            MediaPlayer _MediaPlayer = new MediaPlayer();
            _MediaPlayer.Open(new Uri(@"Resources\streetphenomena.mp3", UriKind.Relative));
            _MediaPlayer.Play();
        }
    }
}

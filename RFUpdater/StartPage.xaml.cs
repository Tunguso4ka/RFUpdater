using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        int GameStatus;
        string Language;
        public StartPage(int gameStatus, string language)
        {
            InitializeComponent();
            GameStatus = gameStatus;
            Language = language;
            if(Language == "ru-RU")
            {
                if (GameStatus == 0)
                {
                    GameStatusTextBlock.Text = "Random Fights: Не установлена. Это хороший шанс попробовать её.";
                }
                else if (GameStatus == 1)
                {
                    GameStatusTextBlock.Text = "Random Fights: Установлена. Прекрасный день чтобы поиграть.";
                }
                else if (GameStatus == 2)
                {
                    GameStatusTextBlock.Text = "Random Fights: Обновление найдено. Скорее попробуйте новые возможности!";
                }
            }
            else if (Language == "en-EN")
            {
                if (GameStatus == 0)
                {
                    GameStatusTextBlock.Text = "Random Fights: Not installed. Its good chance to play new game.";
                }
                else if (GameStatus == 1)
                {
                    GameStatusTextBlock.Text = "Random Fights: Installed. Nice day to play again.";
                }
                else if (GameStatus == 2)
                {
                    GameStatusTextBlock.Text = "Random Fights: Update found. ";
                }
            }
        }
    }
}

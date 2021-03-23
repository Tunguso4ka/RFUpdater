using System.Windows.Controls;

namespace RFUpdater
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            /*if(DateTime.Now.Hour <= 12)
            {
                GDTextBlock.Text = Properties.Resources.HelloMessageTimeMorning;
            }
            else if (DateTime.Now.Hour <= 19)
            {
                GDTextBlock.Text = Properties.Resources.HelloMessageTimeAfternoon;
            }
            else
            {
                GDTextBlock.Text = Properties.Resources.HelloMessageTimeEvening;
            }*/
        }
    }
}

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
using System.Windows.Shapes;

namespace saperdun
{
    /// <summary>
    /// Логика взаимодействия для Achievements.xaml
    /// </summary>
    public partial class Achievements : Window
    {
        string enterString = "";
        public Achievements(string enterString)
        {
            InitializeComponent();
            this.enterString = enterString;
            checkAchievments(enterString);
        }

        private void checkAchievments(string enterString)
        {
            enterString = this.enterString;
            if(enterString.Contains("newJourney"))
            {
                txtEZ.Text = "\"Новое приключение\" - легкая сложность" +
                    "\nОткрыта панель \"Бусты\"";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgEZ.Source = bitmap;
            }
            if (enterString.Contains("localLegend"))
            {
                txtMID.Text = "\"Местная легенда\" - средняя сложность" +
                    "\nОткрыта панель \"Бонус-коды\" в разделе \"Бусты\"";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgMID.Source = bitmap;
            }
            if (enterString.Contains("heroOfTheKingdom"))
            {
                txtHARD.Text = "\"Герой королевства\" - высокая сложность" +
                     "\nОткрыта панель \"Оформление\" в разделе \"Бусты\"";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgHARD.Source = bitmap;
            }
            if (enterString.Contains("hopeOfHumanity"))
            {
                txtPRO.Text = "\"Надежда человечества\" - экспертная сложность" +
                    "\nОткрыто постоянное увеличение очков";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgPRO.Source = bitmap;
            }
            if (enterString.Contains("speedster"))
            {
                txtSpeed.Text = "\"Спидстер\" - скоростное прохождение" +
                    "\nНовый режим игры";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgSpeed.Source = bitmap;
            }
            if(enterString.Contains("worthy"))
            {
                txtChar.Text = "\"Достойный\" - открыты персонажи" +
                    "\nОткрыта возможность получения персонажей";
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgChar.Source = bitmap;
            }
            else
            {
                txtChar.Text = "\"Достойный\"" +
                    "\nПройти \"Среднюю\" сложность на скорость";
            }

        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == System.Windows.WindowState.Normal)
                {
                    WindowState = System.Windows.WindowState.Maximized;
                    TitleGrid.Margin = new Thickness(0, 6, 0, 0);
                }
                else
                {
                    WindowState = System.Windows.WindowState.Normal;
                    TitleGrid.Margin = new Thickness(0);
                }
            }
            else
            {
                if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
                    return;
                this.DragMove();
            }
        }
    }
}

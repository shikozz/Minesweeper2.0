using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using saperdun.Properties;
using static System.Net.WebRequestMethods;

namespace saperdun
{
    /// <summary>
    /// Логика взаимодействия для CharacterSelection.xaml
    /// </summary>
    public partial class CharacterSelection : Window
    {
        public string returnCharacters { get; set; }
        public int returnScrap { get; set; }
        public int returnPowder { get; set; }
        public int returnFlags { get; set; }
        public CharacterSelection(string characterUnlocked, int scrap, int powder, int flags)
        {
            InitializeComponent();
            scrapTxt.Text = "Части: " + scrap;
            powderTxt.Text = "Пыль: " + powder;
            flagsTxt.Text = "Флажки: " + flags;
            returnCharacters = characterUnlocked;
            returnScrap = scrap;
            returnPowder = powder;
            returnFlags = flags;

            if(!characterUnlocked.Contains("zakhar"))
            {
                //stackzh.IsHitTestVisible = false;
                Image close = new Image();
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                imgZH.Source = bitmap;
                chooseZakhar.Content = "Купить - 5000";
                BlurEffect blurEffect = new BlurEffect { Radius = 10 };
                evgenToHide.Effect = blurEffect;
                lehaToHide.Effect= blurEffect;
                lehaToHide.IsHitTestVisible = false;
                evgenToHide.IsHitTestVisible = false;
            }
            else
            {
                
                //stackzh.IsHitTestVisible = true;
                Image open = new Image();
                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgZH.Source = bitmap2;
                if(MainWindow.zakhar_level<5)
                {
                    BlurEffect blurEffect = new BlurEffect { Radius = 10 };
                    evgenToHide.Effect = blurEffect;
                    lehaToHide.Effect = blurEffect;
                    lehaToHide.IsHitTestVisible = false;
                    evgenToHide.IsHitTestVisible = false;
                }
                else
                {
                    lehaToHide.Effect = null;
                    lehaToHide.IsHitTestVisible = true;
                }
                if (!MainWindow.zakhar_player)
                {
                    chooseZakhar.Content = "Выбрать";
                }
                else if(MainWindow.zakhar_level<5)
                {
                    chooseZakhar.Content = "Улучшить - 1000";
                }
                else
                {
                    chooseZakhar.Content = "Выбрано и улучшено";
                    chooseZakhar.IsEnabled = false;
                }
            }

            if (!characterUnlocked.Contains("leha"))
            {
                Image close = new Image();
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                imgLE.Source = bitmap;
                chooseLeha.Content = "Купить - 17500";
                BlurEffect blurEffect = new BlurEffect { Radius = 10 };
                evgenToHide.Effect = blurEffect;
                evgenToHide.IsHitTestVisible = false;
            }
            else
            {
                Image open = new Image();
                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgLE.Source = bitmap2;
                if (MainWindow.leha_level < 5)
                {
                    BlurEffect blurEffect = new BlurEffect { Radius = 10 };
                    evgenToHide.Effect = blurEffect;
                    evgenToHide.IsHitTestVisible = false;
                }
                else
                {
                    evgenToHide.Effect = null;
                    evgenToHide.IsHitTestVisible = true;
                }
                if (!MainWindow.leha_player)
                {
                    chooseLeha.Content = "Выбрать";
                }
                else if (MainWindow.leha_level < 5)
                {
                    chooseLeha.Content = "Улучшить - 5000";
                }
                else
                {
                    chooseLeha.Content = "Выбрано и улучшено";
                    chooseLeha.IsEnabled = false;
                }
            }

            if (!characterUnlocked.Contains("evgen"))
            {
                Image close = new Image();
                BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/333.png"));
                imgEV.Source = bitmap;
                chooseEvgen.Content = "Купить - 30000";
            }
            else
            {
                Image open = new Image();
                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                imgEV.Source = bitmap2;
                if (!MainWindow.evgen_player)
                {
                    chooseEvgen.Content = "Выбрать";
                }
                else if (MainWindow.evgen_level < 5)
                {
                    chooseEvgen.Content = "Улучшить - 7000";
                }
                else
                {
                    chooseEvgen.Content = "Выбрано и улучшено";
                    chooseEvgen.IsEnabled = false;
                }
            }

            if(MainWindow.worker_player)
            {
                chooseWorker.Content = "Выбрано";
                chooseWorker.IsEnabled = false;
            }
            foreach(var childe in mainCharPanel.Children)
            {
                Border parentBorder = childe as Border;
                StackPanel parentStack = parentBorder.Child as StackPanel;
                foreach(var stackPanel in parentStack.Children)
                {
                    if (stackPanel is StackPanel)
                    {
                        StackPanel stcp = (StackPanel)stackPanel;
                        if (stcp.Name.Contains("stackzh"))
                        {
                            redraw_lvl(stcp, MainWindow.zakhar_level);
                        }
                        if(stcp.Name.Contains("stackEV"))
                        {
                            redraw_lvl(stcp, MainWindow.evgen_level);
                        }
                        if(stcp.Name.Contains("stackLE"))
                        {
                            redraw_lvl(stcp, MainWindow.leha_level);
                        }
                    }
                }


            }
        }

        private void redraw_lvl(StackPanel stP, int levelchar)
        {
            foreach (Border bd in stP.Children)
            {
                switch (levelchar)
                {
                    case 1:
                        Image bt = bd.Child as Image;
                        if (bt.Name.Contains("1"))
                        {
                            Image open = new Image();
                            BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                            bt.Source = bitmap2;
                            if (stP.Name.Contains("zh")) mat_neededTxt.Text = "Ч-15,П-25,Ф-0";
                            if (stP.Name.Contains("EV")) mat_neededEVTxt.Text = "Ч-75,п-107,Ф-3";
                            if (stP.Name.Contains("LE")) mat_neededLETxt.Text = "Ч-39,П-64,Ф-1";
                        }
                        break;
                    case 2:
                        Image bt2 = bd.Child as Image;
                        if (bt2.Name.Contains("2") || bt2.Name.Contains("1"))
                        {
                            Image open = new Image();
                            BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                            bt2.Source = bitmap2;
                            if (stP.Name.Contains("zh")) mat_neededTxt.Text = "Ч-25,П-35,Ф-1";
                            if (stP.Name.Contains("EV")) mat_neededEVTxt.Text = "Ч-99,п-141,Ф-3";
                            if (stP.Name.Contains("LE")) mat_neededLETxt.Text = "Ч-65,П-93,Ф-2";
                        }                      
                        break;
                    case 3:
                        Image bt3 = bd.Child as Image;
                        if (bt3.Name.Contains("3") || bt3.Name.Contains("2") || bt3.Name.Contains("1"))
                        {
                            Image open = new Image();
                            BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                            bt3.Source = bitmap2;
                            if (stP.Name.Contains("zh")) mat_neededTxt.Text = "Ч-30,П-50,Ф-1";
                            if (stP.Name.Contains("EV")) mat_neededEVTxt.Text = "Ч-132,п-179,Ф-6";
                            if (stP.Name.Contains("LE")) mat_neededLETxt.Text = "Ч-87,П-127,Ф-3";
                        }         
                        break;
                    case 4:
                        Image bt4 = bd.Child as Image;
                        if (bt4.Name.Contains("4") || bt4.Name.Contains("3") || bt4.Name.Contains("2") || bt4.Name.Contains("1"))
                        {
                            Image open = new Image();
                            BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                            bt4.Source = bitmap2;
                            if (stP.Name.Contains("zh")) mat_neededTxt.Text = "Ч-45,П-75,Ф-3";
                            if (stP.Name.Contains("EV")) mat_neededEVTxt.Text = "Ч-166,п-245,Ф-15";
                            if (stP.Name.Contains("LE")) mat_neededLETxt.Text = "Ч-109,П-159,Ф-7";
                        }
                        break;
                    case 5:
                        Image bt5 = bd.Child as Image;
                        if (bt5.Name.Contains("5") || bt5.Name.Contains("4") || bt5.Name.Contains("3") || bt5.Name.Contains("2") || bt5.Name.Contains("1"))
                        {
                            Image open = new Image();
                            BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                            bt5.Source = bitmap2;
                            if(stP.Name.Contains("zh"))mat_neededTxt.Text = "Максимальный уровень!";
                            if (stP.Name.Contains("EV")) mat_neededEVTxt.Text = "Максимальный уровень!";
                            if (stP.Name.Contains("LE")) mat_neededLETxt.Text = "Максимальный уровень!";
                        }
                        break;
                }
            }
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void chooseZakhar_Click(object sender, RoutedEventArgs e)
        {
            chooseWorker.Content = "Выбрать";
            chooseWorker.IsEnabled = true;
            if(returnCharacters.Contains("evgen"))
            {
                chooseEvgen.Content = "Выбрать";
                chooseEvgen.IsEnabled = true;
            }
            else
            {
                chooseEvgen.Content = "Купить - 25000";
                chooseEvgen.IsEnabled = true;
            }
            if (returnCharacters.Contains("leha"))
            {
                chooseLeha.Content = "Выбрать";
                chooseLeha.IsEnabled = true;
            }
            else
            {
                chooseLeha.Content = "Купить - 15000";
                chooseLeha.IsEnabled = true;
            }

            if(MainWindow.zakhar_level>0&&MainWindow.zakhar_level<5)
            {
                if(MainWindow.zakhar_player)
                {
                    switch (MainWindow.zakhar_level)
                    {
                        case 1:
                            if (MainWindow.shoppoints >= 1000 && returnScrap >= 15 && returnPowder >= 25)
                            {
                                MainWindow.shoppoints -= 1000;
                                returnScrap -= 15;
                                returnPowder -= 25;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.zakhar_level = 2;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                zhlvl2.Source = bitmap2;
                            }
                            break;
                        case 2:
                            if (MainWindow.shoppoints >= 1000 && returnScrap >= 25 && returnPowder >= 35 && returnFlags >= 1)
                            {
                                MainWindow.shoppoints -= 1000;
                                returnScrap -= 25;
                                returnPowder -= 35;
                                returnFlags -= 1;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.zakhar_level = 3;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                zhlvl3.Source = bitmap2;
                            }
                            break;
                        case 3:
                            if (MainWindow.shoppoints >= 1000 && returnScrap >= 25 && returnPowder >= 35 && returnFlags >= 1)
                            {
                                MainWindow.shoppoints -= 1000;
                                returnScrap -= 25;
                                returnPowder -= 35;
                                returnFlags -= 1;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.zakhar_level = 4;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                zhlvl3.Source = bitmap2;
                            }
                            break;
                        case 4:
                            if (MainWindow.shoppoints >= 1000 && returnScrap >= 45 && returnPowder >= 75 && returnFlags >= 3)
                            {
                                MainWindow.shoppoints -= 1000;
                                returnScrap -= 45;
                                returnPowder -= 75;
                                returnFlags -= 3;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.zakhar_level = 5;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                zhlvl5.Source = bitmap2;
                                lehaToHide.Effect = null;
                                lehaToHide.IsHitTestVisible = true;
                                chooseZakhar.IsEnabled = false;
                                chooseZakhar.Content = "Выбрано и улучшено";
                            }
                            break;
                    }
                }
                else
                {
                    MainWindow.zakhar_player = true;
                    chooseZakhar.Content = "Улучшить - 1000";

                }
                MainWindow.worker_player = false;
                Settings.Default.WorkerPlayer = false;
                MainWindow.evgen_player = false;
                MainWindow.leha_player = false;
                Settings.Default.EvgenPlayer = false;
                Settings.Default.LehaPlayer = false;
                Settings.Default.ZakharPlayer = true;
                Settings.Default.Save();
                redraw_lvl(stackzh, MainWindow.zakhar_level);
            }
            else
            {
                if (returnCharacters.Contains("zakhar"))
                {
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgZH.Source = bitmap2;
                    MainWindow.zakhar_player = true;
                    if (MainWindow.zakhar_level == 5 && MainWindow.zakhar_player)
                    {
                        chooseZakhar.IsEnabled = false;
                        chooseZakhar.Content = "Выбрано и улучшено";
                    }
                }
                else if (MainWindow.shoppoints >= 5000)
                {
                    MainWindow.shoppoints -= 5000;
                    returnCharacters += ".zakhar.";
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgZH.Source = bitmap2;
                    MainWindow.zakhar_player = true;
                    MainWindow.zakhar_level = 1;
                    redraw_lvl(stackzh, MainWindow.zakhar_level);
                    chooseZakhar.Content = "Улучшить - 1000";
                }
                MainWindow.worker_player = false;
                MainWindow.evgen_player = false;
                MainWindow.leha_player = false;
                Settings.Default.WorkerPlayer = false;
                Settings.Default.EvgenPlayer = false;
                Settings.Default.LehaPlayer = false;
                Settings.Default.ZakharPlayer = true;
                Settings.Default.Save();

            }           
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            MSGBox.Show("Материалы для прокачки получаются за успешное завершение игр." +
                "\nИндикация уровня персонажа представлена 5-ю кнопками, которые также служат для повышения уровня персонажа." +
                "\nКоличество требуемых материалов отображается под кнопками прокачки. " +
                "\n\"Ч\" - обозначает количество требуемых Частей" +
                "\n\"П\" - обозначает количество требуемой Пыли" +
                "\n\"Ф\" - обозначает количество требуемых Флажков","Справка");
        }

        //standart logic for custom header
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

        private void aboutZH_Click(object sender, RoutedEventArgs e)
        {
            MSGBox.Show("Шизик - очень забывчивый персонаж. " +
                "\nНастолько забывчивый, что немного меняет законы природы, когда забывает о чем-то. Например он может забыть, что наступил на мину и вместо того чтобы взорваться, на нее просто упадет флажок." +
                "\nС прокачкой увеличивается шанс \"забыть\"." +
                "\nТекущиий шанс забыть - " + MainWindow.zakhar_level * 3 + "%"+
                "\nНа последнем уровне он станет настолько забывчивым, что вместе с падающим флажком он начнет применять один из двух случайных бустов раскрытия.",
                "Шизик");
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void bMaximize_Click(object sender, RoutedEventArgs e)
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

        private void bMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void chooseWorker_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.LehaPlayer = false;
            Settings.Default.ZakharPlayer = false;
            Settings.Default.EvgenPlayer = false;
            Settings.Default.WorkerPlayer = true;
            Settings.Default.Save();
            if(returnCharacters.Contains("zakhar"))
            {
                chooseZakhar.Content = "Выбрать";
                chooseZakhar.IsEnabled = true;
                MainWindow.zakhar_player = false;
            }
            if(returnCharacters.Contains("evgen"))
            {
                chooseEvgen.Content = "Выбрать";
                chooseEvgen.IsEnabled = true;
                MainWindow.evgen_player = false;
            }
            if(returnCharacters.Contains("leha"))
            {
                chooseLeha.Content = "Выбрать";
                chooseLeha.IsEnabled = true;
                MainWindow.leha_player = false;
            }
            chooseWorker.Content = "Выбрано";
            chooseWorker.IsEnabled = false;
            MainWindow.worker_player = true;
        }

        private void chooseEvgen_Click(object sender, RoutedEventArgs e)
        {
            chooseZakhar.Content = "Выбрать";
            chooseZakhar.IsEnabled = true;
            chooseLeha.Content = "Выбрать";
            chooseLeha.IsEnabled = true;
            chooseWorker.Content = "Выбрать";
            chooseWorker.IsEnabled = true;
            if (MainWindow.evgen_level > 0 && MainWindow.evgen_level < 5)
            {
                if (MainWindow.evgen_player)
                {
                    switch (MainWindow.evgen_level)
                    {
                        case 1:
                            if (MainWindow.shoppoints >= 7000 && returnScrap >= 76 && returnPowder >= 107 && returnFlags>=3)
                            {
                                MainWindow.shoppoints -= 7000;
                                returnScrap -= 76;
                                returnPowder -= 107;
                                returnFlags -= 3;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.evgen_level = 2;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                evlvl2.Source = bitmap2;
                            }
                            break;
                        case 2:
                            if (MainWindow.shoppoints >= 7000 && returnScrap >= 99 && returnPowder >= 141 && returnFlags >= 3)
                            {
                                MainWindow.shoppoints -= 7000;
                                returnScrap -= 99;
                                returnPowder -= 141;
                                returnFlags -= 3;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.evgen_level = 3;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                evlvl3.Source = bitmap2;
                            }
                            break;
                        case 3:
                            if (MainWindow.shoppoints >= 7000 && returnScrap >= 132 && returnPowder >= 179 && returnFlags >= 6)
                            {
                                MainWindow.shoppoints -= 7000;
                                returnScrap -= 132;
                                returnPowder -= 179;
                                returnFlags -= 4;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.evgen_level = 4;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                evlvl3.Source = bitmap2;
                            }
                            break;
                        case 4:
                            if (MainWindow.shoppoints >= 7000 && returnScrap >= 166 && returnPowder >= 245 && returnFlags >= 15)
                            {
                                MainWindow.shoppoints -= 7000;
                                returnScrap -= 166;
                                returnPowder -= 245;
                                returnFlags -= 15;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.evgen_level = 5;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                evlvl5.Source = bitmap2;
                                chooseEvgen.IsEnabled = false;
                                chooseEvgen.Content = "Выбрано и улучшено";
                            }
                            break;
                    }
                }
                else
                {
                    MainWindow.evgen_player = true;
                    chooseEvgen.Content = "Улучшить - 7000";

                }
                MainWindow.worker_player = false;
                MainWindow.leha_player = false;
                Settings.Default.WorkerPlayer = false;
                MainWindow.zakhar_player = false;
                Settings.Default.EvgenPlayer = true;
                Settings.Default.LehaPlayer = false;
                Settings.Default.ZakharPlayer = false;
                Settings.Default.Save();
                redraw_lvl(stackEV, MainWindow.evgen_level);
            }
            else
            {
                if (returnCharacters.Contains("evgen"))
                {
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgEV.Source = bitmap2;
                    MainWindow.evgen_player = true;
                    if (MainWindow.evgen_level == 5 && MainWindow.evgen_player)
                    {
                        chooseEvgen.IsEnabled = false;
                        chooseEvgen.Content = "Выбрано и улучшено";
                    }
                }
                else if (MainWindow.shoppoints >= 30000)
                {
                    MainWindow.shoppoints -= 30000;
                    returnCharacters += ".evgen.";
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgEV.Source = bitmap2;
                    MainWindow.evgen_player = true;
                    MainWindow.evgen_level = 1;
                    redraw_lvl(stackEV, MainWindow.evgen_level);
                    chooseEvgen.Content = "Улучшить - 7000";
                }
                MainWindow.worker_player = false;
                MainWindow.zakhar_player = false;
                MainWindow.leha_player = false;
                Settings.Default.WorkerPlayer = false;
                Settings.Default.LehaPlayer = false;
                Settings.Default.EvgenPlayer = true;
                Settings.Default.ZakharPlayer = false;
                Settings.Default.Save();

            }
        }

        private void aboutEV_Click(object sender, RoutedEventArgs e)
        {
            MSGBox.Show("Евген - просто гуру саперного дела." +
                "\nОн обладает самым настоящим 6 чувством и иногда может применить \"Улучшенный сонар\", когда просто хотел применить обычный." +
                "\nС прокачкой увеличивается шанс срабатывания." +
                "\nТекущий шанс срабатывания равен "+MainWindow.evgen_level*5+"%." +
                "\nНа последнем уровне увеличивает радиус \"Улучшенного сонара\" любого применения на одну клетку.",
                "Евген");
        }

        private void chooseLeha_Click(object sender, RoutedEventArgs e)
        {
            chooseZakhar.Content = "Выбрать";
            chooseZakhar.IsEnabled = true;
            chooseEvgen.Content = "Выбрать";
            chooseEvgen.IsEnabled = true;
            chooseWorker.Content = "Выбрать";
            chooseWorker.IsEnabled = true;
            if (MainWindow.leha_level > 0 && MainWindow.leha_level < 5)
            {
                if (MainWindow.leha_player)
                {
                    switch (MainWindow.leha_level)
                    {
                        case 1:
                            if (MainWindow.shoppoints >= 5000 && returnScrap >= 39 && returnPowder >= 64 && returnFlags >= 1)
                            {
                                MainWindow.shoppoints -= 5000;
                                returnScrap -= 39;
                                returnPowder -= 64;
                                returnFlags -= 1;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.leha_level = 2;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                lelvl2.Source = bitmap2;
                            }
                            break;
                        case 2:
                            if (MainWindow.shoppoints >= 5000 && returnScrap >= 65 && returnPowder >= 93 && returnFlags >= 2)
                            {
                                MainWindow.shoppoints -= 5000;
                                returnScrap -= 65;
                                returnPowder -= 93;
                                returnFlags -= 2;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.leha_level = 3;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                lelvl3.Source = bitmap2;
                            }
                            break;
                        case 3:
                            if (MainWindow.shoppoints >= 5000 && returnScrap >= 87 && returnPowder >= 127 && returnFlags >= 3)
                            {
                                MainWindow.shoppoints -= 5000;
                                returnScrap -= 87;
                                returnPowder -= 127;
                                returnFlags -= 3;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.leha_level = 4;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                lelvl3.Source = bitmap2;
                            }
                            break;
                        case 4:
                            if (MainWindow.shoppoints >= 5000 && returnScrap >= 109 && returnPowder >= 153 && returnFlags >= 7)
                            {
                                MainWindow.shoppoints -= 5000;
                                returnScrap -= 109;
                                returnPowder -= 153;
                                returnFlags -= 7;
                                scrapTxt.Text = "Части: " + returnScrap;
                                powderTxt.Text = "Пыль: " + returnPowder;
                                flagsTxt.Text = "Флажки: " + returnFlags;
                                MainWindow.leha_level = 5;
                                Image open = new Image();
                                BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                                lelvl5.Source = bitmap2;
                                evgenToHide.Effect = null;
                                evgenToHide.IsHitTestVisible = true;
                                chooseLeha.IsEnabled = false;
                                chooseLeha.Content = "Выбрано и улучшено";
                            }
                            break;
                    }
                }
                else
                {
                    MainWindow.leha_player = true;
                    chooseLeha.Content = "Улучшить - 5000";
                }
                MainWindow.worker_player = false;
                MainWindow.zakhar_player = false;
                MainWindow.leha_player = true;
                Settings.Default.WorkerPlayer = false;
                Settings.Default.LehaPlayer = true;
                Settings.Default.EvgenPlayer = false;
                Settings.Default.ZakharPlayer = false;
                Settings.Default.Save();
                redraw_lvl(stackLE, MainWindow.leha_level);
            }
            else
            {
                if (returnCharacters.Contains("leha"))
                {
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgLE.Source = bitmap2;
                    MainWindow.leha_player = true;
                    if (MainWindow.leha_level == 5 && MainWindow.leha_player)
                    {
                        chooseLeha.IsEnabled = false;
                        chooseLeha.Content = "Выбрано и улучшено";
                    }
                }
                else if (MainWindow.shoppoints >= 17500)
                {
                    MainWindow.shoppoints -= 17500;
                    returnCharacters += ".leha.";
                    Image open = new Image();
                    BitmapImage bitmap2 = new BitmapImage(new Uri("pack://application:,,,/saperdun;component/Images/321.png"));
                    imgLE.Source = bitmap2;
                    MainWindow.leha_player = true;
                    MainWindow.leha_level = 1;
                    redraw_lvl(stackLE, MainWindow.leha_level);
                    chooseLeha.Content = "Улучшить - 5000";
                }
                MainWindow.worker_player = false;
                MainWindow.zakhar_player = false;
                MainWindow.evgen_player = false;
                MainWindow.leha_player = true;
                Settings.Default.LehaPlayer = true;
                Settings.Default.WorkerPlayer = false;
                Settings.Default.EvgenPlayer = false;
                Settings.Default.ZakharPlayer = false;
                Settings.Default.Save();

            }
        }

        private void aboutLE_Click(object sender, RoutedEventArgs e)
        {
            MSGBox.Show("Лексус - начинающий компьютерщик и тот еще фармила." +
                "\nОн создал несколько полезных механизмов, облегчающих работу саперов, а также любил поиграть в танки в былое время." +
                "\nПри завершении игры с некоторой вероятностью может увеличить или уменьшить количество поинтов за игру." +
                "\nВероятность зависит от уров прокачки Лексуса." +
                "\nТекущий разброс результатов - от -"+(MainWindow.leha_chance-MainWindow.leha_level)+"% до +"+ (MainWindow.leha_chance+MainWindow.leha_level) + "% поинтов." +
                "\nНа максимальном уровне начинает немного повышать количство получаемых материалов и дает небольшой шанс получить 1 \"Флажок\" за игру с любым резльтатом.",
                "Лексус");
        }
    }
}

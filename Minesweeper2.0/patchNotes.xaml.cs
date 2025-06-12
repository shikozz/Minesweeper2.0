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
    /// Логика взаимодействия для patchNotes.xaml
    /// </summary>
    public partial class patchNotes : Window
    {
        public patchNotes()
        {
            InitializeComponent();
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

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
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

        private void toLastPatch_Click(object sender, RoutedEventArgs e)
        {
            GeneralTransform transform = release1_toscroll.TransformToAncestor(scrollViewer);
            Point position = transform.Transform(new Point(0, 0));
            scrollViewer.ScrollToVerticalOffset(position.Y);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Логика взаимодействия для MSGBox.xaml
    /// </summary>
    public partial class MSGBox : Window
    {
        public MSGBox()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(string message, string title = "")
        {
            MSGBox msgBox = new MSGBox();
            msgBox.Title = title;
            msgBox.MessageTextBlock.Text = message;
            msgBox.Show();
            return MessageBoxResult.OK;
        }

        private MessageBoxResult Result { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
                return;
            this.DragMove();
        }
    }
}

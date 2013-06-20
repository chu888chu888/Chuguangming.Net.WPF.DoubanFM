using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoubanFM
{
    /// <summary>
    /// ErrorMag.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorMsg : Window
    {
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public ErrorMsg(string errorMessage)
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            errMsg.Text = errorMessage;
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();

        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        public void ShowDialog(MainWindow mainWnd)
        {
            Point pos = mainWnd.PointToScreen(new Point(0, 0));
            this.Left = pos.X - 62;
            this.Top = pos.Y + 72;
            base.ShowDialog();
        }
    }
}

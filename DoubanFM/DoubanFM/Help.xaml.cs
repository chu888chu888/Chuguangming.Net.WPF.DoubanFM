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
    /// Help.xaml 的交互逻辑
    /// </summary>
    public partial class Help : Window
    {
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public Help()
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            line11.Text = @"播放/暂停: Ctrl + P";
            line12.Text = @"喜欢/取消: Ctrl + F";
            line13.Text = @"显/隐主界面: Ctrl + O";
            line14.Text = @"频道选择: Ctrl + C";
            line21.Text = @"下一曲: Ctrl + N";
            line22.Text = @"垃圾桶: Ctrl + D";
            line23.Text = @"登陆: Ctrl + L";
            line24.Text = @"退出: Alt + F4";
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
            this.Top = pos.Y + 45;
            base.ShowDialog();
        }
    }
}

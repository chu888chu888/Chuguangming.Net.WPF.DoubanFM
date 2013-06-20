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
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public About()
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            line1.Text = @"  本软件是豆瓣电台Windows桌面客户端，界面简洁，功能齐全。豆瓣先进的推荐技术让您与好音乐不期而遇！";
            line2.Text = @"  软件是基于.Net Framework 4.0平台，借助WPF技术完成，因此用户需要先安装.Net Framework 4.0后才可使用。软件使用方法和FAQ见作者主页！";

        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            Point headerPoint = e.GetPosition(this);
            if (e.ButtonState == MouseButtonState.Pressed && headerPoint.Y >= 0 && headerPoint.Y <= 30 && headerPoint.X >= 0 && headerPoint.X <= 330)
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

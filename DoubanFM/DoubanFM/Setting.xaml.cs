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
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public Setting()
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            sliderR.Value = Properties.Settings.Default.BKR;
            sliderG.Value = Properties.Settings.Default.BKG;
            sliderB.Value = Properties.Settings.Default.BKB;
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

        private void SliderRClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            border1.Background = new SolidColorBrush(Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value));
        }

        private void SliderGClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            border1.Background = new SolidColorBrush(Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value));
        }

        private void SliderBClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            border1.Background = new SolidColorBrush(Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value));
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.BKR = sliderR.Value;
            Properties.Settings.Default.BKG = sliderG.Value;
            Properties.Settings.Default.BKB = sliderB.Value;
            Properties.Settings.Default.Save();
            mainWnd.mainBorder.Background = new SolidColorBrush(Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value));
            double headPathR = (sliderR.Value - 47 >= 0) ? sliderR.Value - 47 : 0;
            double headPathG = (sliderG.Value - 47 >= 0) ? sliderG.Value - 47 : 0;
            double headPathB = (sliderB.Value - 47 >= 0) ? sliderB.Value - 47 : 0;
            mainWnd.headPath.Fill = new SolidColorBrush(Color.FromRgb((byte)headPathR, (byte)headPathG, (byte)headPathB));
            mainWnd.contextMenu.Background = mainWnd.mainBorder.Background;
            this.Close();
        }

        private void RecoveryClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.BKR = 105;
            Properties.Settings.Default.BKG = 105;
            Properties.Settings.Default.BKB = 105;
            Properties.Settings.Default.Save();
            mainWnd.mainBorder.Background = Brushes.DimGray;
            mainWnd.headPath.Fill = new SolidColorBrush(Color.FromRgb((byte)58, (byte)58, (byte)58));
            mainWnd.contextMenu.Background = mainWnd.mainBorder.Background;
            this.Close();
        }
    }
}

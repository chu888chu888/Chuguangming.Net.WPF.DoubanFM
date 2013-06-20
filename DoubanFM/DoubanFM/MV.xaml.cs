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
using System.Reflection;
using System.Windows.Threading;

namespace DoubanFM
{
    /// <summary>
    /// MV.xaml 的交互逻辑
    /// </summary>
    public partial class MV 
    {
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        string mvUrl = string.Empty;
        public MV(string mvUrl)
        {
            InitializeComponent();
            this.mvUrl = mvUrl;
            /*Window owner = Window.GetWindow(mvCanvas);
            owner.LocationChanged -= delegate { wbo.OnSizeLocationChanged(); };*/
            
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

        public void Show(MainWindow mainWnd)
        {
            Point pos = mainWnd.PointToScreen(new Point(0, 0));
            if (pos.X < 265)
                this.Left = pos.X + 205;
            else
                this.Left = pos.X - 265;
            this.Top = pos.Y ;

            border.Background = mainWnd.mainBorder.Background;
            WebBrowserOverlay wbo = new WebBrowserOverlay(mvCanvas, this.Left, this.Top + 34);
            WebBrowser wb = wbo.WebBrowser;
            wb.Navigate(new Uri(mvUrl));
            WebBrowserExtensions.SuppressScriptErrors(wb, true);

            base.Show();
        }
    }

    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}

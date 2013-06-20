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
using System.Threading;

namespace DoubanFM
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private string CaptchaUrl;
        private string CaptchaID;
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public Login()
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            userID.Text = Properties.Settings.Default.UserID;
            userPassword.Password = Properties.Settings.Default.UserPassword;
            this.Loaded += new RoutedEventHandler(UpdateCaptcha);
        }

        // 加载验证码
        public void UpdateCaptcha(object sender, RoutedEventArgs e)
        {
            CaptchaUrl = null;
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                CaptchaID = new ConnectionBase().Get("http://douban.fm/j/new_captcha");
                CaptchaID = CaptchaID.Trim('\"');
                CaptchaUrl = @"http://douban.fm/misc/captcha?size=m&id=" + CaptchaID;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (string.IsNullOrEmpty(CaptchaID))
                    {
                        CaptchaID = null;
                        CaptchaUrl = null;
                    }
                    else
                    {
                        BitmapImage captcha = new BitmapImage(new Uri(CaptchaUrl, UriKind.Absolute));
                        captchaImage.Source = captcha;
                        captcha.DownloadCompleted += delegate
                        {
                            errMsg.Text = "";
                        };
                    }
                }));
            }));
        }

        //登陆
        private void LoginClick(object sender, MouseButtonEventArgs e)
        {
            Parameters parameters = new Parameters();
            parameters.Add("source", "radio");
            parameters.Add("alias", userID.Text.Trim());
            parameters.Add("form_password", userPassword.Password.Trim());
            parameters.Add("captcha_solution", captcha.Text.Trim());
            parameters.Add("captcha_id", CaptchaID);
            parameters.Add("from", "mainsite");
            parameters.Add("remember", "on");
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                string LogOnJson = new ConnectionBase().Post("http://douban.fm/j/login", Encoding.UTF8.GetBytes(parameters.ToString()));
                Dispatcher.Invoke(new Action(() =>
                {
                    if (string.IsNullOrEmpty(LogOnJson))
                    {
                        ErrorMsg errMsg = new ErrorMsg("网络连接错误，请检查后重试！");
                        errMsg.ShowDialog();
                        return;
                    }
                    else
                    {
                        LogOnResult logonresult = LogOnResult.FromJson(LogOnJson);
                        if (logonresult.r == 1)
                        {
                            errMsg.Foreground = Brushes.Red;
                            errMsg.Text = logonresult.err_msg;
                            return;
                        }
                        else if (logonresult.r == 0)
                        {
                            Properties.Settings.Default.UserID = userID.Text;
                            Properties.Settings.Default.UserPassword = userPassword.Password;
                            Properties.Settings.Default.Save();
                            MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                            mainWnd.loveButton.IsEnabled = true;
                            this.Close();
                        }
                    }
                }));
            }));
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            Point headerPoint = e.GetPosition(this);
            if (e.ButtonState == MouseButtonState.Pressed && headerPoint.Y >= 0 && headerPoint.Y <= 36 && headerPoint.X >= 0 && headerPoint.X <= 205)
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
            this.Top = pos.Y + 37;
            base.ShowDialog();
        }
    }
}

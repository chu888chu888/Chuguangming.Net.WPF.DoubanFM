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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;

namespace DoubanFM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public GetSongList getSonglist = new GetSongList();
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer delaytimer = new DispatcherTimer();//如果歌曲加载10s则跳下一首
        static GetLyric getLyric = new GetLyric(string.Empty);
        private int loginState;
        private bool hasMV=false;
        public MainWindow()
        {
            InitializeComponent();

            //读取背景色
            double bkR = Properties.Settings.Default.BKR;
            double bkG = Properties.Settings.Default.BKG;
            double bkB = Properties.Settings.Default.BKB;
            mainBorder.Background = new SolidColorBrush(Color.FromRgb((byte)bkR, (byte)bkG, (byte)bkB));
            contextMenu.Background = mainBorder.Background;
            double headPathR = (bkR - 47 >= 0) ? bkR - 47 : 0;
            double headPathG = (bkG - 47 >= 0) ? bkG - 47 : 0;
            double headPathB = (bkB - 47 >= 0) ? bkB - 47 : 0;
            headPath.Fill = new SolidColorBrush(Color.FromRgb((byte)headPathR, (byte)headPathG, (byte)headPathB));

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            delaytimer.Interval = TimeSpan.FromSeconds(10);
            delaytimer.Tick+=new EventHandler(delaytimer_Tick);

            getSonglist.currentchannelNo = 1;
            getSonglist.lastchannelNo = -1;
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                loginState = HasLogined();
                if (loginState == -1)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ErrorMsg errMsg = new ErrorMsg("网络连接错误，请检查后重试！");
                            errMsg.ShowDialog(this);
                            return;
                        }));
                }
                else if (loginState == 1)
                {
                    getSonglist.currentchannelNo = Properties.Settings.Default.CloseNo;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //GetChannelInfo(getSonglist.currentchannelNo, out ChannelName, out subChannelName);
                        ChanName.Content = Properties.Settings.Default.ChannelName;
                        subChanName.Content = Properties.Settings.Default.SubChannelName;
                        if (getSonglist.currentchannelNo <= 0)
                        {
                            discardButton.IsEnabled = true;
                            discardButton.Opacity = 1.0;
                            discardButton.ToolTip = "不再播放";
                        }
                    }));
                }
                getSonglist.NextCompleted += new GetSongList.NextCompletedEventHandler(NextCompleted);
                getSonglist.FailLoaded += new GetSongList.FailLoadedEventHandler(FailLoaded);
                getSonglist.Next(true);
            }));
            player.Play();

            //设置视频栏
            /*WebBrowser videoBrowser = new WebBrowser();
            Canvas.SetLeft(videoBrowser, 2);
            Canvas.SetTop(videoBrowser, 2);
            videoBrowser.Width = 200;
            videoBrowser.Height = 150;
            videoBrowser.Navigate(new Uri(@"http://player.youku.com/player.php/sid/XNDA3NjUyMzIw/v.swf", UriKind.Absolute));
            albumCanvas.Children.Clear();
            albumCanvas.Children.Add(videoBrowser);
            WebBrowserExtensions.SuppressScriptErrors(videoBrowser, true);*/
        }

        /// <summary>
        /// 由频道号获取频道名称
        /// </summary>
        /// <param name="channelNo">频道号</param>
        /// <param name="ChannelName">公共/私有频道</param>
        /// <param name="subChannelName">频道名</param>
        private void GetChannelInfo(int channelNo, out string ChannelName, out string subChannelName)
        {
            ChannelName=string.Empty;
            subChannelName=string.Empty;
            Channels channels = new Channels();
            if (channelNo <= 0)
            {
                foreach (Channel c in channels.privateChannelLists)
                {
                    if (c.channelNo == channelNo)
                    {
                        ChannelName = "fm.私有频率";
                        subChannelName = c.channelName;
                        break;
                    }
                }
            }
            else
            {
                foreach (Channel c in channels.publicChannelLists)
                {
                    if (c.channelNo == channelNo)
                    {
                        ChannelName = "fm.公共频率";
                        subChannelName = c.channelName + "频率";
                        break;
                    }
                }
            }
        }

        private void delaytimer_Tick(object sender, EventArgs e)
        {
            if (player.Position == TimeSpan.FromSeconds(0))
            {
                getSonglist.Next(true);
            }
            delaytimer.Stop();
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            Point headerPoint = e.GetPosition(this);
            if (e.ButtonState == MouseButtonState.Pressed && headerPoint.Y >= 0 &&headerPoint.Y <= 36 &&headerPoint.X >=0 && headerPoint.X <= 205)
                this.DragMove();
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.CloseNo = getSonglist.currentchannelNo;
            Properties.Settings.Default.Save();
            notifyicon.Dispose();
            this.Close();
        }

         /// <summary>
         /// 判断服务器端是否已登录
         /// </summary>
         /// <returns>登陆状态：-1[网络异常] 0[未登陆] 1[已登陆]</returns>
        public int HasLogined()
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                string loginresult = new ConnectionBase().Get(@"http://douban.fm/");
                if (!string.IsNullOrEmpty(loginresult))
                {
                    Match indMatch = Regex.Match(loginresult, @"豆瓣", RegexOptions.IgnoreCase);
                    if (indMatch == null || string.IsNullOrEmpty(indMatch.Groups[0].Value))
                        return -1;
                    else
                    {
                        Match match = Regex.Match(loginresult, @"var\s*globalConfig\s*=\s*{\s*uid\s*:\s*'(\d*)'", RegexOptions.IgnoreCase);
                        string s = match.Groups[1].Value;
                        if (string.IsNullOrEmpty(s))
                            return 0;
                        else
                            return 1;
                    }
                }
                else
                    return -1;
            }
        }

        /// <summary>
        /// 频道切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelClick(object sender, MouseButtonEventArgs e)
        {
            Channels channels = new Channels();
            channels.ShowDialog(this);
        }

        /// <summary>
        /// 根据获取到的json值来设置界面
        /// </summary>
        private void NextCompleted(object sender, Song currentSong)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    player.Source = new Uri(currentSong.url, UriKind.Absolute);
                    player.Tag = currentSong.length;
                    //player.Play();
                    Album.Source = new BitmapImage(new Uri(currentSong.picture, UriKind.Absolute));
                    //Album.MouseLeftButtonDown+=new MouseButtonEventHandler
                    Album.Tag = @"http://music.douban.com" + currentSong.album;

                    //下面写法错误！！委托事件每次都会增添，所以事件会重复执行
                    /*Album.MouseLeftButtonDown += delegate
                    {
                        System.Diagnostics.Process.Start(@"http://music.douban.com" + currentSong.album);
                    };*/
                    songName.Text = currentSong.title;
                    artist.Text = currentSong.artist;
                    if (currentSong.like == "1")
                    {
                        loveButton.Source = new BitmapImage(new Uri(@"/DoubanFM;component/Images/RedHeart.png", UriKind.Relative));
                        loveButton.ToolTip = "取消喜欢";
                    }
                    else
                    {
                        loveButton.Source = new BitmapImage(new Uri(@"/DoubanFM;component/Images/Heart.png", UriKind.Relative));
                        loveButton.ToolTip = "喜欢";
                    }

                    //设置任务栏通知
                    string tooltip = currentSong.artist + "<" + currentSong.title + ">";
                    if (tooltip.Length > 20)
                        tooltip = tooltip.Substring(0, 20);
                    notifyicon.ToolTipText = tooltip;

                    delaytimer.Start();

                    keyword = string.Empty;
                    if (currentSong.title.Contains("("))
                        keyword += currentSong.title.Substring(0, currentSong.title.IndexOf("("));
                    else
                        keyword += currentSong.title;
                    keyword += @" ";
                    if (currentSong.artist.Contains("("))
                        keyword += currentSong.artist.Substring(0, currentSong.artist.IndexOf("("));
                    else
                        keyword += currentSong.artist;

                    //开始时禁止点击专辑封面
                    Album.IsEnabled = false;
                }));
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int length = (int)player.Tag;
            if (player.NaturalDuration.HasTimeSpan)
            {
                length = (int)player.NaturalDuration.TimeSpan.TotalSeconds;
            }
            if (length == 0)
                return;

            TimeSpan AfterSpan = player.Position;
            songExtent.Value = player.Position.TotalSeconds / length * 100;
            if (getLyric.songID.Count > 0)
            {
                lyricText.Text = getLyric.Refresh(AfterSpan);
            }
            else
                lyricText.Text = keyword;
        }

        /// <summary>
        /// 点击封面，跳转至专辑主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumClick(object sender, MouseButtonEventArgs e)
        {
            if (player.Position.TotalMilliseconds <= 0)
                return;
            if (hasMV)
            {
                MV mv = new MV(getLyric.mvUrl);
                mv.title.Text = keyword;
                mv.Show(this);
            }
            else
            {
                if (string.IsNullOrEmpty(Album.Tag.ToString()))
                    return;
                System.Diagnostics.Process.Start(Album.Tag.ToString());
            }
        }

        private void FailLoaded()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ErrorMsg errMsg = new ErrorMsg("网络连接错误，请检查后重试！");
                errMsg.ShowDialog(this);
            }));
        }

        private string keyword = string.Empty;
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_Open(object sender, RoutedEventArgs e)
        {
            playButton.ToolTip = "暂停";
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                getLyric = new GetLyric(keyword);
                if (getLyric.songID.Count > 0)
                {
                    getLyric.LyricContent(getLyric.songID[0]);
                }
                else
                {
                    getLyric = new GetLyric(keyword.Substring(0,keyword.IndexOf(" ")));
                    if (getLyric.songID.Count > 0)
                    {
                        getLyric.LyricContent(getLyric.songID[0]);
                    }
                }
                hasMV = getLyric.CanFollowMV(keyword);
                Album.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (hasMV)
                    {
                        Album.ToolTip = "点击观看MV";
                        System.Diagnostics.Debug.WriteLine(getLyric.mvUrl);
                    }
                    else
                        Album.ToolTip = "单击查看专辑信息";
                    Album.IsEnabled = true;
                }));
            }));
            timer.Start();
        }

        /// <summary>
        /// 歌曲自然结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_End(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            songExtent.Value = 0;
            getSonglist.Next(true);
        }

        /// <summary>
        /// 播放、暂停切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayOrPause(object sender, MouseButtonEventArgs e)
        {
            if (playButton.ToolTip == null)
                return;
            if (playButton.ToolTip.ToString() == "暂停")
            {
                player.Pause();
                playButton.Source = new BitmapImage(new Uri(@"/DoubanFm;component/Images/Play.png", UriKind.Relative));
                playButton.ToolTip = "播放";
            }
            else if (playButton.ToolTip.ToString() == "播放")
            {
                player.Play();
                playButton.Source = new BitmapImage(new Uri(@"/DoubanFM;component/Images/Pause.png", UriKind.Relative));
                playButton.ToolTip = "暂停";
            }
        }

        /// <summary>
        /// 下一首
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextClick(object sender, MouseButtonEventArgs e)
        {
            songExtent.Value = 0;
            lyricText.Text = string.Empty;
            timer.Stop();
            getSonglist.Next(true);
        }

        /// <summary>
        /// 添加喜欢列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loveClick(object sender, MouseButtonEventArgs e)
        {
            if (loginState == -1)
            {
                ErrorMsg errMsg = new ErrorMsg("网络连接错误，请检查后重试！");
                errMsg.ShowDialog(this);
                return;
            }
            else if (loginState == 0)
            {
                //GetSongList.currentchannelNo = Properties.Settings.Default.CloseNo;
                Login login = new Login();
                login.ShowDialog(this);
                return;
            }
            else
            {
                if (loveButton.ToolTip.ToString() == "喜欢")
                {
                    loveButton.Source = new BitmapImage(new Uri(@"/DoubanFm;component/Images/RedHeart.png", UriKind.Relative));
                    loveButton.ToolTip = "取消喜欢";
                    getSonglist.Like(true);
                }
                else
                {
                    loveButton.Source = new BitmapImage(new Uri(@"/DoubanFm;component/Images/Heart.png", UriKind.Relative));
                    loveButton.ToolTip = "喜欢";
                    getSonglist.Like(false);
                }
            }
        }

        /// <summary>
        /// 添加不再播放列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscardClick(object sender, MouseButtonEventArgs e)
        {
            getSonglist.Ban();
        }

        //******
        //通知栏事件
        private void ShowClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Minimized;
            else if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.ShowDialog(this);
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog(this);
        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.ShowDialog(this);
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CloseNo = getSonglist.currentchannelNo;
            Properties.Settings.Default.Save();
            notifyicon.Dispose();
            this.Close();
        }

        //快捷键部分
        private void PlayOrPauseKC(object sender, ExecutedRoutedEventArgs e)
        {
            PlayOrPause(null, null);
        }

        private void NextClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            NextClick(null, null);
        }

        private void loveClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            loveClick(null, null);
        }

        private void DiscardClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            DiscardClick(null, null);
        }

        private void ShowClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            ShowClick(null, null);
        }

        private void LoginClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            LoginClick(null, null);
        }
        private void ChannelClickKC(object sender, ExecutedRoutedEventArgs e)
        {
            ChannelClick(null, null);
        }
    }
}

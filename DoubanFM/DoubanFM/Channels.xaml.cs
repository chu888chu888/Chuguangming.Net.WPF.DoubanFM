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
    /// Channels.xaml 的交互逻辑
    /// </summary>
    public partial class Channels : Window
    {
        public List<Channel> publicChannelLists = new List<Channel>();
        public List<Channel> privateChannelLists = new List<Channel>();
        public int selectedChannelNo = 1;
        MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
        public Channels()
        {
            InitializeComponent();
            border.Background = mainWnd.mainBorder.Background;
            GetChannels();
            selectedChannelNo = mainWnd.getSonglist.currentchannelNo;
            if (mainWnd.HasLogined() != 1)
            {
                priChannelRB.IsEnabled = false;
                priChannelRB.ToolTip = "请先登陆！";
            }
            else
            {
                priChannelRB.IsEnabled = true;
                priChannelRB.ToolTip = null;
            }
            if (selectedChannelNo <= 0)
            {
                priChannelRB.IsChecked = true;
                priChecked(null,null);
            }
            else
            {
                pubChannelRB.IsChecked = true;
                pubChecked(null, null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pubChannelRB.IsChecked = true;
            pubChecked(null,null);
            pubChannelRB.Checked += pubChecked;
            priChannelRB.Checked += priChecked;
        }

        private void GetChannels()
        {
            privateChannelLists.Add(new Channel(0, "私人频率"));
            privateChannelLists.Add(new Channel(-3, "红心频率"));
            publicChannelLists.Add(new Channel(61, "新歌"));
            publicChannelLists.Add(new Channel(1, "华语"));
            publicChannelLists.Add(new Channel(2, "欧美"));
            publicChannelLists.Add(new Channel(3, "七零"));
            publicChannelLists.Add(new Channel(4, "八零"));
            publicChannelLists.Add(new Channel(5, "九零"));
            publicChannelLists.Add(new Channel(6, "粤语"));
            publicChannelLists.Add(new Channel(7, "摇滚"));
            publicChannelLists.Add(new Channel(8, "民谣"));
            publicChannelLists.Add(new Channel(13, "爵士"));
            publicChannelLists.Add(new Channel(14, "电子"));
            publicChannelLists.Add(new Channel(15, "说唱"));
            publicChannelLists.Add(new Channel(16, "R&B"));
            publicChannelLists.Add(new Channel(17, "日语"));
            publicChannelLists.Add(new Channel(18, "韩语"));
            publicChannelLists.Add(new Channel(20, "女声"));
            publicChannelLists.Add(new Channel(22, "法语"));
            publicChannelLists.Add(new Channel(27, "古典"));
            publicChannelLists.Add(new Channel(28, "动漫"));
            publicChannelLists.Add(new Channel(77, "Easy"));
            publicChannelLists.Add(new Channel(26, "音乐人"));
            publicChannelLists.Add(new Channel(9, "轻音乐"));
            publicChannelLists.Add(new Channel(76, "小清新"));
            publicChannelLists.Add(new Channel(10, "电影原声"));
        }

        /// <summary>
        /// 选中公共兆赫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pubChecked(object sender, RoutedEventArgs e)
        {
            chanCanvas.Children.Clear();
            for (int i = 0; i < publicChannelLists.Count; i++)
            {
                RadioButton rb = new RadioButton();
                Canvas.SetLeft(rb, 10 + 80 * (i / 6));
                Canvas.SetTop(rb, 10 + 20 * (i % 6));
                rb.Content = publicChannelLists[i].channelName;
                //if (rb.Content.ToString().Length < 4)
                //    rb.Content = rb.Content.ToString() + "兆赫";
                rb.Foreground = Brushes.White;
                int chanNo = publicChannelLists[i].channelNo;
                rb.Checked += delegate
                {
                    if (selectedChannelNo != chanNo)
                    {
                        selectedChannelNo = chanNo;
                        this.Close();
                        Properties.Settings.Default.ChannelName = "fm.公共兆赫";
                        Properties.Settings.Default.SubChannelName = rb.Content + "兆赫";
                        Properties.Settings.Default.Save();
                        mainWnd.discardButton.IsEnabled = false;
                        mainWnd.discardButton.Opacity = 0.5;
                        mainWnd.discardButton.ToolTip = "仅fm.私人兆赫有效";
                        mainWnd.ChanName.Content = "fm.公共兆赫";
                        mainWnd.subChanName.Content = rb.Content.ToString() + "兆赫";
                        mainWnd.getSonglist.currentchannelNo = selectedChannelNo;
                        mainWnd.getSonglist.Next(true);
                    }
                };
                chanCanvas.Children.Add(rb);
            }

            if (selectedChannelNo > 0)
            {
                for (int i = 0; i < publicChannelLists.Count; i++)
                {
                    if (publicChannelLists[i].channelNo == selectedChannelNo)
                    {
                        RadioButton rb = chanCanvas.Children[i] as RadioButton;
                        rb.IsChecked = true;
                    }
                }
            }
        }

        /// <summary>
        /// 选中私人兆赫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void priChecked(object sender, RoutedEventArgs e)
        {
            chanCanvas.Children.Clear();
            for (int i = 0; i < privateChannelLists.Count; i++)
            {
                RadioButton rb = new RadioButton();
                Canvas.SetLeft(rb, 10 + 80 * (i / 6));
                Canvas.SetTop(rb, 10 + 20 * (i % 6));
                rb.Content = privateChannelLists[i].channelName;
                //if (rb.Content.ToString().Length < 4)
                //    rb.Content = rb.Content.ToString() + "兆赫";
                rb.Foreground = Brushes.White;
                int chanNo = privateChannelLists[i].channelNo;
                rb.Checked += delegate
                {
                    if (selectedChannelNo != chanNo)
                    {
                        selectedChannelNo = chanNo;
                        this.Close();
                        Properties.Settings.Default.ChannelName = "fm.私人兆赫";
                        Properties.Settings.Default.SubChannelName = rb.Content.ToString();
                        Properties.Settings.Default.Save();
                        mainWnd.discardButton.IsEnabled = true;
                        mainWnd.discardButton.Opacity = 1.0;
                        mainWnd.discardButton.ToolTip = "不再播放";
                        mainWnd.ChanName.Content = "fm.私人兆赫";
                        mainWnd.subChanName.Content = rb.Content;
                        mainWnd.getSonglist.currentchannelNo = selectedChannelNo;
                        mainWnd.getSonglist.Next(true);
                    }
                };
                chanCanvas.Children.Add(rb);
            }

            if (selectedChannelNo <= 0)
            {
                if (selectedChannelNo == 0)
                {
                    RadioButton rb = chanCanvas.Children[0] as RadioButton;
                    rb.IsChecked = true;
                }
                else if (selectedChannelNo == -3)
                {
                    RadioButton rb = chanCanvas.Children[1] as RadioButton;
                    rb.IsChecked = true;
                }
            }
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

    /// <summary>
    /// 频道类[频道ID和频道名]
    /// </summary>
    public class Channel
    {
        public int channelNo { get; set; }
        public string channelName { get; set; }
        public Channel(int channelNo, string channelName)
        {
            this.channelNo = channelNo;
            this.channelName = channelName;
        }
    }
}

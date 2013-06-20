using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace DoubanFM
{
    public class GetSongList
    {
        private int failcount = 0;//记录加载歌曲失败的次数
        //需要手动设置的参数
        public int currentchannelNo = -1;
        public int lastchannelNo = -1;

        private Queue<Song> currentqueue = new Queue<Song>();
        private Queue<Song> historyqueue = new Queue<Song>();
        public static Song currentsong = null;//当前音乐

        private Queue<string> historysong = new Queue<string>();

        public delegate void NextCompletedEventHandler(object sender, Song currentsong);
        public event NextCompletedEventHandler NextCompleted;

        public delegate void FailLoadedEventHandler();
        public event FailLoadedEventHandler FailLoaded;
        //获取下一首歌曲
        public void Next(bool isplaying)
        {
            if (currentchannelNo == -1)
                return;
            if (currentchannelNo != lastchannelNo)
            {
                lastchannelNo = currentchannelNo;
                historyqueue.Clear();
                currentqueue.Clear();
            }
            if (currentqueue.Count > 0)
            {
                currentsong = currentqueue.Dequeue();//取出当前的歌曲
                if (NextCompleted != null)
                {
                    NextCompleted(this, currentsong);
                    historyqueue.Enqueue(currentsong);
                    while (historyqueue.Count > 20)
                        historyqueue.Dequeue();
                }
                if (currentqueue.Count == 0)//当前队列的最后一首歌
                {
                    if (isplaying)
                        GetNewList("p");
                    else
                        GetNewList("n");
                }
                else
                {
                    if (isplaying)
                        GetNewList("s");
                    else
                        GetNewList("e");
                }
            }
            else
            {
                GetNewList("n");
                //Next(isplaying);
                if (currentqueue.Count > 0)
                {
                    currentsong = currentqueue.Dequeue();//取出当前的歌曲
                    if (NextCompleted != null)
                    {
                        NextCompleted(this, currentsong);
                    }
                }
            }
        }

        //向服务器发送“喜欢”、“不喜欢”标志
        public void Like(bool islike)
        {
            if (islike)
                GetNewList("r");
            else
            {
                GetNewList("u");
            }
        }

        //私人频道，垃圾桶（不再播放）功能
        public void Ban()
        {
            GetNewList("b");
            Next(true);
        }

        private void GetNewList(string type)
        {
            //获取历史歌曲字符串
            if (currentsong != null)
            {
                historysong.Enqueue(@"|" + currentsong.sid + ":" + type);
                while (historysong.Count > 20)
                {
                    historysong.Dequeue();
                }
            }
            StringBuilder historysongstring = new StringBuilder();
            foreach (string s in historysong)
            {
                historysongstring.Append(s);
            }

            Parameters parameters = new Parameters();
            parameters.Add("type", type);
            if (type != "n")
                parameters.Add("sid", currentsong.sid);

            parameters.Add("h", historysongstring.ToString());
            parameters.Add("channel", currentchannelNo.ToString());
            parameters.Add("from", "mainsite");

            string PostUrl = ConnectionBase.ConstructUrlWithParameters(@"http://douban.fm/j/mine/playlist", parameters);
            string SongJson = new ConnectionBase().Get(PostUrl, @"application/json, text/javascript, */*; q=0.01", @"http://douban.fm");
            SongResult songresult = SongResult.FromJson(SongJson);
            //自然结束，不返回列表，其余的均返回新的播放列表，并且按照新列表进行播放
            if (type == "e")
            {
                return;
            }
            if (songresult == null || songresult.r != 0)
            {
                failcount = failcount + 1;
                if (failcount <= 3)
                    Next(false);//如果当前歌曲获取失败的话，重新获取
                else
                {
                    if (FailLoaded != null)
                        FailLoaded();
                }
            }
            else
            {
                failcount = 0;
                if (currentqueue.Count > 0)
                    currentqueue.Clear();//清空之前的列表，开始新的播放列表
                foreach (Song s in songresult.song)
                {
                    if (s.subtype != "T")//剔除广告音频
                        currentqueue.Enqueue(s);
                }
            }
        }
    }
}

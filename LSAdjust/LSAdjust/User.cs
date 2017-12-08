using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace LSAdjust
{
    //连接服务器玩家信息
    class User
    {
        public TcpClient client;
        public StreamReader sr;
        public StreamWriter sw;
        public string userName;

        public bool isPlaying=false;

        public User(TcpClient client)
        {
            this.client = client;
            this.userName = "";
            NetworkStream ns = client.GetStream();
            sr = new StreamReader(ns, System.Text.Encoding.UTF8);
            sw = new StreamWriter(ns, System.Text.Encoding.UTF8);
        }
    }
}

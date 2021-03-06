﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace LSServer
{
    //连接服务器玩家信息
    class User
    {
        public TcpClient client;
        public StreamReader sr;
        public StreamWriter sw;
        public string userName;
        public Thread threadReceive;

        public bool ready=false;
        public bool finished = false;
        public float sumErr;//残差平方和，注意C#类里面的变量如果不赋值，初始值为0.
        //public int score;
        //public int status;
        public int rank;//排名

        public User(TcpClient client)
        {
            this.client = client;
            this.userName = "";
            NetworkStream ns = client.GetStream();
            sr = new StreamReader(ns, System.Text.Encoding.UTF8);
            sw = new StreamWriter(ns, System.Text.Encoding.UTF8);
        }
        public void Close()
        {
            sr.Close();
            sw.Close();
            client.Close();
        }
    }
}

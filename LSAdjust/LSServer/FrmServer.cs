﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LSServer
{
    public partial class FrmServer : Form
    {
        private TcpListener myListener;
        private Service service;
        int MAX_TABLE = 0;
        int SUM_USER;
        private Table[] tables;//桌子表
        System.Collections.Generic.List<User> userList = new List<User>();//创建user链表,因为还有一些users没入座。


        public FrmServer()
        {
            InitializeComponent();
            service = new Service(listBox1);
        }

        private void FrmServer_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
             if (int.TryParse(textBoxMaxTables.Text, out MAX_TABLE) == false
               || int.TryParse(textBoxMaxUsers.Text, out Table.MAX_USER) == false)
            {
                MessageBox.Show("请输入规定范围内的整数"); return;
            }

            if (Table.MAX_USER< 1 || Table.MAX_USER >10)
            {
                MessageBox.Show("允许进入的人数只能在1-10之间"); return;
            }

            if (MAX_TABLE < 1 || MAX_TABLE > 100)
            {
                MessageBox.Show("允许的桌数只能在1-100之间");
                return;
            }
            SUM_USER = MAX_TABLE * Table.MAX_USER;
            textBoxMaxUsers.Enabled = false;
            textBoxMaxTables.Enabled = false;//启动服务器后不可更改
            tables = new Table[MAX_TABLE];//创建桌子对象表
            for (int i = 0; i < MAX_TABLE; i++)
            {
                tables[i] = new Table();
            }
            int port = int.Parse(textBoxPort.Text);
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress localAddress = ips[0];
            myListener = new TcpListener(localAddress, port);
            myListener.Start();
            service.SetListBox(string.Format("开始在{0}：{1}监听客户端连接", localAddress, port));
            ThreadStart ts = new ThreadStart(ListenClientConnect);
            Thread myThread = new Thread(ts);//监听客户端连接线程
            myThread.IsBackground = true;
            //创建一个线程监听客户端连接请求
            myThread.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }
        private void ListenClientConnect()
        {
            while(true)
            {
                TcpClient newClient = null;
                try
                {
                    newClient = myListener.AcceptTcpClient();//创建电话的对方
                }
                catch (Exception)
                {
                    break;
                }
                ParameterizedThreadStart pts = new ParameterizedThreadStart(ReceiveData);
                User user = new User(newClient);//将tcpclient用user封装。
                user.threadReceive = new Thread(pts);
                user.threadReceive.IsBackground = true;
                user.threadReceive.Start(user);//将user作为参数传入
                userList.Add(user);
                service.SetListBox(string.Format("{0}进入", newClient.Client.RemoteEndPoint));
                service.SetListBox(string.Format("当前连接用户数：{0}", userList.Count));

            }
        }
        private void ReceiveData(object obj)//这个线程由一个user独有。
        {
            
            User user = (User)obj;
            bool exitWhile = false;
            int tableIndex = -1;//桌号
            int seat = -1;//座位号
            //用于控制是否退出循环，因为无法在switch中break掉循环
            while (!exitWhile )
            {
                string receiveStr = null;
                try
                {
                    receiveStr = user.sr.ReadLine();//从流中读取一行
                }
                catch (Exception)
                {
                    service.SetListBox("数据接收失败");
                }
                //TcpClient对象将套接字进行了封装，如果TcpClient对象关闭了，
                //但是底层套接字未关闭，并不产生异常，但是读取的结果为null
                if (receiveStr == null)
                {
                    service.SetListBox(string.Format("与{0}失去联系，已终止接收该用户信息",user.client.Client.RemoteEndPoint));
                    //如果该用户正在游戏桌上，则退出游戏桌
                    RemoveClientFromUser(user);//掉线，让他从桌子上离开。
                    return;
                }
                service.SetListBox(string.Format("来自{0}:{1}", user.userName, receiveStr));
                string[] info = receiveStr.Split(',');
                string sendStr = "";
                //信息初始化
                switch (info[0])//信息交换必须遵循关键词，信息的格式,具体格式在case中说明。
                {
                    case "Login":
                        //格式：Login,昵称
                        //刚刚登录
                        if (userList.Count > SUM_USER)
                        {
                            sendStr = "Sorry";
                            service.SetListBox("人数已满，拒绝" + info[1] + "进入游戏室");
                            exitWhile = true;
                        }
                        else
                        {
                            user.userName = string.Format(info[1]);
                            //允许该用户进入游戏室，即将各桌是否有人的情况发给该用户
                            sendStr = "Table," + this.GetSeatingChartStr()+","+MAX_TABLE;
                            service.Send2User(user, sendStr);
                        }
                        break;
                    case "Logout":
                        //格式：Logout
                        //退出
                        service.SetListBox(string.Format("{0}退出游戏", user.userName));
                        sendStr = "Logout";
                        service.Send2User(user, sendStr);
                        userList.Remove(user);
                        RemoveClientFromUser(user);
                        exitWhile = true;
                        break;
                    case "SitDown":
                        //格式：SitDown,桌号，座位号
                        //该用户坐到某座位上
                        tableIndex = int.Parse(info[1]);
                        seat = int.Parse(info[2]);
                        tables[tableIndex].users[seat] = user;
                        //table[tableIndex].players[side].people = true;
                        service.SetListBox(string.Format("{0}在第{1}桌第{2}座入座", user.userName, tableIndex + 1, seat + 1));
                        //先告诉该用户其余人是否入座
                        //发送格式：Message,消息内容
                        for (int i = 0; i < Table.MAX_USER; i++)
                            if (tables[tableIndex].users[i] != null && tables[tableIndex].users[i]!=user)
                            {
                                sendStr = string.Format("Message,"+string.Format("{0}在第{1}座入座",tables[tableIndex].users[i].userName,i+1));
                                service.Send2User(user, sendStr);
                            }
                        //告诉本桌其他用户该用户入座(也可能对方无人）
                        //发送格式：SitDown, 座位号, 用户名
                        sendStr = string.Format("Message,{0}在第{1}座入座", user.userName, seat + 1);
                        service.Send2Table(tables[tableIndex], sendStr,seat);
                        //重新将游戏室各桌情况发送给所有用户
                        service.Send2All(userList, "TableChange," + tableIndex+','+seat+",1");
                        break;
                    case "Ready":
                        //格式：Ready,关数
                        //该用户单击了开始按钮
                        tables[tableIndex].users[seat].ready = true;
                        //添加sendString..............

                        //如果本桌每个人都开始了，就开始发牌deal...........
                        object readylock = new object();
                        lock (readylock)
                        {
                            int sumReady = 0;
                            for (int i = 0; i < Table.MAX_USER; i++)
                                if (tables[tableIndex].users[i] != null)
                                    if (tables[tableIndex].users[i].ready)
                                        sumReady++;
                            if (sumReady == Table.MAX_USER)
                            {
                                sendStr = "Deal"; //格式：Deal ,总点数，每个点的x，y坐标。
                                int n = tables[tableIndex].round;
                                if (n < 4)     ///1-3关是直线模式
                                {
                                    tables[tableIndex].Cal_Line();
                                }
                                else if (n > 3 || n < 7) ///4-6关是横轴抛物线模式
                                {
                                    tables[tableIndex].Cal_Poly1();
                                }
                                else if (n > 6 || n < 10) ///7-9关是竖轴抛物线模式
                                {
                                    tables[tableIndex].Cal_Poly2();
                                }
                                tables[tableIndex].Cal_Line();//注意在calline后面会计算回归。
                                PointF[] points = tables[tableIndex].points;
                                int sum = points.Length;
                                sendStr += ',' + sum.ToString();
                                for (int i = 0; i < sum; i++)
                                {
                                    string x = points[i].X.ToString();
                                    string y = points[i].Y.ToString();
                                    sendStr += ',' + x + ',' + y;
                                }
                                service.Send2Table(tables[tableIndex], sendStr);
                                for (int i = 0; i < Table.MAX_USER; i++)
                                {
                                    if (tables[tableIndex].users[i] != null)
                                    {
                                        tables[tableIndex].users[i].ready = false;
                                    }
                                }
                                tables[tableIndex].round++;//关数加1。
                                
                            }
                        }//保证同一时间只有一个线程在统计、发牌，发完牌之后，ready置为false。
                        

                        break;
                    case "Finish":
                        //格式:Finish,x1,y1,x2,y2
                        //mode =1 直线 mode=2抛物线
                        //算出残差平方和并存入
                        float x1 = int.Parse(info[1]);   //直线或者抛物线准线的 两点坐标
                        float y1 = int.Parse(info[2]);
                        float x2 = int.Parse(info[3]);
                        float y2 = int.Parse(info[4]);
                        float x0 = int.Parse(info[5]);    //抛物线焦点坐标
                        float y0 = int.Parse(info[6]);
                        float a = 0, b = 0, c = 0, p = 0;
                        user.sumErr= tables[tableIndex].Calcu_sumErr(x1, y1, x2, y2,x0,y0,a,b,c,p);//东林写
                        user.finished = true;
                        object finishedlock = new object();
                        lock(finishedlock)
                        {
                            tables[tableIndex].sumFinished++;
                        }
                        //user.rank = tables[tableIndex].Calcu_rank();
                        if(tables[tableIndex].sumFinished==Table.MAX_USER)
                        {
                            sendStr = "Result";//rank sumerr ,
                            sendStr += user.sumErr.ToString() + "," + user.rank.ToString();  //发回误差平方和和名次
                            service.Send2Table(tables[tableIndex], sendStr);
                        }
                        //int n = tables[tableIndex].round; //获取当前关数
                        //炜哥，此处发送回归分析结果，具体发送格式还待定
                        if (tables[tableIndex].round < 4)
                        {
                            ///将回归分析结果发给玩家
                            sendStr = "Result" + "直线" + a.ToString() + b.ToString();
                            service.Send2Table(tables[tableIndex], sendStr);
                        }            ///1-3关
                        else if (tables[tableIndex].round > 3 || tables[tableIndex].round < 7)
                        {
                            /// 4-6关
                            /// ///将回归分析结果发给玩家
                            sendStr = "Result" + "横轴抛物线" + p.ToString();
                            service.Send2Table(tables[tableIndex], sendStr);

                        }
                        else if (tables[tableIndex].round > 6 || tables[tableIndex].round < 10)
                        {
                            ///7-9关
                            //////将回归分析结果发给玩家
                            sendStr = "Result" + "竖轴抛物线" + a.ToString() + b.ToString() + c.ToString();
                            service.Send2Table(tables[tableIndex], sendStr);
                        }
                        tables[tableIndex].round += 1;    //进入下关
                        break;

                    case "Chat":
                        //格式：Chat,对话内容
                        //说的话可能包含逗号
                        sendStr = string.Format("Chat,{0},{1}", user.userName, info[1]);
                        //格式：chat，userName,说话内容
                        service.Send2Table(tables[tableIndex], sendStr);
                        break;
                    case "StandUp":
                        //格式：StandUp
                        //从游戏中退出
                        tables[tableIndex].users[seat] = user;
                        //table[tableIndex].players[side].people = true;
                        service.SetListBox(string.Format("{0}从第{1}桌第{2}座离开", user.userName, tableIndex + 1, seat + 1));
                    
                        //发送格式：Message,消息内容
                        for (int i = 0; i < Table.MAX_USER; i++)
                            if (tables[tableIndex].users[i] != null && tables[tableIndex].users[i]!=user)
                            {
                                sendStr = string.Format("Message,"+string.Format("{0}从第{1}座离开",tables[tableIndex].users[i].userName,i+1));
                                service.Send2User(user, sendStr);
                            }
                        //告诉本桌其他用户该用户离开
                        //发送格式：StandUp, 座位号, 用户名
                        sendStr = string.Format("Message,{0}从第{1}座离开", user.userName, seat + 1);
                        service.Send2Table(tables[tableIndex], sendStr,seat);
                        //重新将游戏室各桌情况发送给所有用户
                        service.Send2All(userList, "TableChange," + tableIndex+','+seat+",0");
                        break;
                }
            }//endwhile
        }


        private void RemoveClientFromUser(User user)//掉线了
        {
            for (int i = 0; i < tables.Length; i++)
                for (int j = 0; j <Table.MAX_USER; j++)
                    if (tables[i].users[j] == user)
                    {
                        //table[i].StopTimer();
                        //发送格式：Lost,座位号,用户名
                        tables[i].users[j] = null;
                        service.Send2Table(tables[i], string.Format("Lost,{0},{1}", j, user.userName),j);
               
                        return;
                    }
        }
        private string GetSeatingChartStr()
        ///得到每个位子是否有人，输出一个01字符串
        {
            string str = "";
            for (int i = 0; i < tables.Length; i++)
            {
                for (int j = 0; j < Table.MAX_USER; j++)
                {
                    str += tables[i].users[j] != null ? "1" : "0";
                }
            }
            return str;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            service.SetListBox(string.Format("当前连接用户数:{0}", userList.Count));
            service.SetListBox("开始停止服务，并依此使用用户退出");
            service.Send2All(userList, "End,see you next time" );//服务器断开，end
            for (int i = 0; i < userList.Count; i++)
            {
               userList[i].threadReceive.Abort();
                userList[i].client.Close();
                
            }
            userList.RemoveRange(0, userList.Count); 
            myListener.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            textBoxMaxTables.Enabled = true;
            textBoxMaxUsers.Enabled = true;
        }

        private void FrmServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myListener != null)
            {
                btnStop_Click(null, null);
            }
        }

        /// <summary>
        /// 仅用于测试一些功能，正式发布的时候会删除。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Matrix.TestMatirx();
        }
    }
}

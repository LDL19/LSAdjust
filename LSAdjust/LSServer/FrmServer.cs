using System;
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
        private Table[] tables;
        /// <summary>
        /// 桌子表，里面有users表，相当于位子。
        /// </summary>
        System.Collections.Generic.List<User> userList = new List<User>();//创建user链表

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
                Thread threadReceive = new Thread(pts);
                threadReceive.IsBackground = true;
                User user = new User(newClient);
                threadReceive.Start(user);//将user作为参数传入
                userList.Add(user);
                service.SetListBox(string.Format("{0}进入", newClient.Client.RemoteEndPoint));
                service.SetListBox(string.Format("当前连接用户数：{0}", userList.Count));

            }
        }
        private void ReceiveData(object obj)
        {
            User user = (User)obj;
            TcpClient tcpclient = user.client;
            bool normalExit = false;
            //是否正常退出线程
            bool exitWhile = false;
            //用于控制是否退出循环
            while (exitWhile == false)
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
                    if (normalExit == false)
                    {
                        if (tcpclient.Connected == true)
                        {
                            service.SetListBox(string.Format(
                                "与{0}失去联系，已终止接收该用户信息",
                                  tcpclient.Client.RemoteEndPoint));
                        }
                        //如果该用户正在游戏桌上，则退出游戏桌
                        RemoveClientfromPlay(user);
                    }
                    break;
                }
                service.SetListBox(string.Format("来自{0}:{1}", user.userName, receiveStr));
                string[] info = receiveStr.Split(',');
                int tableIndex = -1;//桌号
                int seat = -1;//座位号
                //int anotherSide = -1;//对方座位号
                string sendString = "";
                //信息初始化
                switch (info[0])//信息交换必须遵循关键词，信息的格式,具体格式在case中说明。
                {
                    case "Login":
                        //格式：Login,昵称
                        //刚刚登录
                        if (userList.Count > SUM_USER)
                        {
                            sendString = "Sorry";
                            service.SetListBox("人数已满，拒绝" + info[1] + "进入游戏室");
                            exitWhile = true;
                        }
                        else
                        {
                            user.userName = string.Format("[{0}--{1}]", info[1], tcpclient.Client.RemoteEndPoint);
                            //允许该客户进入游戏室，即将各桌是否有人的情况发给该用户
                            sendString = "Tables," + this.GetSeatingChartStr()+","+MAX_TABLE;
                            service.Send2User(user, sendString);
                        }
                        break;
                    case "Logout":
                        //格式：Logout
                        //退出
                        service.SetListBox(string.Format("{0}退出游戏室", user.userName));
                        normalExit = true;
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
                        //发送格式：SitDown,座位号，用户名
                        for (int i = 0; i < Table.MAX_USER; i++)
                            if (tables[tableIndex].users[i] != null && tables[tableIndex].users[i]!=user)
                            {
                                sendString = string.Format("SitDown,{0},{1}", i, tables[tableIndex].users[i].userName);
                                service.Send2User(user, sendString);
                            }
                        //告诉本桌用户该用户入座(也可能对方无人）
                        //发送格式：SitDown, 座位号, 用户名
                        sendString = string.Format("SitDown,{0},{1}", seat, user.userName);
                        service.Send2Table(tables[tableIndex], sendString);
                        //重新将游戏室各桌情况发送给所有用户
                        service.Send2All(userList, "Tables," + this.GetSeatingChartStr());
                        break;
                    case "Start":
                        //格式：Start,桌号，座位号
                        //该用户单击了开始按钮
                        tableIndex = int.Parse(info[1]);
                        seat = int.Parse(info[2]);
                        tables[tableIndex].users[seat].isPlaying = true;
                        //添加sendString..............


                        //if (seat == 0)
                        //{
                        //    anotherSide = 1;
                        //    sendString = "Message," + startCout + ",黑方已经开始";
                        //}
                        //else
                        //{
                        //    anotherSide = 0;
                        //    sendString = "Message," + startCout + ",白方已经开始";
                        //}
                        service.Send2Table(tables[tableIndex], sendString);

                        //如果本桌每个人都开始了，就开始发牌deal...........
                        sendString = "Deal";
                        break;
                    case "chat":
                        //格式：chat,桌号，对话内容
                        tableIndex = int.Parse(info[1]);
                        //说的话可能包含逗号
                        sendString = string.Format("chat,{0},{1}", user.userName, info[2]);
                        //格式：chat，userName,说话内容
                        service.Send2Table(tables[tableIndex], sendString);
                        break;

                }
            }//endwhile
        }


        private void RemoveClientfromPlay(User user)//一方中途逃跑
        {
            for (int i = 0; i < tables.Length; i++)
            {
                for (int j = 0; j <Table.MAX_USER; j++)
                {
                    if (tables[i].users[j] != null)
                    {
                        if (tables[i].users[j] == user)
                        {
                            StopPlayer(i, j);
                            return;
                        }
                    }
                }
            }
        }

        //停止第i桌游戏
        private void StopPlayer(int i, int j)
        {
            //table[i].StopTimer();
            tables[i].users[j].isPlaying = false;
            tables[i].users[j] = null;
            int otherSide = (j + 1) % 2;
            if (tables[i].users[otherSide]!=null)
            {
                if (tables[i].users[otherSide].client.Connected == true)
                {
                    //发送格式：Lost,座位号,用户名
                    service.Send2User(tables[i].users[otherSide],
                    string.Format("Lost,{0},{1}",
                    j, tables[i].users[j].userName));

                }
                tables[i].users[otherSide] = null;
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
            service.SetListBox(string.Format("目前连接用户数:{0}", userList.Count));
            service.SetListBox("开始停止服务，并依此使用用户退出");
            for (int i = 0; i < userList.Count; i++)
            {
                userList[i].client.Close();
            }
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
    }
}

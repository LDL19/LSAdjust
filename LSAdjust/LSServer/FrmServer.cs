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
        private Table[] tables;//桌子表
        System.Collections.Generic.List<User> userList = new List<User>();//创建user链表,因为还有一些users没入座。

        //int ch = 1; Point[] point = null; Point cpoint; double a = 0; double b = 0; double c = 0; double p = 0;Point [] point3;Point[] point4;      //模型参数定义为全局变量
        //int ch1 = 1; Point[] point1 = null; Point[] point2 = null; Point cpoint1; double a1 = 0; double b1 = 0; double c1 = 0; double p1 = 0;
        //int n = 1;                    //所处关数，以此来确定发送散点类型


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
                int tableIndex = -1;//桌号
                int seat = -1;//座位号
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
                        sendStr = "你已经退出游戏";
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
                    case "Start":
                        //格式：Start,桌号，座位号
                        //该用户单击了开始按钮
                        tableIndex = int.Parse(info[1]);
                        seat = int.Parse(info[2]);
                        tables[tableIndex].users[seat].ready = true;
                        //添加sendString..............

                        //如果本桌每个人都开始了，就开始发牌deal...........
                        int sumReady = 0;
                        for (int i = 0; i < Table.MAX_USER; i++)
                            if(tables[tableIndex].users[i]!=null)
                                if (tables[tableIndex].users[i].ready)
                                    sumReady++;
                        if(sumReady==Table.MAX_USER)
                        {
                            sendStr = "Deal";
                            PointF[] points = tables[tableIndex].Cal_Line();
                            int sum = points.Length;
                            sendStr += ',' + sum.ToString();
                            for (int i=0;i<sum;i++)
                            {
                                string x = points[i].X.ToString();
                                string y = points[i].Y.ToString();
                                sendStr += ',' + x + ',' + y;

                            }
                            service.Send2Table(tables[tableIndex], sendStr);
                        }
                        
                        //if (n < 4)
                        //    ch = 1;      //1-3关
                        //else if (n > 3 || n < 6)
                        //    ch = 2;      // 4-5关
                        //else if(n>5||n<10)
                        //    ch = 3;       //6-9关

                        //Cal_point(ch, point, cpoint, a, b, c, p);
                        //for (int i = 0; i < 30; i++)             //将点传输出去
                        //{
                        //    sendStr +=(point[i].ToString()+"\r\n");
                        //}
                        //service.Send2Table(tables[tableIndex], sendStr);
                                                                          
                        //Cal_Huigui(point,point3);                                                //此处还得添加一个函数，根据散点计算样本拟合线，再传回线上的点point[]

                        break;
                    case "Finish":
                        tableIndex = int.Parse(info[1]);
                        seat = int.Parse(info[2]);

                        ////int ch1=1; Point[] point1=null;Point [] point2=null; Point cpoint1; double a1=0; double b1=0; double c1=0; double p1=0;

                        ////?????此处是需for循环读取每个玩家传回的数据吗？由于没看太懂你写的通讯代码，恳请炜哥改正

                        //receiveStr = user.sr.ReadLine();//从流中读取一行，第一，二行为确定直线或准线的两个点
                        ////字符串长度暂定为5，如有其它读取点坐标的方法，再做改正！！！！
                        //point1[0].X =int.Parse( receiveStr.Substring(0,5));      //读取第一个点
                        //point1[0].Y = int.Parse(receiveStr.Substring(5, 5));

                        //receiveStr = user.sr.ReadLine();
                        //point1[1].X =int.Parse( receiveStr.Substring(0,5));     //读取第二个点
                        //point1[1].Y = int.Parse(receiveStr.Substring(5, 5));

                        //receiveStr = user.sr.ReadLine();
                        //cpoint1.X =int.Parse( receiveStr.Substring(0,5)); //读取焦点
                        //cpoint1.Y = int.Parse(receiveStr.Substring(5, 5));

                        //if (n < 4)
                        //{
                        //    ch1 = 1;   
                        //    point4=point3;
                        //}//1-3关
                        //else if (n > 3 || n < 6)
                        //{
                        //    ch1 = 2;      // 4-5关
                        //    point4 = point;                   //！！！！回归分析只能对直线问题进行平差，当为抛物线时，残差平方和只能与原模型所对应的点进行计算
                        //}
                        //else if (n > 5 || n < 10)
                        //{
                        //    ch1 = 3;       //6-9关
                        //    point4 = point;
                        //}
                        //Cal_Line(ch1,point1,point2,cpoint,a1, b1,  c1,  p1) ;      //根据传回的点恢复模型参数并计算点坐标

                        //int dist=0;
                        //Cal_dis(point4, point2, dist);       //计算残差平方和
                        //sendStr = ("Deal" + dist.ToString());
                        //service.Send2Table(tables[tableIndex], sendStr);    //传回积分

                        //n = n + 1;        //通关，进入下一关
                        break;
                    case "Chat":
                        //格式：chat,桌号，对话内容
                        tableIndex = int.Parse(info[1]);
                        //说的话可能包含逗号
                        sendStr = string.Format("Chat,{0},{1}", user.userName, info[2]);
                        //格式：chat，userName,说话内容
                        service.Send2Table(tables[tableIndex], sendStr);
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
            for (int i = 0; i < userList.Count; i++)
            {
                userList[i].client.Close();
                userList[i].threadReceive.Abort();
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

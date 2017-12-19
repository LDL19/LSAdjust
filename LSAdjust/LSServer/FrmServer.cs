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
                        sendStr = "Deal";
                        service.Send2Table(tables[tableIndex], sendStr);
                        break;
                    case "Finish":

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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace LSClient
{
    public partial class FrmRoom : Form
    {
        private TcpClient client = null;
        private StreamReader sr;
        private StreamWriter sw;
        private Service service;
        NetworkStream netSteam;
        int MAX_TABLE=0;//最大桌子数
        int MAX_USER=0;//每桌允许人数。
        //private bool[,] seatingChart;//用于知道此地有没有人
        private int seat = -1;
        //所坐游戏的座位号，-1表示未入座
        //private bool receiveCmd = false;//接受命令而改变checkbox的状态，是true，否则是false
        //private bool normalExit = false;//是否正常退出线程
        private CheckBox[,] CheckBoxGameTables;//注意为了不引发checkchanged事件，在写代码的时候注意保护
        private FrmPlay FrmPlay;
        private Thread threadReceive;

        public FrmRoom()
        {
            InitializeComponent();
        }

        private void FrmRoom_Load(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(textBoxName.Text.Trim().Length==0)
            {
                MessageBox.Show("请输入用户名","", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if(textBoxName.Text.IndexOf(',')!=-1)
            {
                MessageBox.Show("昵称中不能包含逗号", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                //IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(textBoxAddr.Text), int.Parse(textBoxPort.Text));
                client = new TcpClient(Dns.GetHostName(), int.Parse(textBoxPort.Text));
            }
            catch
            {
                MessageBox.Show("与服务器连接失败", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            netSteam = client.GetStream();
            btnLogin.Enabled = false;
            sr = new StreamReader(netSteam, System.Text.Encoding.UTF8);
            sw = new StreamWriter(netSteam, System.Text.Encoding.UTF8);
            service = new Service(listBox1, sw);
            service.Send2Server("Login,"+textBoxName.Text.Trim());
            threadReceive = new Thread(new ThreadStart(ReceiveData));//另起线程监听
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }
        private void ReceiveData()//注意从该线程操作控件时要添加委托。
        {
            bool exitWhile = false;//因为在switch语句中只能break switch
            while(!exitWhile)
            {
                string receiveString = null;
                try
                {
                    receiveString = sr.ReadLine();
                }
                catch
                {
                    service.SetListBox("接收数据失败");
                }
                if(receiveString==null)
                {
                    MessageBox.Show("与服务器失去联系，游戏无法继续！");
                    if(seat!=-1)
                        ExitFrmPlay();
                    seat = -1;
                    break;
                }
                service.SetListBox("收到：" + receiveString);
                string[] info = receiveString.Split(',');
                switch(info[0])
                {
                    case "Sorry":
                        MessageBox.Show("游戏室人数已满，无法进入");
                        exitWhile = true;
                        break;
                    case "Table":
                        ///格式：tables+seatingchartstr+max_table
                        string s = info[1];
                        if (MAX_TABLE == 0)
                        {
                            MAX_TABLE = int.Parse(info[2]);
                            MAX_USER = s.Length / MAX_TABLE;
                            CheckBoxGameTables = new CheckBox[MAX_TABLE, MAX_USER];
                            for(int i=0;i<MAX_TABLE;i++)
                                AddCheckBoxToPanel(s, i);
                        }
                        break;
                    case "TableChange":
                        UpdateCheckBox(CheckBoxGameTables[int.Parse(info[1]), int.Parse(info[2])], int.Parse(info[3]) == 1 ? true : false);
                        break;
                    case "Message":
                        //系统消息
                        FrmPlay.service.SetListBox("系统消息:" + info[1]);
                        break;
                    case "Chat"://收到chat
                        FrmPlay.service.SetListBox(info[1]+"说:"+info[2]);
                        break;
                    
                }
            }
        }
        delegate void ExitFrmPlayDelegate();
        private void ExitFrmPlay()
        {
            if(FrmPlay.InvokeRequired)
            {
                ExitFrmPlayDelegate d = new ExitFrmPlayDelegate(ExitFrmPlay);
                this.Invoke(d);
            }
            else
                FrmPlay.Close();
        }
        delegate void PanelCallback(string s, int i);
        private void AddCheckBoxToPanel(string s,int i)//添加一行
        {
            if(panel1.InvokeRequired==true)
            {
                PanelCallback d = new PanelCallback(AddCheckBoxToPanel);
                this.Invoke(d, s, i);
            }
            else
            {
                Label label = new Label();
                label.Location=new Point(10,15+i*30);
                label.Text = string.Format("第{0}桌：", i+1);
                label.Width = 70;
                this.panel1.Controls.Add(label);
                for(int j=0;j<MAX_USER;j++)
                {
                    int x = 100+j * 60;
                    CheckBoxGameTables[i, j] = new CheckBox();
                    CheckBoxGameTables[i, j].Name = string.Format("check{0:0000}{1:0000}", i, j);
                    CheckBoxGameTables[i, j].Width = 60;
                    CheckBoxGameTables[i, j].Location = new Point(x, 10 + i * 30);
                    CheckBoxGameTables[i, j].Text = string.Format("座位{0}",j+1);
                    CheckBoxGameTables[i, j].TextAlign = ContentAlignment.MiddleLeft;
                    if(s[MAX_USER*i+j]=='1')
                    {
                        CheckBoxGameTables[i, j].Enabled = false;
                        CheckBoxGameTables[i, j].Checked = true;
                    }
                    else
                    {
                        CheckBoxGameTables[i, j].Enabled = true;
                        CheckBoxGameTables[i, j].Checked = false;
                    }
                    this.panel1.Controls.Add(CheckBoxGameTables[i, j]);
                    CheckBoxGameTables[i, j].Click += new EventHandler(checkBox_Click);
                }

            }
        }
        /// <summary>
        /// 每个checkbox的checked属性改变都会触发该事件
        /// </summary>
        private void checkBox_Click(object sender,EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if(checkbox.Checked)//进入房间
            {
                int i = int.Parse(checkbox.Name.Substring(5, 4));
                int j = int.Parse(checkbox.Name.Substring(9, 4));
                seat = j;
                service.Send2Server(string.Format("SitDown,{0},{1}",i,j));
                FrmPlay = new FrmPlay(i, j, sw);
                FrmPlay.Show();
                for (i = 0; i < MAX_TABLE; i++)
                    for (j = 0; j < MAX_USER; j++)
                        CheckBoxGameTables[i, j].Enabled = false;
            }
        }

        delegate void CheckBoxDelegte(CheckBox checkBox, bool flagChecked);
        private void UpdateCheckBox(CheckBox checkBox, bool flagChecked)
        {
            if(checkBox.InvokeRequired)
            {
                CheckBoxDelegte d = new CheckBoxDelegte(UpdateCheckBox);
                this.Invoke(d, checkBox, flagChecked);
            }
            else
            {
                checkBox.Checked = flagChecked;
                if (seat == -1)//没入座
                    checkBox.Enabled = !flagChecked;//别人入座了不能再坐
                //else//自己入座则不能再坐
                //    checkBox.Enabled = false;
            }
        }
    }
}

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
        private bool[,] seatingChart;//用于知道此地有没有人
        private int seat = -1;
        //所坐游戏的座位号，-1表示未入座
        private bool receiveCmd = false;//接受命令而改变checkbox的状态，是true，否则是false
        private bool normalExit = false;//是否正常退出线程
        private CheckBox[,] CheckBoxGameTables;
        private FrmPlay FrmPlay;

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
            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.Start();
        }
        private void ReceiveData()
        {
            bool exitWhile = false;//因为在switch语句中只能break switch
            bool normalExit = false;
            while(exitWhile==false)
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
                    if(normalExit==false)
                        MessageBox.Show("与服务器失去联系，游戏无法继续！");
                    if(seat!=-1)
                        ExitFrmPlay();
                    seat = -1;
                    normalExit = true;
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
                    case "Tables"://注意，每桌允许人数还没有发来
                        ///格式：tables+seatingchartstr+max_table
                        string s = info[1];
                        if (MAX_TABLE == 0)
                        {
                            MAX_TABLE = int.Parse(info[2]);
                            MAX_USER = s.Length / MAX_TABLE;
                            seatingChart = new bool[MAX_TABLE, MAX_USER];
                            for (int i = 0; i < MAX_TABLE; i++)
                                for (int j = 0; j < MAX_USER; j++)
                                    seatingChart[i, j] = s[MAX_USER * i + j] == '1' ? true : false;
                            CheckBoxGameTables = new CheckBox[MAX_TABLE, MAX_USER];
                            receiveCmd = true;
                            for(int i=0;i<MAX_TABLE;i++)
                                AddCheckBoxToPanel(s, i);
                            receiveCmd = false;
                        }
                        else
                        {
                            receiveCmd = true;
                            for (int i = 0; i < MAX_TABLE; i++)
                                for (int j = 0; j < MAX_USER; j++)
                                {
                                    seatingChart[i, j] = s[MAX_USER * i + j] == '1' ? true : false;
                                    if (s[MAX_USER * i + j] == '0')
                                        UpdateCheckBox(CheckBoxGameTables[i, j], false);
                                    else
                                        UpdateCheckBox(CheckBoxGameTables[i, j], true);
                                }
                            receiveCmd = false;
                        }
                        break;
                    case "chat":
                        FrmPlay.service.SetListBox(info[2]);
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
        private void AddCheckBoxToPanel(string s,int i)
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
                    int x = (j + 1) * 60;
                    CheckBoxGameTables[i, j] = new CheckBox();
                    CheckBoxGameTables[i, j].Name = string.Format("check{0:0000}{1:0000}", i, j);
                    CheckBoxGameTables[i, j].Width = 60;
                    CheckBoxGameTables[i, j].Location = new Point(x, 10 + i * 30);
                    CheckBoxGameTables[i, j].Text = string.Format("第{0}位",j+1);
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
                    CheckBoxGameTables[i, j].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                }

            }
        }
        /// <summary>
        /// 每个checkbox的checked属性改变都会触发该事件
        /// </summary>
        private void checkBox_CheckedChanged(object sender,EventArgs e)
        {
            if (receiveCmd == true)
                return;
            CheckBox checkbox = (CheckBox)sender;
            if(checkbox.Checked)
            {
                int i = int.Parse(checkbox.Name.Substring(5, 4));
                int j = int.Parse(checkbox.Name.Substring(9, 4));
                seat = j;
                service.Send2Server(string.Format("SitDown,{0},{1}",i,j));
                FrmPlay = new FrmPlay(i, j, sw);
                FrmPlay.Show();
            }
        }
        delegate void CheckBoxDelegte(CheckBox checkBox, bool isChecked);
        private void UpdateCheckBox(CheckBox checkBox, bool isChecked)
        {
            if(checkBox.InvokeRequired)
            {
                CheckBoxDelegte d = new CheckBoxDelegte(UpdateCheckBox);
                this.Invoke(d, checkBox, isChecked);
            }
            else
            {
                if(seat==-1)
                    checkBox.Enabled= !isChecked;
                else
                    checkBox.Enabled = false;
                checkBox.Checked = isChecked;
            }
        }
    }
}

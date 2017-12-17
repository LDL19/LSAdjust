using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LSClient
{
    public partial class FrmPlay : Form
    {
        private int tableIndex;
        private int seat;
        public Service service;  
        bool btnFinishflag = false;
        public FrmPlay(int tableIndex,int seat,StreamWriter sw)
        {
            InitializeComponent();
            service = new Service(listBox1,sw);
            this.tableIndex = tableIndex;
            this.seat = seat;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("确定要离开吗？", "离开", buttons);
            if(result==DialogResult.Yes )
            {
                if (btnFinishflag == true)
                {  
                    
                    //记录成绩
                    this.Dispose();


                
                }
                else
                { 
                     
                DialogResult result2 =  MessageBox.Show("还没提交，确定离开吗？","提示",buttons);
                if (result2 == DialogResult.Yes)
                {
                    //与重画一样的操作,待补充
                    this.Dispose();

                }
                else
                { }
                }
                }
            else
            {}
            }
       

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonSend_Click(object sender, EventArgs e)//发送chat
        {
            service.Send2Server(string.Format("Chat,{0},{1}", tableIndex, textBox1.Text));
            textBox1.Text = "";
        }
      
        private void btnFinish_Click(object sender, EventArgs e)
        {
            btnFinishflag = false;
            //发送结果到服务器
            btnFinishflag = true;
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            btnFinishflag = false;
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            btnFinishflag = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        public delegate void PutHandler(bool btnfinsh);//委托传值
        public PutHandler putBoolHandler;//委托对象
        private void button1_Click(object sender, EventArgs e)
        {
            if (putBoolHandler != null)
            {
                putBoolHandler(btnFinishflag);
            }
        }
    }
}

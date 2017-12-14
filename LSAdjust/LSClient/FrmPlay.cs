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
            if(result==DialogResult.OK)
            {

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonSend_Click(object sender, EventArgs e)//发送chat
        {
            service.Send2Server(string.Format("Chat,{0},{1}", tableIndex, textBox1.Text));
            textBox1.Text = "";
        }
    }
}

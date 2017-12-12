using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LSClient
{
    class Service
    {
        private ListBox listbox;
        StreamWriter sw;
        private delegate void SetListBoxCallback(string str);
        //声明一个委托，在主线程之外调用窗体需要使用
        private SetListBoxCallback setListBoxCallback;//创建委托的实例
        public Service(ListBox listbox,StreamWriter sw)
        {
            this.listbox = listbox;
            this.sw = sw;
            setListBoxCallback = new SetListBoxCallback(SetListBox);//指向某函数。
        }
        public void SetListBox(string str)
        {
            if(listbox.InvokeRequired)//在主线程之外调用
            {
                listbox.Invoke(setListBoxCallback, str);//在主线程触发该函数。
            }
            else
            {
                listbox.Items.Add(str);
                //listbox.SelectedIndex = listbox.Items.Count - 1;
                //listbox.ClearSelected();//造成一闪而过的画面？
            }
        }
        public void Send2Server(string str)
        {
            try
            {
                sw.WriteLine(str);
                sw.Flush();
            }
            catch
            {
                SetListBox("发送数据失败");
            }
        }

    } 
}

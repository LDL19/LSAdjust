using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSServer
{
    class Service
    {
        private ListBox listbox;
        private delegate void SetListBoxCallback(string str);
        //声明一个委托，在主线程之外调用窗体需要使用
        private SetListBoxCallback setListBoxCallback;//创建委托的实例
        public Service(ListBox listbox)
        {
            this.listbox = listbox;
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
        public void Send2User(User user,string str)
        {
            try
            {
                user.sw.WriteLine(str);
                user.sw.Flush();
                SetListBox(string.Format("向{0}发送{1}", user.userName, str));
            }
            catch(Exception)
            {
                SetListBox(string.Format("向{0}发送信息失败", user.userName));
            }
        }
        public void Send2Table(Table gameTable,string str)
        {
            for(int i=0;i<Table.MAX_PLAYER;i++)
            {
                if(gameTable.users[i]!=null)
                {
                    Send2User(gameTable.users[i],str);
                }
            }
        }

        public void Send2All(List<User> userList,string str)
        {
            for(int i=0;i<userList.Count;i++)
            {
                Send2User(userList[i], str);
            }
        }
    } 
}

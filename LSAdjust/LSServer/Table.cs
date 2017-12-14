using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSServer
{
    class Table
    {
        public static int MAX_USER;
        //public Player[] players;
        public User[] users;
        //private ListBox listBox;
        public Table()
        {
            //players = new Player[];
            //players[0] = new Player();
            //players[1] = new Player();
            users = new User[MAX_USER];//初始化5个指向user的空指针
        }
        public void ResetGame()
        {
            //将积分等信息置零
        }
    }
}

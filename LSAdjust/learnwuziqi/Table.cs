using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSAdjust
{
    class Table
    {
        public static int PLAYER_MAX = 2;
        private const int NONE = -1;//无棋子；
        private const int BLACK = 0;
        private const int WHITE = 1;
        //public Player[] players;
        public User[] users;
        int[,] grid = new int[15, 15];
        //private ListBox listBox;
        public Table()
        {
            //players = new Player[];
            //players[0] = new Player();
            //players[1] = new Player();
            users = new User[PLAYER_MAX];//初始化5个指向user的空指针
        }
        public void ResetGame()
        {
            for(int i=0;i<=grid.GetUpperBound(0);i++)
            {
                for(int j=0;j<=grid.GetUpperBound(1);j++)
                {
                    grid[i, j] = NONE;
                }
            }
        }
    }
}

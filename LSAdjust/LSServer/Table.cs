using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LSServer
{
    class Table
    {
        public static int MAX_USER;
        //public Player[] players;
        public User[] users;
        //private ListBox listBox;
        public int sumFinished=0;//统计本桌上的完成的人数
        public int round=1; //关数
        float[] cof;//参数数组 [0]：0次项系数 [1]:1次项系数
        public PointF[] points;//散点
        int num = 10;//默认产生10个点
        //将发牌方式定义为属性，由round关数完全确定
        public int mode => round % 3 == 0 ? 2 : 1; //如果关数为三的倍数，mode设置为2（抛物线）否则是一般的直线。
        //这种写法是lambda 表达式的写法，一句顶5句。注意mode是（可读）属性，
        //{
        //    get
        //    {
        //        if (round % 3 == 0)
        //            return 1;
        //        else
        //            return 0;
        //    }
        //}

        public Table()
        {
            users = new User[MAX_USER];//初始化指向user的空指针
        }
        public void ResetGame()
        {
            //将积分等信息置零
        }
        /// <summary>
        /// 产生总体回归参数
        /// </summary>
        /// <param name="num">参数个数</param>
        /// <returns></returns>
        public void Cal_Line(int num=20,double limit=3)
        {
            points = new PointF[num];
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            double a = ra.NextDouble()*5-2.5;
            double b = ra.NextDouble()*5-2.5;
            double delta;
            for(int i=0;i<num;i++)
            {
                delta = ra.NextDouble()*limit-limit/2.0;
                points[i].X = i - num/2;
                points[i].Y = (float)(i * a + b + delta);
            }
            Huigui(); //产生散点的同时把回归方程算出来

        }
        public void Cal_Poly2(int num=20,double limit=2)
        {
            points = new PointF[num];
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            double a = ra.NextDouble()*5-2.5;
            double b = ra.NextDouble()*5-2.5;
            double c = ra.NextDouble()*5-2.5;
            double delta;
            for (int i = 0; i < num; i++)
            {
                delta = ra.NextDouble() * limit - limit / 2.0;
                points[i].X = i - num/2;
                points[i].Y = (float)(i*i * a + b*i+c + delta);
            }
            // ............
            Huigui();
        }

        /// <summary>
        /// 根据散点计算回归直线
        /// 将产生的回归的系数再次赋给cof
        /// </summary>
        /// <returns></returns>
        public void Huigui()
        {
            int len = points.Length;
            PointF[] points2 = new PointF[len];
            float[,] A = new float[len, mode];//mode=1 二列 mode=2，抛物线，三个系数， 三列
            float[] Y = new float[len];
            if (mode == 1)
            {
                for (int i = 0; i < len; i++)
                {
                    A[i, 0] = 1;
                    A[i, 1] = points[i].X;
                    Y[i] = points[i].Y;
                }
            }
            else if(mode==2)
            {
                for (int i = 0; i < len; i++)
                {
                    A[i, 0] = 1;
                    A[i, 1] = points[i].X;
                    A[i, 2] = points[i].X * points[i].X;
                    Y[i] = points[i].Y;
                }
            }
            float[,] AT = Matrix.T(A);
            float[,] N = Matrix.Multi(AT, A);
            float[,] invN = Matrix.Inv(N);
            float[,] invA = Matrix.Multi(invN, AT);
            cof = Matrix.Multi1(invA, Y); //依次为beta0，beta1 beta2.0次项系数，一次项系数和二次项系数（如果有的话)。

            //for(int i=0;i<len;i++)
            //{
            //    points2[i].X = points[i].X;
            //    points2[i].Y =(cof[0] * points2[i].X + b);
            //}
               
        }



        //public void Cal_point(int ch, Point[] point, Point cpoint, double a, double b, double c, double p)                   //产生散点
        //{
        //    double[] x = new double[30];
        //    double[] y = new double[30];
        //    //Point[] point = new Point[20];
        //    int i;
        //    if (ch == 1)    //直线模式      1-3关
        //    {
        //        MessageBox.Show("玩家选择直线模式");
        //        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
        //        a = ra.Next(-5, 5);
        //        b = ra.Next(-10, 10);
        //        for (i = 0; i < 30; i++)
        //        {
        //            double d = ra.Next(-1, 1);
        //            x[i] = i - 10;
        //            y[i] = i * a + b + d;
        //            point[i].X = (int)x[i];
        //            point[i].Y = (int)y[i];
        //        }

        //    }
        //    else if (ch == 2)     //横轴抛物线模式      4-5关
        //    {
        //        MessageBox.Show("玩家选择横轴抛物线模式");
        //        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
        //        p = ra.Next(1, 10);
        //        for (i = 0; i < 30; i++)
        //        {
        //            double d = ra.Next(-1, 1);
        //            y[i] = i - 15;
        //            x[i] = (y[i] * y[i]) / (2.0 * p) + d;
        //            point[i].X = (int)x[i];
        //            point[i].Y = (int)y[i];
        //        }
        //    }
        //    else if (ch == 3)     //竖轴抛物线模式      6-9关
        //    {
        //        MessageBox.Show("玩家选择竖轴抛物线模式");
        //        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
        //        a = ra.Next(-10, 10);
        //        b = ra.Next(-10, 10);
        //        c = ra.Next(-10, 10);
        //        double cc = b / ((-2.0) * a);  //焦点横坐标
        //        for (i = 0; i < 30; i++)
        //        {
        //            double d = ra.Next(0, 1);
        //            x[i] = cc - 15 + i;                    //在焦点两端产生散点
        //            y[i] = a * x[i] * x[i] + b * x[i] + c + d;
        //            point[i].X = (int)x[i];
        //            point[i].Y = (int)y[i];

        //        }
        //        //Point cpoint;      //焦点坐标
        //        cpoint.X = (int)cc;
        //        cpoint.Y = (int)((4 * a * c - b * b) / (4.0 * a));
        //    }
        //}
        public float Calcu_sumErr(float x1,float y1,float x2,float y2)   //计算残差平方和,你先根据x1，y1，x2，y2计算beta0和beta1.
        {
            float sumErr = 0;
            //dist = 0;
            //for (int i = 0; i < 30; i++)
            //{
            //    dist += (point1[i].X - point2[i].X) ^ 2 + (point1[i].Y - point2[i].Y) ^ 2;
            //}
            //return 
            return sumErr;
        }
        public void Calcu_rank()
        {
            //根据每个人的sumErr计算rank
            // user就在上面，你要用到的sumerr和rank都可以由其访问，
        }
    }
}

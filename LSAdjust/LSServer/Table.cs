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

        public int round; //关数
        float[] cof;//参数数组
        public PointF[] point;//散点
        int num = 10;//默认产生10个点
        int mode = 1;//默认选择模式1 即直线模式

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
        public PointF[] Cal_Line(int num=20,double limit=1)
        {
            PointF[] points = new PointF[num];
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
            // ............
            return points;
        }
        public PointF[] Cal_Poly2(int num=20,double limit=1)
        {
            PointF[] points = new PointF[num];
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
            return points;

        }

        /// <summary>
        /// 根据散点计算回归直线
        /// 将产生的回归的系数再次赋给cof
        /// </summary>
        /// <returns>回归直线的参数b0 b1</returns>
        public PointF[] Huigui(PointF[] points,double a,double b,double c,int mode = 1)
        {
            int len = points.Length;
            PointF[] points2 = new PointF[len];
            if (mode==1)
            {
                float[,] A = new float[len, 1];
                float[] Y = new float[len];
                for(int i=0;i<len;i++)
                {
                    A[i, 0] = 1;
                    A[i, 1] = points[i].X;
                    Y[i] = points[i].Y;
                }
                float[,] AT = Matrix.T(A);
                float[,] N = Matrix.Multi(AT, A);
                float[,] invN = Matrix.Inv(N);
                float[,] invA = Matrix.Multi(invN, AT);
                float[] B = Matrix.Multi1(invA,Y);
                a = B[1];
                b = B[0];
                
                for(int i=0;i<len;i++)
                {
                    points2[i].X = points[i].X;
                    points2[i].Y = (float)(a * points2[i].X + b);
                }
               
            }
            return points2; 
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
        //public void Cal_Line(int ch, Point[] point1, Point[] point2, Point cpoint, double a, double b, double c, double p)              //由点计算直线和抛物线模型
        //{
        //    if (ch == 1)                                           //直线
        //    {
        //        a = (double)(point1[1].Y - point1[1].Y) / (point1[1].X - point1[0].X);         //斜率a
        //        b = (double)point1[0].Y - a * point1[0].X;                                   //常数b
        //        for (int i = 0; i < 30; i++)
        //        {
        //            point2[i].X = i - 10;
        //            point2[i].Y = (int)(a * i + b);
        //        }
        //    }
        //    else if (ch == 2)                                     //横轴抛物线
        //    {
        //        p = cpoint.X * 4.0;
        //        for (int i = 0; i < 30; i++)
        //        {
        //            point2[i].Y = i - 15;
        //            point2[i].X = (int)((point2[i].Y * point2[i].Y) / p);
        //        }

        //    }
        //    else if (ch == 3)                       //已知焦点和准线，如何确定抛物线方程
        //    {
        //        int y = point1[0].Y - cpoint.Y;
        //        a = 1 / (-4.0 * y);
        //        b = -2.0 * a * cpoint.X;
        //        c = (4.0 * a * cpoint.Y + b * b) / (4.0 * a);
        //        for (int i = 0; i < 30; i++)
        //        {
        //            point2[i].X = i - 15 + cpoint.X;
        //            point2[i].Y = (int)(a * point2[i].X * point2[i].X + b * point2[i].X + c);
        //        }
        //    }
        //}
        //public void Cal_dis(Point[] point1, Point[] point2, int dist)                       //计算残差平方和
        //{
        //    dist = 0;
        //    for (int i = 0; i < 30; i++)
        //    {
        //        dist += (point1[i].X - point2[i].X) ^ 2 + (point1[i].Y - point2[i].Y) ^ 2;
        //    }
        //}
        //public void Cal_Huigui(Point[] point1, Point[] point2)
        //{
        //    double a; double b;
        //    double[,] A = new double[29, 1];
        //    double[,] tA = new double[1, 29];
        //    double[,] mA = new double[1, 1]; double[,] inA = new double[1, 1]; double[,] m = new double[1, 29];
        //    double[] Y = new double[29];
        //    double[] B = new double[2];
        //    for (int i = 0; i < 30; i++)
        //    {
        //        A[i, 0] = 1;
        //        A[i, 1] = point1[i].X;
        //        Y[i] = point1[i].Y;
        //    }
        //    //Cal_Ma.GetTransMatrix(A, tA);
        //    //Cal_Ma.MultiplyMatrix(mA, tA, A);
        //    //Cal_Ma.Cal_Inverse(mA, inA);
        //    //Cal_Ma.MultiplyMatrix(m, inA, tA);
        //    //Cal_Ma.MultiplyMatrix1(B, m, Y);
        //    a = B[1]; b = B[0];
        //    for (int i = 0; i < 30; i++)
        //    {
        //        point2[i].X = i - 10;
        //        point2[i].Y = (int)(a * i + b);
        //    }
        //}
    }
}

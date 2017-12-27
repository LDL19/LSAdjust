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
        public int mode  //如果关数为三的倍数，mode设置为2,3（抛物线）否则是一般的直线。
        //这种写法是lambda 表达式的写法，一句顶5句。注意mode是（可读）属性，
        {
            get
            {
                if (round > 6)
                    return 2;
                else if (round < 4)
                    return 1;
                else
                    return 0;
            }
        }

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
        public PointF[] Cal_Poly1(int num = 20, double limit = 2) ///横轴抛物线
        {
            PointF[] points = new PointF[num];
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            double p = ra.NextDouble() * 5;
            double delta;
            for (int i = 0; i < num; i++)
            {
                delta = ra.NextDouble() * limit - limit / 2.0;
                points[i].Y = i - num/2;
                points[i].Y = (float)((points[i].Y * points[i].Y) / (2.0 * p) + delta);
            }
            return points;
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
        public float Huigui()
        {
            int len = points.Length;
            PointF[] points2 = new PointF[len];
            float[,] A = new float[len, mode];//mode=1 二列 mode=0，横轴抛物线抛物线，一个系数，一列 mode=2,竖轴抛物线，三个系数， 三列
            float[] Y = new float[len];
            if (mode == 1)    //直线
            {
                for (int i = 0; i < len; i++)
                {
                    A[i, 0] = 1;
                    A[i, 1] = points[i].X;
                    Y[i] = points[i].Y;
                }
            }
            else if (mode == 0)     //横轴抛物线
            {
                for (int i = 0; i < len; i++)
                {
                    A[i, 0] = points[i].X;
                    Y[i] = points[i].Y * points[i].Y;
                }
            }
            else if (mode == 2)
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
            float a = cof[2]; float b = cof[1]; float c = cof[0];
            
            if (mode == 0)    //横轴抛物线
            {
                for (int i = 0; i < len; i++)
                {
                    points2[i].Y = points[i].X;
                    points2[i].X = (float)((points2[i].Y * points2[i].Y) / c);
                }
            }
            else if (mode == 1)    //直线
            {
                for (int i = 0; i < len; i++)
                {
                    points2[i].X = points[i].X;
                    points2[i].Y = (float)(b * points2[i].X + c);
                }
            }
            else if (mode == 2)    //竖轴抛物线
            {
                for (int i = 0; i < len; i++)
                {
                    points2[i].X = points[i].X;
                    points2[i].Y = (float)(a * points2[i].X * points2[i].X + b * points2[i].X + c);
                }
            }
            float Qe=0;  //计算残差平方和的
            for (int i = 0; i < 30; i++)
            {
                 Qe+= (points[i].X - points2[i].X) * (points[i].X - points2[i].X) + (points[i].Y - points2[i].Y) * (points[i].Y - points2[i].Y);
            }
            return Qe;
            
            //for(int i=0;i<len;i++)
            //{
            //    points2[i].X = points[i].X;
            //    points2[i].Y =(cof[0] * points2[i].X + b);
            //}
               
        }
        /// <summary>
        /// 计算误差差平方和,你先根据x1，y1，x2，y2计算beta0和beta1.
        /// 并传回回归分析的结果
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>


        
        public float Calcu_sumErr(float x1,float y1,float x2,float y2,float x0,float y0,float a,float b,float c,float p )   //计算误差差平方和,你先根据x1，y1，x2，y2计算beta0和beta1.
        {
            float sumErr = 0; int len = points.Length;
            PointF[] points2 = new PointF[len];
            //double a; double b; double c; double p;
            if (mode == 1)                                           ///直线
            {
                a = (y2 - y1) / (x2 - x1);         ///斜率a
                b = y1 - a * x1;                                   ///常数b
                for (int i = 0; i < num; i++)
                {
                    points2[i].X = i - num / 2;
                    points2[i].Y = (float)(a * points2[i].X + b);
                }
            }
            else if (mode == 0)                                     ///横轴抛物线
            {
                p = x0 * 4;
                for (int i = 0; i < num; i++)
                {
                    points2[i].Y = i - num / 2;
                    points2[i].X = (float)((points2[i].Y * points2[i].Y) / p);
                }

            }
            else if (mode == 2)                       //已知焦点和准线，如何确定抛物线方程
            {
                float y = x1 - x0;
                a = (float)(1 / (-4.0 * y));
                b =(float)( -2.0 * a * x0);
                c = (float)((4.0 * a * y0 + b * b) / (4.0 * a));
                for (int i = 0; i < num; i++)
                {
                    points2[i].X = i - num / 2 + x0;
                    points2[i].Y = (float)(a * points2[i].X * points2[i].X + b * points2[i].X + c);
                }
            }
            for (int i = 0; i < 30; i++)  //计算误差平方和
            {
                sumErr += (points[i].X - points2[i].X) * (points[i].X - points2[i].X) + (points[i].Y - points2[i].Y) * (points[i].Y - points2[i].Y);
            }
            return sumErr;
        }
        public void Calcu_rank()
        {
            //根据每个人的sumErr计算rank
            // user就在上面，你要用到的sumerr和rank都可以由其访问，
            float[] Rank = new float[MAX_USER]; float tem = 0;
            for(int i=0;i<MAX_USER;i++)
            {
                Rank[i]=users[i].sumErr;
            }
            for (int i = 0; i < MAX_USER; i++)    //误差平方和排序
                for (int j = i + 1; j < MAX_USER - 1; j++)
                {
                    if (Rank[i] > Rank[j])
                    {
                        tem = Rank[i];
                        Rank[i] = Rank[j];
                        Rank[j] = tem;
                    }
                }
            for (int i = 0; i < MAX_USER; i++)    //排名次
                for (int j = 0; j < MAX_USER; j++)
                {
                    if (users[i].sumErr == Rank[j])
                        users[i].rank = j+1;
                }
        }
    }
}

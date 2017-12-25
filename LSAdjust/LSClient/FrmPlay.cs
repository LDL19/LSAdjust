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
using System.Drawing.Drawing2D;
  
namespace LSClient
{
    public partial class FrmPlay : Form
    {
        private int tableIndex;
        private int seat;
        public Service service;
        //public PointF[] ReceivePoint; //点
        //public double[] ReceiveBox;
        public bool BeginGame=false;//在接收deal之后变成true
      //边界盒
        bool btnFinishflag = false;
        //关于图形的平移（滚轮的down up） 缩放（滚轮） 开窗放大（鼠标右键 down up） 
        //doubleclick：恢复原图像模样
        double xMin, xMax, yMin, yMax, Wx, Wy; // 用于记录图层数据的范围
        double xMinCopy, xMaxCopy, yMinCopy, yMaxCopy;//用于保存原始数据，便于恢复原来模样
        int w, h;
        double scale;//滚轮每次滚动的比例
        float sx, sy;
        Boolean IsMove = false;
        Boolean IsWindow = false;
        double dx, dy;                          // 为居中显示，计算偏移量               
        double detaX, detaY;                  //用于记录平移的总偏移量
        double Xup, Yup, Xdown, Ydown;        //用于记录鼠标down和up时的用户坐标
        double WindowX, WindowY;              //开窗时的临时变量
        double LeftX, LeftY, RightX, RightY;  //开窗时窗体的左上和右下



        Bitmap bitScreen;
        Bitmap bitPanel;


        Point startPoint = new Point();
        //图形缩放平移开窗内容变量结束


        double[] TestPoints; //= new double[20];//测试点
        //绘制直线的一些变量
        PointF SendPoint1 = new PointF();
        PointF SendPoint2 = new PointF();
        Boolean DrawLineButton = false;
        Boolean ShowDrawLine = false;
        int DrawLineCount = 0;
        int JudgeCount;//判断是否要开始绘制直线和点

        //绘制抛物线的一些变量
        Boolean DrawCurve;
        Boolean ShowCurve;
        double ParaA, ParaB;
        //注意，此时要判断接受的抛物线待拟合点坐标的种类，类型不同，画的方法也不同，此时先用参数1和2 来代替
        Boolean XorY;



        string SendPoint;//传给服务器端的点
        public FrmPlay(int tableIndex,int seat,StreamWriter sw)
        {
            InitializeComponent();
            service = new Service(listBox1,sw);
            this.tableIndex = tableIndex;
            this.seat = seat;
        }
        private void FrmRoomcheckbox(object sender, EventArgs e)
        {
           // FrmRoom a = (FrmRoom)this.Owner;
            
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            BeginGame = false;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("确定要离开吗？", "离开", buttons);
            if(result==DialogResult.Yes )
            {
                if (btnFinishflag == true)
                {           
                    //记录成绩

                  //service.Send2Server(string.Format("StandUp,{0},{1}", i, j)); 
                    this.Dispose();
                   
                    btnFinishflag = false;//提交之后，btnFinishflag变成了false
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
            BeginGame = false;
            btnFinishflag = false;
            //发送结果到服务器
            SendPoint = SendPoint1.X + "," + SendPoint1.Y + "," + SendPoint2.X + "," + SendPoint2.Y;
            service.Send2Server(string.Format("Finish,{0},{1},{2}", tableIndex, seat, SendPoint));
            btnFinishflag = true;
        }

        private void btnReady_Click(object sender, EventArgs e)
        {    btnFinishflag = false;
            service.Send2Server(string.Format("Ready,,{0},{1}", tableIndex, seat));//准备之后接受点
        //更新
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

        private void chkTimeLimit_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void lblTimeLimit_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
   
        private void FrmPlay_Load(object sender, EventArgs e)
        {


            w = DrawPanel.Width;
            h = DrawPanel.Height;
            detaY = 0;
            detaX = 0;

            this.DrawPanel.MouseWheel += new MouseEventHandler(DrawPanel_MouseWheel);//赋予c#mousewheel事件
            //测试点
            //for (int i = 0; i < 10; i++)
            //{
            //    TestPoints[i * 2] = i * 20 + 10;
            //    TestPoints[i * 2 + 1] = i * 20 + 10;
            //}
             xMin = 0; yMin = 0; xMax = 180; yMax = 180; //测试数据
           TestPoints = null;
           BeginGame = false;
             //TestPoints = new double[ReceivePoints.Length];
            // TestPoints = (double[])ReceivePoints.Clone();//点
             
        
           // xMin = ReceiveBox[0]; yMin = ReceiveBox[1]; xMax = ReceiveBox[2]; yMax = ReceiveBox[3];//边界盒
            xMinCopy = xMin;
            xMaxCopy = xMax;
            yMinCopy = yMin;
            yMaxCopy = yMax;

            //由于winrectangle是用来画矩形，没有开始画所以要进行设置
            WinRectangle.Parent = DrawPanel;
            WinRectangle.BackColor = Color.Transparent;
            WinRectangle.BorderStyle = BorderStyle.FixedSingle;
            WinRectangle.Visible = false;
   
        }
        private void FrmPlay_ResizeEnd(object sender, EventArgs e)
        {
            tabControl1.Width = Width;
            DrawPanel.Width = tabControl1.Width - groupBox1.Width - 32;
            //Refresh();不止为何不能直接点 放大框，这时候，就不能同步
        }
        private void DrawPanel_MouseEnter(object sender, EventArgs e)
        {
            this.DrawPanel.Focus();
        }
        private void DrawPanel_DoubleClick(object sender, EventArgs e)
        {
            xMax = xMaxCopy;
            xMin = xMinCopy;
            yMin = yMinCopy;
            yMax = yMaxCopy;
            DrawPanel.Refresh();
        }
        private void DrawPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)//向上滚动滑轮
            {
                scale = 0.2;
            }
            else//向下滚动滑轮
            {
                scale = -0.2;
            }

            double xMin2, xMax2, yMin2, yMax2;

            //将鼠标位置运算到对应图层的坐标位置
            double xm, ym;
            xm = e.X / sx + xMin - Wx * 0.01 + dx;
            ym = -e.Y / sy + yMax + Wy * 0.01 - dy;
            //调整相应的范围，使之以鼠标为中心进行放大缩小
            xMin2 = xMin + (xm - xMin) * scale;
            xMax2 = xMax - (xMax - xm) * scale;
            yMin2 = yMin + (ym - yMin) * scale;
            yMax2 = yMax - (yMax - ym) * scale;
            //防止超限，避免bug
            if (xMax2 - xMin2 > 0.01 && yMax2 - yMin2 > 0.01)
            {
                xMin = xMin2;
                xMax = xMax2;
                yMax = yMax2;
                yMin = yMin2;
            }

            Wx = xMax - xMin;
            Wy = yMax - yMin;
            DrawPanel.Refresh();
        }
        private void DrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //记录鼠标按下时，其用户坐标位置
                Xdown = e.X / sx + xMin - Wx * 0.01 + dx;
                Ydown = -e.Y / sy + yMax + Wy * 0.01 - dy;
                //鼠标按下时，记录其窗体坐标位置
                startPoint.X = e.X;
                startPoint.Y = e.Y;
                this.Cursor = Cursors.NoMove2D;//设置光标样式
                DrawPanel.Capture = true;

                //将窗体内容写入bitscreen
                bitScreen = new Bitmap(DrawPanel.Width, DrawPanel.Height);
                Graphics g = Graphics.FromImage(bitScreen);
                g.CompositingQuality = CompositingQuality.HighQuality;
                DrawPanel.DrawToBitmap(bitScreen, DrawPanel.ClientRectangle);

                //创建窗体*3的大小的bitmap
                if (bitPanel != null) bitPanel.Dispose();
                bitPanel = new Bitmap(3 * DrawPanel.Width, 3 * DrawPanel.Height);
                g = Graphics.FromImage(bitPanel);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.Clear(Color.AliceBlue);//清空其内容，作为背景
                g.DrawImage(bitScreen, DrawPanel.Width, DrawPanel.Height);

                IsMove = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.Cursor = Cursors.Cross;//设置成交叉
                DrawPanel.Capture = true;

                //鼠标按下时，开窗左上角位置
                WindowX = e.X;//+ DrawPanel.Location.X;
                WindowY = e.Y;//+ DrawPanel.Location.Y;

                IsWindow = true;
            }
            else if (e.Button == MouseButtons.Left && DrawLineButton == true)
            {

                ShowDrawLine = true;
                if (DrawLineCount == 0)
                {
                    //获取第一个点的用户坐标
                    SendPoint1.X = Convert.ToSingle(e.X / sx + xMin - Wx * 0.01 + dx);
                    SendPoint1.Y = Convert.ToSingle(-e.Y / sy + yMax + Wy * 0.01 - dy);
                    DrawLineCount += 1;
                    JudgeCount = 0;
                    DrawPanel.Refresh();
                }
                else if (DrawLineCount == 1)
                {
                    //获取第二个点的用户坐标
                    SendPoint2.X = Convert.ToSingle(e.X / sx + xMin - Wx * 0.01 + dx);
                    SendPoint2.Y = Convert.ToSingle(-e.Y / sy + yMax + Wy * 0.01 - dy);
                    DrawLineButton = false;
                    //画完之后，重新缩放图形
                    xMax = xMaxCopy;
                    xMin = xMinCopy;
                    yMin = yMinCopy;
                    yMax = yMaxCopy;

                    JudgeCount = 1;
                    DrawPanel.Refresh();
                    DrawLineCount = 0;

                }
                else { }
            }
            else { }
        }
        private void DrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Xcoor.Text = String.Format("{0:######.## }", e.X / sx + xMin - Wx * 0.01 + dx);
            Ycoor.Text = String.Format("{0:######.## }", -e.Y / sy + yMax + Wy * 0.01 - dy);

            if (IsMove == true)
            {
                int myWidth = e.X - startPoint.X;
                int myHeight = e.Y - startPoint.Y;
                Graphics g = DrawPanel.CreateGraphics();
                g.DrawImage(bitPanel, -DrawPanel.Width + myWidth, -DrawPanel.Height + myHeight);
            }
            if (IsWindow == true)
            {
                //显示开窗
                WinRectangle.Visible = true;
                LeftX = WindowX;
                LeftY = WindowY;
                //正常情况左上到右下，但如果右下到左上，则需要转化位置
                if (LeftX > e.X) LeftX = e.X + DrawPanel.Location.X;
                if (LeftY > e.Y) LeftY = e.Y + DrawPanel.Location.Y;
                //设置winrectangle的左上角和大小
                WinRectangle.Location = new Point(Convert.ToInt32(LeftX), Convert.ToInt32(LeftY));
                WinRectangle.Size = new Size(Convert.ToInt32(Math.Abs(e.X - WindowX)), Convert.ToInt32(Math.Abs(e.Y - WindowY)));
            }
        }
        private void DrawPanel_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Middle && IsMove == true)
            {
                IsMove = false;
                if (DrawLineButton == false)
                {
                    this.Cursor = Cursors.Default;
                }
                else
                {
                    //如果现在正在画直线，那么鼠标应还是十字格式
                    this.Cursor = Cursors.Cross;
                }

                //mouseup时鼠标的用户坐标
                Xup = e.X / sx + xMin - Wx * 0.01 + dx;
                Yup = -e.Y / sy + yMax + Wy * 0.01 - dy;
                //计算平移的累计，之后的开窗要用到
                detaX = detaX + Xup - Xdown;
                detaY = detaY + Yup - Ydown;

                PointF[] pt = new PointF[2];
                double EndX, EndY;
                EndX = e.X / sx + xMin - Wx * 0.01 + dx;//转化为用户坐标
                EndY = -e.Y / sy + yMax + Wy * 0.01 - dy;
                pt[0].X = Convert.ToSingle(Xdown);//其实这个直接用 窗体坐标也可以，因为需要的只是一个delta（差值）
                pt[0].Y = Convert.ToSingle(Ydown);
                pt[1].X = Convert.ToSingle(EndX);
                pt[1].Y = Convert.ToSingle(EndY);
                xMax = xMax - (pt[1].X - pt[0].X);
                xMin = xMin - (pt[1].X - pt[0].X);
                yMax = yMax - (pt[1].Y - pt[0].Y);
                yMin = yMin - (pt[1].Y - pt[0].Y);
                DrawPanel.Refresh();
            }
            else if (e.Button == MouseButtons.Right && IsWindow == true)
            {
                IsWindow = false;
                this.Cursor = Cursors.Default;
                RightX = LeftX + Math.Abs(e.X - WindowX);
                RightY = LeftY + Math.Abs(e.Y - WindowY);

                WinRectangle.Visible = false;
                double xMin4, xMax4, yMin4, yMax4;//记录初始范围，便于恢复全图,最重要的地方
                xMin4 = (LeftX - DrawPanel.Location.X) / sx + xMin - Wx * 0.01 + dx - detaX;
                yMax4 = -(LeftY - DrawPanel.Location.Y) / sy + yMax + Wy * 0.01 - dy - detaY;
                xMax4 = (RightX - DrawPanel.Location.X) / sx + xMin - Wx * 0.01 + dx - detaX;
                yMin4 = -(RightY - DrawPanel.Location.Y) / sy + yMax + Wy * 0.01 - dy - detaY;

                if (xMax4 - xMin4 > 0.01 || yMax4 - yMin4 > 0.01)
                {
                    xMin = xMin4;
                    yMin = yMin4;
                    xMax = xMax4;
                    yMax = yMax4;
                }
                xMin = xMin4 + detaX;
                yMin = yMin4 + detaY;
                xMax = xMax4 + detaX;
                yMax = yMax4 + detaY;

                DrawPanel.Refresh();
            }
            else if (e.Button == MouseButtons.Left && DrawLineButton == false)
            {
                this.Cursor = Cursors.Default;
            }
            else { }
        }
        public void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g;
            g = e.Graphics;
            g.Clear(Color.White);

            Wx = xMax - xMin;
            Wy = yMax - yMin;

            try
            {
                // 计算合适的缩放比例。为了确保图形在视图中完全表达，数据范围放大2%
                sx = Convert.ToSingle(w / (Wx * 1.02)); // Wx是数据范围的宽度
                sy = Convert.ToSingle(h / (Wy * 1.02)); // Wy是数据范围的高度
                // 选择一个较小的比例
                dx = dy = 0.0;
                if (sx > sy)
                {
                    sx = sy;
                    dx = (Wx - w / sx) / 2.0; // 调整横向比例，横向居中
                }
                else
                {
                    sy = sx;
                    dy = (Wy - h / sy) / 2.0; // 调整纵向比例，纵向居中
                }

                // 设立坐标变换矩阵的坐标缩放比例
                g.ScaleTransform(sx, -sy); // 设定缩放比例。由于画板坐标的原点在左上角，向下为正，将y比例设为负数

                // 设置原点平移量
                g.TranslateTransform(Convert.ToSingle(-xMin + Wx * 0.01 - dx), Convert.ToSingle(-yMax - Wy * 0.01 + dy));

                // 设立一个彩色的画笔
                Single penWidth = 1.0F / (sx + sy) * 2;   // 计算画笔宽度，为缩放比例的倒数。

                Pen myPen = new Pen(Color.FromArgb(220, 220, 220), penWidth);
                DrawChess(g, myPen);

                myPen = new Pen(Color.Blue, penWidth);
                Pen myPen1 = new Pen(Color.FromArgb(0, 200, 0), penWidth);
                Pen myPen2 = new Pen(Color.Red, penWidth);

                //测试数据
                if (BeginGame == false)
                { }
                else
                {
                    for (int i = 0; i < (TestPoints.GetUpperBound(0) + 1) * 0.5; i++)
                    {
                        g.DrawLine(myPen, Convert.ToSingle(TestPoints[i * 2] - 0.5), Convert.ToSingle(TestPoints[i * 2 + 1]), Convert.ToSingle(TestPoints[i * 2] + 0.5), Convert.ToSingle(TestPoints[i * 2 + 1]));
                        g.DrawLine(myPen, Convert.ToSingle(TestPoints[i * 2]), Convert.ToSingle(TestPoints[i * 2 + 1] - 0.5), Convert.ToSingle(TestPoints[i * 2]), Convert.ToSingle(TestPoints[i * 2 + 1] + 0.5));
                    }
                }
                //画直线拟合部分的点和直线
                if (ShowDrawLine == true)
                {
                    if (JudgeCount == 0)
                    {
                        g.DrawLine(myPen, SendPoint1.X, Convert.ToSingle(SendPoint1.Y - 0.5), SendPoint1.X, Convert.ToSingle(SendPoint1.Y + 0.5));
                        g.DrawLine(myPen, Convert.ToSingle(SendPoint1.X - 0.5), SendPoint1.Y, Convert.ToSingle(SendPoint1.X + 0.5), SendPoint1.Y);
                    }
                    else if (JudgeCount == 1)
                    {
                        g.DrawLine(myPen, SendPoint1.X, Convert.ToSingle(SendPoint1.Y - 0.5), SendPoint1.X, Convert.ToSingle(SendPoint1.Y + 0.5));
                        g.DrawLine(myPen, Convert.ToSingle(SendPoint1.X - 0.5), SendPoint1.Y, Convert.ToSingle(SendPoint1.X + 0.5), SendPoint1.Y);
                        g.DrawLine(myPen, SendPoint2.X, Convert.ToSingle(SendPoint2.Y - 0.5), SendPoint2.X, Convert.ToSingle(SendPoint2.Y + 0.5));
                        g.DrawLine(myPen, Convert.ToSingle(SendPoint2.X - 0.5), SendPoint2.Y, Convert.ToSingle(SendPoint2.X + 0.5), SendPoint2.Y);
                        g.DrawLine(myPen2, SendPoint1, SendPoint2);
                    }
                    else { }
                }

                //画抛物线部分
                if (ShowCurve == true)
                {
                    PointF[] pt = new PointF[2];
                    if (XorY == false)//假设现在准线平行于X轴
                    {
                        double i = -500;
                        while (i <= 500)
                        {
                            pt[0].X = Convert.ToSingle(i);
                            pt[0].Y = Convert.ToSingle(ParaA * System.Math.Pow(pt[0].X, 2) + ParaB);
                            pt[1].X = Convert.ToSingle(i + 0.5);
                            pt[1].Y = Convert.ToSingle(ParaA * System.Math.Pow(pt[1].X, 2) + ParaB);
                            if (pt[0].Y <= 500 * 20 && pt[1].Y <= 500 * 20)
                            {
                                g.DrawLine(myPen1, pt[0], pt[1]);
                            }
                            i = i + 0.5;
                        }
                    }
                    else
                    {
                        double i = -500;
                        while (i <= 500)
                        {
                            pt[0].Y = Convert.ToSingle(i);
                            pt[0].X = Convert.ToSingle(ParaA * System.Math.Pow(pt[0].X, 2) + ParaB);
                            pt[1].Y = Convert.ToSingle(i + 0.5);
                            pt[1].X = Convert.ToSingle(ParaA * System.Math.Pow(pt[1].X, 2) + ParaB);
                            if (pt[0].X <= 500 * 20 && pt[1].X <= 500 * 20)
                            {
                                g.DrawLine(myPen1, pt[0], pt[1]);
                            }
                            i = i + 0.5;
                        }
                    }
                    DrawCurve = false;
                }

            }
            catch
            {
                MessageBox.Show("catch");
            }
        }
        private void DrawChess(Graphics g, Pen p)
        {
            //以20为间隔画坐标系的网格

            for (int i = -500; i <= 500; i++)
            {
                g.DrawLine(p, i * 20, -10000, i * 20, 10000);

            }
            for (int i = -500; i <= 500; i++)
            {
                g.DrawLine(p, -10000, i * 20, 10000, i * 20);

            }


        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void WinRectangle_Click(object sender, EventArgs e)
        {

        }

        private void DrawPanel_Click(object sender, EventArgs e)
        {
            DrawLineButton = true;
            this.Cursor = Cursors.Cross;
        }

        private void DrawL_Click(object sender, EventArgs e)
        {
            DrawLineButton = true;
            this.Cursor = Cursors.Cross;
        }

        private void DrawXcurve_Click(object sender, EventArgs e)
        {
            if (Para_a.Text == "" || Para_b.Text == "")
            {
                MessageBox.Show("请输入参数");
                return;
            }
            else
            {
                try
                {
                    ParaA = Convert.ToDouble(Para_a.Text);
                    ParaB = Convert.ToDouble(Para_b.Text);
                    DrawCurve = true;
                    ShowCurve = true;
                    DrawPanel.Refresh();
                }
                catch
                {
                    MessageBox.Show("报错，请检查输入是否正确");
                    return;
                }

            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}

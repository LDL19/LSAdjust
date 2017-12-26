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

        public PointF[] points; //点
        public Box box;
        public Box boxCopy;

        public bool beginGame=false;//在接收deal之后变成true
      //边界盒
        bool btnFinishflag = false;
        //关于图形的平移（滚轮的down up） 缩放（滚轮） 开窗放大（鼠标右键 down up） 
        //doubleclick：恢复原图像模样
        //double box.Xmin, box.Xmax, box.Ymin, box.Ymax, Wx, Wy; // 用于记录图层数据的范围
        //double box.XminCopy, xMaxCopy, yMinCopy, yMaxCopy;//用于保存原始数据，便于恢复原来模样
        //float scale;//滚轮每次滚动的比例
        float sx, sy;
        Boolean IsMove = false;
        Boolean IsWindow = false;
        float dx, dy;                          // 为居中显示，计算偏移量               
        float detaX, detaY;                  //用于记录平移的总偏移量
        float Xup, Yup, Xdown, Ydown;        //用于记录鼠标down和up时的用户坐标
        float WindowX, WindowY;              //开窗时的临时变量
        float LeftX, LeftY, RightX, RightY;  //开窗时窗体的左上和右下


        FrmRoom frmRoom;


        Bitmap bitScreen;
        Bitmap bitPanel;


        Point startPoint = new Point();
        //图形缩放平移开窗内容变量结束


        //double[] points; //= new double[20];//测试点
        //绘制直线的一些变量
        PointF p1 = new PointF();
        PointF p2 = new PointF();
        Boolean DrawLineButton = false;
        Boolean ShowDrawLine = false;
        int DrawLineCount = 0;
        int JudgeCount;//判断是否要开始绘制直线和点

        //绘制抛物线的一些变量
        Boolean DrawCurve;
        Boolean ShowCurve;
        float ParaA, ParaB;
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
            beginGame = false;
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
                }
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)//发送chat
        {
            service.Send2Server(string.Format("Chat,{0},{1}", tableIndex, textBox1.Text));
            textBox1.Text = "";
        }
      
        private void btnFinish_Click(object sender, EventArgs e)
        {
            beginGame = false;
            btnFinishflag = false;
            //发送结果到服务器
            SendPoint = p1.X + "," + p1.Y + "," + p2.X + "," + p2.Y;
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
            frmRoom = (FrmRoom)this.Owner;

            this.DrawPanel.MouseWheel += new MouseEventHandler(DrawPanel_MouseWheel);//赋予c#mousewheel事件
            //测试点
            //for (int i = 0; i < 10; i++)
            //{
            //    TestPoints[i * 2] = i * 20 + 10;
            //    TestPoints[i * 2 + 1] = i * 20 + 10;
            //}
             //box.Xmin = 0; box.Ymin = 0; box.Xmax = 180; box.Ymax = 180; //测试数据
            box.Xmin = 0;
            box.Xmax = 180;
            box.Ymin = 0;
            box.Ymax = 180;
            //TestPoints = new double[ReceivePoints.Length];
            // TestPoints = (double[])ReceivePoints.Clone();//点


            // box.Xmin = ReceiveBox[0]; box.Ymin = ReceiveBox[1]; box.Xmax = ReceiveBox[2]; yMax = ReceiveBox[3];//边界盒
            boxCopy = box;

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
        //private void DrawPanel_DoubleClick(object sender, EventArgs e)
        //{
        //    box = boxCopy;
        //    DrawPanel.Refresh();
        //}
        private void DrawPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            float scale;
            if (e.Delta > 0)//向上滚动滑轮
            {
                scale = 0.2f;
            }
            else//向下滚动滑轮
            {
                scale = -0.2f;
            }

            //double box.Xmin2, xMax2, yMin2, yMax2;
            Box box2;

            //将鼠标位置运算到对应图层的坐标位置
            float xm, ym;
            float Wx = box.Xmax - box.Xmin;
            float Wy = box.Ymax - box.Ymin;
            xm = e.X / sx + box.Xmin - Wx * 0.01f + dx;
            ym = -e.Y / sy + box.Ymax + Wy * 0.01f - dy;
            //调整相应的范围，使之以鼠标为中心进行放大缩小
            box2.Xmin = box.Xmin + (xm - box.Xmin) * scale;
            box2.Xmax =box.Xmax - (box.Xmax - xm) * scale;
            box2.Ymin = box.Ymin + (ym - box.Ymin) * scale;
            box2.Ymax = box.Ymax - (box.Ymax - ym) * scale;
            //防止超限，避免bug
            //if (box2.Xmax-box2.Xmin > 0.01 && box2.Ymax - box2.Ymin > 0.01)
            //{
            box=box2;
            //}

            DrawPanel.Refresh();
        }
        private void DrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //记录鼠标按下时，其用户坐标位置
                Xdown = e.X / sx + box.Xmin -Wx * 0.01f + dx;
                Ydown = -e.Y / sy + box.Ymax +Wy * 0.01f - dy;
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
                    p1.X = e.X / sx + box.Xmin - Wx * 0.01f + dx;
                    p1.Y = -e.Y / sy + box.Ymax + Wy * 0.01f - dy;
                    DrawLineCount += 1;
                    JudgeCount = 0;
                    DrawPanel.Refresh();
                }
                else if (DrawLineCount == 1)
                {
                    //获取第二个点的用户坐标
                    p2.X = e.X / sx + box.Xmin - Wx * 0.01f + dx;
                    p2.Y = -e.Y / sy + box.Ymax + Wy * 0.01f - dy;
                    DrawLineButton = false;
                   

                    JudgeCount = 1;
                    DrawPanel.Refresh();
                    DrawLineCount = 0;

                }
            }
        }
        private void DrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            labelX.Text = "X:"+String.Format("{0:######.## }", e.X / sx + box.Xmin - Wx * 0.01 + dx);
            labelY.Text = "Y:"+String.Format("{0:######.## }", -e.Y / sy + box.Ymax + Wy * 0.01 - dy);

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
                Xup = e.X / sx + box.Xmin - Wx * 0.01f + dx;
                Yup = -e.Y / sy + box.Ymax + Wy * 0.01f - dy;
                //计算平移的累计，之后的开窗要用到
                detaX = detaX + Xup - Xdown;
                detaY = detaY + Yup - Ydown;

                PointF[] pt = new PointF[2];
                float EndX, EndY;
                EndX = e.X / sx + box.Xmin - Wx * 0.01f + dx;//转化为用户坐标
                EndY = -e.Y / sy + box.Ymax + Wy * 0.01f - dy;
                pt[0].X = Xdown;//其实这个直接用 窗体坐标也可以，因为需要的只是一个delta（差值）
                pt[0].Y = Ydown;
                pt[1].X = EndX;
                pt[1].Y = EndY;
                box.Xmax = box.Xmax - (pt[1].X - pt[0].X);
                box.Xmin = box.Xmin - (pt[1].X - pt[0].X);
                box.Ymax = box.Ymax - (pt[1].Y - pt[0].Y);
                box.Ymin = box.Ymin - (pt[1].Y - pt[0].Y);
                DrawPanel.Refresh();
            }
            else if (e.Button == MouseButtons.Right && IsWindow == true)
            {
                IsWindow = false;
                this.Cursor = Cursors.Default;
                RightX = LeftX + Math.Abs(e.X - WindowX);
                RightY = LeftY + Math.Abs(e.Y - WindowY);

                WinRectangle.Visible = false;
                float Xmin4, xMax4, yMin4, yMax4;//记录初始范围，便于恢复全图,最重要的地方
                Xmin4 = (LeftX - DrawPanel.Location.X) / sx + box.Xmin - Wx * 0.01f + dx - detaX;
                yMax4 = -(LeftY - DrawPanel.Location.Y) / sy + box.Ymax + Wy * 0.01f - dy - detaY;
                xMax4 = (RightX - DrawPanel.Location.X) / sx + box.Xmin - Wx * 0.01f + dx - detaX;
                yMin4 = -(RightY - DrawPanel.Location.Y) / sy + box.Ymax + Wy * 0.01f - dy - detaY;

                if (xMax4 - Xmin4 > 0.01 || yMax4 - yMin4 > 0.01)
                {
                    box.Xmin = Xmin4;
                    box.Ymin = yMin4;
                    box.Xmax = xMax4;
                    box.Ymax = yMax4;
                }
                box.Xmin = Xmin4 + detaX;
                box.Ymin = yMin4 + detaY;
                box.Xmax = xMax4 + detaX;
                box.Ymax = yMax4 + detaY;

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

            //try
            //{
                // 计算合适的缩放比例。为了确保图形在视图中完全表达，数据范围放大2%
                sx = DrawPanel.Width / (Wx * 1.02f); // Wx是数据范围的宽度
                sy = DrawPanel.Height / (Wy * 1.02f); // Wy是数据范围的高度
                // 选择一个较小的比例
                dx = dy = 0.0f;
                if (sx > sy)
                {
                    sx = sy;
                    dx = (Wx - DrawPanel.Width / sx) / 2.0f; // 调整横向比例，横向居中
                }
                else
                {
                    sy = sx;
                    dy = (Wy - DrawPanel.Height / sy) / 2.0f; // 调整纵向比例，纵向居中
                }

                // 设立坐标变换矩阵的坐标缩放比例
                g.ScaleTransform(sx, -sy); // 设定缩放比例。由于画板坐标的原点在左上角，向下为正，将y比例设为负数

                // 设置原点平移量
                g.TranslateTransform(-box.Xmin + Wx * 0.01f - dx, -box.Ymax - Wy * 0.01f+ dy);

                // 设立一个彩色的画笔
                Single penWidth = 1.0F / (sx + sy) * 2;   // 计算画笔宽度，为缩放比例的倒数。

                Pen myPen = new Pen(Color.FromArgb(220, 220, 220), penWidth);//灰笔
                DrawChess(g, myPen);

                myPen = new Pen(Color.Blue, penWidth);//蓝笔
                Pen penGreen = new Pen(Color.FromArgb(0, 200, 0), penWidth);
                Pen penRed = new Pen(Color.Red, penWidth);

                //测试数据
                if (beginGame)
                {
                    this.points = frmRoom.points;//将它的points指向frmroom的points
                    for (int i = 0; i < points.GetUpperBound(0)+1; i++)
                    {
                        g.DrawLine(penRed, points[i].X - 0.5f, points[i].Y, points[i].X + 0.5f, points[i].Y);
                        g.DrawLine(penRed, points[i].X, points[i].Y - 0.5f, points[i].X, points[i].Y + 0.5f);
                    }
                    GetBoundingBox(points);
                    //在这里缩放
                }
                //画直线拟合部分的点和直线
                if (ShowDrawLine == true)
                {
                    if (JudgeCount == 0)
                    {
                        g.DrawLine(myPen, p1.X, p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
                        g.DrawLine(myPen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
                    }
                    else if (JudgeCount == 1)
                    {
                        g.DrawLine(myPen, p1.X,p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
                        g.DrawLine(myPen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
                        g.DrawLine(myPen, p2.X, p2.Y - 0.5f, p2.X, p2.Y + 0.5f);
                        g.DrawLine(myPen, p2.X - 0.5f, p2.Y, p2.X + 0.5f, p2.Y);
                        g.DrawLine(penGreen, p1, p2);
                    }
                    else { }
                }

                //画抛物线部分
                if (ShowCurve == true)
                {
                    PointF[] pt = new PointF[2];
                    float i = -500;
                    if (XorY == false)//假设现在准线平行于X轴
                    {
                        while (i <= 500)
                        {
                            pt[0].X = i;
                            pt[0].Y = ParaA * pt[0].X*pt[0].X + ParaB;
                            pt[1].X = i + 0.5f;
                            pt[1].Y = ParaA *pt[1].X*pt[1].X + ParaB;
                            if (pt[0].Y <= 500 * 20 && pt[1].Y <= 500 * 20)
                            {
                                g.DrawLine(penGreen, pt[0], pt[1]);
                            }
                            i = i + 0.5f;
                        }
                    }
                    else
                    {
                        while (i <= 500)
                        {
                            pt[0].Y = i;
                            pt[0].X = ParaA * pt[0].X*pt[0].X + ParaB;
                            pt[1].Y = i + 0.5f;
                            pt[1].X = ParaA * pt[1].X*pt[1].X + ParaB;
                            if (pt[0].X <= 500 * 20 && pt[1].X <= 500 * 20)
                            {
                                g.DrawLine(penGreen, pt[0], pt[1]);
                            }
                            i = i + 0.5f;
                        }
                    }
                    DrawCurve = false;
                }

            //}
            //catch
            //{
            //    MessageBox.Show("catch");
            //}
        }
        private void DrawChess(Graphics g, Pen p)
        {
            //以20为间隔画坐标系的网格

            for (int i = -500; i <= 500; i++)
            {
                g.DrawLine(p, i * 20, -1000, i * 20, 1000);

            }
            for (int i = -500; i <= 500; i++)
            {
                g.DrawLine(p, -1000, i * 20, 1000, i * 20);

            }


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
                    ParaA = Convert.ToSingle(Para_a.Text);
                    ParaB = Convert.ToSingle(Para_b.Text);
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


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        public void GetBoundingBox(PointF[] points)
        {
            object thislock = new object();
            lock (thislock) //这是通过监听线程访问的，防止别的线程访问box，先将它锁住。
            {
                box.Xmin = points[0].X;
                box.Xmax = points[0].X;
                box.Ymin = points[0].Y;
                box.Ymax = points[0].Y;
                //选取边界盒
                for (int i = 1; i < points.Length; i++)
                {
                    if (points[i].X < box.Xmin) box.Xmin = points[i].X;
                    if (points[i].Y < box.Ymin) box.Ymin = points[i].Y;
                    if (points[i].X > box.Xmax) box.Xmax = points[i].X;
                    if (points[i].Y > box.Ymax) box.Ymax = points[i].Y;
                }
            }
        }

        public float Wx => box.Xmax - box.Ymin;
        public float Wy => box.Ymax - box.Ymin;
    }


}

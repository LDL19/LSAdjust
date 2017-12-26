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

        public bool gameStart=false;//在接收deal之后变成true
        Boolean leftDown = false, midDown =false,  rightDown=false;//按下鼠标左键、中键、右键。
        Boolean initial=false;//是否为第一次
        PointF mouseDown, mouseUp;//鼠标按下、鼠标抬起时page的坐标
        FrmRoom frmRoom;
        ////RectangleF rect;//数据范围
        Box box;//数据的显示范围world

        PointF p1, p2;//表示画的直线的起点和终点坐标
        int leftCount = 0;//左键点击计数

        Matrix mat;//将page坐标变到world坐标

        /// <summary>
        /// 窗体初始化，将service，tableIndex，seat初始化
        /// </summary>
        /// <param name="tableIndex">父窗体的tableIndex</param>
        /// <param name="seat">父窗体的seat</param>
        /// <param name="sw">父窗体的StreamWriter</param>
        public FrmPlay(int tableIndex,int seat,StreamWriter sw)
        {
            InitializeComponent();
            service = new Service(listChat,sw);
            this.tableIndex = tableIndex;
            this.seat = seat;
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要离开吗？", "离开", MessageBoxButtons.YesNo);
            if(result==DialogResult.Yes )
            {         
                service.Send2Server("StandUp");
                //让frmRoom的房间的checkbox可以点击
                this.Close();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)//发送chat
        {
            service.Send2Server(string.Format("Chat,{0}", txtChat.Text));
            txtChat.Text = "";
        }
      
        private void btnFinish_Click(object sender, EventArgs e)
        {
            //beginGame = false;
            //发送结果到服务器
            string sendStr = p1.X + "," + p1.Y + "," +p2.X + "," + p2.Y;
            service.Send2Server(string.Format("Finish,{0}", sendStr));
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            service.Send2Server(string.Format("Ready,"));//后面还有一个关数
            btnReady.Enabled = false;
        }
        //public delegate void PutHandler(bool btnfinsh);//委托传值
        //public PutHandler putBoolHandler;//委托对象
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (putBoolHandler != null)
        //    {
        //        putBoolHandler(btnFinishflag);
        //    }
        //}
   
        private void FrmPlay_Load(object sender, EventArgs e)
        {
            frmRoom = (FrmRoom)this.Owner;
            this.DrawPanel.MouseWheel += new MouseEventHandler(DrawPanel_MouseWheel);//赋予c#mousewheel事件
            //TestPoints = new double[ReceivePoints.Length];
            // TestPoints = (double[])ReceivePoints.Clone();//
            //由于winrectangle是用来画矩形，没有开始画所以要进行设置
            //panel.Parent = DrawPanel;
            //panel.BackColor = Color.Transparent;
            //panel.BorderStyle = BorderStyle.FixedSingle;
            //panel.Visible = false;
            box.Xmin = -50;
            box.Xmax = 50;
            box.Ymin = -50;
            box.Ymax = 50;
   
        }
        private void FrmPlay_ResizeEnd(object sender, EventArgs e)
        {
            tabControl1.Width = Width;
            DrawPanel.Width = tabControl1.Width - groupBox1.Width - 32;
            //Refresh();不止为何不能直接点 放大框，这时候，就不能同步
        }
        public void Zoom(float scale, PointF mouse)
        {
            box.Xmin = ZoomChange(scale, mouse.X, box.Xmin);
            box.Ymin = ZoomChange(scale, mouse.Y, box.Ymin);
            box.Xmax = ZoomChange(scale, mouse.X, box.Xmax);
            box.Ymax = ZoomChange(scale, mouse.Y, box.Ymax);
        }
        public float ZoomChange(float scale, float mouse, float old) => mouse + scale * (old - mouse);
        private void DrawPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            float scale;
            if (e.Delta > 0)//向上滚动滑轮
                scale = 1.2f;
            else//向下滚动滑轮
                scale = 0.8f;
            PointF[] mousePo = { new PointF(e.X, e.Y) };
            mat.TransformPoints(mousePo);
            PointF mouse = mousePo[0];
            Zoom(scale, mouse);
            DrawPanel.Refresh();
        }
        private void DrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            PointF[] mousePo = { new PointF(e.X, e.Y) };
            mat.TransformPoints(mousePo);
            mouseDown = mousePo[0];

            if (e.Button == MouseButtons.Middle)//中间平移，右键开窗
            {
                DrawPanel.Cursor = Cursors.NoMove2D;//设置光标样式
                midDown = true;
                ////将窗体内容写入bitscreen
                //bitScreen = new Bitmap(DrawPanel.Width, DrawPanel.Height);
                //Graphics g = Graphics.FromImage(bitScreen);
                //g.CompositingQuality = CompositingQuality.HighQuality;
                //DrawPanel.DrawToBitmap(bitScreen, DrawPanel.ClientRectangle);

                ////创建窗体*3的大小的bitmap
                //if (bitPanel != null) bitPanel.Dispose();
                //bitPanel = new Bitmap(3 * DrawPanel.Width, 3 * DrawPanel.Height);
                //g = Graphics.FromImage(bitPanel);
                //g.CompositingQuality = CompositingQuality.HighQuality;
                //g.Clear(Color.AliceBlue);//清空其内容，作为背景
                //g.DrawImage(bitScreen, DrawPanel.Width, DrawPanel.Height);
            }
            else if (e.Button == MouseButtons.Right)
            {
                DrawPanel.Cursor = Cursors.Cross;//设置成交叉
                rightDown = true;
            }
            else if (e.Button == MouseButtons.Left)
            {
                leftDown = true;
                if (leftCount == 0)
                {
                    //获取第一个点的用户坐标
                    p1 = mousePo[0];
                    leftCount += 1;
                }
                else if (leftCount == 1)
                {
                    //获取第二个点的用户坐标
                    p2 = mousePo[0];
                    leftCount = 0;
                }
                DrawPanel.Refresh();
            }
        }
        private void DrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            PointF[] mousePo = { new PointF(e.X, e.Y) };
            if (mat != null) 
            {
                mat.TransformPoints(mousePo);
            }
            mouseUp = mousePo[0];
            lblX.Text = "X:"+String.Format("{0:F}",mousePo[0].X);
            lblY.Text = "Y:"+String.Format("{0:F}",mousePo[0].Y);

            if (e.Button==MouseButtons.Middle)
            {
                float dx = mouseUp.X - mouseDown.X;
                float dy = mouseUp.Y - mouseDown.Y;
                mouseUp = mousePo[0];
                box.Xmin -= dx;
                box.Xmax -= dx;
                box.Ymin -= dy;
                box.Ymax -= dy;
                //Graphics g = DrawPanel.CreateGraphics();
                //g.DrawImage(bitPanel, -DrawPanel.Width + dx, -DrawPanel.Height + dy);
            }
            else if(e.Button==MouseButtons.Right)
            {
                mouseUp = mousePo[0];
            }
            DrawPanel.Refresh();
            //if (IsWindow == true)
            //{
            //    //显示开窗
            //    panel.Visible = true;
            //    LeftX = WindowX;
            //    LeftY = WindowY;
            //    //正常情况左上到右下，但如果右下到左上，则需要转化位置
            //    if (LeftX > e.X) LeftX = e.X + DrawPanel.Location.X;
            //    if (LeftY > e.Y) LeftY = e.Y + DrawPanel.Location.Y;
            //    //设置winrectangle的左上角和大小
            //    panel.Location = new Point(Convert.ToInt32(LeftX), Convert.ToInt32(LeftY));
            //    panel.Size = new Size(Convert.ToInt32(Math.Abs(e.X - WindowX)), Convert.ToInt32(Math.Abs(e.Y - WindowY)));
            //}
        }
        private void DrawPanel_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Middle)
            {
                midDown = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                rightDown = false;
                if (mouseDown.X < mouseUp.X)
                {
                    box.Xmin = mouseDown.X;
                    box.Xmax = mouseUp.X;
                }
                else
                {
                    box.Xmin = mouseUp.X;
                    box.Xmax = mouseDown.X;
                }
                if (mouseDown.Y < mouseUp.Y)
                {
                   box.Ymin = mouseDown.Y;
                   box.Ymax = mouseUp.Y;
                }
                else
                {
                    box.Ymin = mouseUp.Y;
                    box.Ymax = mouseDown.Y;
                }
            }
            DrawPanel.Cursor = Cursors.Cross;
            leftDown = false;
            DrawPanel.Refresh();
        }
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.ResetTransform();
            if(initial)
            {
                box.Xmin = -50;
                box.Xmax = 50;
                box.Ymin = -50;
                box.Ymax = 50;
            }
            RectangleF viewport = new RectangleF(box.Xmin, box.Ymin, box.Xmax - box.Xmin, box.Ymax - box.Ymin);
            float s = ViewPort2Page(g, viewport, DrawPanel.Width, DrawPanel.Height);
            
            mat = g.Transform.Clone();
            mat.Invert();
            // 设立一个彩色的画笔
            float penWidth = s;   // 计算画笔宽度，为缩放比例的倒数。

            Pen pen = new Pen(Color.FromArgb(220, 220, 220), penWidth);//灰笔画棋盘
            DrawChessboard(g, pen);
            pen.Color = Color.Red;//红笔画点
            if(gameStart)
            {
                PointF[] points = frmRoom.points;
                for(int i=0;i<points.Length;i++)
                {
                    g.DrawLine(pen, points[i].X - penWidth * 10, points[i].Y, points[i].X + penWidth * 10, points[i].Y);
                    g.DrawLine(pen, points[i].X, points[i].Y - penWidth * 10, points[i].X, points[i].Y + penWidth * 10);
                }
                pen.Color = Color.Blue;//蓝笔画线
                Pen penGreen = new Pen(Color.Green, penWidth);
                if (leftCount == 1)
                {
                    g.DrawLine(pen, p1.X, p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
                    g.DrawLine(pen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
                   
                }
                else if (leftCount == 0)
                {
                    g.DrawLine(pen, p1.X, p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
                    g.DrawLine(pen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
                    g.DrawLine(pen, p2.X, p2.Y - 0.5f, p2.X, p2.Y + 0.5f);
                    g.DrawLine(pen, p2.X - 0.5f, p2.Y, p2.X + 0.5f, p2.Y);
                    g.DrawLine(penGreen, p1, p2);

                }
            }
           
            
            //Pen penGreen = new Pen(Color.FromArgb(0, 200, 0), penWidth);

            //测试数据

            //}
            ////画直线拟合部分的点和直线
            //if (ShowDrawLine == true)
            //{
            //    if (JudgeCount == 0)
            //    {
            //        g.DrawLine(pen, p1.X, p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
            //        g.DrawLine(pen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
            //    }
            //    else if (JudgeCount == 1)
            //    {
            //        g.DrawLine(pen, p1.X,p1.Y - 0.5f, p1.X, p1.Y + 0.5f);
            //        g.DrawLine(pen, p1.X - 0.5f, p1.Y, p1.X + 0.5f, p1.Y);
            //        g.DrawLine(pen, p2.X, p2.Y - 0.5f, p2.X, p2.Y + 0.5f);
            //        g.DrawLine(pen, p2.X - 0.5f, p2.Y, p2.X + 0.5f, p2.Y);
            //        g.DrawLine(penGreen, p1, p2);
            //    }
            //    else { }
            //}

            ////画抛物线部分
            //if (ShowCurve == true)
            //{
            //    PointF[] pt = new PointF[2];
            //    float i = -500;
            //    Boolean parallel_X = true;
            //    if (parallel_X == true)//假设现在准线平行于X轴
            //    {
            //        while (i <= 500)
            //        {
            //            pt[0].X = i;
            //            pt[0].Y = ParaA * pt[0].X*pt[0].X + ParaB;
            //            pt[1].X = i + 0.5f;
            //            pt[1].Y = ParaA *pt[1].X*pt[1].X + ParaB;
            //            if (pt[0].Y <= 500 * 20 && pt[1].Y <= 500 * 20)
            //            {
            //                g.DrawLine(penGreen, pt[0], pt[1]);
            //            }
            //            i = i + 0.5f;
            //        }
            //    }
            //    else
            //    {
            //        while (i <= 500)
            //        {
            //            pt[0].Y = i;
            //            pt[0].X = ParaA * pt[0].X*pt[0].X + ParaB;
            //            pt[1].Y = i + 0.5f;
            //            pt[1].X = ParaA * pt[1].X*pt[1].X + ParaB;
            //            if (pt[0].X <= 500 * 20 && pt[1].X <= 500 * 20)
            //                g.DrawLine(penGreen, pt[0], pt[1]);
            //            i = i + 0.5f;
            //        }
            //    }
            //    DrawCurve = false;
            //}

            //}
            //catch
            //{
            //    MessageBox.Show("catch");
            //}
            if (rightDown)
            {
                Pen p1 = new Pen(Color.Black);
                p1.Width = (float)0.000001;
                PointF[] Lp = new PointF[5];
                Lp[0].X = this.mouseDown.X;
                Lp[0].Y = this.mouseDown.Y;
                Lp[1].X = this.mouseDown.X;
                Lp[1].Y = mouseUp.Y;
                Lp[2].X = mouseUp.X;
                Lp[2].Y = mouseUp.Y;
                Lp[3].X = mouseUp.X;
                Lp[3].Y = this.mouseDown.Y;
                Lp[4].X = this.mouseDown.X;
                Lp[4].Y = this.mouseDown.Y;
                e.Graphics.DrawLines(p1, Lp);
            }
        }

        private void btnRedraw_Click(object sender, EventArgs e)
        {
            initial = true;
            DrawPanel.Refresh();
            initial = false;
        }

        float ViewPort2Page(Graphics gg, RectangleF viewport, int w, int h)
        {
            gg.ResetTransform();
            gg.TranslateTransform((-viewport.X + 0.01f * viewport.Width), (-(viewport.Y + viewport.Height) - 0.01f * viewport.Height), MatrixOrder.Append);
            gg.ScaleTransform(1, -1, MatrixOrder.Append);
            float sx = 1.02f * viewport.Width / w, sy = 1.02f * viewport.Height / h;
            float dx = 0, dy = 0;
            if (sx != 0 && sy != 0)
                if (sx < sy)
                {
                    sx = sy;
                    gg.ScaleTransform(1 / sy, 1 / sy, MatrixOrder.Append);
                    dx = (w - viewport.Width * 1.01f / sx) / 2f;
                }
                else
                {
                    sy = sx;
                    gg.ScaleTransform(1 / sx, 1 / sx, MatrixOrder.Append);
                    dy = (h - viewport.Height * 1.01f / sy) / 2f;
                }
            gg.TranslateTransform(dx, dy, MatrixOrder.Append);
            return sx;
        }
        private void DrawChessboard(Graphics g, Pen p)
        {
            //以1为间隔画坐标系的网格
            int interval = 2;
            for (int i = -100; i <= 100; i+=interval)
                g.DrawLine(p, i , -100, i , 100);
            for (int i = -100; i <= 100; i+=interval)
                g.DrawLine(p, -100, i , 100, i );
        }

        private void DrawPoly2_Click(object sender, EventArgs e)
        {
            //if (lblParam1.Text == "" || lblParam2.Text == "")
            //{
            //    MessageBox.Show("请输入参数");
            //    return;
            //}
            //else
            //{
            //    try
            //    {
            //        ParaA = Convert.ToSingle(lblParam1.Text);
            //        ParaB = Convert.ToSingle(lblParam2.Text);
            //        DrawCurve = true;
            //        ShowCurve = true;
            //        DrawPanel.Refresh();
            //    }
            //    catch
            //    {
            //        MessageBox.Show("报错，请检查输入是否正确");
            //        return;
            //    }

            //}
        }
        public Box GetBoundingBox(PointF[] points)
        {
            //object thislock = new object();
            //lock (thislock) //这是通过监听线程访问的，防止别的线程访问box，先将它锁住。
            //{
            Box box;
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
            //}
            return box;
        }
    }


}

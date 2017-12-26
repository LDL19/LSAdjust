namespace LSClient
{
    partial class FrmPlay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.DrawPanel = new System.Windows.Forms.PictureBox();
            this.btnReady = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.chkTimeLimit = new System.Windows.Forms.CheckBox();
            this.txtTimeLimit = new System.Windows.Forms.TextBox();
            this.lblTimeLimit = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelY = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.Para_b = new System.Windows.Forms.TextBox();
            this.Para_a = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.DrawXcurve = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.DrawL = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.WinRectangle = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.DrawPanel)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WinRectangle)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(7, 25);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(338, 139);
            this.listBox1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 172);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(338, 39);
            this.textBox1.TabIndex = 7;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(7, 219);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(338, 43);
            this.buttonSend.TabIndex = 8;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // DrawPanel
            // 
            this.DrawPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.DrawPanel.Location = new System.Drawing.Point(3, 3);
            this.DrawPanel.Margin = new System.Windows.Forms.Padding(4);
            this.DrawPanel.Name = "DrawPanel";
            this.DrawPanel.Size = new System.Drawing.Size(868, 742);
            this.DrawPanel.TabIndex = 9;
            this.DrawPanel.TabStop = false;
            this.DrawPanel.Click += new System.EventHandler(this.DrawPanel_Click);
            this.DrawPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawPanel_Paint);
            this.DrawPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawPanel_MouseDown);
            this.DrawPanel.MouseEnter += new System.EventHandler(this.DrawPanel_MouseEnter);
            this.DrawPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawPanel_MouseMove);
            this.DrawPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawPanel_MouseUp);
            // 
            // btnReady
            // 
            this.btnReady.Location = new System.Drawing.Point(230, 187);
            this.btnReady.Margin = new System.Windows.Forms.Padding(4);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(104, 49);
            this.btnReady.TabIndex = 10;
            this.btnReady.Text = "准备";
            this.btnReady.UseVisualStyleBackColor = true;
            this.btnReady.Click += new System.EventHandler(this.btnReady_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(230, 381);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(104, 49);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "离开";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(199, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "关卡";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(242, 39);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(67, 25);
            this.textBox2.TabIndex = 13;
            this.textBox2.Text = "1";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = " 积分";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(193, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = " 排名";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(242, 77);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(67, 25);
            this.textBox3.TabIndex = 16;
            this.textBox3.Text = "0";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(242, 108);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(67, 25);
            this.textBox4.TabIndex = 17;
            this.textBox4.Text = "1/5";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(230, 250);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(104, 49);
            this.btnDraw.TabIndex = 18;
            this.btnDraw.Text = "重绘";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(230, 318);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(104, 49);
            this.btnFinish.TabIndex = 19;
            this.btnFinish.Text = "提交";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // chkTimeLimit
            // 
            this.chkTimeLimit.AutoSize = true;
            this.chkTimeLimit.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.chkTimeLimit.Location = new System.Drawing.Point(194, 155);
            this.chkTimeLimit.Name = "chkTimeLimit";
            this.chkTimeLimit.Size = new System.Drawing.Size(74, 19);
            this.chkTimeLimit.TabIndex = 21;
            this.chkTimeLimit.Text = "限时：";
            this.chkTimeLimit.UseVisualStyleBackColor = true;
            // 
            // txtTimeLimit
            // 
            this.txtTimeLimit.Location = new System.Drawing.Point(257, 152);
            this.txtTimeLimit.Name = "txtTimeLimit";
            this.txtTimeLimit.Size = new System.Drawing.Size(34, 25);
            this.txtTimeLimit.TabIndex = 22;
            this.txtTimeLimit.Text = "1";
            // 
            // lblTimeLimit
            // 
            this.lblTimeLimit.AutoSize = true;
            this.lblTimeLimit.Location = new System.Drawing.Point(297, 155);
            this.lblTimeLimit.Name = "lblTimeLimit";
            this.lblTimeLimit.Size = new System.Drawing.Size(37, 15);
            this.lblTimeLimit.TabIndex = 23;
            this.lblTimeLimit.Text = "分钟";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1274, 777);
            this.tabControl1.TabIndex = 25;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.WinRectangle);
            this.tabPage1.Controls.Add(this.DrawPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1266, 748);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "拟合画图部分";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.LightCyan;
            this.groupBox1.Controls.Add(this.labelY);
            this.groupBox1.Controls.Add(this.labelX);
            this.groupBox1.Controls.Add(this.Para_b);
            this.groupBox1.Controls.Add(this.Para_a);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.DrawXcurve);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.DrawL);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnReady);
            this.groupBox1.Controls.Add(this.btnDraw);
            this.groupBox1.Controls.Add(this.lblTimeLimit);
            this.groupBox1.Controls.Add(this.txtTimeLimit);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkTimeLimit);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnFinish);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(872, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 742);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "game";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelY.Location = new System.Drawing.Point(29, 442);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(29, 20);
            this.labelY.TabIndex = 35;
            this.labelY.Text = "Y:";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelX.Location = new System.Drawing.Point(29, 410);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(29, 20);
            this.labelX.TabIndex = 34;
            this.labelX.Text = "X:";
            // 
            // Para_b
            // 
            this.Para_b.Location = new System.Drawing.Point(20, 302);
            this.Para_b.Name = "Para_b";
            this.Para_b.Size = new System.Drawing.Size(100, 25);
            this.Para_b.TabIndex = 33;
            // 
            // Para_a
            // 
            this.Para_a.Location = new System.Drawing.Point(20, 243);
            this.Para_a.Name = "Para_a";
            this.Para_a.Size = new System.Drawing.Size(100, 25);
            this.Para_a.TabIndex = 32;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.buttonSend);
            this.groupBox2.Location = new System.Drawing.Point(28, 465);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 271);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "聊天";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(24, 131);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 20);
            this.label6.TabIndex = 30;
            this.label6.Text = "抛物线";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(24, 187);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 20);
            this.label7.TabIndex = 29;
            this.label7.Text = "X=aY^2+b";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(24, 33);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 20);
            this.label5.TabIndex = 27;
            this.label5.Text = "直线";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(24, 157);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 20);
            this.label8.TabIndex = 28;
            this.label8.Text = "Y=aX^2+b";
            // 
            // DrawXcurve
            // 
            this.DrawXcurve.Location = new System.Drawing.Point(20, 345);
            this.DrawXcurve.Name = "DrawXcurve";
            this.DrawXcurve.Size = new System.Drawing.Size(105, 54);
            this.DrawXcurve.TabIndex = 26;
            this.DrawXcurve.Text = "绘制曲线";
            this.DrawXcurve.UseVisualStyleBackColor = true;
            this.DrawXcurve.Click += new System.EventHandler(this.DrawXcurve_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(25, 281);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 18);
            this.label9.TabIndex = 27;
            this.label9.Text = "系数b";
            // 
            // DrawL
            // 
            this.DrawL.Location = new System.Drawing.Point(20, 56);
            this.DrawL.Name = "DrawL";
            this.DrawL.Size = new System.Drawing.Size(105, 55);
            this.DrawL.TabIndex = 25;
            this.DrawL.Text = "绘制直线";
            this.DrawL.UseVisualStyleBackColor = true;
            this.DrawL.Click += new System.EventHandler(this.DrawL_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(25, 222);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 18);
            this.label10.TabIndex = 26;
            this.label10.Text = "系数a";
            // 
            // WinRectangle
            // 
            this.WinRectangle.Location = new System.Drawing.Point(8, 6);
            this.WinRectangle.Name = "WinRectangle";
            this.WinRectangle.Size = new System.Drawing.Size(100, 50);
            this.WinRectangle.TabIndex = 25;
            this.WinRectangle.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1266, 748);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FrmPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 777);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmPlay";
            this.Text = "眼神平差大师";
            this.Load += new System.EventHandler(this.FrmPlay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DrawPanel)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WinRectangle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSend;
        public System.Windows.Forms.PictureBox DrawPanel;
        private System.Windows.Forms.Button btnReady;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.CheckBox chkTimeLimit;
        private System.Windows.Forms.TextBox txtTimeLimit;
        private System.Windows.Forms.Label lblTimeLimit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox WinRectangle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button DrawL;
        private System.Windows.Forms.Button DrawXcurve;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Para_b;
        private System.Windows.Forms.TextBox Para_a;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelX;
    }
}
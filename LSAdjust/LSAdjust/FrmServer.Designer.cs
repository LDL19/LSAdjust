namespace LSAdjust
{
    partial class FrmServer
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBoxMaxUsers = new System.Windows.Forms.TextBox();
            this.textBoxMaxTables = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(13, 13);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(817, 274);
            this.listBox1.TabIndex = 1;
            // 
            // textBoxMaxUsers
            // 
            this.textBoxMaxUsers.Location = new System.Drawing.Point(376, 313);
            this.textBoxMaxUsers.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMaxUsers.Name = "textBoxMaxUsers";
            this.textBoxMaxUsers.Size = new System.Drawing.Size(60, 25);
            this.textBoxMaxUsers.TabIndex = 8;
            this.textBoxMaxUsers.Text = "10";
            // 
            // textBoxMaxTables
            // 
            this.textBoxMaxTables.Location = new System.Drawing.Point(375, 346);
            this.textBoxMaxTables.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMaxTables.Name = "textBoxMaxTables";
            this.textBoxMaxTables.Size = new System.Drawing.Size(61, 25);
            this.textBoxMaxTables.TabIndex = 9;
            this.textBoxMaxTables.Text = "3";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(290, 398);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 29);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "启动服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(425, 398);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 29);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "停止服务";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // FrmServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 475);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.textBoxMaxTables);
            this.Controls.Add(this.textBoxMaxUsers);
            this.Controls.Add(this.listBox1);
            this.Name = "FrmServer";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmServer_FormClosing);
            this.Load += new System.EventHandler(this.FrmServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBoxMaxUsers;
        private System.Windows.Forms.TextBox textBoxMaxTables;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    }
}


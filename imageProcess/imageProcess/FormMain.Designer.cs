namespace imageProcess
{
    partial class FormMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelXY = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGray = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistogram = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMirror = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.labelB = new System.Windows.Forms.Label();
            this.labelG = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.labelDim = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(10, 21);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 300);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(9, 23);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(300, 300);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox3);
            this.groupBox2.Controls.Add(this.labelInfo);
            this.groupBox2.Location = new System.Drawing.Point(389, 52);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(465, 350);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "資訊";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(139, 263);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(320, 80);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.labelInfo.Location = new System.Drawing.Point(6, 20);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(0, 18);
            this.labelInfo.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chart1);
            this.groupBox3.Location = new System.Drawing.Point(12, 456);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 350);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "直方圖";
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            this.chart1.Location = new System.Drawing.Point(10, 23);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(300, 250);
            this.chart1.TabIndex = 1;
            this.chart1.Visible = false;
            // 
            // labelXY
            // 
            this.labelXY.AutoSize = true;
            this.labelXY.Location = new System.Drawing.Point(6, 19);
            this.labelXY.Name = "labelXY";
            this.labelXY.Size = new System.Drawing.Size(0, 16);
            this.labelXY.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 350);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "原始圖像";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pictureBox2);
            this.groupBox4.Location = new System.Drawing.Point(389, 456);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(465, 350);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "處理結果";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTool});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(866, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpen,
            this.menuSave});
            this.menuFile.Name = "menuFile";
            this.menuFile.ShortcutKeyDisplayString = "";
            this.menuFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.menuFile.ShowShortcutKeys = false;
            this.menuFile.Size = new System.Drawing.Size(57, 20);
            this.menuFile.Text = "檔案(F)";
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeyDisplayString = "";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(185, 22);
            this.menuOpen.Text = "開啟圖檔(O)";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.menuSave.Size = new System.Drawing.Size(185, 22);
            this.menuSave.Text = "另存圖檔(A)";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuTool
            // 
            this.menuTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInvert,
            this.menuGray,
            this.menuHistogram,
            this.menuMirror,
            this.menuRotate});
            this.menuTool.Name = "menuTool";
            this.menuTool.Size = new System.Drawing.Size(43, 20);
            this.menuTool.Text = "工具";
            // 
            // menuInvert
            // 
            this.menuInvert.Name = "menuInvert";
            this.menuInvert.Size = new System.Drawing.Size(110, 22);
            this.menuInvert.Text = "負片";
            this.menuInvert.Click += new System.EventHandler(this.menuInvert_Click);
            // 
            // menuGray
            // 
            this.menuGray.Name = "menuGray";
            this.menuGray.Size = new System.Drawing.Size(110, 22);
            this.menuGray.Text = "灰階";
            this.menuGray.Click += new System.EventHandler(this.menuGray_Click);
            // 
            // menuHistogram
            // 
            this.menuHistogram.Name = "menuHistogram";
            this.menuHistogram.Size = new System.Drawing.Size(110, 22);
            this.menuHistogram.Text = "直方圖";
            this.menuHistogram.Click += new System.EventHandler(this.menuHistogram_Click);
            // 
            // menuMirror
            // 
            this.menuMirror.Name = "menuMirror";
            this.menuMirror.Size = new System.Drawing.Size(110, 22);
            this.menuMirror.Text = "鏡像";
            this.menuMirror.Click += new System.EventHandler(this.menuMirror_Click);
            // 
            // menuRotate
            // 
            this.menuRotate.Name = "menuRotate";
            this.menuRotate.Size = new System.Drawing.Size(110, 22);
            this.menuRotate.Text = "旋轉";
            this.menuRotate.Click += new System.EventHandler(this.menuRotate_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.labelB);
            this.groupBox5.Controls.Add(this.labelG);
            this.groupBox5.Controls.Add(this.labelR);
            this.groupBox5.Controls.Add(this.labelDim);
            this.groupBox5.Controls.Add(this.labelXY);
            this.groupBox5.Location = new System.Drawing.Point(12, 812);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(842, 47);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            // 
            // labelB
            // 
            this.labelB.AutoSize = true;
            this.labelB.Location = new System.Drawing.Point(226, 19);
            this.labelB.Name = "labelB";
            this.labelB.Size = new System.Drawing.Size(0, 16);
            this.labelB.TabIndex = 4;
            // 
            // labelG
            // 
            this.labelG.AutoSize = true;
            this.labelG.Location = new System.Drawing.Point(177, 19);
            this.labelG.Name = "labelG";
            this.labelG.Size = new System.Drawing.Size(0, 16);
            this.labelG.TabIndex = 3;
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.Location = new System.Drawing.Point(128, 19);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(0, 16);
            this.labelR.TabIndex = 2;
            // 
            // labelDim
            // 
            this.labelDim.AutoSize = true;
            this.labelDim.Location = new System.Drawing.Point(310, 19);
            this.labelDim.Name = "labelDim";
            this.labelDim.Size = new System.Drawing.Size(0, 16);
            this.labelDim.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 863);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.Text = "PCX - B043040003";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelXY;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuTool;
        private System.Windows.Forms.ToolStripMenuItem menuInvert;
        private System.Windows.Forms.ToolStripMenuItem menuGray;
        private System.Windows.Forms.ToolStripMenuItem menuHistogram;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label labelDim;
        private System.Windows.Forms.ToolStripMenuItem menuMirror;
        private System.Windows.Forms.Label labelB;
        private System.Windows.Forms.Label labelG;
        private System.Windows.Forms.Label labelR;
        private System.Windows.Forms.ToolStripMenuItem menuRotate;
    }
}


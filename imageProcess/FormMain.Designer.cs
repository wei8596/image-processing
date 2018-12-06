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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelXY = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGray = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNegative = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMirror = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRGB = new System.Windows.Forms.ToolStripMenuItem();
            this.menuThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScaling = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTransparency = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBitPlane = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContrastStretch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistEqual = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistSpec = new System.Windows.Forms.ToolStripMenuItem();
            this.menuComponent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOutlier = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMedian = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPseudoMedian = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLowpass = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHighpass = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHighBoost = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdge = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGradient = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRoberts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSobel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrewitt = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCoding = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHuffman = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFractal = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelB = new System.Windows.Forms.Label();
            this.labelG = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.labelDim = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(503, 52);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(351, 350);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Information";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.Location = new System.Drawing.Point(6, 20);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(339, 320);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.TabStop = false;
            this.dataGridView1.Visible = false;
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.HeaderText = "Info";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 180;
            // 
            // Column2
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column2.HeaderText = "Value";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 139;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(61, 60);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(240, 240);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chart1);
            this.groupBox3.Location = new System.Drawing.Point(12, 456);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(485, 350);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Histogram";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(10, 23);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(469, 321);
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
            this.groupBox1.Size = new System.Drawing.Size(485, 350);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Original Image";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pictureBox3);
            this.groupBox4.Location = new System.Drawing.Point(503, 456);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 350);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Palette";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTool,
            this.menuFilter,
            this.menuCoding});
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
            this.menuSave,
            this.menuClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.ShortcutKeyDisplayString = "";
            this.menuFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.menuFile.ShowShortcutKeys = false;
            this.menuFile.Size = new System.Drawing.Size(38, 20);
            this.menuFile.Text = "&File";
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeyDisplayString = "";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(160, 22);
            this.menuOpen.Text = "&Open";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.menuSave.Size = new System.Drawing.Size(160, 22);
            this.menuSave.Text = "Save &As";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuClose.Size = new System.Drawing.Size(160, 22);
            this.menuClose.Text = "E&xit";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuTool
            // 
            this.menuTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGray,
            this.menuNegative,
            this.menuMirror,
            this.menuRotate,
            this.menuRGB,
            this.menuThreshold,
            this.menuScaling,
            this.menuTransparency,
            this.menuBitPlane,
            this.menuContrastStretch,
            this.menuHistEqual,
            this.menuHistSpec,
            this.menuComponent,
            this.menuSelect});
            this.menuTool.Name = "menuTool";
            this.menuTool.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.menuTool.ShowShortcutKeys = false;
            this.menuTool.Size = new System.Drawing.Size(50, 20);
            this.menuTool.Text = "&Tools";
            // 
            // menuGray
            // 
            this.menuGray.Name = "menuGray";
            this.menuGray.Size = new System.Drawing.Size(209, 22);
            this.menuGray.Text = "Gray Level";
            this.menuGray.Click += new System.EventHandler(this.menuGray_Click);
            // 
            // menuNegative
            // 
            this.menuNegative.Name = "menuNegative";
            this.menuNegative.Size = new System.Drawing.Size(209, 22);
            this.menuNegative.Text = "Negative";
            this.menuNegative.Click += new System.EventHandler(this.menuNegative_Click);
            // 
            // menuMirror
            // 
            this.menuMirror.Name = "menuMirror";
            this.menuMirror.Size = new System.Drawing.Size(209, 22);
            this.menuMirror.Text = "Mirror";
            this.menuMirror.Click += new System.EventHandler(this.menuMirror_Click);
            // 
            // menuRotate
            // 
            this.menuRotate.Name = "menuRotate";
            this.menuRotate.Size = new System.Drawing.Size(209, 22);
            this.menuRotate.Text = "Rotate";
            this.menuRotate.Click += new System.EventHandler(this.menuRotate_Click);
            // 
            // menuRGB
            // 
            this.menuRGB.Name = "menuRGB";
            this.menuRGB.Size = new System.Drawing.Size(209, 22);
            this.menuRGB.Text = "RGB Color";
            this.menuRGB.Click += new System.EventHandler(this.menuRGB_Click);
            // 
            // menuThreshold
            // 
            this.menuThreshold.Name = "menuThreshold";
            this.menuThreshold.Size = new System.Drawing.Size(209, 22);
            this.menuThreshold.Text = "Thresholding";
            this.menuThreshold.Click += new System.EventHandler(this.menuThreshold_Click);
            // 
            // menuScaling
            // 
            this.menuScaling.Name = "menuScaling";
            this.menuScaling.Size = new System.Drawing.Size(209, 22);
            this.menuScaling.Text = "Scaling";
            this.menuScaling.Click += new System.EventHandler(this.menuScaling_Click);
            // 
            // menuTransparency
            // 
            this.menuTransparency.Name = "menuTransparency";
            this.menuTransparency.Size = new System.Drawing.Size(209, 22);
            this.menuTransparency.Text = "Transparency";
            this.menuTransparency.Click += new System.EventHandler(this.menuTransparency_Click);
            // 
            // menuBitPlane
            // 
            this.menuBitPlane.Name = "menuBitPlane";
            this.menuBitPlane.Size = new System.Drawing.Size(209, 22);
            this.menuBitPlane.Text = "Bit-plane Slicing";
            this.menuBitPlane.Click += new System.EventHandler(this.menuBitPlane_Click);
            // 
            // menuContrastStretch
            // 
            this.menuContrastStretch.Name = "menuContrastStretch";
            this.menuContrastStretch.Size = new System.Drawing.Size(209, 22);
            this.menuContrastStretch.Text = "Contrast Stretching";
            this.menuContrastStretch.Click += new System.EventHandler(this.menuContrastStretch_Click);
            // 
            // menuHistEqual
            // 
            this.menuHistEqual.Name = "menuHistEqual";
            this.menuHistEqual.Size = new System.Drawing.Size(209, 22);
            this.menuHistEqual.Text = "Histogram Equalization";
            this.menuHistEqual.Click += new System.EventHandler(this.menuHistEqual_Click);
            // 
            // menuHistSpec
            // 
            this.menuHistSpec.Name = "menuHistSpec";
            this.menuHistSpec.Size = new System.Drawing.Size(209, 22);
            this.menuHistSpec.Text = "Histogram Specification";
            this.menuHistSpec.Click += new System.EventHandler(this.menuHistSpec_Click);
            // 
            // menuComponent
            // 
            this.menuComponent.Name = "menuComponent";
            this.menuComponent.Size = new System.Drawing.Size(209, 22);
            this.menuComponent.Text = "Connected Component";
            this.menuComponent.Click += new System.EventHandler(this.menuComponent_Click);
            // 
            // menuSelect
            // 
            this.menuSelect.Name = "menuSelect";
            this.menuSelect.Size = new System.Drawing.Size(209, 22);
            this.menuSelect.Text = "Select";
            this.menuSelect.Click += new System.EventHandler(this.menuSelect_Click);
            // 
            // menuFilter
            // 
            this.menuFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOutlier,
            this.menuMedian,
            this.menuPseudoMedian,
            this.menuLowpass,
            this.menuHighpass,
            this.menuHighBoost,
            this.menuEdge,
            this.menuGradient});
            this.menuFilter.Name = "menuFilter";
            this.menuFilter.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this.menuFilter.ShowShortcutKeys = false;
            this.menuFilter.Size = new System.Drawing.Size(51, 20);
            this.menuFilter.Text = "F&ilters";
            // 
            // menuOutlier
            // 
            this.menuOutlier.Name = "menuOutlier";
            this.menuOutlier.Size = new System.Drawing.Size(193, 22);
            this.menuOutlier.Text = "Outlier Filter";
            this.menuOutlier.Click += new System.EventHandler(this.menuOutlier_Click);
            // 
            // menuMedian
            // 
            this.menuMedian.Name = "menuMedian";
            this.menuMedian.Size = new System.Drawing.Size(193, 22);
            this.menuMedian.Text = "Median Filter";
            this.menuMedian.Click += new System.EventHandler(this.menuMedian_Click);
            // 
            // menuPseudoMedian
            // 
            this.menuPseudoMedian.Name = "menuPseudoMedian";
            this.menuPseudoMedian.Size = new System.Drawing.Size(193, 22);
            this.menuPseudoMedian.Text = "Pseudo Median Filter";
            this.menuPseudoMedian.Click += new System.EventHandler(this.menuPseudoMedian_Click);
            // 
            // menuLowpass
            // 
            this.menuLowpass.Name = "menuLowpass";
            this.menuLowpass.Size = new System.Drawing.Size(193, 22);
            this.menuLowpass.Text = "Lowpass Filter";
            this.menuLowpass.Click += new System.EventHandler(this.menuLowpass_Click);
            // 
            // menuHighpass
            // 
            this.menuHighpass.Name = "menuHighpass";
            this.menuHighpass.Size = new System.Drawing.Size(193, 22);
            this.menuHighpass.Text = "Highpass Filter";
            this.menuHighpass.Click += new System.EventHandler(this.menuHighpass_Click);
            // 
            // menuHighBoost
            // 
            this.menuHighBoost.Name = "menuHighBoost";
            this.menuHighBoost.Size = new System.Drawing.Size(193, 22);
            this.menuHighBoost.Text = "High-Boost Filter";
            this.menuHighBoost.Click += new System.EventHandler(this.menuHighBoost_Click);
            // 
            // menuEdge
            // 
            this.menuEdge.Name = "menuEdge";
            this.menuEdge.Size = new System.Drawing.Size(193, 22);
            this.menuEdge.Text = "Edge Crispening";
            this.menuEdge.Click += new System.EventHandler(this.menuEdge_Click);
            // 
            // menuGradient
            // 
            this.menuGradient.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRoberts,
            this.menuSobel,
            this.menuPrewitt});
            this.menuGradient.Name = "menuGradient";
            this.menuGradient.Size = new System.Drawing.Size(193, 22);
            this.menuGradient.Text = "Gradient";
            // 
            // menuRoberts
            // 
            this.menuRoberts.Name = "menuRoberts";
            this.menuRoberts.Size = new System.Drawing.Size(148, 22);
            this.menuRoberts.Text = "Roberts Filter";
            this.menuRoberts.Click += new System.EventHandler(this.menuRoberts_Click);
            // 
            // menuSobel
            // 
            this.menuSobel.Name = "menuSobel";
            this.menuSobel.Size = new System.Drawing.Size(148, 22);
            this.menuSobel.Text = "Sobel Filter";
            this.menuSobel.Click += new System.EventHandler(this.menuSobel_Click);
            // 
            // menuPrewitt
            // 
            this.menuPrewitt.Name = "menuPrewitt";
            this.menuPrewitt.Size = new System.Drawing.Size(148, 22);
            this.menuPrewitt.Text = "Prewitt Filter";
            this.menuPrewitt.Click += new System.EventHandler(this.menuPrewitt_Click);
            // 
            // menuCoding
            // 
            this.menuCoding.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHuffman,
            this.menuFractal});
            this.menuCoding.Name = "menuCoding";
            this.menuCoding.Size = new System.Drawing.Size(61, 20);
            this.menuCoding.Text = "&Coding";
            // 
            // menuHuffman
            // 
            this.menuHuffman.Name = "menuHuffman";
            this.menuHuffman.Size = new System.Drawing.Size(123, 22);
            this.menuHuffman.Text = "Huffman";
            this.menuHuffman.Click += new System.EventHandler(this.menuHuffman_Click);
            // 
            // menuFractal
            // 
            this.menuFractal.Name = "menuFractal";
            this.menuFractal.Size = new System.Drawing.Size(123, 22);
            this.menuFractal.Text = "Fractal";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.pictureBox2);
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
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(289, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(15, 15);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.Text = "Image Processing  By ChenWeiFan";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
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
        private System.Windows.Forms.ToolStripMenuItem menuNegative;
        private System.Windows.Forms.ToolStripMenuItem menuGray;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label labelDim;
        private System.Windows.Forms.ToolStripMenuItem menuMirror;
        private System.Windows.Forms.Label labelB;
        private System.Windows.Forms.Label labelG;
        private System.Windows.Forms.Label labelR;
        private System.Windows.Forms.ToolStripMenuItem menuRotate;
        private System.Windows.Forms.ToolStripMenuItem menuRGB;
        private System.Windows.Forms.ToolStripMenuItem menuThreshold;
        private System.Windows.Forms.ToolStripMenuItem menuScaling;
        private System.Windows.Forms.ToolStripMenuItem menuTransparency;
        private System.Windows.Forms.ToolStripMenuItem menuBitPlane;
        private System.Windows.Forms.ToolStripMenuItem menuHistEqual;
        private System.Windows.Forms.ToolStripMenuItem menuContrastStretch;
        private System.Windows.Forms.ToolStripMenuItem menuHistSpec;
        private System.Windows.Forms.ToolStripMenuItem menuFilter;
        private System.Windows.Forms.ToolStripMenuItem menuOutlier;
        private System.Windows.Forms.ToolStripMenuItem menuMedian;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.ToolStripMenuItem menuPseudoMedian;
        private System.Windows.Forms.ToolStripMenuItem menuLowpass;
        private System.Windows.Forms.ToolStripMenuItem menuHighpass;
        private System.Windows.Forms.ToolStripMenuItem menuHighBoost;
        private System.Windows.Forms.ToolStripMenuItem menuEdge;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolStripMenuItem menuGradient;
        private System.Windows.Forms.ToolStripMenuItem menuRoberts;
        private System.Windows.Forms.ToolStripMenuItem menuSobel;
        private System.Windows.Forms.ToolStripMenuItem menuPrewitt;
        private System.Windows.Forms.ToolStripMenuItem menuCoding;
        private System.Windows.Forms.ToolStripMenuItem menuHuffman;
        private System.Windows.Forms.ToolStripMenuItem menuFractal;
        private System.Windows.Forms.ToolStripMenuItem menuComponent;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.ToolStripMenuItem menuSelect;
    }
}


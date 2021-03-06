﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            try
            {
                // 使用thread執行splash screen(載入畫面)
                Thread t = new Thread(new ThreadStart(StartForm));
                t.Start();
                Thread.Sleep(2000);

                InitializeComponent();
                // 載入完成後結束splash screen
                t.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // thread start function
        public void StartForm()
        {
            Application.Run(new Splashscreen());
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 未開檔時, 無法使用存檔與工具
            menuSave.Enabled = false;
            menuTool.Enabled = false;
            menuFilter.Enabled = false;
            menuCoding.Enabled = false;
        }

        // PCX物件
        ImgPcx pcxOrigin;
        // 原始影像灰階圖
        ImgPcx pcxGray;

        // 讀檔
        private void menuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // 清空顯示圖片與資訊
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                dataGridView1.Visible = false;
                labelXY.Text = "";
                labelR.Text = "";
                labelG.Text = "";
                labelB.Text = "";
                labelDim.Text = "";
                chart1.Visible = false;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "開啟圖檔";
                openFileDialog.Filter = "pcx files (*.pcx)|*.pcx";
                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName.Length > 0)
                {
                    pcxOrigin = new ImgPcx(openFileDialog.FileName);

                    // test Save()
                    //pcx.Save("Sample_save");

                    // 將載入的圖像放到pictureBox1
                    pictureBox1.Image = pcxOrigin.pcxImg;
                    // 顯示圖像header資訊
                    string[,] info = pcxOrigin.GetInfo();
                    for(int i = 0; i < 16; ++i)
                    {
                        dataGridView1.Rows.Add(new object[] { info[i, 0], info[i, 1] });
                    }
                    dataGridView1.Visible = true;
                    // nPlanes = 3, 無調色盤
                    if (pcxOrigin.head.nPlanes == 1)
                    {
                        // 畫出調色盤
                        var colors = pcxOrigin.pcxImg.Palette.Entries;      // 取得調色盤
                        int cntColors = colors.Count();                     // 計算顏色總數 (ex: 256)
                        int row = cntColors / 16;                           // (ex: 256 / 16 = 16)
                        int column = cntColors / 16;                        // (ex: 256 / 16 = 16)
                        Bitmap palette = new Bitmap(row * 15, column * 15); // 建立調色盤圖像
                        Graphics g = Graphics.FromImage(palette);           // 繪圖介面

                        /*
                         * @curX : 圖像x座標
                         * @curY : 圖像y座標
                         * @cnt  : 計算此列已畫好的顏色數 (用來判斷何時換行)
                         */
                        int curX = 0, curY = 0, cnt = 0;
                        for (int i = 0; i < cntColors; ++i)
                        {
                            // 挑出顏色給筆刷
                            SolidBrush brush = new SolidBrush(Color.FromArgb(colors[i].A, colors[i].R, colors[i].G, colors[i].B));
                            // 畫矩形
                            g.FillRectangle(brush, curX, curY, 15, 15);
                            ++cnt;
                            if (cnt == row) // 換行繼續畫
                            {
                                curX = 0;
                                curY += 15;
                                cnt = 0;
                            }
                            else           // 右移繼續畫
                            {
                                curX += 15;
                            }
                        }
                        // 顯示調色盤
                        pictureBox3.Image = palette;
                    }
                    // 顯示圖像長寬
                    labelDim.Text = pcxOrigin.head.width.ToString() + " x " + pcxOrigin.head.height.ToString();

                    // 開檔後, 開放使用存檔與工具
                    menuSave.Enabled = true;
                    menuTool.Enabled = true;
                    menuFilter.Enabled = true;
                    menuCoding.Enabled = true;

                    // 預先製作並保存灰階影像
                    pcxGray = new ImgPcx(pcxOrigin);
                    pcxGray.Gray();

                    // 清除圖表資料(可重複使用)
                    chart1.Series.Clear();

                    // 繪製histogram
                    Bitmap origin = (Bitmap)pictureBox1.Image;
                    Bitmap target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                    int w = target.Width;
                    int h = target.Height;
                    double[] yValue1 = new double[256];
                    double[] yValue2 = new double[256];
                    double[] yValue3 = new double[256];
                    Array.Clear(yValue1, 0, yValue1.Length);
                    Array.Clear(yValue2, 0, yValue2.Length);
                    Array.Clear(yValue3, 0, yValue3.Length);
                    for (int x = 0; x < w; ++x)
                    {
                        for (int y = 0; y < h; ++y)
                        {
                            Color c = target.GetPixel(x, y);
                            int r = c.R;
                            int g = c.G;
                            int b = c.B;
                            ++yValue1[r];
                            ++yValue2[g];
                            ++yValue3[b];
                        }
                    }
                    // 將點個數轉換為頻率
                    int total = w * h * 3;
                    for (int i = 0; i < 256; ++i)
                    {
                        yValue1[i] /= total;
                        yValue2[i] /= total;
                        yValue3[i] /= total;
                    }

                    // Range(start, cnt)
                    string[] xValue = Enumerable.Range(0, 256).ToArray().Select(x => x.ToString()).ToArray();
                    // 設定x軸區間
                    chart1.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                    chart1.Series.Add("R");
                    chart1.Series.Add("G");
                    chart1.Series.Add("B");
                    chart1.Series["R"].ChartType = SeriesChartType.Column;
                    chart1.Series["R"].Points.DataBindXY(xValue, yValue1);
                    chart1.Series["R"].Color = Color.FromArgb(150, Color.Red); // 半透明顏色
                    chart1.Series["G"].ChartType = SeriesChartType.Column;
                    chart1.Series["G"].Points.DataBindXY(xValue, yValue2);
                    chart1.Series["G"].Color = Color.FromArgb(150, Color.Green);
                    chart1.Series["B"].ChartType = SeriesChartType.Column;
                    chart1.Series["B"].Points.DataBindXY(xValue, yValue3);
                    chart1.Series["B"].Color = Color.FromArgb(150, Color.Blue);
                    chart1.Visible = true;
                }
                else
                {
                    // 開檔失敗, 無法使用存檔與工具
                    menuSave.Enabled = false;
                    menuTool.Enabled = false;
                    menuFilter.Enabled = false;
                    menuCoding.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 存檔
        private void menuSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "另存圖檔";
                if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "")
                {
                    FileStream fs = (FileStream)saveFileDialog.OpenFile();
                    Bitmap origin = (Bitmap)pictureBox1.Image;
                    Bitmap target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format8bppIndexed);
                    //target.Save(fs, ImageFormat.Bmp);

                    /*MemoryStream ms = new MemoryStream();
                    target.Save(ms, ImageFormat.Bmp);
                    byte[] bytes = ms.ToArray();*/
                    /*ImageConverter converter = new ImageConverter();
                    byte[] bytes = (byte[])converter.ConvertTo(target, typeof(byte[]));*/
                    /*ImgPcx savePcx = new ImgPcx(bytes);
                    savePcx.Save(saveFileDialog.FileName);*/
                    fs.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 結束程式
        private void menuClose_Click(object sender, EventArgs e)
        {
            if (Application.MessageLoop)
            {
                // WinForms app
                Application.Exit();
            }
            else
            {
                // Console app
                Environment.Exit(1);
            }
        }

        // override ProcessCmdKey() 處理快捷鍵
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == (Keys.Alt | Keys.F))
            {
                menuFile.ShowDropDown();
                return true;
            }
            if (keyData == (Keys.Alt | Keys.T))
            {
                if(menuTool.Enabled == true)
                {
                    menuTool.ShowDropDown();
                    return true;
                }
            }
            if (keyData == (Keys.Alt | Keys.I))
            {
                if (menuFilter.Enabled == true)
                {
                    menuFilter.ShowDropDown();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 取得滑鼠座標
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (pictureBox1.Image != null)
                {
                    int x = e.X;
                    int y = e.Y;
                    int w = pictureBox1.Image.Width;
                    int h = pictureBox1.Image.Height;

                    if (x < w && y < h)
                    {
                        Bitmap origin = (Bitmap)pictureBox1.Image;
                        Bitmap target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                        Color c = target.GetPixel(x, y);
                        int r = c.R;
                        int g = c.G;
                        int b = c.B;

                        string xy = "(x, y) = " + x.ToString() + ", " + y.ToString() + " ";
                        labelXY.Text = xy;

                        labelR.ForeColor = Color.Red;
                        labelR.Text = "R(" + r.ToString() + ") ";
                        labelG.ForeColor = Color.Green;
                        labelG.Text = "G(" + g.ToString() + ") ";
                        labelB.ForeColor = Color.Blue;
                        labelB.Text = "B(" + b.ToString() + ") ";
                        // 畫出該顏色在狀態列
                        Bitmap color = new Bitmap(15, 15);          // 建立圖像
                        Graphics gg = Graphics.FromImage(color);    // 繪圖介面
                        // 挑出顏色給筆刷
                        SolidBrush brush = new SolidBrush(Color.FromArgb(255, r, g, b));
                        // 畫矩形
                        gg.FillRectangle(brush, 0, 0, 15, 15);
                        pictureBox2.Image = color;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 開啟負片功能視窗
        private void menuNegative_Click(object sender, EventArgs e)
        {
            FormNegative f = new FormNegative();    // 建立FormNegative物件
            f.pcxOrigin = pcxOrigin;                // 傳送ImgPcx物件
            f.pcxGray = pcxGray;
            f.ShowDialog(this);                     // 設定FormNegative為FormMain的上層，並開啟FormNegative視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟灰階功能視窗
        private void menuGray_Click(object sender, EventArgs e)
        {
            FormGray f = new FormGray();    // 建立FormGray物件
            f.pcxOrigin = pcxOrigin;        // 傳送ImgPcx物件
            f.ShowDialog(this);             // 設定FormGray為FormMain的上層，並開啟FormGray視窗
                                            // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟鏡像功能視窗
        private void menuMirror_Click(object sender, EventArgs e)
        {
            FormMirror f = new FormMirror();    // 建立FormMirror物件
            f.pcxOrigin = pcxOrigin;            // 傳送ImgPcx物件
            f.ShowDialog(this);                 // 設定FormMirror為FormMain的上層，並開啟FormMirror視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟旋轉功能視窗
        private void menuRotate_Click(object sender, EventArgs e)
        {
            FormRotate f = new FormRotate();    // 建立FormRotate物件
            f.pcxOrigin = pcxOrigin;            // 傳送ImgPcx物件
            f.ShowDialog(this);                 // 設定FormRotate為FormMain的上層，並開啟FormRotate視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟顯示RGB畫面視窗
        private void menuRGB_Click(object sender, EventArgs e)
        {
            FormRGB f = new FormRGB();    // 建立FormRGB物件
            f.pcxOrigin = pcxOrigin;            // 傳送ImgPcx物件
            f.ShowDialog(this);                 // 設定FormRGB為FormMain的上層，並開啟FormRGB視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Thresholding功能視窗
        private void menuThreshold_Click(object sender, EventArgs e)
        {
            FormThreshold f = new FormThreshold();  // 建立FormThreshold物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormThreshold為FormMain的上層，並開啟FormThreshold視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟縮放功能視窗
        private void menuScaling_Click(object sender, EventArgs e)
        {
            FormScaling f = new FormScaling();  // 建立FormScaling物件
            f.pcxOrigin = pcxOrigin;            // 傳送ImgPcx物件
            f.ShowDialog(this);                 // 設定FormScaling為FormMain的上層，並開啟FormScaling視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟透明度功能視窗
        private void menuTransparency_Click(object sender, EventArgs e)
        {
            FormTransparency f = new FormTransparency();    // 建立FormTransparency物件
            f.pcxOrigin = pcxOrigin;                        // 傳送ImgPcx物件
            f.ShowDialog(this);                             // 設定FormTransparency為FormMain的上層，並開啟FormTransparency視窗
                                                            // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Bit-plane slicing功能視窗
        private void menuBitPlane_Click(object sender, EventArgs e)
        {
            FormBitPlane f = new FormBitPlane();    // 建立FormBitPlane物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormBitPlane為FormMain的上層，並開啟FormBitPlane視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Histogram Equalization功能視窗
        private void menuHistEqual_Click(object sender, EventArgs e)
        {
            FormHistEqual f = new FormHistEqual();  // 建立FormHistEqual物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormHistEqual為FormMain的上層，並開啟FormHistEqual視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Histogram Equalization功能視窗
        private void menuContrastStretch_Click(object sender, EventArgs e)
        {
            FormContrastStretch f = new FormContrastStretch();  // 建立FormContrastStretch物件
            f.pcxGray = pcxGray;                                // 傳送灰階圖像
            f.ShowDialog(this);                                 // 設定FormContrastStretch為FormMain的上層，並開啟FormContrastStretch視窗
                                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Histogram Specification功能視窗
        private void menuHistSpec_Click(object sender, EventArgs e)
        {
            FormHistSpec f = new FormHistSpec();    // 建立FormContrastStretch物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormContrastStretch為FormMain的上層，並開啟FormContrastStretch視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Outlier Filter功能視窗
        private void menuOutlier_Click(object sender, EventArgs e)
        {
            FormOutlier f = new FormOutlier();  // 建立FormOutlier物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormOutlier為FormMain的上層，並開啟FormOutlier視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Median Filter功能視窗
        private void menuMedian_Click(object sender, EventArgs e)
        {
            FormMedian f = new FormMedian();    // 建立FormMedian物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormMedian為FormMain的上層，並開啟FormMedian視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Pseudo Median Filter功能視窗
        private void menuPseudoMedian_Click(object sender, EventArgs e)
        {
            FormPseudoMedian f = new FormPseudoMedian();    // 建立FormPseudoMedian物件
            f.pcxGray = pcxGray;                            // 傳送灰階圖像
            f.ShowDialog(this);                             // 設定FormPseudoMedian為FormMain的上層，並開啟FormPseudoMedian視窗
                                                            // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Lowpass Filter功能視窗
        private void menuLowpass_Click(object sender, EventArgs e)
        {
            FormLowpass f = new FormLowpass();  // 建立FormLowpass物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormLowpass為FormMain的上層，並開啟FormLowpass視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Highpass Filter功能視窗
        private void menuHighpass_Click(object sender, EventArgs e)
        {
            FormHighpass f = new FormHighpass();    // 建立FormHighpass物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormHighpass為FormMain的上層，並開啟FormHighpass視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Highpass Filter功能視窗
        private void menuHighBoost_Click(object sender, EventArgs e)
        {
            FormHighBoost f = new FormHighBoost();  // 建立FormHighBoost物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormHighBoost為FormMain的上層，並開啟FormHighBoost視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Edge Crispening功能視窗
        private void menuEdge_Click(object sender, EventArgs e)
        {
            FormEdge f = new FormEdge();    // 建立FormEdge物件
            f.pcxGray = pcxGray;            // 傳送灰階圖像
            f.ShowDialog(this);             // 設定FormEdge為FormMain的上層，並開啟FormEdge視窗
                                            // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Roberts Filter功能視窗
        private void menuRoberts_Click(object sender, EventArgs e)
        {
            FormRoberts f = new FormRoberts();  // 建立FormRoberts物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormRoberts為FormMain的上層，並開啟FormRoberts視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Sobel Filter功能視窗
        private void menuSobel_Click(object sender, EventArgs e)
        {
            FormSobel f = new FormSobel();  // 建立FormSobel物件
            f.pcxGray = pcxGray;            // 傳送灰階圖像
            f.ShowDialog(this);             // 設定FormSobel為FormMain的上層，並開啟FormSobel視窗
                                            // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Prewitt Filter功能視窗
        private void menuPrewitt_Click(object sender, EventArgs e)
        {
            FormPrewitt f = new FormPrewitt();  // 建立FormPrewitt物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormPrewitt為FormMain的上層，並開啟FormPrewitt視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Connected Component Analysis功能視窗
        private void menuComponent_Click(object sender, EventArgs e)
        {
            FormComponent f = new FormComponent();  // 建立FormComponent物件
            f.pcxGray = pcxGray;                    // 傳送灰階圖像
            f.ShowDialog(this);                     // 設定FormComponent為FormMain的上層，並開啟FormComponent視窗
                                                    // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟Huffman編碼功能視窗
        private void menuHuffman_Click(object sender, EventArgs e)
        {
            FormHuffman f = new FormHuffman();  // 建立FormHuffman物件
            f.pcxGray = pcxGray;                // 傳送灰階圖像
            f.ShowDialog(this);                 // 設定FormHuffman為FormMain的上層，並開啟FormHuffman視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }

        // 開啟選取功能視窗
        private void menuSelect_Click(object sender, EventArgs e)
        {
            FormSelect f = new FormSelect();    // 建立FormSelect物件
            f.pcxOrigin = pcxOrigin;            // 傳送ImgPcx物件
            f.ShowDialog(this);                 // 設定FormSelect為FormMain的上層，並開啟FormSelect視窗
                                                // 由於在FormMain的程式碼內使用this，所以this為FormMain物件本身
        }
    }
}

using System;
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
            // 使用thread執行splash screen(載入畫面)
            Thread t = new Thread(new ThreadStart(StartForm));
            t.Start();
            Thread.Sleep(2000);

            InitializeComponent();
            // 載入完成後結束splash screen
            t.Abort();
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
        }

        // PCX物件
        ImgPcx pcxOrigin, pcxAfter;

        // 讀檔
        private void menuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // 清空顯示圖片與資訊
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                labelInfo.Text = "";
                labelXY.Text = "";
                chart1.Visible = false;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "開啟圖檔";
                openFileDialog.Filter = "pcx files (*.pcx)|*.pcx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pcxOrigin = new ImgPcx(openFileDialog.FileName);

                    // test Save()
                    //pcx.Save("Sample_save");

                    // 將載入的圖像放到pictureBox1
                    pictureBox1.Image = pcxOrigin.pcxImg;
                    // 顯示圖像header資訊
                    labelInfo.Text = pcxOrigin.PrintInfo();
                    // nPlanes = 3, 無調色盤
                    if (pcxOrigin.head.nPlanes == 1)
                    {
                        // 畫出調色盤
                        var colors = pcxOrigin.pcxImg.Palette.Entries;            // 取得調色盤
                        int cntColors = colors.Count();                     // 計算顏色總數 (ex: 256)
                        int row = cntColors / 8;                            // (ex: 256 / 8 = 32)
                        int column = cntColors / 32;                        // (ex: 256 / 32 = 8)
                        Bitmap palette = new Bitmap(row * 50, column * 50); // 建立調色盤圖像
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
                            g.FillRectangle(brush, curX, curY, 10, 10);
                            ++cnt;
                            if (cnt == row) // 換行繼續畫
                            {
                                curX = 0;
                                curY += 10;
                                cnt = 0;
                            }
                            else           // 右移繼續畫
                            {
                                curX += 10;
                            }
                        }
                        // 顯示調色盤
                        pictureBox3.Image = palette;
                        // 顯示圖像長寬
                        labelDim.Text = pcxOrigin.head.width.ToString() + " x " + pcxOrigin.head.height.ToString();
                    }
                    // 開檔後, 開放使用存檔與工具
                    menuSave.Enabled = true;
                    menuTool.Enabled = true;
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
                    Bitmap origin, target;
                    if (pictureBox2.Image == null)
                    {
                        origin = (Bitmap)pictureBox1.Image;
                        target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format8bppIndexed);
                        //target.Save(fs, ImageFormat.Bmp);
                    }
                    else
                    {
                        origin = (Bitmap)pictureBox2.Image;
                        target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format8bppIndexed);
                        //target.Save(fs, ImageFormat.Bmp);
                    }
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

        // 直方圖參數
        string histogramFlag = "";

        // 負片
        private void menuInvert_Click(object sender, EventArgs e)
        {
            try
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                pcxAfter.Invert();
                pictureBox2.Image = pcxAfter.pcxImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 灰階
        private void menuGray_Click(object sender, EventArgs e)
        {
            try
            {
                histogramFlag = "Gray";

                pcxAfter = new ImgPcx(pcxOrigin);
                pcxAfter.Gray();
                pictureBox2.Image = pcxAfter.pcxImg;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        // 直方圖
        private void menuHistogram_Click(object sender, EventArgs e)
        {
            try
            {
                // 清除圖表資料(可重複使用)
                chart1.Legends.Clear();
                chart1.Series.Clear();

                Bitmap origin;
                if (pictureBox2.Image == null)
                {
                    origin = (Bitmap)pictureBox1.Image;
                }
                else
                {
                    origin = (Bitmap)pictureBox2.Image;
                }
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
                        switch (histogramFlag)
                        {
                            case "Gray":
                                ++yValue1[r];
                                ++yValue1[g];
                                ++yValue1[b];
                                break;
                            default:
                                ++yValue1[r];
                                ++yValue2[g];
                                ++yValue3[b];
                                break;
                        }
                    }
                }
                // 將點個數轉換為頻率
                int size = w * h;
                for (int i = 0; i < 256; ++i)
                {
                    yValue1[i] /= size;
                    yValue2[i] /= size;
                    yValue3[i] /= size;
                }

                // Range(start, cnt)
                string[] xValue = Enumerable.Range(0, 256).ToArray().Select(x => x.ToString()).ToArray();
                //chart1.Titles.Add("直方圖");
                switch (histogramFlag)
                {
                    case "Gray":
                        // Intensities - 強度
                        chart1.Legends.Add("Intensities"); // 圖例
                        chart1.Series.Add("Intensities"); // 資料序列集合
                        chart1.Series["Intensities"].ChartType = SeriesChartType.Column; // 直方圖
                        chart1.Series["Intensities"].Points.DataBindXY(xValue, yValue1);
                        break;
                    default:
                        chart1.Legends.Add("R");
                        chart1.Legends.Add("G");
                        chart1.Legends.Add("B");
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
                        break;
                }
                chart1.Visible = true;
                // 重設標籤
                histogramFlag = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
    }
}

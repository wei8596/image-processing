using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    public partial class FormGray : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter;

        public FormGray()
        {
            InitializeComponent();
        }

        private void FormGray_Load(object sender, EventArgs e)
        {
            // 灰階
            try
            {
                pictureBox1.Image = pcxOrigin.pcxImg;
                pcxAfter = new ImgPcx(pcxOrigin);
                pcxAfter.Gray();
                pictureBox2.Image = pcxAfter.pcxImg;

                // 顯示SNR值 (到小數後2位)
                textBox1.Text = pcxOrigin.GetSNR(pcxOrigin, pcxAfter).ToString("f2");

                // 清除圖表資料
                chart1.Series.Clear();
                // 設定圖表的高度與圖片相同
                chart1.Size = new Size(chart1.Width, pictureBox1.Image.Height);

                // 繪製histogram
                Bitmap origin = (Bitmap)pictureBox2.Image;
                Bitmap target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                int w = target.Width;
                int h = target.Height;
                double[] yValue1 = new double[256];
                Array.Clear(yValue1, 0, yValue1.Length);
                for (int x = 0; x < w; ++x)
                {
                    for (int y = 0; y < h; ++y)
                    {
                        Color c = target.GetPixel(x, y);
                        int r = c.R;
                        int g = c.G;
                        int b = c.B;
                        ++yValue1[r];
                        ++yValue1[g];
                        ++yValue1[b];
                    }
                }
                // 將點個數轉換為頻率
                int total = w * h * 3;
                for (int i = 0; i < 256; ++i)
                {
                    yValue1[i] /= total;
                }

                // Range(start, cnt)
                string[] xValue = Enumerable.Range(0, 256).ToArray().Select(x => x.ToString()).ToArray();
                // 設定x軸區間
                chart1.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                // Intensities - 強度
                chart1.Series.Add("Intensities"); // 資料序列集合
                chart1.Series["Intensities"].ChartType = SeriesChartType.Column; // 直方圖
                chart1.Series["Intensities"].Points.DataBindXY(xValue, yValue1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    // 轉換成二值圖像
    public partial class FormThreshold : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormThreshold()
        {
            InitializeComponent();
        }

        private void FormThreshold_Load(object sender, EventArgs e)
        {
            // 顯示FormMain傳來ImgPcx物件的圖片 (原始圖片)
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            // 初始值為0
            pcxAfter.Threshold(0);
            pictureBox2.Image = pcxAfter.pcxImg;

            // 顯示SNR值 (到小數後2位)
            textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");

            // 清除圖表資料
            chart1.Series.Clear();
            chart1.ChartAreas["ChartArea1"].AxisX.StripLines.Clear();
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
            chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 1;
            chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
            chart1.Series["Intensities"].Points.DataBindXY(xValue, yValue1);
            chart1.Series["Intensities"].Color = Color.Blue;

            // 畫出threshold
            StripLine stripline = new StripLine();
            stripline.Interval = 0;
            stripline.IntervalOffset = 0;
            stripline.StripWidth = 1;
            stripline.BackColor = Color.Red;
            chart1.ChartAreas["ChartArea1"].AxisX.StripLines.Add(stripline);
        }

        // 移動拉桿時
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            textBox1.Text = value.ToString();
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Threshold(value);
            pictureBox2.Image = pcxAfter.pcxImg;

            // 顯示SNR值 (到小數後2位)
            textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");

            // 清除圖表資料
            chart1.Series.Clear();
            chart1.ChartAreas["ChartArea1"].AxisX.StripLines.Clear();
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
            chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 1;
            chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
            chart1.Series["Intensities"].Points.DataBindXY(xValue, yValue1);
            chart1.Series["Intensities"].Color = Color.Blue;

            // 畫出threshold
            StripLine stripline = new StripLine();
            stripline.Interval = 0;
            stripline.IntervalOffset = value;
            stripline.StripWidth = 1;
            stripline.BackColor = Color.Red;
            chart1.ChartAreas["ChartArea1"].AxisX.StripLines.Add(stripline);
        }

        // 數值變動時
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int value;
            if(int.TryParse(textBox1.Text, out value))
            {
                if(value > 255)
                {
                    textBox1.Text = "255";
                    value = 255;
                }
                else if(value < 0)
                {
                    textBox1.Text = "0";
                    value = 0;
                }

                trackBar1.Value = value;
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.Threshold(value);
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }

        // 計算Otsu's thresholding
        private void btnOtsu_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxGray);
            // Otsu's threshold值
            int threshold = pcxAfter.OtsuThreshold();
            trackBar1.Value = threshold;
            textBox1.Text = threshold.ToString();
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    public partial class FormContrastStretch : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;
        // histogram的橫軸(強度)
        // Range(start, cnt)
        string[] xValue = Enumerable.Range(0, 256).ToArray().Select(x => x.ToString()).ToArray();
        // histogram的縱軸(頻率)
        double[] yValue1 = new double[256];
        double[] yValue2 = new double[256];
        // 統計直方圖(各強度的數量)
        double[] hist = new double[256];

        public FormContrastStretch()
        {
            InitializeComponent();
        }

        private void FormContrastStretch_Load(object sender, EventArgs e)
        {
            // 原灰階圖
            pictureBox1.Image = pcxGray.pcxImg;
            Bitmap target = pcxGray.pcxImg.Clone(new Rectangle(0, 0, pcxGray.pcxImg.Width, pcxGray.pcxImg.Height), PixelFormat.Format24bppRgb);
            int w = target.Width;
            int h = target.Height;
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
            // 保存轉換成頻率前的統計結果
            Array.Copy(yValue1, hist, yValue1.Length);
            // 將點個數轉換為頻率
            int total = w * h * 3;
            for (int i = 0; i < 256; ++i)
            {
                yValue1[i] /= total;
            }

            // Contrast Stretching
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.ContrastStretch(hist, chart1, textBoxX1, textBoxY1, textBoxX2, textBoxY2);
            pictureBox2.Image = pcxAfter.pcxImg;

            // 繪製Contrast Stretching後的直方圖
            chart2.Series.Clear();
            chart2.Size = chart1.Size;
            target = pcxAfter.pcxImg.Clone(new Rectangle(0, 0, pcxAfter.pcxImg.Width, pcxAfter.pcxImg.Height), PixelFormat.Format24bppRgb);
            Array.Clear(yValue2, 0, yValue2.Length);
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    Color c = target.GetPixel(x, y);
                    int r = c.R;
                    int g = c.G;
                    int b = c.B;
                    ++yValue2[r];
                    ++yValue2[g];
                    ++yValue2[b];
                }
            }
            // 將點個數轉換為頻率
            for (int i = 0; i < 256; ++i)
            {
                yValue2[i] /= total;
            }
            // 設定x軸區間
            chart2.ChartAreas["ChartArea1"].AxisX.Interval = 32;
            // Intensities - 強度
            chart2.Series.Add("Intensities"); // 資料序列集合
            chart2.Series["Intensities"].ChartType = SeriesChartType.Column; // 直方圖
            chart2.Series["Intensities"].Points.DataBindXY(xValue, yValue2);

            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }

        // 自訂
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBoxX1.Text != "" && textBoxY1.Text != "" && textBoxX2.Text != "" && textBoxY2.Text != "")
            {
                int x1 = int.Parse(textBoxX1.Text);
                int y1 = int.Parse(textBoxY1.Text);
                int x2 = int.Parse(textBoxX2.Text);
                int y2 = int.Parse(textBoxY2.Text);
                if(x1 < 0 || x1 > 255 || y1 < 0 || y1 > 255 || x2 < 0 || x2 > 255 || y2 < 0 || y2 > 255)
                {
                    MessageBox.Show("座標超出範圍");
                }
                else
                {
                    try
                    {
                        pcxAfter = new ImgPcx(pcxGray);
                        pcxAfter.PiecewiseLinear(chart1, x1, y1, x2, y2);
                        pictureBox2.Image = pcxAfter.pcxImg;

                        // 繪製Contrast Stretching後的直方圖
                        chart2.Series.Clear();
                        Bitmap target = pcxAfter.pcxImg.Clone(new Rectangle(0, 0, pcxAfter.pcxImg.Width, pcxAfter.pcxImg.Height), PixelFormat.Format24bppRgb);
                        int w = target.Width;
                        int h = target.Height;
                        Array.Clear(yValue2, 0, yValue2.Length);
                        for (int x = 0; x < w; ++x)
                        {
                            for (int y = 0; y < h; ++y)
                            {
                                Color c = target.GetPixel(x, y);
                                int r = c.R;
                                int g = c.G;
                                int b = c.B;
                                ++yValue2[r];
                                ++yValue2[g];
                                ++yValue2[b];
                            }
                        }
                        // 將點個數轉換為頻率
                        int total = w * h * 3;
                        for (int i = 0; i < 256; ++i)
                        {
                            yValue2[i] /= total;
                        }
                        // 設定x軸區間
                        chart2.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                        // Intensities - 強度
                        chart2.Series.Add("Intensities"); // 資料序列集合
                        chart2.Series["Intensities"].ChartType = SeriesChartType.Column; // 直方圖
                        chart2.Series["Intensities"].Points.DataBindXY(xValue, yValue2);

                        // 顯示SNR值 (到小數後2位)
                        textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("請輸入座標");
            }
        }
    }
}

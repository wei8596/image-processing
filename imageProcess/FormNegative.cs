using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    public partial class FormNegative : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxGray, pcxAfter;

        public FormNegative()
        {
            InitializeComponent();
        }

        private void FormInvert_Load(object sender, EventArgs e)
        {
            try
            {
                // RGB
                pictureBox1.Image = pcxOrigin.pcxImg;
                pcxAfter = new ImgPcx(pcxOrigin);
                // 負片
                pcxAfter.Invert();
                pictureBox2.Image = pcxAfter.pcxImg;

                // 顯示SNR值 (到小數後2位)
                textBox1.Text = pcxOrigin.GetSNR(pcxOrigin, pcxAfter).ToString("f2");

                // 灰階
                pictureBox3.Image = pcxGray.pcxImg;
                pcxAfter = new ImgPcx(pcxGray);
                // 負片
                pcxAfter.Invert();
                pictureBox4.Image = pcxAfter.pcxImg;

                // 顯示SNR值 (到小數後2位)
                textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");

                // 繪製histogram
                // 設定圖表的高度與圖片相同
                chart1.Size = new Size(chart1.Width, pictureBox1.Image.Height);
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

                // 設定圖表的高度與圖片相同
                chart2.Size = new Size(chart2.Width, pictureBox2.Image.Height);
                origin = (Bitmap)pictureBox2.Image;
                target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                w = target.Width;
                h = target.Height;
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
                total = w * h * 3;
                for (int i = 0; i < 256; ++i)
                {
                    yValue1[i] /= total;
                    yValue2[i] /= total;
                    yValue3[i] /= total;
                }
                // 設定x軸區間
                chart2.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                chart2.Series.Add("R");
                chart2.Series.Add("G");
                chart2.Series.Add("B");
                chart2.Series["R"].ChartType = SeriesChartType.Column;
                chart2.Series["R"].Points.DataBindXY(xValue, yValue1);
                chart2.Series["R"].Color = Color.FromArgb(150, Color.Red); // 半透明顏色
                chart2.Series["G"].ChartType = SeriesChartType.Column;
                chart2.Series["G"].Points.DataBindXY(xValue, yValue2);
                chart2.Series["G"].Color = Color.FromArgb(150, Color.Green);
                chart2.Series["B"].ChartType = SeriesChartType.Column;
                chart2.Series["B"].Points.DataBindXY(xValue, yValue3);
                chart2.Series["B"].Color = Color.FromArgb(150, Color.Blue);

                // 設定圖表的高度與圖片相同
                chart3.Size = new Size(chart3.Width, pictureBox3.Image.Height);
                origin = (Bitmap)pictureBox3.Image;
                target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                w = target.Width;
                h = target.Height;
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
                total = w * h * 3;
                for (int i = 0; i < 256; ++i)
                {
                    yValue1[i] /= total;
                }
                // 設定x軸區間
                chart3.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                // Intensities - 強度
                chart3.Series.Add("Intensities"); // 資料序列集合
                chart3.Series["Intensities"].ChartType = SeriesChartType.Column;
                chart3.Series["Intensities"].Points.DataBindXY(xValue, yValue1);

                // 設定圖表的高度與圖片相同
                chart4.Size = new Size(chart4.Width, pictureBox4.Image.Height);
                origin = (Bitmap)pictureBox4.Image;
                target = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                w = target.Width;
                h = target.Height;
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
                total = w * h * 3;
                for (int i = 0; i < 256; ++i)
                {
                    yValue1[i] /= total;
                }
                // 設定x軸區間
                chart4.ChartAreas["ChartArea1"].AxisX.Interval = 32;
                // Intensities - 強度
                chart4.Series.Add("Intensities"); // 資料序列集合
                chart4.Series["Intensities"].ChartType = SeriesChartType.Column;
                chart4.Series["Intensities"].Points.DataBindXY(xValue, yValue1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

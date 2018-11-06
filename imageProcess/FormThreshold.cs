using System;
using System.Windows.Forms;

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
        }

        // 移動拉桿時
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            textBox1.Text = value.ToString();
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Threshold(value);
            pictureBox2.Image = pcxAfter.pcxImg;
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
        }
    }
}

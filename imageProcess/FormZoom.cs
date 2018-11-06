using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormZoom : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter;

        public FormZoom()
        {
            InitializeComponent();
        }

        private void FormZoom_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxOrigin.pcxImg;
        }

        // 檢查輸入倍率
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double ratio;
            if (double.TryParse(textBox1.Text, out ratio))
            {
                if (ratio < 1)
                {
                    textBox1.Text = "1.0";
                }
            }
        }

        // 放大(複製)
        private void button1_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.ZoomIn_Duplication(Double.Parse(textBox1.Text));
            pictureBox1.Image = pcxAfter.pcxImg;
        }

        // 放大(線性插值)
        private void button2_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.ZoomIn_Interpolation(Double.Parse(textBox1.Text));
            pictureBox1.Image = pcxAfter.pcxImg;
        }

        // 縮小(抽取)
        private void button3_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.ZoomOut_Decimation(Double.Parse(textBox1.Text));
            pictureBox1.Image = pcxAfter.pcxImg;
        }

        // 縮小(線性插值)
        private void button4_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.ZoomOut_Interpolation(Double.Parse(textBox1.Text));
            pictureBox1.Image = pcxAfter.pcxImg;
        }
    }
}

using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormRotate : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter;

        public FormRotate()
        {
            InitializeComponent();
        }

        private void FormRotate_Load(object sender, EventArgs e)
        {
            // 顯示FormMain傳來ImgPcx物件的圖片 (原始圖片)
            pictureBox1.Image = pcxOrigin.pcxImg;
            radioButton1.Checked = true;
        }

        // 旋轉角度拉條
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            textBox1.Text = value.ToString();
            if (radioButton1.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                // double.Parse() : 字串轉為double
                pcxAfter.RotateForward(value);
                pictureBox1.Image = pcxAfter.pcxImg;
            }
            else if (radioButton2.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                // double.Parse() : 字串轉為double
                pcxAfter.RotateBackward(value);
                pictureBox1.Image = pcxAfter.pcxImg;
            }
        }

        // 正旋轉
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                // double.Parse() : 字串轉為double
                pcxAfter.RotateForward(double.Parse(textBox1.Text));
                pictureBox1.Image = pcxAfter.pcxImg;
            }
        }

        // 反旋轉
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                // double.Parse() : 字串轉為double
                pcxAfter.RotateBackward(double.Parse(textBox1.Text));
                pictureBox1.Image = pcxAfter.pcxImg;
            }
        }
    }
}

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
        }

        // 正旋轉
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("請輸入旋轉角度");
            }
            else
            {
                pcxAfter = new ImgPcx(pcxOrigin);
                // double.Parse() : 字串轉為double
                pcxAfter.RotateForward(double.Parse(textBox1.Text));
                pictureBox1.Image = pcxAfter.pcxImg;
            }
        }

        // 反旋轉
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("請輸入旋轉角度");
            }
        }
    }
}

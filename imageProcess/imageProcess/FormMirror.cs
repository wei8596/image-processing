using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormMirror : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter;

        public FormMirror()
        {
            InitializeComponent();
        }

        private void FormMirror_Load(object sender, EventArgs e)
        {
            // 顯示FormMain傳來ImgPcx物件的圖片 (原始圖片)
            pictureBox1.Image = pcxOrigin.pcxImg;
        }

        // 水平翻轉
        private void button1_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.RightSideLeft();
            pictureBox2.Image = pcxAfter.pcxImg;
        }

        // 垂直翻轉
        private void button2_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.UpSideDown();
            pictureBox2.Image = pcxAfter.pcxImg;
        }

        // 左斜翻轉
        private void button3_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.LeftDiagonal();
            pictureBox2.Image = pcxAfter.pcxImg;
        }

        // 右斜翻轉
        private void button4_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxOrigin);
            pcxAfter.RightDiagonal();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormBitPlane : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxMark;

        public FormBitPlane()
        {
            InitializeComponent();
        }

        private void FormBitPlane_Load(object sender, EventArgs e)
        {
            // 顯示原始灰階圖
            pictureBox9.Image = pcxGray.pcxImg;
        }

        // Binary Code
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = pcxGray.pcxImg;         // 原始灰階圖
            pictureBox1.Image = pcxGray.BitPlane(128);  // 2^7
            pictureBox2.Image = pcxGray.BitPlane(64);
            pictureBox3.Image = pcxGray.BitPlane(32);
            pictureBox4.Image = pcxGray.BitPlane(16);
            pictureBox5.Image = pcxGray.BitPlane(8);
            pictureBox6.Image = pcxGray.BitPlane(4);
            pictureBox7.Image = pcxGray.BitPlane(2);
            pictureBox8.Image = pcxGray.BitPlane(1);    // 2^0
        }

        // Gray Code
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] gray = new byte[8];
            // 取得Gray code
            gray = pcxGray.GetGrayCode();
            pictureBox9.Image = pcxGray.pcxImg;         // 原始灰階圖
            pictureBox1.Image = pcxGray.BitPlane(gray[7]);
            pictureBox2.Image = pcxGray.BitPlane(gray[6]);
            pictureBox3.Image = pcxGray.BitPlane(gray[5]);
            pictureBox4.Image = pcxGray.BitPlane(gray[4]);
            pictureBox5.Image = pcxGray.BitPlane(gray[3]);
            pictureBox6.Image = pcxGray.BitPlane(gray[2]);
            pictureBox7.Image = pcxGray.BitPlane(gray[1]);
            pictureBox8.Image = pcxGray.BitPlane(gray[0]);
        }

        // 加入浮水印
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "開啟圖檔";
            openFileDialog.Filter = "pcx files (*.pcx)|*.pcx";
            if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName.Length > 0)
            {
                pcxMark = new ImgPcx(openFileDialog.FileName);
                // 浮水印轉灰階
                pcxMark.Gray();
                pcxGray.Watermark(pcxMark.pcxImg);
                pictureBox9.Image = pcxGray.pcxImg;
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
            }
            else
            {
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox9.Image = null;
            }
        }
    }
}

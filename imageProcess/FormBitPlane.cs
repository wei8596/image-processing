using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormBitPlane : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxMark, pcxBackup;
        bool markFlag = false;

        public FormBitPlane()
        {
            InitializeComponent();
        }

        private void FormBitPlane_Load(object sender, EventArgs e)
        {
            // 顯示原始灰階圖
            pictureBox9.Image = pcxGray.pcxImg;
            // 備份
            pcxBackup = new ImgPcx(pcxGray);
            // 隱藏SNR
            label3.Visible = false;
            textBox1.Visible = false;
        }

        // Binary Code
        private void button1_Click(object sender, EventArgs e)
        {
            if(markFlag)
            {
                // 還原
                pcxGray = new ImgPcx(pcxBackup);
                // 浮水印處理
                pcxGray.Watermark(pcxMark, "binary");
                pictureBox10.Image = pcxGray.pcxImg;
                label1.Text = "Original Image";
                label2.Text = "WaterMark Inserted";
                // 顯示SNR值 (到小數後2位)
                label3.Visible = true;
                textBox1.Visible = true;
                textBox1.Text = pcxBackup.GetSNR(pcxBackup, pcxGray).ToString("f2");
            }
            pictureBox1.Image = pcxGray.BitPlane(128, "binary");  // 2^7
            pictureBox2.Image = pcxGray.BitPlane(64, "binary");
            pictureBox3.Image = pcxGray.BitPlane(32, "binary");
            pictureBox4.Image = pcxGray.BitPlane(16, "binary");
            pictureBox5.Image = pcxGray.BitPlane(8, "binary");
            pictureBox6.Image = pcxGray.BitPlane(4, "binary");
            pictureBox7.Image = pcxGray.BitPlane(2, "binary");
            pictureBox8.Image = pcxGray.BitPlane(1, "binary");    // 2^0
        }

        // Gray Code
        private void button2_Click(object sender, EventArgs e)
        {
            if (markFlag)
            {
                // 還原
                pcxGray = new ImgPcx(pcxBackup);
                // 浮水印處理
                pcxGray.Watermark(pcxMark, "Gray");
                pictureBox10.Image = pcxGray.pcxImg;
                label1.Text = "Original Image";
                label2.Text = "WaterMark Inserted";
                // 顯示SNR值 (到小數後2位)
                label3.Visible = true;
                textBox1.Visible = true;
                textBox1.Text = pcxBackup.GetSNR(pcxBackup, pcxGray).ToString("f2");
            }
            pictureBox1.Image = pcxGray.BitPlane(128, "Gray");
            pictureBox2.Image = pcxGray.BitPlane(64, "Gray");
            pictureBox3.Image = pcxGray.BitPlane(32, "Gray");
            pictureBox4.Image = pcxGray.BitPlane(16, "Gray");
            pictureBox5.Image = pcxGray.BitPlane(8, "Gray");
            pictureBox6.Image = pcxGray.BitPlane(4, "Gray");
            pictureBox7.Image = pcxGray.BitPlane(2, "Gray");
            pictureBox8.Image = pcxGray.BitPlane(1, "Gray");
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
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                markFlag = true;
            }
            else
            {
                // 還原
                pcxGray = new ImgPcx(pcxBackup);
                markFlag = false;
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;
                pictureBox7.Image = null;
                pictureBox8.Image = null;
                pictureBox10.Image = null;
                label1.Text = "";
                label2.Text = "";
                // 隱藏SNR
                label3.Visible = false;
                textBox1.Visible = false;
            }
        }
    }
}

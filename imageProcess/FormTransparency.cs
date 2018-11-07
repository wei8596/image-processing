using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormTransparency : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter, pcxBase;

        public FormTransparency()
        {
            InitializeComponent();
        }

        private void FormTransparency_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxOrigin.pcxImg;
            // 設為初始值
            pictureBox2.Image = null;
            trackBar1.Value = 0;
            textBox1.Text = "0";
            // 未載入底圖時, 無法使用工具
            trackBar1.Enabled = false;
            textBox1.Enabled = false;
        }

        // 載入底圖
        private void button1_Click(object sender, EventArgs e)
        {
            // 設為初始值
            pictureBox2.Image = null;
            trackBar1.Value = 0;
            textBox1.Text = "0";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "開啟圖檔";
            openFileDialog.Filter = "pcx files (*.pcx)|*.pcx";
            if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName.Length > 0)
            {
                pcxBase = new ImgPcx(openFileDialog.FileName);
                pictureBox2.Image = pcxBase.pcxImg;
                // 載入底圖後, 開放使用工具
                trackBar1.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                // 載入底圖失敗, 無法使用工具
                trackBar1.Enabled = false;
                textBox1.Enabled = false;
            }
        }

        // 移動拉桿時
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            // 轉成百分比
            float percentage = (float)(value / 100.0);
            textBox1.Text = percentage.ToString();
            pcxAfter = new ImgPcx(pcxBase);
            pcxAfter.Transparency(pcxOrigin, percentage);
            pictureBox2.Image = pcxAfter.pcxImg;
        }

        // 數值變動時
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float value;
            if (float.TryParse(textBox1.Text, out value))
            {
                if (value > 1)
                {
                    textBox1.Text = "1";
                    value = 1;
                }
                else if (value < 0)
                {
                    textBox1.Text = "0";
                    value = 0;
                }

                // 百分比轉成數值, trackBar只能用整數
                trackBar1.Value = (int)(value * 100);
                pcxAfter = new ImgPcx(pcxBase);
                pcxAfter.Transparency(pcxOrigin, value);
                pictureBox2.Image = pcxAfter.pcxImg;
            }
        }
    }
}

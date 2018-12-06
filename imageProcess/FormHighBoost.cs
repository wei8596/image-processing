using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormHighBoost : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormHighBoost()
        {
            InitializeComponent();
        }

        private void FormHighBoost_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.HighBoost(1.0);
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.HighBoost((double)numericUpDown1.Value);
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }
    }
}

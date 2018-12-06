using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormEdge : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;
        string mask1 = "0\t-1\t0" + Environment.NewLine +
                       Environment.NewLine +
                       "-1\t5\t-1" + Environment.NewLine +
                       Environment.NewLine +
                       "0\t-1\t0";
        string mask2 = "-1\t-1\t-1" + Environment.NewLine +
                       Environment.NewLine +
                       "-1\t9\t-1" + Environment.NewLine +
                       Environment.NewLine +
                       "-1\t-1\t-1";
        string mask3 = "1\t-2\t1" + Environment.NewLine +
                       Environment.NewLine +
                       "-2\t5\t-2" + Environment.NewLine +
                       Environment.NewLine +
                       "1\t-2\t1";

        public FormEdge()
        {
            InitializeComponent();
        }

        private void FormEdge_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            radioButton1.Checked = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                textBox1.Text = mask1;
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.EdgeCrispening("mask1");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                textBox1.Text = mask2;
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.EdgeCrispening("mask2");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                textBox1.Text = mask3;
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.EdgeCrispening("mask3");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox2.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }
    }
}

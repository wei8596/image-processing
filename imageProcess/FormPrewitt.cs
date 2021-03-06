﻿using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormPrewitt : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormPrewitt()
        {
            InitializeComponent();
        }

        private void FormPrewitt_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            radioButton1.Checked = true;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Prewitt("vertical");
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }

        // 垂直
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.Prewitt("vertical");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }

        // 水平
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.Prewitt("horizontal");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }

        // 兩者
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                pcxAfter = new ImgPcx(pcxGray);
                pcxAfter.Prewitt("both");
                pictureBox2.Image = pcxAfter.pcxImg;
                // 顯示SNR值 (到小數後2位)
                textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
            }
        }
    }
}

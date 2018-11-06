﻿using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormLowpass : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormLowpass()
        {
            InitializeComponent();
        }

        private void FormLowpass_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Lowpass();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

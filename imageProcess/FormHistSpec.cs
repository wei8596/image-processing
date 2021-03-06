﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormHistSpec : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormHistSpec()
        {
            InitializeComponent();
        }

        private void FormHistSpec_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            // 清除圖表資料
            chart1.Series.Clear();
            // 設定圖表的高度與圖片相同
            chart1.Size = new Size(chart1.Width, pictureBox1.Image.Height);
        }
    }
}

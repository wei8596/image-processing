using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormRGB : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxR, pcxG, pcxB;

        private void FormRGB_Load(object sender, EventArgs e)
        {
            pcxR = new ImgPcx(pcxOrigin);
            pcxG = new ImgPcx(pcxOrigin);
            pcxB = new ImgPcx(pcxOrigin);
            pcxR.R_Plane();
            pcxG.G_Plane();
            pcxB.B_Plane();
            // 顯示SNR值
            textBox1.Text = pcxOrigin.GetSNR(pcxOrigin, pcxR).ToString("f2");
            textBox2.Text = pcxOrigin.GetSNR(pcxOrigin, pcxG).ToString("f2");
            textBox3.Text = pcxOrigin.GetSNR(pcxOrigin, pcxB).ToString("f2");
            pictureBox1.Image = pcxR.pcxImg;
            pictureBox2.Image = pcxG.pcxImg;
            pictureBox3.Image = pcxB.pcxImg;
        }

        public FormRGB()
        {
            InitializeComponent();
        }
    }
}

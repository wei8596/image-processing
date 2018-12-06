using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormComponent : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxBinary;

        public FormComponent()
        {
            InitializeComponent();
        }

        private void FormComponent_Load(object sender, EventArgs e)
        {
            // 先將原圖用Otsu's threshold處理成二值圖
            pcxBinary = new ImgPcx(pcxGray);
            // Otsu's threshold值
            int threshold = pcxBinary.OtsuThreshold();
            pictureBox1.Image = pcxBinary.pcxImg;

            // Connected Component Analysis
            label2.Text += pcxBinary.Component();
            pictureBox2.Image = pcxBinary.pcxImg;
        }
    }
}

using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormMedian : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormMedian()
        {
            InitializeComponent();
        }

        private void FormMedian_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Median();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormPseudoMedian : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormPseudoMedian()
        {
            InitializeComponent();
        }

        private void FormPseudoMedian_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.PseudoMedian();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

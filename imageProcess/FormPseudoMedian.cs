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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.PseudoMedian("MaxMin");
            pictureBox2.Image = pcxAfter.pcxImg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.PseudoMedian("MinMax");
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

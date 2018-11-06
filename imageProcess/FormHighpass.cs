using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormHighpass : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormHighpass()
        {
            InitializeComponent();
        }

        private void FormHighpass_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Highpass();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

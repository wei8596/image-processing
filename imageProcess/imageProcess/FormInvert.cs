using System;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormInvert : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxGray, pcxAfter;

        public FormInvert()
        {
            InitializeComponent();
        }

        private void FormInvert_Load(object sender, EventArgs e)
        {
            try
            {
                // RGB
                pictureBox1.Image = pcxOrigin.pcxImg;
                pcxAfter = new ImgPcx(pcxOrigin);
                // 負片
                pcxAfter.Invert();
                pictureBox2.Image = pcxAfter.pcxImg;

                // 灰階
                pictureBox3.Image = pcxGray.pcxImg;
                pcxAfter = new ImgPcx(pcxGray);
                // 負片
                pcxAfter.Invert();
                pictureBox4.Image = pcxAfter.pcxImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

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
        }

        // 增加椒鹽雜訊(黑白雜訊)
        private void button1_Click(object sender, EventArgs e)
        {
            pcxAfter.SaltPepper();
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }

        // Median
        private void button2_Click(object sender, EventArgs e)
        {
            pcxAfter.Median();
            pictureBox2.Image = pcxAfter.pcxImg;
            // 顯示SNR值 (到小數後2位)
            textBox1.Text = pcxGray.GetSNR(pcxGray, pcxAfter).ToString("f2");
        }
    }
}

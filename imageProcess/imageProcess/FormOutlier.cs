using System;
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
    public partial class FormOutlier : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormOutlier()
        {
            InitializeComponent();
        }

        private void FormOutlier_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            pcxAfter = new ImgPcx(pcxGray);
            pcxAfter.Outlier();
            pictureBox2.Image = pcxAfter.pcxImg;
        }
    }
}

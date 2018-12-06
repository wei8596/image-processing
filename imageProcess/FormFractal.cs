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
    public partial class FormFractal : Form
    {
        // PCX物件
        public ImgPcx pcxGray, pcxAfter;

        public FormFractal()
        {
            InitializeComponent();
        }

        private void FormFractal_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
        }
    }
}

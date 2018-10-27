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
    public partial class FormRotate : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin, pcxAfter;

        public FormRotate()
        {
            InitializeComponent();
        }

        private void FormRotate_Load(object sender, EventArgs e)
        {
            // 顯示FormMain傳來ImgPcx物件的圖片 (原始圖片)
            pictureBox1.Image = pcxOrigin.pcxImg;
        }
    }
}

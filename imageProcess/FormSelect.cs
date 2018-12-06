using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormSelect : Form
    {
        // PCX物件
        public ImgPcx pcxOrigin;
        int option = 0; // 0:矩形選取, 1:橢圓選取
        int downX, downY, upX, upY;

        public FormSelect()
        {
            InitializeComponent();
        }

        private void FormSelect_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxOrigin.pcxImg;
            radioButton1.Checked = true;
        }

        // 使用矩形選取
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                option = 0;
            }
        }

        // 使用橢圓選取
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                option = 1;
            }
        }

        // 按下滑鼠記錄座標
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Cross; // 更改滑鼠游標為"+"
            downX = e.X;
            downY = e.Y;
        }

        // 滑鼠移動選取範圍
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Rectangle rect = new Rectangle(downX, downY, e.X - downX, e.Y - downY);
        }

        // 放開滑鼠記錄座標
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default; // 回復滑鼠游標
            upX = e.X;
            upY = e.Y;
            if (downX >= pictureBox1.Image.Width || upX >= pictureBox1.Image.Width || downY >= pictureBox1.Image.Height || upY >= pictureBox1.Image.Height)
            {
                MessageBox.Show("請在圖像範圍內進行選取");
            }
            else if(option == 0) // 矩形選取
            {
                Bitmap origin = (Bitmap)pictureBox1.Image;
                // 計算位置與大小
                Point location = new Point(Math.Min(downX, upX), Math.Min(downY, upY));
                Size size = new Size(Math.Abs(upX - downX) + 1, Math.Abs(upY - downY) + 1);
                pictureBox2.Image = origin.Clone(new Rectangle(location, size), origin.PixelFormat);
            }
            else if(option == 1) // 橢圓選取
            {
                Bitmap origin = (Bitmap)pictureBox1.Image;
                // 複製原始影像並轉換格式
                Bitmap converted = origin.Clone(new Rectangle(0, 0, origin.Width, origin.Height), PixelFormat.Format24bppRgb);
                // 計算位置與大小
                Point location = new Point(Math.Min(downX, upX), Math.Min(downY, upY));
                Size size = new Size(Math.Abs(upX - downX) + 1, Math.Abs(upY - downY) + 1);

                GraphicsPath path = new GraphicsPath();
                // 以矩形範圍定義橢圓
                Rectangle rect = new Rectangle(location, size);
                path.AddEllipse(rect);

                // 建立目標影像
                Bitmap dst = new Bitmap(converted.Width, converted.Height, converted.PixelFormat);
                Graphics g = Graphics.FromImage(dst);
                // 設定裁剪區域
                g.SetClip(path);
                g.DrawImage(converted, 0, 0);

                // 設定預設顏色為透明
                dst.MakeTransparent();
                // 取得橢圓所屬的矩形範圍
                pictureBox2.Image = dst.Clone(rect, dst.PixelFormat);
                // 釋放資源
                path.Dispose();
                g.Dispose();
            }
        }
    }
}

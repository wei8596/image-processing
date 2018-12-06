using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace imageProcess
{
    public partial class FormHuffman : Form
    {
        // PCX物件
        public ImgPcx pcxGray;

        public FormHuffman()
        {
            InitializeComponent();
        }

        private void FormHuffman_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = pcxGray.pcxImg;
            dataGridView1.Height = pcxGray.pcxImg.Height;
        }

        // 進行Huffman編碼
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = pcxGray.GetBytes();
            HuffmanTree huffmanTree = new HuffmanTree();

            // 建立Huffman tree
            huffmanTree.Build(bytes);

            Dictionary<byte, int> Frequencies = huffmanTree.GetDic();
            byte[] pixel = new byte[1];
            BitArray encoded;
            string code;
            double totalBits = 0;
            foreach (KeyValuePair<byte, int> kv in Frequencies)
            {
                pixel[0] = kv.Key;
                encoded = huffmanTree.Encode(pixel);
                code = "";
                foreach (bool bit in encoded)
                {
                    code += (bit ? 1 : 0);
                }
                dataGridView1.Rows.Add(new object[] { pixel[0], code, kv.Value, code.Length});
                // 長度 * 次數
                totalBits += (code.Length * kv.Value);
            }
            // 資料壓縮比 (前 / 後) (到小數後2位)
            textBox1.Text = (bytes.Length * 8.0 / totalBits).ToString("f2");
        }
    }
}

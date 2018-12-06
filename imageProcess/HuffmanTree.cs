using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace imageProcess
{
    // Huffman tree
    public class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();    // 節點
        public Node Root { get; set; }                  // 根節點
        public Dictionary<byte, int> Frequencies = new Dictionary<byte, int>();

        // 建立Huffman tree
        public void Build(byte[] source)
        {
            // 計算頻率
            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }

                Frequencies[source[i]]++;
            }

            // 加入到節點中
            foreach (KeyValuePair<byte, int> symbol in Frequencies)
            {
                nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            // 每次依頻率排序, 取最小的兩個計算頻率和
            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = 0,
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                this.Root = nodes.FirstOrDefault();

            }

        }

        // 取得編碼
        public BitArray Encode(byte[] source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        // 取得解碼
        public byte[] Decode(BitArray bits)
        {
            Node current = this.Root;
            List<byte> decoded = new List<byte>();

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded.Add(current.Symbol);
                    current = this.Root;
                }
            }

            return decoded.ToArray();
        }

        // 判斷是否為最底層節點
        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

        // 取得各pixel排序後的個數
        public Dictionary<byte, int> GetDic()
        {
            return Frequencies.OrderBy(y => y.Value).ToDictionary(x => x.Key, y => y.Value);
        }
    }
}

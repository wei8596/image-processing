using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace imageProcess
{
    // PCX處理類別
    public class ImgPcx
    {
        const int PCX_HEADSIZE = 128;   // header大小
        const int ColorMapSize = 48;    // 16色調色盤大小
        const int FillerSize = 54;      // 填充大小

        // PCX header處理類別
        public class PcxHead
        {
            // PCX header data
            public byte[] header = new byte[PCX_HEADSIZE];

            // Flag, 10 = Zshoft .pcx
            public byte manufacturer
            {
                get { return header[0]; }
                set { header[0] = value; }
            }
            // Version information
            public byte version
            {
                get { return header[1]; }
                set { header[1] = value; }
            }
            // 1 = Run length encoding
            public byte encoding
            {
                get { return header[2]; }
                set { header[2] = value; }
            }
            // # of bits to represent a pixel(per Plane)–1, 2, 4, 8
            // 每個像素的位元數
            public byte bitsPerPixel
            {
                get { return header[3]; }
                set { header[3] = value; }
            }
            /*
             * Image Dimensions - xMin, yMin, xMax, yMax
             */
            public ushort xMin
            {
                get { return BitConverter.ToUInt16(header, 4); }
                set { SetUshort(4, value); }
            }
            public ushort yMin
            {
                get { return BitConverter.ToUInt16(header, 6); }
                set { SetUshort(6, value); }
            }
            public ushort xMax
            {
                get { return BitConverter.ToUInt16(header, 8); }
                set { SetUshort(8, value); }
            }
            public ushort yMax
            {
                get { return BitConverter.ToUInt16(header, 10); }
                set { SetUshort(10, value); }
            }
            // Horizontal Resolution
            public ushort hDpi
            {
                get { return BitConverter.ToUInt16(header, 12); }
                set { SetUshort(12, value); }
            }
            // Vertical Resolution
            public ushort vDpi
            {
                get { return BitConverter.ToUInt16(header, 14); }
                set { SetUshort(14, value); }
            }
            // 16色調色盤
            public byte[] colorMap
            {
                get
                {
                    byte[] palette = new byte[ColorMapSize];
                    // Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length)
                    Array.Copy(header, 16, palette, 0, ColorMapSize);
                    return palette;
                }
                set
                {
                    if (value.Length != ColorMapSize)
                    {
                        throw new Exception("錯誤, 長度不為48");
                    }
                    // Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length)
                    Array.Copy(value, 0, header, 16, ColorMapSize);
                }
            }
            // Should be set to 0
            public byte reserved
            {
                get { return header[64]; }
                set { header[64] = value; }
            }
            // # of color planes
            public byte nPlanes
            {
                get { return header[65]; }
                set { header[65] = value; }
            }
            // # of bytes to allocate for a scanline plane(MUST be an EVEN number)
            public ushort bytesPerLine
            {
                get { return BitConverter.ToUInt16(header, 66); }
                set { SetUshort(66, value); }
            }
            // 1 = Color.BW,  2 = Grayscale
            public ushort paletteInfo
            {
                get { return BitConverter.ToUInt16(header, 68); }
                set { SetUshort(68, value); }
            }
            // Horizontal screen size in pixels
            public ushort hScreen
            {
                get { return BitConverter.ToUInt16(header, 70); }
                set { SetUshort(70, value); }
            }
            // Vertical screen size in pixels
            public ushort vScreen
            {
                get { return BitConverter.ToUInt16(header, 72); }
                set { SetUshort(72, value); }
            }
            // Blank to fill out 128 byte header.
            public byte[] filler
            {
                get
                {
                    byte[] bytes = new byte[FillerSize];
                    // Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length)
                    Array.Copy(header, 74, bytes, 0, FillerSize);
                    return bytes;
                }
            }

            /*
             * class PcxHead constructor
             */
            public PcxHead(byte[] data)
            {
                // Array.Copy(srcArray, dstArray, length)
                Array.Copy(data, header, PCX_HEADSIZE);
            }
            public PcxHead()
            {
                header[0] = 10;     // manufacturer
                header[1] = 5;      // version
                header[2] = 1;      // encoding
                header[3] = 8;      // bitsPerPixel
                SetUshort(4, 0);    // xMin
                SetUshort(6, 0);    // yMin
                SetUshort(8, 255);  // xMax
                SetUshort(10, 255); // yMax
                SetUshort(12, 72);  // hDpi
                SetUshort(14, 72);  // vDpi

                header[64] = 0;     // reserved
                header[65] = 1;     // nPlanes
                SetUshort(66, 256); // bytesPerLine
                SetUshort(68, 1);   // paletteInfo
                SetUshort(70, 640); // hScreen
                SetUshort(72, 480); // vScreen
            }

            /*
             * 圖像寬與高
             */
            public int width
            {
                get { return (xMax - xMin + 1); }
            }
            public int height
            {
                get { return (yMax - yMin + 1); }
            }
            
            // 設定ushort(2 bytes)類型到header
            public void SetUshort(int index, ushort data)
            {
                // 從ushort取出2 bytes
                byte[] bytes = BitConverter.GetBytes(data);
                // 存到header佔2 bytes
                header[index] = bytes[0];
                header[index + 1] = bytes[1];
            }
        }


        // header
        public PcxHead head = new PcxHead();
        // 圖像
        private Bitmap img;

        // 取得與設定圖像
        public Bitmap pcxImg
        {
            get { return img; }
            set { img = value; }
        }

        /*
         * class ImgPcx constructor
         * @name : 檔案路徑
         */
        public ImgPcx(string name)
        {
            // 檢查檔案是否存在
            if (!File.Exists(name))
            {
                return;
            }
            Load(File.ReadAllBytes(name));
        }
        // @data : 檔案資料陣列
        public ImgPcx(byte[] data)
        {
            Load(data);
        }
        // @pcx : ImgPcx物件
        public ImgPcx(ImgPcx pcx)
        {
            head = pcx.head;
            pcxImg = pcx.pcxImg;
        }

        // 記錄目前讀到的位置
        private int readIndex = 0;
        private int saveIndex = 0;

        /*
         * 解碼圖像資料
         * @bytes : 圖檔資料陣列
         */
        private void Load(byte[] bytes)
        {
            // 10 = Zshoft .pcx
            if (bytes[0] != 10)
            {
                return;
            }
            head = new PcxHead(bytes);
            // 已讀完header
            readIndex = PCX_HEADSIZE;
            PixelFormat pixelFormat = PixelFormat.Format24bppRgb; // for nPlanes = 3

            // int numOfColors = (1 << (head.bitsPerPixel * head.nPlanes))

            // nPlanes = 1, bitsPerPixel = 8, 有調色盤, 調整格式
            if (head.nPlanes == 1)
            {
                switch (head.bitsPerPixel)
                {
                    case 8:
                        // 指定格式為每像素有8bits, 索引方式, 調色盤裡有256色
                        pixelFormat = PixelFormat.Format8bppIndexed;
                        break;
                }
            }
            img = new Bitmap(head.width, head.height, pixelFormat);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, pixelFormat);
            /*
             * Stride - 影像scan的寬度
             * 為資料陣列中的寬, 以byte為單位, 可能會擴展幾個bytes
             * 考慮效率, 通常會加上padding(填充)封裝成4的整數倍
             * (data.Stride : 3 * head.bytePerLine)
             */
            byte[] bmpData = new byte[data.Stride * data.Height]; // 儲存整個圖像資料

            // 讀影像資料, 每次讀一橫列
            for (int i = 0; i != head.height; ++i)
            {
                // 用來儲存一列資料
                // empty array
                byte[] data_row = new byte[0];
                // 資料解碼
                switch (head.nPlanes)
                {
                    case 1:
                        switch (head.bitsPerPixel)
                        {
                            // 調色盤1, 位元深度8
                            case 8:
                                // @bytes:整個圖的資料
                                data_row = LoadPcxLine8(bytes);
                                break;
                        }
                        break;
                    case 3: // for Sample.pcx
                         // 調色盤3, 位元深度8: 24-bit全彩
                        data_row = LoadPcxLine24(bytes);
                        break;
                }
                // 每次儲存一行資料
                // Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length)
                // Stride - 影像scan的寬度
                Array.Copy(data_row, 0, bmpData, i * data.Stride, data.Stride);
            }
            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);


            /*
            // 檢查解碼後的資料
            MemoryStream saveData = new MemoryStream();
            saveData.Write(bmpData, 0, bmpData.Length);
            saveData.WriteByte(0x0C);
            saveData.Write(new byte[768], 0, 768);
            FileStream fileStream = new FileStream("Sample_decode.pcx", FileMode.Create, FileAccess.Write);
            // 寫入header
            fileStream.Write(head.header, 0, PCX_HEADSIZE);
            byte[] fileData = saveData.ToArray();
            // 寫入資料
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
            */


            // 解除鎖定記憶體
            img.UnlockBits(data);

            // 讀調色盤資料
            switch (head.nPlanes)
            {
                // nPlanes = 1, 有調色盤
                case 1:
                    switch (head.bitsPerPixel)
                    {
                        // 調色盤1, 位元深度8: 256色
                        case 8:
                            ColorPalette palette = img.Palette;
                            // index移到調色盤位置
                            // 最後面往前退(256 * 3) = 768
                            readIndex = bytes.Length - 768;
                            for (int i = 0; i != 256; ++i)
                            {
                                palette.Entries[i] = Color.FromArgb(bytes[readIndex], bytes[readIndex + 1], bytes[readIndex + 2]);
                                readIndex += 3;
                            }
                            img.Palette = palette;
                            break;
                    }
                    break;
                // nPlanes = 3, 無調色盤
            }
        }

        /*
         * 保存PCX
         * @name    : 檔案路徑
         * 
         * @return  : 0成功, -1失敗
         */
        public int Save(string name)
        {
            if (img == null)
            {
                return -1;
            }

            // header處理
            head.xMax = (ushort)(img.Width - 1);
            head.yMax = (ushort)(img.Height - 1);
            //head.hDpi = (ushort)(head.yMax + 1);
            //head.vDpi = (ushort)(head.xMax + 1);

            // 建立可存放在記憶體的資料流
            MemoryStream saveData = new MemoryStream();

            // 從img取出圖像與調色盤資料
            switch (img.PixelFormat)
            {
                // 8
                case PixelFormat.Format8bppIndexed:
                    head.nPlanes = 1;
                    // 鎖定影像內容到記憶體
                    // 將圖的資料存到記憶體, 可以直接對它操作
                    BitmapData data = img.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, img.PixelFormat);
                    // Stride - 影像scan的寬度
                    byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
                    // 將圖像資料複製到陣列
                    // Marshal.Copy(srcPtr, dst, startIndex, length)
                    // Scan0 - 影像資料的起始位置
                    Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                    // 解除鎖定記憶體
                    img.UnlockBits(data);

                    // Run length encoding(RLE)
                    saveIndex = 0;
                    // @bytes:整個圖的資料
                    byte[] rle = SavePcxLine8(bytes);
                    // 存到記憶體
                    saveData.Write(rle, 0, rle.Length);

                    // 調色盤最前面的byte值為12
                    // ‭12 = 0x0C
                    saveData.WriteByte(0x0C);
                    // 取出調色盤資料, 並寫入記憶體
                    // 256色, 0~255
                    for (int i = 0; i < 256; ++i)
                    {
                        // PCX調色盤的顏色總是使用1 byte, 不管圖像的pixel深度
                        saveData.WriteByte((byte)img.Palette.Entries[i].R);
                        saveData.WriteByte((byte)img.Palette.Entries[i].G);
                        saveData.WriteByte((byte)img.Palette.Entries[i].B);
                    }
                    break;
                // 其他以24bits保存
                default:
                    head.nPlanes = 3;
                    // Format24bppRgb : 圖像像素以BGR順序儲存
                    Bitmap bmp24 = new Bitmap(head.width, head.height, PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(bmp24);
                    g.DrawImage(img, 0, 0, head.width, head.height);
                    // 釋放Graphics資源
                    g.Dispose();
                    // 鎖定影像內容到記憶體
                    // 將圖的資料存到記憶體, 可以直接對它操作
                    BitmapData data24 = bmp24.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, bmp24.PixelFormat);
                    // Stride - 影像scan的寬度
                    byte[] bytes24 = new byte[data24.Stride * data24.Height]; // 存放整個圖像資料
                    // 將圖像資料複製到陣列
                    // Marshal.Copy(srcPtr, dst, startIndex, length)
                    // Scan0 - 影像資料的起始位置
                    Marshal.Copy(data24.Scan0, bytes24, 0, bytes24.Length);
                    // 解除鎖定記憶體
                    bmp24.UnlockBits(data24);

                    // Run length encoding(RLE)
                    saveIndex = 0;
                    for (int i = 0; i < data24.Height; ++i)
                    {
                        saveIndex = i * data24.Stride; // 更新讀取位置
                        byte[] rle24 = SavePcxLine24(bytes24);
                        // 存到記憶體
                        saveData.Write(rle24, 0, rle24.Length);
                    }

                    // 調色盤最前面的byte值為12
                    // ‭12 = 0x0C
                    //saveData.WriteByte(0x0C);
                    // 調色盤位置設為0
                    //saveData.Write(new byte[768], 0, 768);
                    break;
            }

            // 加上副檔名.pcx
            name += ".pcx";

            FileStream fileStream = new FileStream(name, FileMode.Create, FileAccess.Write);
            // 寫入header
            fileStream.Write(head.header, 0, PCX_HEADSIZE);
            // 取出存在記憶體的資料轉成陣列
            byte[] fileData = saveData.ToArray();
            // 寫入資料
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
            return 0;
        }

        /*
         * 解讀PCX一列資料 (nPlanes = 1)
         * @data    : 圖像的一列資料陣列
         * 
         * @return  : 解碼後的資料陣列
         */
        private byte[] LoadPcxLine8(byte[] data)
        {
            // 用來存放bytesPerLine*nPlanes長度的資料並回傳 (bytesPerLine * 1)
            byte[] bytes = new byte[head.bytesPerLine];
            // 影像資料的最後一個位置
            // 最後面往前退(256 * 3) - 1 = 769
            int endBytesLength = data.Length - 769;

            // readIndex為整個圖像資料的位置
            // index為一列(bytesPerLine)資料的位置
            int index = 0;
            // 直到讀完一列資料
            while (index < head.bytesPerLine)
            {
                // 已讀完全部的影像資料, 結束
                if (readIndex > endBytesLength)
                {
                    break;
                }


                /* Run length encoding(RLE)解碼 */

                // 取出目前位置資料(Get a byte)
                byte _data = data[readIndex];
                // 0xC0 = ‭1100 0000‬
                if (_data > 0xC0) // 最高兩位元為1
                {
                    // 取出連續相同資料的個數
                    // 0x3F = ‭0011 1111
                    int cnt = _data & 0x3F;
                    // 重複儲存下一個byte共cnt次(Duplicate next byte)
                    ++readIndex; // 移到下一個byte
                    for (int i = 0; i < cnt; ++i)
                    {
                        bytes[i + index] = data[readIndex];
                    }
                    index += cnt;
                    ++readIndex;
                }
                else // 最高兩位元不為1
                {
                    // 直接儲存此位置的資料(Write this byte)
                    bytes[index] = _data;
                    ++readIndex;
                    ++index;
                }
            }

            return bytes;
        }

        /*
         * 解讀PCX一列資料 (nPlanes = 3)
         * @data    : 圖像的一列資料陣列
         * 
         * @return  : 解碼後的資料陣列
         */
        private byte[] LoadPcxLine24(byte[] data)
        {
            // 用來存放bytesPerLine*nPlanes長度的資料並回傳 (bytesPerLine * 3)
            byte[] bytes = new byte[head.bytesPerLine * 3];
            // 影像資料的最後一個位置
            int endBytesLength = data.Length - 1;

            // readIndex為整個圖像資料的位置
            // index為一列(bytesPerLine)資料的位置
            int index = 0;
            // 圖像資料陣列的位置
            // 2:存R的位置, 1:存G的位置, 0:存B的位置, -1:RGB都讀完了
            // 格式Format24bppRgb的圖像像素以BGR順序儲存
            int colorIndex = 2;
            // 回傳陣列的位置
            // 用來指定存放BGR資料的位置
            int bgrIndex;

            // 直到RGB都讀完了
            while (colorIndex != -1)
            {
                // 已讀完全部的影像資料, 結束
                if (readIndex > endBytesLength)
                {
                    break;
                }


                /* Run length encoding(RLE)解碼 */

                // 取出目前位置資料(Get a byte)
                byte _data = data[readIndex];
                // 0xC0 = ‭1100 0000‬
                if (_data > 0xC0) // 最高兩位元為1
                {
                    // 取出連續相同資料的個數
                    // 0x3F = ‭0011 1111
                    int cnt = _data & 0x3F;
                    // 重複儲存下一個byte共cnt次(Duplicate next byte)
                    ++readIndex; // 移到下一個byte
                    for (int i = 0; i < cnt; ++i)
                    {
                        if (i + index >= head.bytesPerLine) // RLE資料會換列
                        {
                            --colorIndex;
                            index = 0;
                            cnt -= i;
                            i = 0;
                        }
                        bgrIndex = ((i + index) * 3) + colorIndex;
                        bytes[bgrIndex] = data[readIndex];
                    }
                    index += cnt;
                    ++readIndex;
                }
                else // 最高兩位元不為1
                {
                    // 直接儲存此位置的資料(Write this byte)
                    bgrIndex = (index * 3) + colorIndex;
                    bytes[bgrIndex] = _data;
                    ++readIndex;
                    ++index;
                }

                if (index >= head.bytesPerLine)
                {
                    --colorIndex;
                    index = 0;
                }
            }

            return bytes;
        }

        /*
         * 儲存PCX資料 Run length encoding(RLE)
         * 將3個以上連續且相同的值做壓縮
         * (2個以下無法用此方法達到壓縮)
         * @data    : 整個圖像的資料陣列
         * 
         * @return  : RLE編碼後的資料陣列
         */
        private byte[] SavePcxLine8(byte[] data)
        {
            // 建立可存放在記憶體的資料流
            MemoryStream memory = new MemoryStream();
            // 取出第一個byte (saveIndex = 0)
            byte value = data[saveIndex];
            // 計數
            byte cnt = 1;
            // index為一列(bytesPerLine)資料的位置
            int index = 0;

            for (int i = 1; i < data.Length; ++i)
            {
                
                // debug
                if (value==0x0B && data[saveIndex + i]==0x0D && index ==281)
                {
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Hex: {0:X}", value);
                    Console.WriteLine(index);
                    Console.WriteLine("Hex: {0:X}", data[saveIndex + i]);
                    Console.WriteLine(index+1);
                    //Console.WriteLine("Hex: {0:X}", data[saveIndex + i+1]);
                    //Console.WriteLine(index + 2);
                    Console.WriteLine(head.bytesPerLine);
                    Console.WriteLine("------------------------------");
                }
                // 取得下一個byte
                byte next = data[saveIndex + i];
                if (next == value) // 下一個與目前byte相同
                {
                    ++cnt; // 計數+1
                    if ((index + cnt) > head.bytesPerLine) // 若相同資料數量超過bytesPerLine, 先不算超過的
                    {
                        --cnt; // 扣除超過的
                        // 0xC0 = ‭1100 0000‬
                        if (cnt < 2 && value < 0xC0) // 只有一筆(cnt=0,1), 直接寫入byte
                        {
                            memory.WriteByte(value);
                        }
                        else // 一筆以上或pixel值>=C0, 寫入計數與byte
                        {
                            // 計數加上0xC0後寫入
                            memory.WriteByte((byte)(0xC0 + cnt));
                            // 寫入byte
                            memory.WriteByte(value);
                        }
                        cnt = 1; // 重設計數
                        value = next; // 目前處理的值改為下一個byte的值
                        index = 0; // 重設index
                    }
                    // 若計數到0x3F, 先將此計數與byte寫入 (最高兩位是標示用途)
                    // 0x3F = ‭0011 1111‬ = 63
                    else if (cnt == 0x3F)
                    {
                        // 計數加上0xC0後寫入
                        // 0xC0 + 0x3F = 0xFF = ‭1111 1111
                        memory.WriteByte(0xFF);
                        memory.WriteByte(value);
                        index += cnt; // 移動index
                        cnt = 0; // 計數歸零 (不是設為1, 因為計數已計算到下一個byte)
                    }
                }
                else // 下一個與目前byte不同
                {
                    // 0xC0 = ‭1100 0000‬
                    if (cnt < 2 && value < 0xC0) // 只有一筆(cnt=0,1), 直接寫入byte
                    {
                        memory.WriteByte(value);
                        ++index; // 移動index
                    }
                    else // 一筆以上或pixel值>=C0, 寫入計數與byte
                    {
                        // 計數加上0xC0後寫入
                        memory.WriteByte((byte)(0xC0 + cnt));
                        // 寫入byte
                        memory.WriteByte(value);
                        index += cnt; // 移動index
                    }
                    cnt = 1; // 重設計數
                    value = next; // 目前處理的值改為下一個byte的值
                }
                // 重設index
                if (index == head.bytesPerLine)
                {
                    index = 0;
                }
            }
            // 處理最後一個byte
            if (cnt < 2 && value < 0xC0) // 只有一筆(cnt=0,1), 直接寫入byte
            {
                memory.WriteByte(value);
            }
            else // 一筆以上或pixel值>=C0, 寫入計數與byte
            {
                // 計數加上0xC0後寫入
                memory.WriteByte((byte)(0xC0 + cnt));
                // 寫入byte
                memory.WriteByte(value);
            }

            return memory.ToArray();
        }

        /*
         * 儲存PCX資料 Run length encoding(RLE)
         * 將3個以上連續且相同的值做壓縮
         * (2個以下無法用此方法達到壓縮)
         * @data    : 整個圖像的資料陣列
         * 
         * @return  : RLE編碼後的資料陣列
         */
        private byte[] SavePcxLine24(byte[] data)
        {
            // 建立可存放在記憶體的資料流
            MemoryStream r = new MemoryStream();
            MemoryStream g = new MemoryStream();
            MemoryStream b = new MemoryStream();

            // bytesPerLine (not width)
            for (int i = 0; i < head.bytesPerLine; ++i)
            {
                // 格式Format24bppRgb的圖像像素以BGR順序儲存
                r.WriteByte(data[saveIndex + 2]);
                g.WriteByte(data[saveIndex + 1]);
                b.WriteByte(data[saveIndex]);
                saveIndex += 3;
            }

            MemoryStream all = new MemoryStream();
            int storeIndex = saveIndex;
            saveIndex = 0;
            byte[] bytes = SavePcxLine8(r.ToArray());
            all.Write(bytes, 0, bytes.Length);

            saveIndex = 0;
            bytes = SavePcxLine8(g.ToArray());
            all.Write(bytes, 0, bytes.Length);

            saveIndex = 0;
            bytes = SavePcxLine8(b.ToArray());
            all.Write(bytes, 0, bytes.Length);
            saveIndex = storeIndex;

            return all.ToArray();
        }

        /*
         * 回傳header資訊
         * 
         * @return : header資訊字串陣列
         */
        public string[,] GetInfo()
        {
            string[,] info = { { "Manufacturer", head.manufacturer.ToString() },
                               { "Version", head.version.ToString() },
                               { "Encoding", head.encoding.ToString() },
                               { "BitsPerPixel", head.bitsPerPixel.ToString() },
                               { "xMin", head.xMin.ToString() },
                               { "yMin", head.yMin.ToString() },
                               { "xMax", head.xMax.ToString() },
                               { "yMax", head.yMin.ToString() },
                               { "hDpi", head.hDpi.ToString() },
                               { "vDpi", head.vDpi.ToString() },
                               { "Reserved", head.reserved.ToString() },
                               { "NPlanes", head.nPlanes.ToString() },
                               { "BytesPerLine", head.bytesPerLine.ToString() },
                               { "PaletteInfo", head.paletteInfo.ToString() },
                               { "HscreenSize", head.hScreen.ToString() },
                               { "VscreenSize", head.vScreen.ToString() } };
            return info;
        }

        /*
         * 負片
         */
        public void Invert()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            const byte max = 255;

            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = (byte)(max - bytes[i]);
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 灰階
         */
        public void Gray()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    double b = bytes[index];
                    double g = bytes[index + 1];
                    double r = bytes[index + 2];
                    byte gray = (byte)(r * 0.299 + g * 0.587 + b * 0.114);
                    bytes[index] = gray;
                    bytes[index + 1] = gray;
                    bytes[index + 2] = gray;
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 水平翻轉
         */
        public void RightSideLeft()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int mirrorX = (width - 1) - x;
                    int newIndex = 3 * (y * bytesPerLine + mirrorX); // 使用bytesPerLine
                    // B
                    mirror_bytes[newIndex] = bytes[index];
                    // G
                    mirror_bytes[newIndex + 1] = bytes[index + 1];
                    // R
                    mirror_bytes[newIndex + 2] = bytes[index + 2];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(mirror_bytes, 0, data.Scan0, mirror_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 垂直翻轉
         */
        public void UpSideDown()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int mirrorY = (height - 1) - y;
                    int newIndex = 3 * (mirrorY * bytesPerLine + x); // 使用bytesPerLine
                    // B
                    mirror_bytes[newIndex] = bytes[index];
                    // G
                    mirror_bytes[newIndex + 1] = bytes[index + 1];
                    // R
                    mirror_bytes[newIndex + 2] = bytes[index + 2];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(mirror_bytes, 0, data.Scan0, mirror_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 左斜翻轉
         */
        public void LeftDiagonal()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // 對稱線 y = x
                    if (x == y)
                    {
                        mirror_bytes[index] = bytes[index];
                        mirror_bytes[index + 1] = bytes[index + 1];
                        mirror_bytes[index + 2] = bytes[index + 2];
                    }
                    else
                    {
                        int mirrorX = y;
                        int mirrorY = x;
                        int mirrorIndex = 3 * (mirrorY * bytesPerLine + mirrorX); // 使用bytesPerLine
                        // B
                        mirror_bytes[mirrorIndex] = bytes[index];
                        // G
                        mirror_bytes[mirrorIndex + 1] = bytes[index + 1];
                        // R
                        mirror_bytes[mirrorIndex + 2] = bytes[index + 2];
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(mirror_bytes, 0, data.Scan0, mirror_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 右斜翻轉
         */
        public void RightDiagonal()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // 對稱線 y = x - (width - 1)
                    if (y == (x - (width - 1))) // 對角線上像素不變
                    {
                        // B
                        mirror_bytes[index] = bytes[index];
                        // G
                        mirror_bytes[index + 1] = bytes[index + 1];
                        // R
                        mirror_bytes[index + 2] = bytes[index + 2];
                    }
                    else
                    {
                        int mirrorX = (width - 1) - y;
                        int mirrorY = (width - 1) - x;
                        int mirrorIndex = 3 * (mirrorY * bytesPerLine + mirrorX); // 使用bytesPerLine
                        // B
                        mirror_bytes[mirrorIndex] = bytes[index];
                        // G
                        mirror_bytes[mirrorIndex + 1] = bytes[index + 1];
                        // R
                        mirror_bytes[mirrorIndex + 2] = bytes[index + 2];
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(mirror_bytes, 0, data.Scan0, mirror_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 正旋轉 (有洞)
         * @angle : 旋轉角度
         */
        public void RotateForward(double angle)
        {
            // 等同沒有旋轉, 不做變化
            if(angle == 0)
            {
                return;
            }

            double radians = angle * Math.PI / 180.0;   // 角度轉成弧度
            double sin = Math.Sin(radians);             // 用弧度計算
            double cos = Math.Cos(radians);
            double[,] rotation = new double[,] { { cos, sin }, { -sin, cos } }; // 旋轉矩陣

            int oldHeight = head.height;
            int oldWidth = head.width;
            int oldRow_center = oldHeight / 2;
            int oldCol_center = oldWidth / 2;

            // 計算新的大小
            int newHeight = (int)(oldWidth * Math.Abs(sin) + oldHeight * Math.Abs(cos));
            int newWidth = (int)(oldHeight * Math.Abs(sin) + oldWidth * Math.Abs(cos));
            int newRow_center = newHeight / 2;
            int newCol_center = newWidth / 2;

            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 建立空間較大的影像並轉換格式
            Bitmap rotate = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData rotateData = rotate.LockBits(new Rectangle(0, 0, rotate.Width, rotate.Height), ImageLockMode.ReadWrite, rotate.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] rotate_bytes = new byte[rotateData.Stride * rotateData.Height]; // 存放旋轉後的資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(rotateData.Scan0, rotate_bytes, 0, rotate_bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int index = 0;
            int newOffset = rotateData.Stride - rotate.Width * 3;
            for (int row = 0; row < oldHeight; ++row)
            {
                for (int col = 0; col < oldWidth; ++col)
                {
                    // 計算旋轉後對應的座標
                    // Round() : 四捨五入為最接近的整數
                    int newRow = (int)Math.Round(rotation[0, 0] * (row - oldRow_center) + rotation[0, 1] * (col - oldCol_center)) + newRow_center;
                    int newCol = (int)Math.Round(rotation[1, 0] * (row - oldRow_center) + rotation[1, 1] * (col - oldCol_center)) + newCol_center;
                    if (newRow < 0 || newCol < 0 || newRow > newHeight - 1 || newCol > newWidth - 1)
                    {
                        continue;
                    }
                    else
                    {
                        int newIndex = 3 * (newRow * newWidth + newCol);
                        newIndex += newRow * newOffset; // 加上前面每列的offset
                        rotate_bytes[newIndex] = bytes[index];
                        rotate_bytes[newIndex + 1] = bytes[index + 1];
                        rotate_bytes[newIndex + 2] = bytes[index + 2];
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(rotate_bytes, 0, rotateData.Scan0, rotate_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            rotate.UnlockBits(rotateData);
            img = rotate;
        }

        /*
         * 逆旋轉 (無洞)
         * @angle : 旋轉角度
         */
        public void RotateBackward(double angle)
        {
            // 要逆推, 角度先取反向
            angle = -angle;

            // 等同沒有旋轉, 不做變化
            if (angle == 0)
            {
                return;
            }

            double radians = angle * Math.PI / 180.0;   // 角度轉成弧度
            double sin = Math.Sin(radians);             // 用弧度計算
            double cos = Math.Cos(radians);
            double[,] rotation = new double[,] { { cos, sin }, { -sin, cos } }; // 旋轉矩陣

            int oldHeight = head.height;
            int oldWidth = head.width;
            int oldRow_center = oldHeight / 2;
            int oldCol_center = oldWidth / 2;

            // 計算新的大小
            int newHeight = (int)(oldWidth * Math.Abs(sin) + oldHeight * Math.Abs(cos));
            int newWidth = (int)(oldHeight * Math.Abs(sin) + oldWidth * Math.Abs(cos));
            int newRow_center = newHeight / 2;
            int newCol_center = newWidth / 2;

            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 建立空間較大的影像並轉換格式
            Bitmap rotate = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData rotateData = rotate.LockBits(new Rectangle(0, 0, rotate.Width, rotate.Height), ImageLockMode.ReadWrite, rotate.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] rotate_bytes = new byte[rotateData.Stride * rotateData.Height]; // 存放旋轉後的資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(rotateData.Scan0, rotate_bytes, 0, rotate_bytes.Length);

            int offset = rotateData.Stride - rotate.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int index = 0;
            for (int row = 0; row < newHeight; ++row)
            {
                for (int col = 0; col < newWidth; ++col)
                {
                    // 計算旋轉後對應的座標
                    // Round() : 四捨五入為最接近的整數
                    int oldRow = (int)Math.Round(rotation[0, 0] * (row - newRow_center) + rotation[0, 1] * (col - newCol_center)) + oldRow_center;
                    int oldCol = (int)Math.Round(rotation[1, 0] * (row - newRow_center) + rotation[1, 1] * (col - newCol_center)) + oldCol_center;
                    int oldIndex = 3 * (oldRow * head.bytesPerLine + oldCol);
                    if (oldRow < 0 || oldCol < 0 || oldRow > oldHeight - 1 || oldCol > oldWidth - 1)
                    {
                        rotate_bytes[index] = 0;
                        rotate_bytes[index + 1] = 0;
                        rotate_bytes[index + 2] = 0;
                    }
                    else
                    {
                        rotate_bytes[index] = bytes[oldIndex];
                        rotate_bytes[index + 1] = bytes[oldIndex + 1];
                        rotate_bytes[index + 2] = bytes[oldIndex + 2];
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(rotate_bytes, 0, rotateData.Scan0, rotate_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            rotate.UnlockBits(rotateData);
            img = rotate;
        }

        /*
         * 取得R顏色平面
         */
        public void R_Plane()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] bytesR = new byte[data.Stride * data.Height]; // 存放R資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // R
                    bytesR[index + 2] = bytes[index + 2];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytesR, 0, data.Scan0, bytesR.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 取得G顏色平面
         */
        public void G_Plane()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] bytesG = new byte[data.Stride * data.Height]; // 存放G資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // R
                    bytesG[index + 1] = bytes[index + 1];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytesG, 0, data.Scan0, bytesG.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 取得B顏色平面
         */
        public void B_Plane()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte[] bytesB = new byte[data.Stride * data.Height]; // 存放B資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // B
                    bytesB[index] = bytes[index];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytesB, 0, data.Scan0, bytesB.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * thresholding
         * @value : threshold值
         */
        public void Threshold(int value)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // 小於threshold值設為0, 其餘設為255
                    if(bytes[index] <= value)
                    {
                        bytes[index] = 0;
                        bytes[index + 1] = 0;
                        bytes[index + 2] = 0;
                    }
                    else
                    {
                        bytes[index] = 255;
                        bytes[index + 1] = 255;
                        bytes[index + 2] = 255;
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 檢查value是否為0
         * @value : 任意整數值
         */
        private Boolean NonZero(int value)
        {
            return (value != 0) ? true : false;
        }

        /*
         * Otsu's thresholding
         * 
         * @return : Otsu's threshold值
         */
        public int OtsuThreshold()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);

            /*
             * 演算法假定該圖像根據雙模直方圖(前景像素和背景像素)把包含兩類像素,
             * 於是它要計算能將兩類分開的最佳閾值,使得它們的類內變異數最小;
             * 由於兩兩平方距離恆定,所以即它們的類間變異數最大
             */

            // 統計各強度的數量
            int[] histogram = new int[256];
            Array.Clear(histogram, 0, histogram.Length); // 初始化
            for (int i = 0; i < bytes.Length; i += 3)
            {
                ++histogram[bytes[i]]; // 3通道只取1通道做統計
            }

            // 質量總和
            int sum_total = 0;
            for(int i = 1; i < 256; ++i)
            {
                sum_total += i * histogram[i];
            }

            // 計算最大類間變異數(= 最小類內變異數)
            int pixel_total = bytes.Length / 3;     // 單通道pixel總數
            int w_back = 0, w_fore = 0;             // 背景, 前景pixel數
            double mean_back = 0, mean_fore = 0;    // 背景, 前景平均值
            int sum_back = 0;                       // 背景總和
            double var_inter = 0;                   // 類間變異數
            double varMax = 0;                      // 最大類間變異數
            int threshold = 0;                      // Otsu's threshold值
            // threshold嘗試1~254
            for (int i = 1; i < 255; ++i)
            {
                w_back = w_back + histogram[i];
                w_fore = pixel_total - w_back;
                if(w_back == 0 || w_fore == 0)
                {
                    continue;
                }
                sum_back += i * histogram[i];
                mean_back = sum_back / w_back;
                mean_fore = (sum_total - sum_back) / w_fore;
                // 計算類間變異數
                var_inter = w_back * w_fore * Math.Pow(mean_back - mean_fore, 2);
                if(var_inter > varMax)
                {
                    threshold = i;
                    varMax = var_inter;
                }
            }

            // 使用Otsu's thresholding找到的threshold值執行thresholding
            Threshold(threshold);

            // 回傳threshold值供元件操作
            return threshold;
        }

        /*
         * 放大(複製, Nearest Neighbor)
         * 最簡單, 最快, 易產生明顯鋸齒狀
         * @ratio : 放大倍率
         */
        public void ZoomIn_Duplication(double ratio)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 放大後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(converted.Width * ratio + 0.5), (int)(converted.Height * ratio + 0.5), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int offset = newData.Stride - newImg.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = newImg.Height;
            int width = newImg.Width;
            int index = 0;
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int map_x = (int)(x / ratio + 0.5); // +0.5後取整等同做四捨五入
                    if(map_x >= bytesPerLine)
                    {
                        map_x = bytesPerLine - 1;
                    }
                    int map_y = (int)(y / ratio + 0.5); // +0.5後取整等同做四捨五入
                    if (map_y >= head.height)
                    {
                        map_y = head.height - 1;
                    }
                    int mapIndex = 3 * (map_y * bytesPerLine + map_x); // width使用原圖的bytesPerLine
                    newBytes[index] = bytes[mapIndex];
                    newBytes[index + 1] = bytes[mapIndex + 1];
                    newBytes[index + 2] = bytes[mapIndex + 2];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newBytes, 0, newData.Scan0, newBytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            newImg.UnlockBits(newData);
            img = newImg;
        }

        /*
         * 放大(雙線性插值, Bilinear Interpolation)
         * 不易產生鋸齒狀, 較模糊, 速度慢
         * @ratio : 放大倍率
         */
        public void ZoomIn_Interpolation(double ratio)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 放大後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(converted.Width * ratio + 0.5), (int)(converted.Height * ratio + 0.5), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int offset = newData.Stride - newImg.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = newImg.Height;
            int width = newImg.Width;
            int index = 0;
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double map_x = x / ratio;
                    if (map_x >= bytesPerLine - 1)
                    {
                        map_x = bytesPerLine - 2;
                    }
                    double map_y = y / ratio;
                    if (map_y >= head.height - 1)
                    {
                        map_y = head.height - 2;
                    }
                    // pixel值由對應的周圍四個pixel值決定
                    int i = (int)map_x;
                    int j = (int)map_y;
                    int mapIndex1 = 3 * (j * bytesPerLine + i); // width使用原圖的bytesPerLine
                    int mapIndex2 = 3 * (j * bytesPerLine + (i + 1));
                    int mapIndex3 = 3 * ((j + 1) * bytesPerLine + i);
                    int mapIndex4 = 3 * ((j + 1) * bytesPerLine + (i + 1));
                    int b = (bytes[mapIndex1] + bytes[mapIndex2] + bytes[mapIndex3] + bytes[mapIndex4]) / 4;
                    int g = (bytes[mapIndex1 + 1] + bytes[mapIndex2 + 1] + bytes[mapIndex3 + 1] + bytes[mapIndex4 + 1]) / 4;
                    int r = (bytes[mapIndex1 + 2] + bytes[mapIndex2 + 2] + bytes[mapIndex3 + 2] + bytes[mapIndex4 + 2]) / 4;
                    newBytes[index] = (byte)Math.Min(b, 255);
                    newBytes[index + 1] = (byte)Math.Min(g, 255);
                    newBytes[index + 2] = (byte)Math.Min(r, 255);
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newBytes, 0, newData.Scan0, newBytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            newImg.UnlockBits(newData);
            img = newImg;
        }

        /*
         * 縮小(抽取, Decimation)
         * @ratio : 縮小倍率
         */
        public void ZoomOut_Decimation(double ratio)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 縮小後的影像
            Bitmap newImg = new Bitmap((int)(converted.Width / ratio + 0.5), (int)(converted.Height / ratio + 0.5), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int offset = newData.Stride - newImg.Width * 3; // 掃描寬度與顯示寬度的間隙, Stride為大於等於Width的最小4的整數倍
            int height = newImg.Height;
            int width = newImg.Width;
            int index = 0;
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int map_x = (int)(x * ratio + 0.5); // +0.5後取整等同做四捨五入
                    if (map_x >= bytesPerLine)
                    {
                        map_x = bytesPerLine - 1;
                    }
                    int map_y = (int)(y * ratio + 0.5); // +0.5後取整等同做四捨五入
                    if (map_y >= head.height)
                    {
                        map_y = head.height - 1;
                    }
                    int mapIndex = 3 * (map_y * bytesPerLine + map_x); // width使用原圖的bytesPerLine
                    newBytes[index] = bytes[mapIndex];
                    newBytes[index + 1] = bytes[mapIndex + 1];
                    newBytes[index + 2] = bytes[mapIndex + 2];
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newBytes, 0, newData.Scan0, newBytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            newImg.UnlockBits(newData);
            img = newImg;
        }

        /*
         * 縮小(線性插值)
         * @ratio : 縮小倍率
         */
        public void ZoomOut_Interpolation(double ratio)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 縮小後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(converted.Width / ratio + 0.5), (int)(converted.Height / ratio + 0.5), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int offset = newData.Stride - newImg.Width * 3; // 掃描寬度與顯示寬度的間隙
            int height = newImg.Height;
            int width = newImg.Width;
            int index = 0;
            int bytesPerLine = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double map_x = x * ratio;
                    if (map_x >= bytesPerLine - 1)
                    {
                        map_x = bytesPerLine - 2;
                    }
                    double map_y = y * ratio;
                    if (map_y >= head.height - 1)
                    {
                        map_y = head.height - 2;
                    }
                    // pixel值由對應的周圍四個pixel值決定
                    int i = (int)map_x;
                    int j = (int)map_y;
                    int mapIndex1 = 3 * (j * bytesPerLine + i); // width使用原圖的bytesPerLine
                    int mapIndex2 = 3 * (j * bytesPerLine + (i + 1));
                    int mapIndex3 = 3 * ((j + 1) * bytesPerLine + i);
                    int mapIndex4 = 3 * ((j + 1) * bytesPerLine + (i + 1));
                    int b = (bytes[mapIndex1] + bytes[mapIndex2] + bytes[mapIndex3] + bytes[mapIndex4]) / 4;
                    int g = (bytes[mapIndex1 + 1] + bytes[mapIndex2 + 1] + bytes[mapIndex3 + 1] + bytes[mapIndex4 + 1]) / 4;
                    int r = (bytes[mapIndex1 + 2] + bytes[mapIndex2 + 2] + bytes[mapIndex3 + 2] + bytes[mapIndex4 + 2]) / 4;
                    newBytes[index] = (byte)Math.Min(b, 255);
                    newBytes[index + 1] = (byte)Math.Min(g, 255);
                    newBytes[index + 2] = (byte)Math.Min(r, 255);
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newBytes, 0, newData.Scan0, newBytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            newImg.UnlockBits(newData);
            img = newImg;
        }

        /*
         * 透明疊圖
         * @cover : 要疊的圖
         * @t : 要疊的圖的透明百分比
         */
        public void Transparency(ImgPcx cover, float t)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 複製要疊的圖並轉換格式
            Bitmap cover_converted = cover.pcxImg.Clone(new Rectangle(0, 0, cover.pcxImg.Width, cover.pcxImg.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData coverData = cover_converted.LockBits(new Rectangle(0, 0, cover_converted.Width, cover_converted.Height), ImageLockMode.ReadOnly, cover_converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] coverBytes = new byte[coverData.Stride * coverData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(coverData.Scan0, coverBytes, 0, coverBytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            //int height = Math.Min(converted.Height, cover.pcxImg.Height);
            //int width = Math.Min(head.bytesPerLine, cover.head.bytesPerLine);
            /*int height = converted.Height;
            int width = converted.Width;*/
            int coverWidth = cover.head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int coverIndex = 3 * (y * coverWidth + x);
                    float p = 1 - t;
                    bytes[index] = (byte)(t * coverBytes[coverIndex] + p * bytes[index]);
                    bytes[index + 1] = (byte)(t * coverBytes[coverIndex + 1] + p * bytes[index + 1]);
                    bytes[index + 2] = (byte)(t * coverBytes[coverIndex + 2] + p * bytes[index + 2]);
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            cover_converted.UnlockBits(coverData);
            img = converted;
        }

        /*
         * 字串反轉
         * @s : 字串
         * 
         * @return : 反向後的字串
         */
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /*
         * Bit string to byte
         * @bits : bit string
         * 
         * @return : 轉換後的數值
         */
        public byte BitStringToByte(string bits)
        {
            char[] reverseBits = Reverse(bits).ToCharArray();
            byte num = 0;
            for(int power = 0; power < reverseBits.Length; ++power)
            {
                char currentBit = reverseBits[power];
                if(currentBit == '1')
                {
                    byte currentNum = (byte)(Math.Pow(2, power));
                    num += currentNum;
                }
            }

            return num;
        }

        /*
         * 轉換成Gray code
         * 
         * @return : Gray code
         */
        public byte GrayCode(byte n)
        {
            return (byte)(n ^ (n >> 1));
        }

        /*
         * 反轉換Gray code
         * 
         * @return : binary code
         */
        public byte InverseGrayCode(byte n)
        {
            byte inv = 0;
            while(n != 0)
            {
                inv ^= n;
                n = (byte)(n >> 1);
            }
            return inv;
        }

        /*
         * Bit-plane slicing
         * @n : 指定不同的Bit-plane (2^7 ~ 2^0)
         * @option : 指定使用binary或Gray code
         * 
         * @return : Bit-plane圖像
         */
        public Bitmap BitPlane(byte n, string option)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            byte max = 255;
            byte min = 0;
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    if(option == "binary")
                    {
                        if ((n & bytes[index]) == n)
                        {
                            bytes[index] = max;
                            bytes[index + 1] = max;
                            bytes[index + 2] = max;
                        }
                        else
                        {
                            bytes[index] = min;
                            bytes[index + 1] = min;
                            bytes[index + 2] = min;
                        }
                    }
                    else if(option == "Gray")
                    {
                        if ((n & GrayCode(bytes[index])) == n)
                        {
                            bytes[index] = max;
                            bytes[index + 1] = max;
                            bytes[index + 2] = max;
                        }
                        else
                        {
                            bytes[index] = min;
                            bytes[index + 1] = min;
                            bytes[index + 2] = min;
                        }
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);

            return converted;
        }

        /*
         * 增加浮水印
         * @mark : 浮水印圖片
         * @option : 指定使用binary或Gray code
         */
        public void Watermark(ImgPcx mark, string option)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 複製浮水印影像並轉換格式
            Bitmap mark_converted = mark.pcxImg.Clone(new Rectangle(0, 0, mark.pcxImg.Width, mark.pcxImg.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData markData = mark_converted.LockBits(new Rectangle(0, 0, mark_converted.Width, mark_converted.Height), ImageLockMode.ReadWrite, mark_converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] markBytes = new byte[markData.Stride * markData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(markData.Scan0, markBytes, 0, markBytes.Length);

            // 將浮水印的前4個bits加入原圖的後面4個bits
            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int index = 0;
            int height = Math.Min(head.height, mark.head.height);
            int width = Math.Min(head.bytesPerLine, mark.head.bytesPerLine);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if(option == "binary")
                    {
                        // F0 = 1111 0000
                        bytes[index] &= 0xF0;
                        bytes[index + 1] &= 0xF0;
                        bytes[index + 2] &= 0xF0;
                        // 取浮水印的前4個bits
                        bytes[index] += (byte)(markBytes[index] >> 4);
                        bytes[index + 1] += (byte)(markBytes[index + 1] >> 4);
                        bytes[index + 2] += (byte)(markBytes[index + 2] >> 4);
                    }
                    else if(option == "Gray")
                    {
                        // F0 = 1111 0000
                        byte tmp = (byte)(GrayCode(bytes[index]) & 0xF0);
                        // 取浮水印的前4個bits
                        tmp += (byte)(GrayCode(markBytes[index]) >> 4);
                        bytes[index] = InverseGrayCode(tmp);
                        tmp = (byte)(GrayCode(bytes[index + 1]) & 0xF0);
                        tmp += (byte)(GrayCode(markBytes[index + 1]) >> 4);
                        bytes[index + 1] = InverseGrayCode(tmp);
                        tmp = (byte)(GrayCode(bytes[index + 2]) & 0xF0);
                        tmp += (byte)(GrayCode(markBytes[index + 2]) >> 4);
                        bytes[index + 2] = InverseGrayCode(tmp);
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            mark_converted.UnlockBits(markData);
            img = converted;
        }

        /*
         * Contrast Stretching
         * 對比度拉伸, 繪製函數圖表, 設定文字方塊內容
         * @n : 統計直方圖
         * @chart : 函數圖表
         * @box1 : 文字方塊
         */
        public void ContrastStretch(double[] n, Chart chart, TextBox box1, TextBox box2, TextBox box3, TextBox box4)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 找原圖最大最小值
            double min = 0, max = 255;
            for (int i = 0; i < 256; ++i)
            {
                if (n[i] != 0)
                {
                    min = i;
                    break;
                }
            }
            for (int i = 255; i >= 0; --i)
            {
                if (n[i] != 0)
                {
                    max = i;
                    break;
                }
            }

            double scale = 255.0 / (max - min);
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte pixel = (byte)(Math.Round(bytes[i] - min) * scale);
                if (pixel < 0)
                {
                    bytes[i] = 0;
                }
                else if (pixel > 255)
                {
                    bytes[i] = 255;
                }
                else
                {
                    bytes[i] = pixel;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;

            // 繪製函數圖表
            chart.Series.Clear(); // 清除圖表資料
            chart.Size = new Size(chart.Width, converted.Height); // 設定圖表的高度與圖片相同
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "";
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "";
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = 255;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 255;
            chart.Series.Add("Piecewise");
            chart.Series["Piecewise"].ChartType = SeriesChartType.Line;
            chart.Series["Piecewise"].Color = Color.Red;
            int y;
            bool first = true;
            int x1 = 0, x2 = 0;
            for(int x = 0; x < 256; ++x)
            {
                y = (int)(Math.Round(x - min) * scale);
                if(y < 0)
                {
                    y = 0;
                    x1 = x;
                }
                else if(y > 255)
                {
                    y = 255;
                    if(first)
                    {
                        x2 = x;
                        first = false;
                    }
                }
                chart.Series["Piecewise"].Points.AddXY(x, y);
            }
            box1.Text = x1.ToString();
            box2.Text = "0";
            box3.Text = x2.ToString();
            box4.Text = "255";
        }

        /*
         * Piecewise Linear (對比度做分段線性拉伸)
         * @chart : 函數圖表
         * @x, y : xy座標
         */
        public void PiecewiseLinear(Chart chart, int x1, int y1, int x2, int y2)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            for (int i = 0; i < bytes.Length; ++i)
            {
                if(bytes[i] < x1)       // 0 <= bytes[i] < x1
                {
                    bytes[i] = (byte)(bytes[i] * y1 / x1);
                }
                else if(bytes[i] < x2)  // x1 <= bytes[i] < x2
                {
                    bytes[i] = (byte)(((y2 - y1) / (x2 - x1)) * (bytes[i] - x1) + y1);
                }
                else                    // x2 <= bytes[i] <= 255
                {
                    bytes[i] = (byte)(((255 - y2) / (255 - x2)) * (bytes[i] - x2) + y2);
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;

            // 繪製函數圖表
            chart.Series.Clear(); // 清除圖表資料
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "";
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "";
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = 255;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 255;
            chart.Series.Add("Piecewise");
            chart.Series["Piecewise"].ChartType = SeriesChartType.Line;
            chart.Series["Piecewise"].Color = Color.Red;
            int y = 0;
            for (int x = 0; x < 256; ++x)
            {
                if (x < x1)         // 0 <= x < x1
                {
                    y = (byte)(x * y1 / x1);
                }
                else if (x < x2)    // x1 <= x < x2
                {
                    y = (byte)(((y2 - y1) / (x2 - x1)) * (x - x1) + y1);
                }
                else                // x2 <= x <= 255
                {
                    y = (byte)(((255 - y2) / (255 - x2)) * (x - x2) + y2);
                }
                chart.Series["Piecewise"].Points.AddXY(x, y);
            }
        }

        /*
         * Histogram Equalization
         * @n : 統計直方圖
         */
        public void HistEqual(double[] n)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // http://bouwhat.pixnet.net/blog/post/22814961-%E7%9B%B4%E6%96%B9%E5%9C%96%E5%9D%87%E5%8C%96%28histogram-equalization%29
            // Sk = T(Rk) = (L - 1) * SUM(P(Rj))(0~k) = (L - 1) * SUM(Nj)(0~k) / total
            for (int i = 0; i < bytes.Length; ++i)
            {
                double sum = 0;
                for(int j = 0; j < bytes[i]; ++j)
                {
                    sum += n[j];
                }
                bytes[i] = (byte)(255 * sum / bytes.Length);
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * 取得訊號雜訊比(SNR)
         * @pcxOrigin : 原始影像物件
         * @pcxAfter : 處理後影像物件
         * 
         * @return : SNR值
         */
        public double GetSNR(ImgPcx pcxOrigin, ImgPcx pcxAfter)
        {
            // 複製原始影像並轉換格式
            Bitmap converted = pcxOrigin.pcxImg.Clone(new Rectangle(0, 0, pcxOrigin.pcxImg.Width, pcxOrigin.pcxImg.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 複製處理後影像並轉換格式
            Bitmap after_converted = pcxAfter.pcxImg.Clone(new Rectangle(0, 0, pcxAfter.pcxImg.Width, pcxAfter.pcxImg.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData afterData = after_converted.LockBits(new Rectangle(0, 0, after_converted.Width, after_converted.Height), ImageLockMode.ReadWrite, after_converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] afterBytes = new byte[afterData.Stride * afterData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(afterData.Scan0, afterBytes, 0, afterBytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int index = 0;
            int height = converted.Height;
            int width = converted.Width;
            double b1, g1, r1, b2, g2, r2;
            double signal = 0, noise = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    b1 = bytes[index];
                    g1 = bytes[index + 1];
                    r1 = bytes[index + 2];
                    b2 = afterBytes[index];
                    g2 = afterBytes[index + 1];
                    r2 = afterBytes[index + 2];
                    signal += b1 * b1 + g1 * g1 + r1 * r1;
                    noise += (b1 - b2) * (b1 - b2) +
                             (g1 - g2) * (g1 - g2) +
                             (r1 - r2) * (r1 - r2);
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            return 10 * Math.Log10(signal / noise); // SNR
        }

        /*
         * Outlier Filter
         */
        public void Outlier()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if(x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if(y == 0)
                    {
                        mapY = 0;
                    }
                    else if(y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Outlier
            byte[] pixel = new byte[8];
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[5] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    double sum = 0;
                    for(int i = 0; i < 8; ++i)
                    {
                        sum += pixel[i];
                    }
                    double avg = sum / 8.0;
                    int index = 3 * (y * width + x);
                    byte threshold = 10; // 設多少?????????????????
                    if (extend_bytes[index] - avg > threshold)
                    {
                        bytes[newIndex] = (byte)avg;
                        bytes[newIndex + 1] = (byte)avg;
                        bytes[newIndex + 2] = (byte)avg;
                    }
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Median Filter
         */
        public void Median()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Median
            byte[] pixel = new byte[9];
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    Array.Sort(pixel);
                    byte median = pixel[4];
                    bytes[newIndex] = median;
                    bytes[newIndex + 1] = median;
                    bytes[newIndex + 2] = median;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * find min of 3 numbers
         * 
         * @return : min
         */
        public byte MyMin3(byte a, byte b, byte c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        /*
         * find max of 3 numbers
         * 
         * @return : max
         */
        public byte MyMax3(byte a, byte b, byte c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        /*
         * Pseudo Median (MaxMin)
         * MaxXMin always results in the median of the sequence
         * or a value smaller than the median
         * 
         * @return : MaxMin
         */
        public byte MaxMin(byte[] pixel)
        {
            int length = pixel.Length;
            byte[] min = new byte[84];
            int index = 0;
            byte max = 0;
            for(int i = 0; i < length - 2; ++i)
            {
                for (int j = i + 1; j < length - 1; ++j)
                {
                    for (int k = j + 1; k < length; ++k)
                    {
                        min[index] = MyMin3(pixel[i], pixel[j], pixel[k]);
                        if(min[index] > max)
                        {
                            max = min[index];
                        }
                        ++index;
                    }
                }
            }

            return max;
        }

        /*
         * Pseudo Median (MinMax)
         * MinMax always results in the median of the sequence
         * or a value larger than the median
         * 
         * @return : MinMax
         */
        public byte MinMax(byte[] pixel)
        {
            int length = pixel.Length;
            byte[] max = new byte[84];
            int index = 0;
            byte min = 255;
            for (int i = 0; i < length - 2; ++i)
            {
                for (int j = i + 1; j < length - 1; ++j)
                {
                    for (int k = j + 1; k < length; ++k)
                    {
                        max[index] = MyMax3(pixel[i], pixel[j], pixel[k]);
                        if (max[index] < min)
                        {
                            min = max[index];
                        }
                        ++index;
                    }
                }
            }

            return min;
        }

        // filter形狀需改成十字...............................
        /*
         * Pseudo Median Filter
         */
        public void PseudoMedian()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Pseudo Median
            byte[] pixel = new byte[9];
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    byte pseudoMedian = (byte)((MaxMin(pixel) + MinMax(pixel)) / 2.0);
                    bytes[newIndex] = pseudoMedian;
                    bytes[newIndex + 1] = pseudoMedian;
                    bytes[newIndex + 2] = pseudoMedian;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Lowpass Filter
         */
        public void Lowpass()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Lowpass
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    double sum = 0;
                    for(int i = 0; i < 9; ++i)
                    {
                        sum += pixel[i];
                    }
                    byte value = (byte)(sum / 9);
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Highpass Filter
         */
        public void Highpass()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Highpass
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    double sum = 0;
                    for (int i = 0; i < 9; ++i)
                    {
                        if(i != 4)
                        {
                            sum += pixel[i] * -1.0;
                        }
                        else
                        {
                            sum += pixel[i] * 8.0;
                        }
                    }
                    byte value = (byte)(sum / 9);
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * High-Boost Filter
         * High boost = (factor-1)(Original) + Highpass
         * http://ip.csie.ncu.edu.tw/course/IP/IP1406cp.pdf
         * @factor : amplification factor (放大倍率)
         */
        public void HighBoost(double factor)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // High-Boost
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    double sum = 0;
                    for (int i = 0; i < 9; ++i)
                    {
                        if (i != 4)
                        {
                            sum += pixel[i] * -1.0;
                        }
                        else
                        {
                            sum += pixel[i] * (9 * factor - 1);
                        }
                    }
                    byte value = (byte)(sum / 9);
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Edge Crispening
         * Sharpened = Original - Blurred
         * @mask : 指定不同的mask
         */
        public void EdgeCrispening(string mask)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Edge Crispening
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    double sum = 0;
                    if(mask == "mask1")
                    {
                        for (int i = 1; i < 9; i += 2)
                        {
                            sum += pixel[i] * -1.0;
                        }
                        sum += pixel[4] * 5.0;
                    }
                    else if (mask == "mask2")
                    {
                        for (int i = 0; i < 9; ++i)
                        {
                            if(i != 4)
                            {
                                sum += pixel[i] * -1.0;
                            }
                            else
                            {
                                sum += pixel[i] * 9.0;
                            }
                        }
                    }
                    else if (mask == "mask3")
                    {
                        for(int i = 0; i < 9; i += 2)
                        {
                            if(i != 4)
                            {
                                sum += pixel[i];
                            }
                            else
                            {
                                sum += pixel[i] * 5.0;
                            }
                        }
                        for (int i = 1; i < 9; i += 2)
                        {
                            sum += pixel[i] * -2.0;
                        }
                    }
                    byte value = (byte)(sum / 9);
                    // Sharpened = Original - Blurred
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Roberts Filter
         * @option : 選擇垂直, 水平或兩者
         */
        public void Roberts(string option)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 右與下增加一排, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 1) * (head.height + 1)];
            int height = head.height + 1;
            int width = head.bytesPerLine + 1;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x;
                    mapY = y;
                    if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Roberts Operator
            byte[] pixel = new byte[4]; // mask size = 2 x 2 = 4
            int newIndex = 0;
            for (int y = 0; y < height - 1; ++y)
            {
                for (int x = 0; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * (y * width + x)];
                    pixel[1] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[2] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[3] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    // 梯度近似(相減取絕對值)
                    byte value = 0;
                    if(option == "vertical")
                    {
                        value = (byte)Math.Abs(pixel[3] - pixel[0]);
                    }
                    else if(option == "horizontal")
                    {
                        value = (byte)Math.Abs(pixel[2] - pixel[1]);
                    }
                    else if(option == "both")
                    {
                        value = (byte)(Math.Abs(pixel[3] - pixel[0]) + Math.Abs(pixel[2] - pixel[1]));
                    }
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Sobel Filter
         * @option : 選擇垂直, 水平或兩者
         */
        public void Sobel(string option)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Sobel
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    // 梯度近似(相減取絕對值)
                    byte value = 0;
                    if (option == "vertical")
                    {
                        value = (byte)Math.Abs((pixel[2] + 2 * pixel[5] + pixel[8]) - (pixel[0] + 2 * pixel[3] + pixel[6]));
                    }
                    else if (option == "horizontal")
                    {
                        value = (byte)Math.Abs((pixel[6] + 2 * pixel[7] + pixel[8]) - (pixel[0] + 2 * pixel[1] + pixel[2]));
                    }
                    else if (option == "both")
                    {
                        value = (byte)(Math.Abs((pixel[6] + 2 * pixel[7] + pixel[8]) - (pixel[0] + 2 * pixel[1] + pixel[2])) +
                                       Math.Abs((pixel[2] + 2 * pixel[5] + pixel[8]) - (pixel[0] + 2 * pixel[3] + pixel[6])));
                    }
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        /*
         * Prewitt Filter
         * @option : 選擇垂直, 水平或兩者
         */
        public void Prewitt(string option)
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 周圍增加一圈, 利用周圍的值填補
            byte[] extend_bytes = new byte[3 * (head.bytesPerLine + 2) * (head.height + 2)];
            int height = head.height + 2;
            int width = head.bytesPerLine + 2;
            int mapHeight = head.height;
            int mapWidth = head.bytesPerLine;
            int mapX = 0, mapY = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    mapX = x - 1;
                    mapY = y - 1;
                    if (x == 0)
                    {
                        mapX = 0;
                    }
                    else if (x == width - 1)
                    {
                        mapX = mapWidth - 1;
                    }
                    if (y == 0)
                    {
                        mapY = 0;
                    }
                    else if (y == height - 1)
                    {
                        mapY = mapHeight - 1;
                    }
                    int mapIndex = 3 * (mapY * mapWidth + mapX);
                    extend_bytes[index] = bytes[mapIndex];
                    extend_bytes[index + 1] = bytes[mapIndex + 1];
                    extend_bytes[index + 2] = bytes[mapIndex + 2];
                }
            }
            // Prewitt
            byte[] pixel = new byte[9]; // mask size = 3 x 3 = 9
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    pixel[0] = extend_bytes[3 * ((y - 1) * width + x - 1)];
                    pixel[1] = extend_bytes[3 * ((y - 1) * width + x)];
                    pixel[2] = extend_bytes[3 * ((y - 1) * width + x + 1)];
                    pixel[3] = extend_bytes[3 * (y * width + x - 1)];
                    pixel[4] = extend_bytes[3 * (y * width + x)];
                    pixel[5] = extend_bytes[3 * (y * width + x + 1)];
                    pixel[6] = extend_bytes[3 * ((y + 1) * width + x - 1)];
                    pixel[7] = extend_bytes[3 * ((y + 1) * width + x)];
                    pixel[8] = extend_bytes[3 * ((y + 1) * width + x + 1)];
                    // 梯度近似(相減取絕對值)
                    byte value = 0;
                    if (option == "vertical")
                    {
                        value = (byte)Math.Abs((pixel[2] + pixel[5] + pixel[8]) - (pixel[0] + pixel[3] + pixel[6]));
                    }
                    else if (option == "horizontal")
                    {
                        value = (byte)Math.Abs((pixel[6] + pixel[7] + pixel[8]) - (pixel[0] + pixel[1] + pixel[2]));
                    }
                    else if (option == "both")
                    {
                        value = (byte)(Math.Abs((pixel[6] + pixel[7] + pixel[8]) - (pixel[0] + pixel[1] + pixel[2])) +
                                       Math.Abs((pixel[2] + pixel[5] + pixel[8]) - (pixel[0] + pixel[3] + pixel[6])));
                    }
                    bytes[newIndex] = value;
                    bytes[newIndex + 1] = value;
                    bytes[newIndex + 2] = value;
                    newIndex += 3;
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }

        // 產生隨機不同顏色
        public Color[] GetUniqueRandomColor(int cnt)
        {
            Color[] colors = new Color[cnt];
            HashSet<Color> hs = new HashSet<Color>();
            Random randomColor = new Random();
            for (int i = 0; i < cnt; ++i)
            {
                Color c;
                while (!hs.Add(c = Color.FromArgb(randomColor.Next(255), randomColor.Next(255), randomColor.Next(255)))) ;
                colors[i] = c;
            }

            return colors;
        }

        /*
         * Connected Component Analysis
         * (8鄰域連接, 8-connected neighborhood, 左, 左上, 上, 右上)
         * (兩次掃描法, two-pass algorithm)
         * 
         * @return : 物件數量
         */
        public int Component()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // Connected Component Analysis
            int height = converted.Height;
            int width = converted.Width;
            int total = height * width;
            int[] neighbors = new int[4];
            List<List<int>> linked = new List<List<int>>(); // 儲存相等的標號
            linked.Add(new List<int>());                    // 建立第一個標號的位置(不儲存其他標號, 代表0)
            List<int> neighborsLabels = new List<int>(4);   // 儲存鄰居的標號
            int[] labels = new int[total];                  // 儲存整個圖的標號
            Array.Clear(labels, 0, labels.Length);
            bool neighborEmpty = true;
            int nextLabel = 1;
            int minLabel;
            int labelIndex = 0;
            // First pass
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    labelIndex = y * width + x;
                    if (bytes[3 * (y * width + x)] != 0) // 若pixel值不為0(為255)
                    {
                        // 處理邊緣情況
                        if(x == 0 || y == 0)
                        {
                            neighbors[0] = 0;
                        }
                        else
                        {
                            neighbors[0] = labels[(y - 1) * width + x - 1];
                        }
                        if(y == 0)
                        {
                            neighbors[1] = 0;
                        }
                        else
                        {
                            neighbors[1] = labels[(y - 1) * width + x];
                        }
                        if(x == width - 1 || y == 0)
                        {
                            neighbors[2] = 0;
                        }
                        else
                        {
                            neighbors[2] = labels[(y - 1) * width + x + 1];
                        }
                        if (x == 0)
                        {
                            neighbors[3] = 0;
                        }
                        else
                        {
                            neighbors[3] = labels[y * width + x - 1];
                        }
                        neighborEmpty = true;
                        for (int i = 0; i < 4; ++i)
                        {
                            if(neighbors[i] != 0)
                            {
                                neighborEmpty = false;
                            }
                        }
                        if(neighborEmpty) // 鄰居都是0
                        {
                            // 新增相等標號
                            linked.Add(new List<int>());
                            linked[nextLabel].Add(nextLabel);
                            // 目標像素設新標號
                            labels[labelIndex] = nextLabel;
                            ++nextLabel;
                        }
                        else // 鄰居有非0值, 找最小label值
                        {
                            neighborsLabels.Clear();
                            // 取得鄰居最小label
                            minLabel = Int32.MaxValue;
                            for (int i = 0; i < 4; ++i)
                            {
                                if(neighbors[i] == 0)
                                {
                                    continue;
                                }
                                neighborsLabels.Add(neighbors[i]);
                                if (neighbors[i] < minLabel)
                                {
                                    minLabel = neighbors[i];
                                }
                            }
                            // 設為鄰居中最小的label
                            labels[labelIndex] = minLabel;
                            // 更新相等標號
                            for (int i = 0; i < 4; ++i)
                            {
                                if (neighbors[i] == 0)
                                {
                                    continue;
                                }
                                foreach(int l in neighborsLabels)
                                {
                                    linked[neighbors[i]] = linked[neighbors[i]].Union(linked[l]).ToList();
                                }
                            }
                        }
                    }
                    else // 背景像素設為0
                    {
                        labels[labelIndex] = 0;
                    }
                }
            }

            // Second pass
            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int index = 0;
            Color[] colors = GetUniqueRandomColor(500);
            List<int> list = new List<int>(); // 用來計算物件的數量
            int value;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    labelIndex = y * width + x;
                    if (bytes[3 * (y * width + x)] != 0) // 物件
                    {
                        // 更新label
                        labels[labelIndex] = linked[labels[labelIndex]].Min();

                        // 統計物件數
                        list.Add(labels[labelIndex]);
                    }
                    // 不同物件顏色不同
                    value = colors[labels[labelIndex]].ToArgb();
                    bytes[index] = (byte)(value & 0x000000FF);
                    bytes[index + 1] = (byte)((value >> 8) & 0x000000FF);
                    bytes[index + 2] = (byte)((value >> 16) & 0x000000FF);
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }
            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;

            return list.Distinct().Count(); // 物件數量
        }

        /*
         * 取得圖像資料陣列
         * 
         * @return : 資料陣列
         */
         public byte[] GetBytes()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);

            return bytes;
        }

        /*
         * 透過高斯分布(常態分布)增加椒鹽雜訊(黑白雜訊)
         */
        public void SaltPepper()
        {
            /*
             * 索引類型的圖片，其header有一個顏色表，這個表按照一定的規律存儲了所有的可能在這張圖片中出現的顏色。
             * 它的每一個點的像素值(ARGB)並不是直接存儲的。在存儲具體點的數據的地方之只是存儲其在顏色表中的索引，
             * 在進行的解碼的時候，讀取索引然後在顏色表中查找，找到對應所以的顏色值之後將其顯示出來作為這個點的顏色值。
             * System.Drawing.Image不支援通過索引的方式存儲圖片資料的圖片實現SetPixel()
             * 
             * 可以通過Bitmap.Clone()來將索引圖片的像素資料複製到新建的圖片上，
             * 並且在函數中指定新圖片的pixelFormat(不設置成索引類型)
             * 接下來就對新建的圖片進行操作
             */
            // 複製原始影像並轉換格式
            Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int offset = data.Stride - converted.Width * 3; // 掃描寬度與顯示寬度的間隙
            int height = converted.Height;
            int width = converted.Width;
            int index = 0;
            double mean = 0;    // 期望值
            double std = 1;     // 標準差
            double u, v;        // 均勻分布數值(0~1間的亂數)
            double X;           // 常態分佈數值(結果)
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    u = new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                    v = new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                    X = Math.Sqrt(-2.0 * Math.Log(u)) * Math.Cos(2.0 * Math.PI * v) * std + mean;
                    if(X > 2)       // salt(white)
                    {
                        bytes[index] = 255;
                        bytes[index + 1] = 255;
                        bytes[index + 2] = 255;
                    }
                    else if(X < -2) // pepper(black)
                    {
                        bytes[index] = 0;
                        bytes[index + 1] = 0;
                        bytes[index + 2] = 0;
                    }
                    index += 3;
                }
                index += offset; // 將index移過那段間隙
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }
    }
}

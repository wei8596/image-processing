using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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
         * @return : header資訊字串
         */
        public string PrintInfo()
        {
            string info = "Manufacturer\t:\t" + head.manufacturer.ToString() + Environment.NewLine +
                          "Version\t\t:\t" + head.version.ToString() + Environment.NewLine +
                          "Encoding\t\t:\t" + head.encoding.ToString() + Environment.NewLine +
                          "BitsPerPixel\t:\t" + head.bitsPerPixel.ToString() + Environment.NewLine +
                          "xMin\t\t:\t" + head.xMin.ToString() + Environment.NewLine +
                          "yMin\t\t:\t" + head.yMin.ToString() + Environment.NewLine +
                          "xMax\t\t:\t" + head.xMax.ToString() + Environment.NewLine +
                          "yMax\t\t:\t" + head.yMax.ToString() + Environment.NewLine +
                          "hDpi\t\t:\t" + head.hDpi.ToString() + Environment.NewLine +
                          "vDpi\t\t:\t" + head.vDpi.ToString() + Environment.NewLine +
                          "Reserved\t\t:\t" + head.reserved.ToString() + Environment.NewLine +
                          "NPlanes\t\t:\t" + head.nPlanes.ToString() + Environment.NewLine +
                          "BytesPerLine\t:\t" + head.bytesPerLine.ToString() + Environment.NewLine +
                          "PaletteInfo\t:\t" + head.paletteInfo.ToString() + Environment.NewLine +
                          "HscreenSize\t:\t" + head.hScreen.ToString() + Environment.NewLine +
                          "VscreenSize\t:\t" + head.vScreen.ToString();
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            for (int i = 0; i < bytes.Length; i += 3)
            {
                byte b = bytes[i];
                byte g = bytes[i + 1];
                byte r = bytes[i + 2];
                byte gray = (byte)(r * 0.299 + g * 0.587 + b * 0.114);
                bytes[i] = gray;
                bytes[i + 1] = gray;
                bytes[i + 2] = gray;
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            for (int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    int mirrorX = (width - 1) - x;
                    int oldIndex = 3 * (y * width + x);
                    int newIndex = 3 * (y * width + mirrorX);
                    // B
                    mirror_bytes[newIndex] = bytes[oldIndex];
                    // G
                    mirror_bytes[newIndex + 1] = bytes[oldIndex + 1];
                    // R
                    mirror_bytes[newIndex + 2] = bytes[oldIndex + 2];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int mirrorY = (height - 1) - y;
                    int oldIndex = 3 * (y * width + x);
                    int newIndex = 3 * (mirrorY * width + x);
                    // B
                    mirror_bytes[newIndex] = bytes[oldIndex];
                    // G
                    mirror_bytes[newIndex + 1] = bytes[oldIndex + 1];
                    // R
                    mirror_bytes[newIndex + 2] = bytes[oldIndex + 2];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // 對稱線 y = x
                    int index = 3 * (y * width + x);
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
                        int mirrorIndex = 3 * (mirrorY * width + mirrorX);
                        // B
                        mirror_bytes[mirrorIndex] = bytes[index];
                        // G
                        mirror_bytes[mirrorIndex + 1] = bytes[index + 1];
                        // R
                        mirror_bytes[mirrorIndex + 2] = bytes[index + 2];
                    }
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] mirror_bytes = new byte[data.Stride * data.Height]; // 存放鏡像資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // 對稱線 y = x - (width - 1)
                    int index = 3 * (y * width + x);
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
                        int mirrorIndex = 3 * (mirrorY * width + mirrorX);
                        // B
                        mirror_bytes[mirrorIndex] = bytes[index];
                        // G
                        mirror_bytes[mirrorIndex + 1] = bytes[index + 1];
                        // R
                        mirror_bytes[mirrorIndex + 2] = bytes[index + 2];
                    }
                }
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
         * 正旋轉
         * @angle : 旋轉角度
         */
        public void RotateForward(double angle)
        {
            // 每360度轉一圈, 轉換到0~359
            angle %= 360;

            // 等同沒有旋轉, 不做變化
            if(angle == 0)
            {
                return;
            }

            // 用頂點計算邊界
            int[,] vertices = { { 0, 0 }, { 0, img.Width - 1 }, { img.Height - 1, 0 }, { img.Height - 1, img.Width - 1 } };
            int[,] newVertices = new int[4, 2];
            for(int i = 0; i < 4; ++i)
            {
                //newVertices[i, 0] = ;
                //newVertices[i, 1] = ;
            }

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
            // 建立空間較大的影像並轉換格式
            Bitmap converted = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            // 使Bitmap預設為透明
            //converted.MakeTransparent();
            // 把原圖畫上去
            Graphics g = Graphics.FromImage(converted);
            g.DrawImage(img, 0, 0);
            // 影像中心
            int row_center = converted.Height / 2;
            int col_center = converted.Width / 2;
            // 畫出影像中心 test......................................................................
            SolidBrush brush = new SolidBrush(Color.Blue);
            g.FillRectangle(brush, col_center, row_center, 10, 10);

            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData data = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            double radians = angle * Math.PI / 180.0;   // 角度轉成弧度
            double sin = Math.Sin(radians);             // 用弧度計算
            double cos = Math.Cos(radians);
            double[,] rotation = new double[,] { { cos, sin }, { -sin, cos } }; // 旋轉矩陣

            byte[] rotate_bytes = new byte[data.Stride * data.Height]; // 存放旋轉後的資料

            int height = converted.Height;
            int width = converted.Width;
            for (int row = 0; row < head.height; ++row)
            {
                for (int col = 0; col < head.width; ++col)
                {
                    // 計算旋轉後對應的座標
                    // Round() : 四捨五入為最接近的整數
                    int newRow = (int)Math.Round(rotation[0, 0] * (row - row_center) + rotation[0, 1] * (col - col_center)) + row_center;
                    int newCol = (int)Math.Round(rotation[1, 0] * (row - row_center) + rotation[1, 1] * (col - col_center)) + col_center;
                    if (newRow < 0 || newCol < 0 || newRow > height - 1 || newCol > width - 1)
                    {
                        continue;
                    }
                    int oldIndex = 3 * (row * width + col);
                    int newIndex = 3 * (newRow * width + newCol);
                    rotate_bytes[newIndex] = bytes[oldIndex];
                    rotate_bytes[newIndex + 1] = bytes[oldIndex + 1];
                    rotate_bytes[newIndex + 2] = bytes[oldIndex + 2];
                }
            }

            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(rotate_bytes, 0, data.Scan0, rotate_bytes.Length);
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] bytesR = new byte[data.Stride * data.Height]; // 存放R資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    // R
                    bytesR[index + 2] = bytes[index + 2];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] bytesG = new byte[data.Stride * data.Height]; // 存放G資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    // R
                    bytesG[index + 1] = bytes[index + 1];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            byte[] bytesB = new byte[data.Stride * data.Height]; // 存放B資料
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    // B
                    bytesB[index] = bytes[index];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            int height = head.height;
            // 要用bytesPerLine
            int width = head.bytesPerLine;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 放大後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(head.bytesPerLine * ratio), (int)(head.height * ratio), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int height = newImg.Height;
            int width = newImg.Width;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int map_x = (int)(x / ratio + 0.5); // +0.5後取整等同做四捨五入
                    if(map_x >= head.bytesPerLine)
                    {
                        map_x = head.bytesPerLine - 1;
                    }
                    int map_y = (int)(y / ratio + 0.5); // +0.5後取整等同做四捨五入
                    if (map_y >= head.height)
                    {
                        map_y = head.height - 1;
                    }
                    int newIndex = 3 * (y * width + x);
                    int mapIndex = 3 * (map_y * head.bytesPerLine + map_x); // width使用原圖的bytesPerLine
                    newBytes[newIndex] = bytes[mapIndex];
                    newBytes[newIndex + 1] = bytes[mapIndex + 1];
                    newBytes[newIndex + 2] = bytes[mapIndex + 2];
                }
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 放大後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(head.bytesPerLine * ratio), (int)(head.height * ratio), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int height = newImg.Height;
            int width = newImg.Width;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double map_x = x / ratio;
                    if (map_x >= head.bytesPerLine - 1)
                    {
                        map_x = head.bytesPerLine - 2;
                    }
                    double map_y = y / ratio;
                    if (map_y >= head.height - 1)
                    {
                        map_y = head.height - 2;
                    }
                    // pixel值由對應的周圍四個pixel值決定
                    int i = (int)map_x;
                    int j = (int)map_y;
                    int newIndex = 3 * (y * width + x);
                    int mapIndex1 = 3 * (j * head.bytesPerLine + i); // width使用原圖的bytesPerLine
                    int mapIndex2 = 3 * (j * head.bytesPerLine + (i + 1));
                    int mapIndex3 = 3 * ((j + 1) * head.bytesPerLine + i);
                    int mapIndex4 = 3 * ((j + 1) * head.bytesPerLine + (i + 1));
                    int b = (bytes[mapIndex1] + bytes[mapIndex2] + bytes[mapIndex3] + bytes[mapIndex4]) / 4;
                    int g = (bytes[mapIndex1 + 1] + bytes[mapIndex2 + 1] + bytes[mapIndex3 + 1] + bytes[mapIndex4 + 1]) / 4;
                    int r = (bytes[mapIndex1 + 2] + bytes[mapIndex2 + 2] + bytes[mapIndex3 + 2] + bytes[mapIndex4 + 2]) / 4;
                    newBytes[newIndex] = (byte)Math.Min(b, 255);
                    newBytes[newIndex + 1] = (byte)Math.Min(g, 255);
                    newBytes[newIndex + 2] = (byte)Math.Min(r, 255);
                }
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
            // 只接受整數倍率
            ratio = Math.Round(ratio);

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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 縮小後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(head.bytesPerLine / ratio), (int)(head.height / ratio), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int height = newImg.Height;
            // 要用bytesPerLine
            int width = newImg.Width;
            for(int newY = 0, y = 0; newY < height; ++newY, y += (int)ratio)
            {
                for(int newX = 0, x = 0; newX < width; ++newX, x += (int)ratio)
                {
                    int index = 3 * (y * head.bytesPerLine + x); // width使用原圖的bytesPerLine
                    int newIndex = 3 * (newY * width + newX);
                    newBytes[newIndex] = bytes[index];
                    newBytes[newIndex + 1] = bytes[index + 1];
                    newBytes[newIndex + 2] = bytes[index + 2];
                }
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
            // 只接受整數倍率
            ratio = Math.Round(ratio);

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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 縮小後的影像 (width使用原圖的bytesPerLine)
            Bitmap newImg = new Bitmap((int)(head.bytesPerLine / ratio), (int)(head.height / ratio), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData newData = newImg.LockBits(new Rectangle(0, 0, newImg.Width, newImg.Height), ImageLockMode.ReadWrite, newImg.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] newBytes = new byte[newData.Stride * newData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(newData.Scan0, newBytes, 0, newBytes.Length);

            int height = newImg.Height;
            int width = newImg.Width;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double map_x = x * ratio;
                    if (map_x >= head.bytesPerLine - 1)
                    {
                        map_x = head.bytesPerLine - 2;
                    }
                    double map_y = y * ratio;
                    if (map_y >= head.height - 1)
                    {
                        map_y = head.height - 2;
                    }
                    // pixel值由對應的周圍四個pixel值決定
                    int i = (int)map_x;
                    int j = (int)map_y;
                    int newIndex = 3 * (y * width + x);
                    int mapIndex1 = 3 * (j * head.bytesPerLine + i); // width使用原圖的bytesPerLine
                    int mapIndex2 = 3 * (j * head.bytesPerLine + (i + 1));
                    int mapIndex3 = 3 * ((j + 1) * head.bytesPerLine + i);
                    int mapIndex4 = 3 * ((j + 1) * head.bytesPerLine + (i + 1));
                    int b = (bytes[mapIndex1] + bytes[mapIndex2] + bytes[mapIndex3] + bytes[mapIndex4]) / 4;
                    int g = (bytes[mapIndex1 + 1] + bytes[mapIndex2 + 1] + bytes[mapIndex3 + 1] + bytes[mapIndex4 + 1]) / 4;
                    int r = (bytes[mapIndex1 + 2] + bytes[mapIndex2 + 2] + bytes[mapIndex3 + 2] + bytes[mapIndex4 + 2]) / 4;
                    newBytes[newIndex] = (byte)Math.Min(b, 255);
                    newBytes[newIndex + 1] = (byte)Math.Min(g, 255);
                    newBytes[newIndex + 2] = (byte)Math.Min(r, 255);
                }
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
        public void Transparency(Bitmap cover, float t)
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 複製要疊的圖並轉換格式
            Bitmap cover_converted = cover.Clone(new Rectangle(0, 0, cover.Width, cover.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData coverData = cover_converted.LockBits(new Rectangle(0, 0, cover_converted.Width, cover_converted.Height), ImageLockMode.ReadOnly, cover_converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] coverBytes = new byte[coverData.Stride * coverData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(coverData.Scan0, coverBytes, 0, coverBytes.Length);

            int height = converted.Height;
            int width = converted.Width;
            int coverWidth = cover.Width;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = 3 * (y * width + x);
                    int coverIndex = 3 * (y * coverWidth + x);
                    float p = 1 - t;
                    bytes[index] = (byte)(t * coverBytes[coverIndex] + p * bytes[index]);
                    bytes[index + 1] = (byte)(t * coverBytes[coverIndex + 1] + p * bytes[index + 1]);
                    bytes[index + 2] = (byte)(t * coverBytes[coverIndex + 2] + p * bytes[index + 2]);
                }
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
         * 取得Gray code
         * 
         * @return : Gray code陣列
         */
        public byte[] GetGrayCode()
        {
            string[] gray = new string[256]; // 2^8 (8 bits)
            // 從1-bit開始
            gray[0] = "0";
            gray[1] = "1";
            int index = 2;
            // 每次迴圈從前一次的i個產生2 * i個
            for(int i = 2; i < 256; i *= 2)
            {
                // 取前一次產生的反向加入
                for(int j = i-1; j >= 0; --j)
                {
                    gray[index] = gray[j];
                    ++index;
                }

                // 前半前面補0
                for(int j = 0; j < i; ++j)
                {
                    gray[j] = "0" + gray[j];
                }

                // 後半前面補0
                for (int j = i; j < 2 * i; ++j)
                {
                    gray[j] = "1" + gray[j];
                }
            }

            // 將bit string轉成byte數值
            byte[] num = new byte[8];
            for(int i = 1, j = 0; i <= 128; i *= 2, ++j)
            {
                num[j] = BitStringToByte(gray[i]);
            }

            return num;
        }

        /*
         * Bit-plane slicing
         * @n : 指定不同的Bit-plane (2^7 ~ 2^0)
         * 
         * @return : Bit-plane圖像
         */
        public Bitmap BitPlane(byte n)
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            byte max = 255;
            byte min = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                if((n & bytes[i]) == n)
                {
                    bytes[i] = max;
                }
                else
                {
                    bytes[i] = min;
                }
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
         */
        public void Watermark(Bitmap mark)
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 複製浮水印影像並轉換格式
            Bitmap mark_converted = mark.Clone(new Rectangle(0, 0, mark.Width, mark.Height), PixelFormat.Format24bppRgb);
            // 鎖定影像內容到記憶體
            // 將圖的資料存到記憶體, 可以直接對它操作
            BitmapData markData = mark_converted.LockBits(new Rectangle(0, 0, mark_converted.Width, mark_converted.Height), ImageLockMode.ReadWrite, mark_converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] markBytes = new byte[markData.Stride * markData.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(markData.Scan0, markBytes, 0, markBytes.Length);

            // 修改後面4個bits來加入浮水印
            for(int i = 0; i < markBytes.Length; ++i)
            {
                // F0 = 1111 0000
                // F  = 0000 1111
                bytes[i] &= 0xF0;
                bytes[i] += (byte)(0xF & markBytes[i]);
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
         * @n : 統計直方圖
         */
        public void ContrastStretch(double[] n)
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // 原圖最大最小值
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
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
         * Outlier
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadWrite, converted.PixelFormat);
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
            int newIndex = 0;
            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    byte[] pixel = new byte[8];
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
                    double avg = sum / 8;
                    int index = 3 * (y * width + x);
                    byte threshold = 50; // 設多少?????????????????
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
    }
}

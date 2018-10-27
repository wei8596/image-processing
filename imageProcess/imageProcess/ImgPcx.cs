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
            string info = "Manufacturer:".PadRight(1) + head.manufacturer.ToString() + Environment.NewLine +
                          "Version:".PadRight(1) + head.version.ToString() + Environment.NewLine +
                          "Encoding:".PadRight(1) + head.encoding.ToString() + Environment.NewLine +
                          "BitsPerPixel:".PadRight(1) + head.bitsPerPixel.ToString() + Environment.NewLine +
                          "xMin:".PadRight(1) + head.xMin.ToString() + Environment.NewLine +
                          "yMin:".PadRight(1) + head.yMin.ToString() + Environment.NewLine +
                          "xMax:".PadRight(1) + head.xMax.ToString() + Environment.NewLine +
                          "yMax:".PadRight(1) + head.yMax.ToString() + Environment.NewLine +
                          "hDpi:".PadRight(1) + head.hDpi.ToString() + Environment.NewLine +
                          "vDpi:".PadRight(1) + head.vDpi.ToString() + Environment.NewLine +
                          "Reserved:".PadRight(1) + head.reserved.ToString() + Environment.NewLine +
                          "NPlanes:".PadRight(1) + head.nPlanes.ToString() + Environment.NewLine +
                          "BytesPerLine:".PadRight(16) + head.bytesPerLine.ToString() + Environment.NewLine +
                          "PaletteInfo:".PadRight(1) + head.paletteInfo.ToString() + Environment.NewLine +
                          "HscreenSize:".PadRight(1) + head.hScreen.ToString() + Environment.NewLine +
                          "VscreenSize:".PadRight(1) + head.vScreen.ToString();
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
            // Stride - 影像scan的寬度
            byte[] bytes = new byte[data.Stride * data.Height]; // 存放整個圖像資料
            // 將圖像資料複製到陣列
            // Marshal.Copy(srcPtr, dst, startIndex, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            for (int i = 0; i < bytes.Length; i += 3)
            {
                int b = bytes[i];
                int g = bytes[i + 1];
                int r = bytes[i + 2];
                int gray = (int)(r * 0.299 + g * 0.587 + b * 0.114);
                bytes[i] = (byte)gray;
                bytes[i + 1] = (byte)gray;
                bytes[i + 2] = (byte)gray;
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
                    // B
                    mirror_bytes[3 * (y * width + mirrorX)] = bytes[3 * (y * width + x)];
                    // G
                    mirror_bytes[3 * (y * width + mirrorX) + 1] = bytes[3 * (y * width + x) + 1];
                    // R
                    mirror_bytes[3 * (y * width + mirrorX) + 2] = bytes[3 * (y * width + x) + 2];
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
                    // B
                    mirror_bytes[3 * (mirrorY * width + x)] = bytes[3 * (y * width + x)];
                    // G
                    mirror_bytes[3 * (mirrorY * width + x) + 1] = bytes[3 * (y * width + x) + 1];
                    // R
                    mirror_bytes[3 * (mirrorY * width + x) + 2] = bytes[3 * (y * width + x) + 2];
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
            BitmapData data = converted.LockBits(new Rectangle(0, 0, head.width, head.height), ImageLockMode.ReadOnly, converted.PixelFormat);
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
         */
        public void RotateForward()
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





            // 將資料複製到圖像物件
            // Marshal.Copy(src, startIndex, dstPtr, length)
            // Scan0 - 影像資料的起始位置
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length); //...........
            // 解除鎖定記憶體
            converted.UnlockBits(data);
            img = converted;
        }
    }
}

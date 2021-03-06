﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DXVisualTestFixer.Native {
    public static class ImageHelper {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2) {
            if((b1 == null) != (b2 == null)) return false;
            if(b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }
        public static unsafe bool CompareUnsafe(Bitmap b1, Bitmap b2) {
            if((b1 == null) != (b2 == null)) return false;
            if(b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;
                byte* x1 = (byte*)bd1scan0.ToPointer(), x2 = (byte*)bd2scan0.ToPointer();
                int l = len;
                for(int i = 0; i < l / 4; i++, x1 += 4, x2 += 4)
                    if(*((int*)x1) != *((int*)x2))
                        return false;
                return true;
            }
            finally {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }
        public static unsafe int DeltaUnsafe(Bitmap b1, Bitmap b2) {
            if((b1 == null) != (b2 == null)) return -1;
            if(b1.Size != b2.Size) return -1;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;
                int result = 0;
                byte* x1 = (byte*)bd1scan0.ToPointer(), x2 = (byte*)bd2scan0.ToPointer();
                int l = len;
                for(int i = 0; i < l / 4; i++, x1 += 4, x2 += 4)
                    if(*((int*)x1) != *((int*)x2))
                        result++;
                return result;
            }
            finally {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }
        public static unsafe int RedCount(Bitmap bitmap) {
            var bd = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try {
                IntPtr bdScan0 = bd.Scan0;

                int stride = bd.Stride;
                int len = stride * bitmap.Height;
                int result = 0;
                byte* x = (byte*)bdScan0.ToPointer();
                int l = len;
                for(int i = 0; i < l / 4; i++, x += 4) {
                    if(*((uint*)x) == 0b11111111111111110000000000000000)//red
                        result++;
                }
                return result;
            }
            finally {
                bitmap.UnlockBits(bd);
            }
        }

        public static Bitmap CreateImageFromArray(byte[] arr) {
            using(MemoryStream s = new MemoryStream(arr)) {
                return Image.FromStream(s) as Bitmap;
            }
        }
        public static byte[] ImageToByte(Image img) {
            using(var stream = new MemoryStream()) {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
    }
}

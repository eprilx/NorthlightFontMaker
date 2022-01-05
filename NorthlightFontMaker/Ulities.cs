/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;
using System.IO;
using System.Linq;
using Gibbed.IO;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

namespace NorthlightFontMaker
{
    class Ulities
    {
        public static string StringBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString, Pos1);
            if (Pos2 == -1)
            {
                Pos2 = STR.LastIndexOf(STR.Last()) + 1;
            }
            FinalString = STR[Pos1..Pos2];
            return FinalString;
        }

        public static (float, float, float, float) getUVmappingFromPoint(float x, float y, float width, float height, int WidthImg, int HeightImg)
        {
            float UVLeft = x / (float)WidthImg;
            float UVTop = y / (float)HeightImg;
            float UVRight = (x + width) / (float)WidthImg;
            float UVBottom = (y + height) / (float)HeightImg;
            return (UVLeft, UVTop, UVRight, UVBottom);
        }

        public static (float, float, float, float) getPointFromUVmapping(float UVLeft, float UVTop, float UVRight, float UVBottom, int WidthImg, int HeightImg)
        {

            float x = UVLeft * WidthImg;
            float y = UVTop * HeightImg;
            float width = (UVRight * WidthImg) - x;
            float height = (UVBottom * HeightImg) - y;
            return (x, y, width, height);
        }

        public static int intScaleInt(int number, float Scale)
        {
            return (int)((float)number * Scale);
        }
        public static int floatScaleInt(float number, float Scale)
        {
            return (int)((float)number * Scale);
        }

        public static float floatRevScale(float number, float Scale)
        {
            return ((float)number / Scale);
        }
        public static void PNGtoBGRA8(string inputPath)
        {
            Bitmap bitmap = new Bitmap(inputPath);
            string outPath;
            if (inputPath.EndsWith("_0.png", StringComparison.OrdinalIgnoreCase))
            {
                outPath = inputPath.Replace(".png", ".dds", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                outPath = inputPath.Replace(".png", "_0.dds", StringComparison.OrdinalIgnoreCase);
            }
            var outDDS = File.Create(outPath);
            //write new header
            var headerBGRA = File.OpenRead("BGRA8.header");
            outDDS.WriteBytes(headerBGRA.ReadBytes(12));
            outDDS.WriteValueU32((uint)bitmap.Height);
            outDDS.WriteValueU32((uint)bitmap.Width);
            outDDS.WriteValueU32((uint)(bitmap.Width * 2));
            headerBGRA.Position = 24;
            outDDS.WriteFromStream(headerBGRA, headerBGRA.Length - headerBGRA.Position);
            headerBGRA.Close();

            for (int j = 0; j < bitmap.Height; j++)
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    Color clr = bitmap.GetPixel(i, j);
                    outDDS.WriteValueU8(clr.B);
                    outDDS.WriteValueU8(clr.G);
                    outDDS.WriteValueU8(clr.R);
                    outDDS.WriteValueU8(clr.A);
                }
            }
            outDDS.Close();
        }
        public static void BGRA8toR16F(FileStream input, FileStream output)
        {
            input.Position = 12;
            //get height 
            uint heightImg = input.ReadValueU32();
            //get width
            uint widthImg = input.ReadValueU32();

            input.Position = 84;
            if(input.ReadValueU32() == 111)
            {
                Console.Write("input DDS is R16F... no convert... ");
                input.Position = 0;
                output.WriteFromStream(input, input.Length - input.Position);
                input.Close();
                output.Close();
                return;
            }    
            //write new header
            var headerR16F = File.OpenRead("R16F.header");
            output.WriteBytes(headerR16F.ReadBytes(12));
            output.WriteValueU32(heightImg);
            output.WriteValueU32(widthImg);
            output.WriteValueU32(widthImg * 2);
            headerR16F.Position = 24;
            output.WriteFromStream(headerR16F, headerR16F.Length - headerR16F.Position);
            headerR16F.Close();

            // read BGRA8 and convert to grayscale
            input.Position = 128;
            for(int i = 0; i < (input.Length - 128)/4; i++)
            {
                var B = input.ReadValueU8();
                var R = input.ReadValueU8();
                var G = input.ReadValueU8();
                var A = input.ReadValueU8();

                // convert to gray
                double Gray = (B + R + G) / 3;

                // normalize alpha to [-8,8]
                float hGray = -(float)(16 * A / 255.0 - 8);

                if (A > 0)
                    output.WriteBytes(ToInt(hGray));
                else
                    output.WriteValueU16(32767);
            }
            input.Close();
            output.Close();
        }

        // source: https://stackoverflow.com/questions/37759848/convert-byte-array-to-16-bits-float
        private static byte[] I2B(int input)
        {
            var bytes = BitConverter.GetBytes(input);
            return new byte[] { bytes[0], bytes[1] };
        }

        public static byte[] ToInt(float twoByteFloat)
        {
            int fbits = BitConverter.ToInt32(BitConverter.GetBytes(twoByteFloat), 0);
            int sign = fbits >> 16 & 0x8000;
            int val = (fbits & 0x7fffffff) + 0x1000;
            if (val >= 0x47800000)
            {
                if ((fbits & 0x7fffffff) >= 0x47800000)
                {
                    if (val < 0x7f800000) return I2B(sign | 0x7c00);
                    return I2B(sign | 0x7c00 | (fbits & 0x007fffff) >> 13);
                }
                return I2B(sign | 0x7bff);
            }
            if (val >= 0x38800000) return I2B(sign | val - 0x38000000 >> 13);
            if (val < 0x33000000) return I2B(sign);
            val = (fbits & 0x7fffffff) >> 23;
            return I2B(sign | ((fbits & 0x7fffff | 0x800000) + (0x800000 >> val - 102) >> 126 - val));
        }
    }
}

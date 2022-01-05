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
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;
namespace NorthlightFontMaker
{
    public static class ConverterFunction
    {
        public static void ConvertBINFNTtoFNT(string inputBINFNT, string outputFNT)
        {
            //Load BINFNT
            Console.Write("Load BINFNT... ");
            BINFNTStruct binfnt = BINFNTFormat.Load(inputBINFNT);
            Console.WriteLine("Success");

            // create BMF
            BMFontStruct bmf = new();

            Console.Write("Convert BINFNT to FNT... ");
            //convert infoBINFNT 2 infoBMF
            bmf.generalInfo.charsCount = (int)binfnt.generalInfo.charsCount;
            bmf.generalInfo.kernsCount = (int)binfnt.generalInfo.kernsCount;
            bmf.generalInfo.lineHeight = binfnt.generalInfo.lineHeight;
            bmf.generalInfo.size = binfnt.generalInfo.size;
            bmf.generalInfo.pages = 1;
            for (int i = 0; i < bmf.generalInfo.pages; i++)
            {
                bmf.generalInfo.idImg.Add(i);
                bmf.generalInfo.fileImg.Add(Path.GetFileName(inputBINFNT + "_0.dds"));
            }
            bmf.generalInfo.WidthImg = (int)binfnt.generalInfo.widthImg;
            bmf.generalInfo.HeightImg = (int)binfnt.generalInfo.heightImg;

            //convert charDescBINFNT 2 charDescBMF
            foreach (BINFNTStruct.charDesc charBINFNT in binfnt.charDescList)
            {
                (float x, float y, float width, float height) = Ulities.getPointFromUVmapping(charBINFNT.xMin_1, charBINFNT.yMin_1, charBINFNT.xMax_1, charBINFNT.yMax_1, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);

                BMFontStruct.charDesc charBMF = new();
                //charBMF.id = charBINFNT.id;
                charBMF.x = x;
                charBMF.y = y;
                charBMF.width = width;
                charBMF.height = height;
                charBMF.xoffset = charBINFNT.bearingX1_1 * binfnt.generalInfo.size;
                charBMF.yoffset = binfnt.generalInfo.lineHeight - charBINFNT.bearingY2_1 * binfnt.generalInfo.size - height;
                //charBMF.yoffset = binfnt.generalInfo.lineHeight - charBINFNT.bearingY1_1 * binfnt.generalInfo.size;
                charBMF.page = 0;
                bmf.charDescList.Add(charBMF);
            }

            // convert idList
            int count = 0;
            foreach (ushort id in binfnt.idList)
            {
                bmf.charDescList[count].id = id;
                count += 1;
            }
            // convert advanceList
            count = 0;
            foreach (BINFNTStruct.advanceDesc advanceBINFNT in binfnt.advanceDescList)
            {
                bmf.charDescList[count].xadvance = advanceBINFNT.xadvance2_1 * binfnt.generalInfo.size;
                // swap channel
                // 0 1 2 RGB
                // 1 2 4 BGR
                if (advanceBINFNT.chnl == 0)
                {
                    advanceBINFNT.chnl = 4;
                }
                else if (advanceBINFNT.chnl == 1)
                    advanceBINFNT.chnl = 2;
                else if (advanceBINFNT.chnl == 2)
                    advanceBINFNT.chnl = 1;
                bmf.charDescList[count].chnl = (int)advanceBINFNT.chnl;
                count += 1;
            }

            //convert kernel list
            if (binfnt.generalInfo.version == 7)
            {
                foreach (BINFNTStruct.kernelDescType7 kernelBINFNT in binfnt.kernelDescListType7)
                {
                    BMFontStruct.kernelDesc kernelBMF = new();
                    kernelBMF.first = kernelBINFNT.first;
                    kernelBMF.second = kernelBINFNT.second;
                    kernelBMF.amount = kernelBINFNT.amount * binfnt.generalInfo.size;
                    bmf.kernelDescList.Add(kernelBMF);
                }
            }
            else if (binfnt.generalInfo.version == 4)
            {
                foreach (BINFNTStruct.kernelDescType4 kernelBINFNT in binfnt.kernelDescListType4)
                {
                    BMFontStruct.kernelDesc kernelBMF = new();
                    kernelBMF.first = (int)kernelBINFNT.first;
                    kernelBMF.second = (int)kernelBINFNT.second;
                    kernelBMF.amount = kernelBINFNT.amount / binfnt.generalInfo.size;
                    bmf.kernelDescList.Add(kernelBMF);
                }
            }
            Console.WriteLine("Success");

            Console.Write("Create FNT... ");
            BMFontFormat.CreateText(outputFNT, bmf);
            Console.WriteLine("Success");

            Console.Write("Export DDS... ");
            File.WriteAllBytes(inputBINFNT + "_0.dds", binfnt.DDSTextures);
            Console.WriteLine("SUCCESS");
        }

        public static void CreateBINFNTfromFNT(string inputBINFNT, string inputBMF, string outputBINFNT)
        {
            //Load BINFNT
            Console.Write("Load BINFNT... ");
            BINFNTStruct binfnt = BINFNTFormat.Load(inputBINFNT);
            Console.WriteLine("Success");

            //Load BMFont
            Console.Write("Load FNT... ");
            BMFontStruct bmf = BMFontFormat.Load(inputBMF);
            Console.WriteLine("Success");

            Console.Write("Convert FNT to BINFNT... ");
            //Create BINFNT
            var output = File.Create(outputBINFNT);

            //convert infoBMF 2 infoBINFNT
            binfnt.generalInfo.charsCount = (uint)bmf.generalInfo.charsCount;
            binfnt.generalInfo.kernsCount = (uint)bmf.generalInfo.kernsCount;
            binfnt.generalInfo.lineHeight = bmf.generalInfo.lineHeight;
            bmf.generalInfo.size = Math.Abs(bmf.generalInfo.size);
            binfnt.generalInfo.size = bmf.generalInfo.size;

            binfnt.generalInfo.widthImg = (uint)bmf.generalInfo.WidthImg;
            binfnt.generalInfo.heightImg = (uint)bmf.generalInfo.HeightImg;

            // write header
            BINFNTFormat.WriteHeader(output, binfnt.generalInfo);
            //convert charDescBMF 2 charDescBINFNT
            binfnt.charDescList.Clear();
            float lineHeightbmf = bmf.generalInfo.lineHeight;
            float sizeBMF = bmf.generalInfo.size;
            foreach (BMFontStruct.charDesc charBMF in bmf.charDescList)
            {
                if (charBMF.width == 0 && charBMF.height == 0)
                {
                    charBMF.width = 6;
                    charBMF.height = 6;
                }
                //(float x, float y, float width, float height) = Ulities.getPointFromUVmapping(charBINFNT.UVLeft_1, charBINFNT.UVTop_1, charBINFNT.UVRight_1, charBINFNT.UVBottom_1, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                (float UVLeft, float UVTop, float UVRight, float UVBottom) = Ulities.getUVmappingFromPoint(charBMF.x, charBMF.y, charBMF.width, charBMF.height, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                BINFNTStruct.charDesc charBINFNT = new();

                charBINFNT.bearingX1_1 = charBMF.xoffset / sizeBMF;
                charBINFNT.bearingX1_2 = charBINFNT.bearingX1_1;
                charBINFNT.bearingY2_1 = (lineHeightbmf - charBMF.yoffset - charBMF.height) / sizeBMF;
                charBINFNT.bearingY2_2 = charBINFNT.bearingY2_1;
                charBINFNT.xMin_1 = UVLeft;
                charBINFNT.xMin_2 = UVLeft;
                charBINFNT.yMin_1 = UVTop;
                charBINFNT.yMin_2 = UVTop;
                charBINFNT.xMax_1 = UVRight;
                charBINFNT.xMax_2 = UVRight;
                charBINFNT.yMax_1 = UVBottom;
                charBINFNT.yMax_2 = UVBottom;
                charBINFNT.bearingX2_1 = (charBMF.xoffset + charBMF.width) / sizeBMF;
                charBINFNT.bearingX2_2 = charBINFNT.bearingX2_1;
                charBINFNT.bearingY1_1 = (lineHeightbmf - charBMF.yoffset) / sizeBMF;
                charBINFNT.bearingY1_2 = charBINFNT.bearingY1_1;

                if (charBMF.id == 32 || charBMF.id == 9 || charBMF.id == 10 || charBMF.id == 13)
                {
                    charBINFNT.bearingX1_1 = 0;
                    charBINFNT.bearingX1_2 = charBINFNT.bearingX1_1;
                    charBINFNT.bearingY2_1 = 0;
                    charBINFNT.bearingY2_2 = charBINFNT.bearingY2_1;
                    charBINFNT.bearingX2_1 = 0;
                    charBINFNT.bearingX2_2 = charBINFNT.bearingX2_1;
                    charBINFNT.bearingY1_1 = 0;
                    charBINFNT.bearingY1_2 = charBINFNT.bearingY1_1;
                }
                binfnt.charDescList.Add(charBINFNT);
            }
            BINFNTFormat.WriteTableCharDesc(output, binfnt.generalInfo, binfnt);

            // write table unk
            BINFNTFormat.WriteTableUnkDesc(output, binfnt.generalInfo, binfnt);

            // convert advance Desc
            ushort plus4 = binfnt.advanceDescList[0].plus4;
            ushort numb4 = binfnt.advanceDescList[0].numb4;
            ushort plus6 = binfnt.advanceDescList[0].plus6;
            ushort numb6 = binfnt.advanceDescList[0].numb6;
            binfnt.advanceDescList.Clear();
            int count = 0;
            foreach (BMFontStruct.charDesc charBMF in bmf.charDescList)
            {
                BINFNTStruct.advanceDesc advanceBINFNT = new();
                advanceBINFNT.plus4 = (ushort)(numb4 * count);
                advanceBINFNT.numb4 = numb4;
                advanceBINFNT.plus6 = (ushort)(numb6 * count);
                advanceBINFNT.numb6 = numb6;
                // swap channel
                // 0 1 2 RGB
                // 1 2 4 BGR
                if (charBMF.chnl == 2)
                    charBMF.chnl = 1;
                else if (charBMF.chnl == 1)
                    charBMF.chnl = 2;
                else
                    charBMF.chnl = 0;
                advanceBINFNT.chnl = (uint)charBMF.chnl;
                //advanceBINFNT.chnl = 0;
                advanceBINFNT.xadvance1_1 = 0;
                advanceBINFNT.xadvance1_2 = 0;
                advanceBINFNT.yoffset2_1 = -charBMF.yoffset / sizeBMF;
                advanceBINFNT.yoffset2_2 = advanceBINFNT.yoffset2_1;
                advanceBINFNT.xadvance2_1 = charBMF.xadvance / sizeBMF;
                advanceBINFNT.xadvance2_2 = advanceBINFNT.xadvance2_1;

                advanceBINFNT.yoffset1_1 = advanceBINFNT.yoffset2_1 - charBMF.height / sizeBMF;
                advanceBINFNT.yoffset1_2 = advanceBINFNT.yoffset1_1;
                count += 1;
                binfnt.advanceDescList.Add(advanceBINFNT);
            }
            BINFNTFormat.WriteTableAdvanceDesc(output, binfnt.generalInfo, binfnt);

            // convert idList
            List<ushort> idList = new();
            foreach (BMFontFormat.charDesc _char in bmf.charDescList)
            {
                idList.Add((ushort)_char.id);
            }
            binfnt.idList = idList.ToArray();
            BINFNTFormat.WriteTableID(output, binfnt.idList);

            // convert kernel

            if (binfnt.generalInfo.version == 7)
            {
                binfnt.kernelDescListType7.Clear();
                foreach (BMFontFormat.kernelDesc kernelBMF in bmf.kernelDescList)
                {
                    BINFNTStruct.kernelDescType7 kernelBINFNT = new();
                    kernelBINFNT.first = (ushort)kernelBMF.first;
                    kernelBINFNT.second = (ushort)kernelBMF.second;
                    kernelBINFNT.amount = (float)kernelBMF.amount / bmf.generalInfo.size;
                    binfnt.kernelDescListType7.Add(kernelBINFNT);
                }
            }
            else if (binfnt.generalInfo.version == 4)
            {
                binfnt.kernelDescListType4.Clear();
                foreach (BMFontFormat.kernelDesc kernelBMF in bmf.kernelDescList)
                {
                    BINFNTStruct.kernelDescType4 kernelBINFNT = new();
                    kernelBINFNT.first = (uint)kernelBMF.first;
                    kernelBINFNT.second = (uint)kernelBMF.second;
                    kernelBINFNT.amount = (int)(kernelBMF.amount * bmf.generalInfo.size);
                    binfnt.kernelDescListType4.Add(kernelBINFNT);
                }
            }
            BINFNTFormat.WriteTableKernelDesc(output, binfnt.generalInfo, binfnt);

            Console.WriteLine("Success");

            Console.Write("Import DDS... ");
            // write textures
            string pathDDS = inputBMF.Replace(".fnt", "_0.dds", StringComparison.OrdinalIgnoreCase);
            string pathPNG1 = inputBMF.Replace(".fnt", ".png", StringComparison.OrdinalIgnoreCase);
            string pathPNG2 = inputBMF.Replace(".fnt", "_0.png", StringComparison.OrdinalIgnoreCase);
            if ( !File.Exists(pathDDS) && !File.Exists(pathPNG1) && !File.Exists(pathPNG2))
            {
                throw new Exception("Missing textures file: " + pathPNG1);
            }

            if (File.Exists(pathPNG1) || File.Exists(pathPNG2))
            {
                Console.Write("\nPNG detected... convert PNG to BGRA8... \n\n");
                string pathPNG = pathPNG2;
                if (File.Exists(pathPNG1))
                    pathPNG = pathPNG1;
                Ulities.PNGtoBGRA8(pathPNG);
            }
            WriteDDS(output, pathDDS, binfnt);

            Console.WriteLine("SUCCESS");
            output.Close();
        }

        private static void WriteDDS(FileStream output, string pathDDS, BINFNTStruct binfnt)
        {
            var inputDDS = File.OpenRead(pathDDS);
            // replace background for version 7
            if (binfnt.generalInfo.version == 7)
            {

                Console.Write("\nVersion 7 detected... convert BGRA8 to R16_FLOAT distance field... ");
                var outputDDStmp = File.Create(pathDDS + ".tmp");
                Ulities.BGRA8toR16F(inputDDS, outputDDStmp);

                inputDDS = File.OpenRead(pathDDS + ".tmp");
            }
            BINFNTFormat.WriteTextures(output, binfnt.generalInfo, inputDDS, binfnt);
            inputDDS.Close();
        }
    }
}

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

namespace NorthlightFontMaker
{
    public static class ConverterFunction
    {
        public static void ConvertBINFNTtoFNT(string inputBINFNT, string outputFNT)
        {
            //get default config
            //Config config = DefaultConfig.Get(versionGame);

            //Load BINFNT
            BINFNTStruct binfnt = BINFNTFormat.Load(inputBINFNT);


            //generalInfoBMF BMFinfo, List< charDescBMF > charDescList, List<kernelDescBMF> kernelDescList)

            // create BMF
            BMFontStruct bmf = new();
            //convert infoBINFNT 2 infoBMF
            bmf.generalInfo.charsCount = (int)binfnt.generalInfo.charsCount;
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
                (float x, float y, float width, float height) = Ulities.getPointFromUVmapping(charBINFNT.UVLeft_1, charBINFNT.UVTop_1, charBINFNT.UVRight_1, charBINFNT.UVBottom_1, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);

                BMFontStruct.charDesc charBMF = new();
                //charBMF.id = charBINFNT.id;
                charBMF.x = x;
                charBMF.y = y;
                charBMF.width = width;
                charBMF.height = height;
                charBMF.xoffset = charBINFNT.bearingX1_1 * binfnt.generalInfo.size;
                //charBMF.yoffset = binfnt.generalInfo.lineHeight - charBINFNT.bearingY1_1 * binfnt.generalInfo.size;
                //charBMF.xadvance = Ulities.floatRevScale(charBINFNT.xadvance.xadvanceScale, config.scaleXadvance);
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
                bmf.charDescList[count].yoffset = -advanceBINFNT.yoffset2_1 * binfnt.generalInfo.size;
                bmf.charDescList[count].xadvance = advanceBINFNT.xadvance2_1 * binfnt.generalInfo.size;
                count += 1;
            }

            BMFontFormat.CreateText(outputFNT, bmf);
            File.WriteAllBytes(inputBINFNT + "_0.dds", binfnt.DDSTextures);
        }

        public static void CreateBINFNTfromFNT(string inputBINFNT, string inputBMF, string outputBINFNT)
        {
            //get default config
            //Config config = DefaultConfig.Get(versionGame);

            //Load BINFNT
            BINFNTStruct binfnt = BINFNTFormat.Load(inputBINFNT);

            //Load BMFont
            BMFontStruct bmf = BMFontFormat.Load(inputBMF);

            //Create BINFNT
            var output = File.Create(outputBINFNT);

            //convert infoBMF 2 infoBINFNT
            binfnt.generalInfo.charsCount = (uint)bmf.generalInfo.charsCount;
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
                if (charBMF.id == 32)
                {
                    if (charBMF.width == 0)
                    {
                        charBMF.width = 6;
                        charBMF.height = 6;
                    }
                }
                //(float x, float y, float width, float height) = Ulities.getPointFromUVmapping(charBINFNT.UVLeft_1, charBINFNT.UVTop_1, charBINFNT.UVRight_1, charBINFNT.UVBottom_1, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                (float UVLeft, float UVTop, float UVRight, float UVBottom) = Ulities.getUVmappingFromPoint(charBMF.x, charBMF.y, charBMF.width, charBMF.height, bmf.generalInfo.WidthImg, bmf.generalInfo.HeightImg);
                BINFNTStruct.charDesc charBINFNT = new();

                charBINFNT.bearingX1_1 = charBMF.xoffset / sizeBMF;
                charBINFNT.bearingX1_2 = charBINFNT.bearingX1_1;
                charBINFNT.bearingY2_1 = (lineHeightbmf - charBMF.yoffset - charBMF.height) / sizeBMF;
                charBINFNT.bearingY2_2 = charBINFNT.bearingY2_1;
                charBINFNT.UVLeft_1 = UVLeft;
                charBINFNT.UVLeft_2 = UVLeft;
                charBINFNT.UVTop_1 = UVTop;
                charBINFNT.UVTop_2 = UVTop;
                charBINFNT.UVRight_1 = UVRight;
                charBINFNT.UVRight_2 = UVRight;
                charBINFNT.UVBottom_1 = UVBottom;
                charBINFNT.UVBottom_2 = UVBottom;
                charBINFNT.bearingX2_1 = (charBMF.xoffset + charBMF.width) / sizeBMF;
                charBINFNT.bearingX2_2 = charBINFNT.bearingX2_1;
                charBINFNT.bearingY1_1 = (lineHeightbmf - charBMF.yoffset) / sizeBMF;
                charBINFNT.bearingY1_2 = charBINFNT.bearingY1_1;

                if (charBMF.id == 32)
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

            // write textures
            string pathDDS = inputBMF.Replace(".fnt", "_0.dds", StringComparison.OrdinalIgnoreCase);
            if (File.Exists(pathDDS))
            {
                var inputDDS = File.OpenRead(pathDDS);
                BINFNTFormat.WriteTextures(output, inputDDS);
                inputDDS.Close();
            }
            else
            {
                throw new Exception("Missing textures file: " + pathDDS);
            }

            output.Close();
        }
    }
}

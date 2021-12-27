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

using Gibbed.IO;
using System;
using System.IO;

namespace NorthlightFontMaker
{
    public class BINFNTFormat : BINFNTStruct
    {
        public static BINFNTStruct Load(string inputBINFNT)
        {
            BINFNTStruct binfnt = new();
            var input = File.OpenRead(inputBINFNT);
            input.Position = 0;

            //Read header
            ReadHeader(input, ref binfnt.generalInfo);

            ReadTableCharDesc(input, ref binfnt.generalInfo, ref binfnt);

            ReadTableUnkDesc(input, ref binfnt.generalInfo, ref binfnt);

            ReadTableAdvanceDesc(input, ref binfnt.generalInfo, ref binfnt);

            ReadTableID(input, ref binfnt.idList, binfnt.generalInfo.charsCount);

            ReadTextures(input, ref binfnt.generalInfo, ref binfnt);

            get_Baseline_LineHeight(ref binfnt.generalInfo, binfnt);

            input.Close();

            return binfnt;
        }

        private static void ReadHeader(FileStream input, ref general infoBINFNT)
        {
            input.Position = 0;
            infoBINFNT.magicBytes = input.ReadValueS32(); // =3
        }

        public static void WriteHeader(FileStream output, general infoBINFNT)
        {
            output.Position = 0;
            output.WriteValueS32(infoBINFNT.magicBytes);
        }

        private static void ReadTableCharDesc(FileStream input, ref general infoBINFNT, ref BINFNTStruct binfnt)
        {
            input.Position = 4;
            infoBINFNT.charsCount = input.ReadValueU32() / 4;

            for (int i = 0; i < infoBINFNT.charsCount; i++)
            {
                binfnt.charDescList.Add(new charDesc
                {
                    bearingX1_1 = input.ReadValueF32(), // = xoffset
                    bearingY2_1 = input.ReadValueF32(), // = lineHeight - yoffset - height char
                    UVLeft_1 = input.ReadValueF32(),
                    UVBottom_1 = input.ReadValueF32(),
                    bearingX2_1 = input.ReadValueF32(), // = xoffset + width char
                    bearingY2_2 = input.ReadValueF32(), // = lineHeight - yoffset - height char
                    UVRight_1 = input.ReadValueF32(),
                    UVBottom_2 = input.ReadValueF32(),
                    bearingX2_2 = input.ReadValueF32(), // = xoffset + width char
                    bearingY1_1 = input.ReadValueF32(), // = bearingY2 + height char = lineHeight - yoffset
                    UVRight_2 = input.ReadValueF32(),
                    UVTop_1 = input.ReadValueF32(),
                    bearingX1_2 = input.ReadValueF32(), // = xoffset
                    bearingY1_2 = input.ReadValueF32(), // = bearingY2 + height char = lineHeight - yoffset
                    UVLeft_2 = input.ReadValueF32(),
                    UVTop_2 = input.ReadValueF32()
                });
            }
        }
        public static void WriteTableCharDesc(FileStream output, general infoBINFNT, BINFNTStruct binfnt)
        {
            output.Position = 4;
            output.WriteValueU32(infoBINFNT.charsCount * 4);
            foreach (charDesc _char in binfnt.charDescList)
            {
                output.WriteValueF32(_char.bearingX1_1);
                output.WriteValueF32(_char.bearingY2_1);
                output.WriteValueF32(_char.UVLeft_1);
                output.WriteValueF32(_char.UVBottom_1);
                output.WriteValueF32(_char.bearingX2_1);
                output.WriteValueF32(_char.bearingY2_2);
                output.WriteValueF32(_char.UVRight_1);
                output.WriteValueF32(_char.UVBottom_2);
                output.WriteValueF32(_char.bearingX2_2);
                output.WriteValueF32(_char.bearingY1_1);
                output.WriteValueF32(_char.UVRight_2);
                output.WriteValueF32(_char.UVTop_1);
                output.WriteValueF32(_char.bearingX1_2);
                output.WriteValueF32(_char.bearingY1_2);
                output.WriteValueF32(_char.UVLeft_2);
                output.WriteValueF32(_char.UVTop_2);
            }
        }
        private static void ReadTableUnkDesc(FileStream input, ref general infoBINFNT, ref BINFNTStruct binfnt)
        {
            //input.Position = 4 + 4 + infoBINFNT.charsCount * 64;
            uint charsCount = input.ReadValueU32() / 6; // = infoBINFNT.charsCount
            for (int i = 0; i < infoBINFNT.charsCount; i++)
            {
                binfnt.unkDescList.Add(new unkDesc
                {
                    n1 = input.ReadValueU16(), // 0
                    n2 = input.ReadValueU16(), // 1
                    n3 = input.ReadValueU16(), // 2
                    n4 = input.ReadValueU16(), // 0
                    n5 = input.ReadValueU16(), // 2
                    n6 = input.ReadValueU16() // 3
                });
            }
        }
        public static void WriteTableUnkDesc(FileStream output, general infoBINFNT, BINFNTStruct binfnt)
        {
            output.WriteValueU32(infoBINFNT.charsCount * 6);

            for (int i = 0; i < infoBINFNT.charsCount; i++)
            {
                output.WriteValueU16(binfnt.unkDescList[0].n1);
                output.WriteValueU16(binfnt.unkDescList[0].n2);
                output.WriteValueU16(binfnt.unkDescList[0].n3);
                output.WriteValueU16(binfnt.unkDescList[0].n4);
                output.WriteValueU16(binfnt.unkDescList[0].n5);
                output.WriteValueU16(binfnt.unkDescList[0].n6);
            }
        }
        private static void ReadTableAdvanceDesc(FileStream input, ref general infoBINFNT, ref BINFNTStruct binfnt)
        {
            //input.Position = 4 + 4 + infoBINFNT.charsCount * 64 + 4 + infoBINFNT.charsCount * 12 + 4 + infoBINFNT.charsCount * 44;
            uint charsCount = input.ReadValueU32(); // = infoBINFNT.charsCount
            for (int i = 0; i < infoBINFNT.charsCount; i++)
            {
                binfnt.advanceDescList.Add(new advanceDesc
                {
                    plus4 = input.ReadValueU16(), // +4
                    numb4 = input.ReadValueU16(), // =4
                    plus6 = input.ReadValueU16(), // +6
                    numb6 = input.ReadValueU16(), // =6
                    zero = input.ReadValueF32(), // =0
                    xadvance1_1 = input.ReadValueF32(), // = 0 ??
                    yoffset1_1 = input.ReadValueF32(), // = yoffset2 - height
                    xadvance2_1 = input.ReadValueF32(), // = xadvance
                    yoffset1_2 = input.ReadValueF32(), // = yoffset2 - height
                    xadvance2_2 = input.ReadValueF32(), // = xadvance 
                    yoffset2_1 = input.ReadValueF32(), // = -Yoffset
                    xadvance1_2 = input.ReadValueF32(), // = 0 ??
                    yoffset2_2 = input.ReadValueF32() // = -Yoffset
                });
            }
        }
        public static void WriteTableAdvanceDesc(FileStream output, general infoBINFNT, BINFNTStruct binfnt)
        {
            output.WriteValueU32(infoBINFNT.charsCount);
            foreach (advanceDesc _char in binfnt.advanceDescList)
            {
                output.WriteValueU16(_char.plus4);
                output.WriteValueU16(_char.numb4);
                output.WriteValueU16(_char.plus6);
                output.WriteValueU16(_char.numb6);
                output.WriteValueF32(_char.zero);
                output.WriteValueF32(_char.xadvance1_1);
                output.WriteValueF32(_char.yoffset1_1);
                output.WriteValueF32(_char.xadvance2_1);
                output.WriteValueF32(_char.yoffset1_2);
                output.WriteValueF32(_char.xadvance2_2);
                output.WriteValueF32(_char.yoffset2_1);
                output.WriteValueF32(_char.xadvance1_2);
                output.WriteValueF32(_char.yoffset2_2);
            }
        }
        private static void ReadTableID(FileStream input, ref ushort[] idList, uint charsCount)
        {
            //TODO
            int startPos = (int)input.Position;
            idList = new ushort[charsCount];
            int baseId = 0;
            while ((input.Position - startPos) / 2 <= 0xFFFF)
            {
                ushort idx = input.ReadValueU16();
                if (idx != 0)
                {
                    idList[idx] = (ushort)(baseId);
                }
                baseId += 1;
            }
            idList[0] = (ushort)(idList[1] - 1);
        }
        public static void WriteTableID(FileStream output, ushort[] idList)
        {
            ushort[] tableID = new ushort[0xFFFF + 1];
            ushort baseID = 0;
            foreach (ushort id in idList)
            {
                tableID[id] = baseID;
                baseID += 1;
            }
            foreach (ushort value in tableID)
            {
                output.WriteValueU16(value);
            }
        }
        private static void ReadTextures(FileStream input, ref general infoBINFNT, ref BINFNTStruct binfnt)
        {
            //input.Position = 4 + 4 + infoBINFNT.charsCount * 64 + 4 + infoBINFNT.charsCount * 44 + 131072;
            uint sizeDDS = input.ReadValueU32();
            long posDDS = input.Position;
            input.ReadBytes(12);
            infoBINFNT.widthImg = input.ReadValueU32();
            infoBINFNT.heightImg = input.ReadValueU32();
            input.Position = posDDS;
            binfnt.DDSTextures = input.ReadBytes((int)(input.Length - input.Position));
        }
        public static void WriteTextures(FileStream output, FileStream inputDDS)
        {
            output.WriteValueU32((uint)inputDDS.Length);
            output.WriteFromStream(inputDDS, inputDDS.Length);
        }
        private static void get_Baseline_LineHeight(ref general infoBINFNT, BINFNTStruct binfnt)
        {
            int id = 4;
            //for (int id = 0; id < infoBINFNT.charsCount; id++)
            {
                // get point char
                (float x, float y, float width, float height) = Ulities.getPointFromUVmapping(binfnt.charDescList[id].UVLeft_1, binfnt.charDescList[id].UVTop_1, binfnt.charDescList[id].UVRight_1, binfnt.charDescList[id].UVBottom_1, (int)infoBINFNT.widthImg, (int)infoBINFNT.heightImg);

                // get base line
                infoBINFNT.baseLine = height / (binfnt.charDescList[id].bearingY1_1 - binfnt.charDescList[id].bearingY2_1);

                infoBINFNT.baseLine = (float)Math.Abs(Math.Round(infoBINFNT.baseLine, 3));

                // get line height
                infoBINFNT.lineHeight = (-binfnt.advanceDescList[id].yoffset2_1 * infoBINFNT.baseLine + height + binfnt.charDescList[id].bearingY2_1 * infoBINFNT.baseLine);
                infoBINFNT.lineHeight = (float)Math.Abs(Math.Round(infoBINFNT.lineHeight, 3));

                //Console.WriteLine(infoBINFNT.lineHeight);
            }
        }
    }
}

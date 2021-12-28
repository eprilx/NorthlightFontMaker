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

using System.Collections.Generic;

namespace NorthlightFontMaker
{
    public class BINFNTStruct
    {
        public general generalInfo;
        public List<charDesc> charDescList;
        public List<unkDesc> unkDescList;
        public List<advanceDesc> advanceDescList;
        public ushort[] idList;
        public byte[] DDSTextures;
        public BINFNTStruct()
        {
            generalInfo = new();
            charDescList = new();
            advanceDescList = new();
            unkDescList = new();
        }
        public class general
        {
            public int magicBytes; // = 3
            public uint charsCount;
            public float baseLine;
            public float lineHeight;
            public uint widthImg;
            public uint heightImg;
        }

        public class charDesc
        {
            // 64 BYTE
            // _1 = _2
            // every value should be divided for baseLine except UVMapping (divided for width/height of texture)
            public float bearingX1_1; // = xoffset
            public float bearingY2_1; // = lineHeight - yoffset - height char
            public float UVLeft_1;
            public float UVBottom_1;
            public float bearingX2_1; // = xoffset + width char
            public float bearingY2_2; // = lineHeight - yoffset - height char
            public float UVRight_1;
            public float UVBottom_2;
            public float bearingX2_2; // = xoffset + width char
            public float bearingY1_1; // = bearingY2 + height char = lineHeight - yoffset
            public float UVRight_2;
            public float UVTop_1;
            public float bearingX1_2; // = xoffset
            public float bearingY1_2; // = bearingY2 + height char = lineHeight - yoffset
            public float UVLeft_2;
            public float UVTop_2;
        }
        public class unkDesc
        {
            // 12 byte
            public ushort n1; // =0
            public ushort n2; // =1
            public ushort n3; // =2
            public ushort n4; // =0
            public ushort n5; // =2
            public ushort n6; // =3
        }
        public class advanceDesc
        {
            // 44 byte
            // _1 = _2
            // every value should be divided for baseLine
            public ushort plus4; // +4
            public ushort numb4; // = 4
            public ushort plus6; // +6
            public ushort numb6; // = 6
            public uint chnl;
            public float xadvance1_1; // = 0 ??
            public float yoffset1_1; // = yoffset2 - height
            public float xadvance2_1; // = xadvance
            public float yoffset1_2; // = yoffset2 - height
            public float xadvance2_2; // = xadvance
            public float yoffset2_1; // = -Yoffset
            public float xadvance1_2; // = 0 ??
            public float yoffset2_2; // = -Yoffset
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    public class SkillItem
    {
        public enum skillType
        {
            ReviveType = 8,
            HealHPType = 7,
            HealSPType = 6,
            Seal_Reviving = 3,
        }
        public string Name;
        public byte UnknownByte1; // (val ^ 0xFD) - 4
        public UInt16 SkillID; // (val ^ 0x6EA0) - 4
        public UInt16 SP; // (val ^ 0x6EA0) - 4
        public byte Elementfor; // (val ^ 0xFD) - 4
        public UInt16 UnknownWord2; // (val ^ 0x6EA0) - 4
        public skillType SkillType; // (val ^ 0xFD) - 4
        public byte SkillTreeRow; // (val ^ 0xFD) - 4
        public byte UnknownByte5; // (val ^ 0xFD) - 4
        string Reserved; // ?????
        public byte UnknownByte6; // (val ^ 0xFD) - 4
        public byte UnknownByte7; // (val ^ 0xFD) - 4
        public byte UnknownByte8; // (val ^ 0xFD) - 4
        //UInt16 HP+/-? ; // (val ^ 0x6EA0) - 4
        public byte EffectLasts; // (val ^ 0xFD) - 4
        public byte UnknownByte24; // (val ^ 0xFD) - 4
        public byte UnknownByte9; // (val ^ 0xFD) - 4
        public UInt16 UnknownWord4; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord5; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord6; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord7; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord8; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord9; // (val ^ 0x6EA0) - 4
        byte DescritpionLength;
        public string Description;
        public UInt16 UnknownWord10; // (val ^ 0x6EA0) - 4
        public byte UnknownByte10; // (val ^ 0xFD) - 4
        public UInt16 UnknownWord11; // Skill ID repeated... (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord12; // (val ^ 0x6EA0) - 4
        public byte UnknownByte11; // (val ^ 0xFD) - 4
        public byte UnknownByte12; // (val ^ 0xFD) - 4
        public byte UnknownByte13; // (val ^ 0xFD) - 4
        public UInt16 UnknownWord13; // (val ^ 0x6EA0) - 4
        public byte MaxSkillLevel; // (val ^ 0xFD) - 4
        public byte UnknownByte15; // (val ^ 0xFD) - 4
        public byte UnknownByte16; // (val ^ 0xFD) - 4
        public byte UnknownByte17; // (val ^ 0xFD) - 4
        public byte Enemiestargeted; // (val ^ 0xFD) - 4
        public byte UnknownByte19; // (val ^ 0xFD) - 4
        public byte UnknownByte20; // (val ^ 0xFD) - 4
        public byte UnknownByte21; // (val ^ 0xFD) - 4
        public byte UnknownByte22; // (val ^ 0xFD) - 4
        public UInt16 UnknownWord14; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord15; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord16; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord17; // (val ^ 0x6EA0) - 4
        public UInt16 UnknownWord18; // (val ^ 0x6EA0) - 4
        public UInt32 UnknownDword1; // (val ^ 0x0BDEDEBF) - 4
        public UInt32 UnknownDword2; // (val ^ 0x0BDEDEBF) - 4
        public UInt32 UnknownDword3; // (val ^ 0x0BDEDEBF) - 4
        public UInt32 UnknownDword4; // (val ^ 0x0BDEDEBF) - 4
        public UInt32 UnknownDword5; // (val ^ 0x0BDEDEBF) - 4

        private UInt16 getWord(byte[] data, int ptr)
        {
            return (UInt16)((data[ptr + 1] << 8) + data[ptr]);
        }
        private UInt32 getDWord(byte[] data, int ptr)
        {
            return (UInt32)((data[ptr + 3] << 24) + (data[ptr + 2] << 16) + (data[ptr + 1] << 8) + data[ptr]);
        }
        private byte byteXor(byte v) { return (byte)((v ^ 0xFD) - 4); }
        private UInt16 wordXor(UInt16 v) { return (UInt16)((v ^ 0x6EA0) - 4); }
        private UInt32 dwordXor(UInt32 v) { return (UInt32)((v ^ 0x0BDEDEBF) - 4); }
        public void Load(byte[] data, int ptr)
        {
            byte len = data[ptr];
            Name = "";
            for (int n = 0; n < len; n++) { Name += (char)data[ptr + (20 - n)]; } ptr += 21;
            UnknownByte1 = byteXor(data[ptr]); ptr++;
            SkillID = wordXor(getWord(data, ptr)); ptr += 2;
            if (SkillID == 10023)
            {
            }
            SP = wordXor(getWord(data, ptr)); ptr += 2;
            Elementfor = byteXor(data[ptr]); ptr++;
            UnknownWord2 = wordXor(getWord(data, ptr)); ptr += 2;
            SkillType = (skillType)byteXor(data[ptr]); ptr++;
            SkillTreeRow = byteXor(data[ptr]); ptr++;
            UnknownByte5 = byteXor(data[ptr]); ptr++;
            ptr++;
            ptr += 13;
            ptr += 2;
            UnknownByte6 = byteXor(data[ptr]); ptr++;
            UnknownByte7 = byteXor(data[ptr]); ptr++;
            UnknownByte8 = byteXor(data[ptr]); ptr++;
            EffectLasts = byteXor(data[ptr]); ptr++;
            UnknownByte24 = byteXor(data[ptr]); ptr++;
            UnknownByte9 = byteXor(data[ptr]); ptr++;
            UnknownWord4 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord5 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord6 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord7 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord8 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord9 = wordXor(getWord(data, ptr)); ptr += 2;
            len = data[ptr]; Description = "";
            for (int n = 0; n < len; n++) { Description += (char)data[ptr + (30 - n)]; } ptr += 31;
            UnknownWord10 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownByte10 = byteXor(data[ptr]); ptr++;
            UnknownWord11 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord12 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownByte11 = byteXor(data[ptr]); ptr++;//skill id repeated
            UnknownByte12 = byteXor(data[ptr]); ptr++;
            UnknownByte13 = byteXor(data[ptr]); ptr++;
            UnknownWord13 = wordXor(getWord(data, ptr)); ptr += 2;
            MaxSkillLevel = byteXor(data[ptr]); ptr++;
            UnknownByte15 = byteXor(data[ptr]); ptr++;
            UnknownByte16 = byteXor(data[ptr]); ptr++;
            UnknownByte17 = byteXor(data[ptr]); ptr++;
            Enemiestargeted = byteXor(data[ptr]); ptr++;
            UnknownByte19 = byteXor(data[ptr]); ptr++;
            UnknownByte20 = byteXor(data[ptr]); ptr++;
            UnknownByte21 = byteXor(data[ptr]); ptr++;
            UnknownByte22 = byteXor(data[ptr]); ptr++;
            UnknownWord14 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord15 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord16 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord17 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord18 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownDword1 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDword2 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDword3 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDword4 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDword5 = dwordXor(getDWord(data, ptr)); ptr += 4;

        }
    }
}

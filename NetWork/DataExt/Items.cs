using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.Managers;
using PServer_v2.Other;
using PServer_v2.NetWork;

namespace PServer_v2.NetWork.DataExt
{
    public class cItem
    {
        public byte[] mydata;
        public string Name;// byte[] ItemName;
        public byte ItemType;
        public UInt16 ItemID;
        public UInt16 IconNum;
        public UInt16 LargeIconNum;
        public UInt16[] EquipImageNum;
        public UInt16[] statType;
        public byte UnknownByte1;
        public byte UnknownByte2;
        public UInt16[] statVal;
        public UInt16 unknonVal;
        public UInt16 unknonVal1;
        public byte UnknownByte3;
        public byte ItemRank;
        public byte EquipPos;
        public byte UnknownByte4;
        public UInt32[] ColorDef;
        public byte Unused;
        public byte Level;
        public UInt32 BuyingPrice;
        public UInt32 SellingPrice;
        public byte EquipLimit;
        public UInt16 UnknownWord1;
        public UInt32 UnknownDWord1;
        public byte SetID;
        public UInt32 AntiSeal;
        public UInt16 SkillID;
        public UInt16[] MaterialTypes;
        public string Desccription;
        public byte TentWidth;
        public byte TentHeight;
        public byte TentDepth;
        public UInt16 UnknownWord2;
        public byte InvWidth;
        public byte InvHeight;
        public byte UnknownByte5;
        public UInt16[] InTentImages;
        public UInt16 NpcID;
        public byte UnknownByte6;
        public byte UnknownByte7;
        public byte UnknownByte8;
        public byte UnknownByte9;
        public byte UnknownByte10;
        public byte UnknownByte11;
        public UInt16 Duration;
        public UInt16 UnknownWord4;
        public UInt16 CapsuleForm;
        public UInt16 UnknownWord6;
        public UInt16 UnknownWord7;
        public UInt32 UnknownDWord2;
        public UInt32 UnknownDWord3;
        public UInt32 UnknownDWord4;
        public UInt32 UnknownDWord5;
        public UInt32 UnknownDWord6;

        bool stackable;
        public bool tradeable;
        public bool dropable;
        public eWearSlot wearAt;
        public eWeaponType weaponType;
        public bool Stackable
        {
            get
            {
                return stackable;
            }
        }

        public bool Tool { get { if (ItemType == 29)return true; return false; } }
        public bool Fuel { get { if (ItemType == 29)return true; return false; } }
        public bool Food { get { if (ItemType == 29)return true; return false; } }
        public bool Veichle { get { if (ItemType == 39)return true; return false; } }
        public bool WarpTool { get { if (ItemType == 29)return true; return false; } }

        private UInt16 getWord(byte[] data, int ptr)
        {
            return (UInt16)((data[ptr + 1] << 8) + data[ptr]);
        }
        private UInt32 getDWord(byte[] data, int ptr)
        {
            return (UInt32)((data[ptr + 3] << 24) + (data[ptr + 2] << 16) + (data[ptr + 1] << 8) + data[ptr]);
        }
        private byte byteXor(byte v) { return (byte)((v ^ 0x9A) - 9); }
        private UInt16 wordXor(UInt16 v) { return (UInt16)((v ^ 0xEFC3) - 9); }
        private UInt32 dwordXor(UInt32 v) { return (UInt32)((v ^ 0x0B80F4B4) - 9); }

        public void Load()
        {
            int ptr = 0;
            byte[] data = mydata;
            byte len = data[ptr]; Name = "";
            for (int n = 0; n < len; n++) { Name += (char)data[ptr + (20 - n)]; } ptr += 21;
            ItemType = byteXor(data[ptr]); ptr++;
            ItemID = wordXor(getWord(data, ptr)); ptr += 2;
            IconNum = wordXor(getWord(data, ptr)); ptr += 2;
            LargeIconNum = wordXor(getWord(data, ptr)); ptr += 2;
            EquipImageNum = new UInt16[4];
            for (int n = 0; n < EquipImageNum.Length; n++) { EquipImageNum[n] = wordXor(getWord(data, ptr)); ptr += 2; }
            statType = new UInt16[2];
            for (int n = 0; n < statType.Length; n++) { statType[n] = wordXor(getWord(data, ptr)); ptr += 2; }
            UnknownByte1 = byteXor(data[ptr]); ptr++;
            UnknownByte2 = byteXor(data[ptr]); ptr++;
            statVal = new UInt16[2];
            statVal[0] = (UInt16)((getWord(data, ptr) ^ 0xF4B4) - 109); ptr += 2;
            unknonVal = (UInt16)((getWord(data, ptr) ^ 0xF4B4) - 109); ptr += 2;
            statVal[1] = (UInt16)((getWord(data, ptr) ^ 0xF4B4) - 109); ptr += 2;
            unknonVal1 = (UInt16)((getWord(data, ptr) ^ 0xF4B4) - 109); ptr += 2;
            UnknownByte3 = byteXor(data[ptr]); ptr++;
            ItemRank = byteXor(data[ptr]); ptr++;
            EquipPos = byteXor(data[ptr]); ptr++;
            UnknownByte4 = byteXor(data[ptr]); ptr++;
            ColorDef = new UInt32[16];
            for (int n = 0; n < ColorDef.Length; n++) { ColorDef[n] = dwordXor(getDWord(data, ptr)); ptr += 4; }
            Unused = byteXor(data[ptr]); ptr++;
            Level = byteXor(data[ptr]); ptr++;
            BuyingPrice = dwordXor(getDWord(data, ptr)); ptr += 4;
            SellingPrice = dwordXor(getDWord(data, ptr)); ptr += 4;
            EquipLimit = byteXor(data[ptr]); ptr++;
            UnknownWord1 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownDWord1 = dwordXor(getDWord(data, ptr)); ptr += 4;
            SetID = byteXor(data[ptr]); ptr++;
            AntiSeal = dwordXor(getDWord(data, ptr)); ptr += 4;
            SkillID = wordXor(getWord(data, ptr)); ptr += 2;
            if (ItemID == 34201)
            {
            }
            MaterialTypes = new UInt16[5];
            for (int n = 0; n < MaterialTypes.Length; n++) { MaterialTypes[n] = wordXor(getWord(data, ptr)); ptr += 2; }
            len = data[ptr]; Desccription = "";
            for (int n = 0; n < len; n++) { Desccription += (char)data[ptr + (254 - n)]; } ptr += 255;
            TentWidth = byteXor(data[ptr]); ptr++;
            TentHeight = byteXor(data[ptr]); ptr++;
            TentDepth = byteXor(data[ptr]); ptr++;
            UnknownWord2 = wordXor(getWord(data, ptr)); ptr += 2;
            InvWidth = byteXor(data[ptr]); ptr++;
            InvHeight = byteXor(data[ptr]); ptr++;
            UnknownByte5 = byteXor(data[ptr]); ptr++;
            InTentImages = new UInt16[2]; for (int n = 0; n < InTentImages.Length; n++) { InTentImages[n] = wordXor(getWord(data, ptr)); ptr += 2; }
            NpcID = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownByte6 = byteXor(data[ptr]); ptr++;
            UnknownByte7 = byteXor(data[ptr]); ptr++;
            UnknownByte8 = byteXor(data[ptr]); ptr++;
            UnknownByte9 = byteXor(data[ptr]); ptr++;
            UnknownByte10 = byteXor(data[ptr]); ptr++;
            UnknownByte11 = byteXor(data[ptr]); ptr++;
            Duration = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord4 = wordXor(getWord(data, ptr)); ptr += 2;
            CapsuleForm = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord6 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownWord7 = wordXor(getWord(data, ptr)); ptr += 2;
            UnknownDWord2 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDWord3 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDWord4 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDWord5 = dwordXor(getDWord(data, ptr)); ptr += 4;
            UnknownDWord6 = dwordXor(getDWord(data, ptr)); ptr += 4;

            #region Stackable flag
            switch (ItemType)
            {
                case 29: //noob items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = false;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.noob;
                    } break;
                case 28: //sword items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.sword;
                    } break;
                case 27: //spear items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.spear;
                    } break;
                case 26: //bow items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.bow;
                    } break;
                case 25: //wand items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.wand;
                    } break;
                case 24: //claw items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.claw;
                    } break;
                case 6: //axe items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.axe;
                    } break;
                case 5: //club items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.club;
                    } break;
                case 4: //fan items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.fan;
                    } break;
                case 3: //gun items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.hand;
                        weaponType = eWeaponType.gun;
                    } break;
                case 2: //clothes items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.body;
                        weaponType = eWeaponType.none;
                    } break;
                case 1: //headwear items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.head;
                        weaponType = eWeaponType.none;
                    } break;
                case 0: //arm items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.arm;
                        weaponType = eWeaponType.none;
                    } break;
                case 15: //foot items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.feet;
                        weaponType = eWeaponType.none;
                    } break;
                case 14: //special items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.special;
                        weaponType = eWeaponType.none;
                    } break;
                case 13: //box items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 10: //pet items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = false;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 9: //quest items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = false;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 55: //food items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 54: //int combat items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 53: //misc stack items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 52: //msic non stack items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 51: //tent
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = false;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 50: //manufactured items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 49: //tool items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 48: //furniture items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 63: //plant drops
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 62: //eatable drops items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 61: //ore items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 60: //rock items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 59: //clay items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 58: //wood items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 57: //other drops items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 56: //oil drops items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 39: //vehicles items
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 41: //castle item
                    {
                        stackable = false;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 38: //card items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 37: //pack items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 43: //water items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 42: //gem items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 40: // bomb diamond
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 30: //unknown items
                    {
                        stackable = true;
                        dropable = false;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
                case 32: //food items
                    {
                        stackable = true;
                        dropable = true;
                        tradeable = true;
                        wearAt = eWearSlot.none;
                        weaponType = eWeaponType.none;
                    } break;
            }
            #endregion
        }

    }

    

    public class cInvItem : ItemTemplate
    {       

        public cInvItem(cGlobals g)
        {
            this.globals = g;
            Clear();
        }       
    }


    public class cGroundItem : ItemTemplate
    {
        public UInt16 id = 0;
        public Int32 lifetime = 0;
        public TimeSpan Removein;
        public UInt16 x = 0;
        public UInt16 y = 0;
        public bool reserved;
        public ItemsinMapEntries Info;
        public byte entryid = 0;
        public bool Remove(TimeSpan time)
        {
            if (lifetime-- == 0 && !reserved && id > 0 || Removein < time && id > 0 && !reserved)
                return true;
            else
                return false;
        }

        public cGroundItem(cGlobals g)
        {
            this.globals = g;
        }
        public void CopyFrom(ItemsinMapEntries i)
        {
            id = (ushort)i.itemID;
            itemtype = globals.gItemManager.GetItemByID((ushort)i.itemID);
            damage = 0;
            lifetime = 0;
            x = (ushort)i.x;
            y = (ushort)i.y;
            entryid = (byte)i.clickID;
        }
    }
}

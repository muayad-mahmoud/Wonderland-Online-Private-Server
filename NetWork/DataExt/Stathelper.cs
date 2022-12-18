using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    public class cStats
    {
        UInt16 sStr; public UInt16 Str { get { return sStr; } set { sStr = value; } }
        UInt16 sInt; public UInt16 Int { get { return sInt; } set { sInt = value; } }
        UInt16 sWis; public UInt16 Wis { get { return sWis; } set { sWis = value; } }
        UInt16 sCon; public UInt16 Con { get { return sCon; } set { sCon = value; } }
        UInt16 sAgi; public UInt16 Agi { get { return sAgi; } set { sAgi = value; } }
        UInt16 sAtk;
        UInt16 sDef;
        UInt16 sMatk;
        UInt16 sMdef;
        UInt16 sSpd;
        UInt16 sPts;
        UInt32 maxHP;
        UInt16 maxSP;
        UInt16 potential; public UInt16 Potential { get { return potential; } set { potential = value; } }
        UInt16 sPoints; public UInt16 SPoints { get { return sPoints; } set { sPoints = value; } }
        public UInt32 maxExp { get { return (uint)CalcMaxExp(own.rebirth, own.level); } }
        uint totalExp; public uint TotalExp { get { return totalExp; } set { totalExp = value; } }
        int curExp; public int CurExp { 
            get 
            {
                int res = (int)totalExp;
                for (int a = 0; a < own.level; a++)
                {
                    res -= CalcMaxExp(own.rebirth, a);
                }
                return res;// ((int)totalExp - CalcMaxExp(own.rebirth, (own.level - 1)));
            }
        }
        UInt32 curHP; public UInt32 CurHP { get { return curHP; } set { curHP = value; } }
        UInt16 curSP; public UInt16 CurSP { get { return curSP; } set { curSP = value; } }

        public UInt16 sFullAtk;
        public UInt16 sFullDef;
        public UInt16 sFullMatk;
        public UInt16 sFullMdef;
        public UInt16 sFullSpd;
        UInt32 fullHP; public UInt32 MaxHP { get { return fullHP + hpPlus; } }
        UInt16 fullSP; public UInt16 MaxSP { get { return (UInt16)(fullSP + spPlus); } }
        UInt16 hpPlus = 0;
        UInt16 spPlus = 0;
        cGlobals globals;
        cCharacter own;


        public cStats(cCharacter t,cGlobals src)
        {
            sStr = 0;
            sInt = 0;
            sWis = 0;
            sCon = 0;
            sAgi = 0;
            sPts = 0;
            own = t;
            globals = src;
        }

        public void LoadStats(string statStr, UInt32 curHP, UInt16 curSP)
        {
            string[] words = statStr.Split(' ');
            if (words.Length > 8)
            {
                sPts = UInt16.Parse(words[0]);
                sStr = UInt16.Parse(words[1]);
                sInt = UInt16.Parse(words[2]);
                sWis = UInt16.Parse(words[3]);
                sCon = UInt16.Parse(words[4]);
                sAgi = UInt16.Parse(words[5]);
                totalExp = UInt16.Parse(words[6]);
                sPoints = UInt16.Parse(words[7]);
                potential = UInt16.Parse(words[8]);

                this.curHP = curHP;
                this.curSP = curSP;
            }
        }
        public string GetStatString()
        {
            string s = "'" + sPts + " " + sStr + " " + sInt + " " + sWis + " " + sCon + " " + sAgi + " " + totalExp +
                " " + sPoints + " " + potential + "'";
            return s;
        }

        public int CalcMaxExp(byte rebirth, int Level)
        {
            if (rebirth == 0)
            {
                var e = (int)Math.Round(Math.Pow((Level + 1), 3.1) + 5);
                return (int)Math.Round(Math.Pow((Level + 1), 3.1) + 5);
            }
            else
            {
                if (Level < 150)
                {
                    return (int)Math.Pow((double)(Level + 1), (3.3)) + 50;
                }
                else
                {
                    return (int)Math.Pow((Level + 1), (3.3)) + (int)Math.Pow((Level + 1 - 150), 4.9);
                }
            }
        }
        public void CalcBaseStats(byte element, byte lvl, byte rebirth, byte job) //this function sets the stas according to pts
        {
            if (rebirth > 0) lvl += 100;
            switch (element)
            {
                case 1: //earth
                    {
                        sAtk = (UInt16)((lvl * 2) + (sStr * 2));
                        sDef = (UInt16)((lvl * 2) + (sCon * 2));
                        sMatk = (UInt16)((lvl * 2) + (sInt * 2));
                        sMdef = (UInt16)((lvl * 2) + (sWis * 2));
                        sSpd = (UInt16)((lvl * 2) + (sAgi * 2));
                        maxHP = (UInt32)((lvl * 1) + (sCon * 5) + 180);
                        maxSP = (UInt16)((lvl * 1) + (sWis * 6) + 94);
                    } break;
                case 2: //water
                    {
                        sAtk = (UInt16)((lvl * 2) + (sStr * 2));
                        sDef = (UInt16)((lvl * 2) + (sCon * 2));
                        sMatk = (UInt16)((lvl * 2) + (sInt * 2));
                        sMdef = (UInt16)((lvl * 2) + (sWis * 2));
                        sSpd = (UInt16)((lvl * 2) + (sAgi * 2));
                        maxHP = (UInt32)((lvl * 1) + (sCon * 5) + 180);
                        maxSP = (UInt16)((lvl * 1) + (sWis * 6) + 94);
                    } break;
                case 3: //fire
                    {
                        sAtk = (UInt16)((lvl * 2) + (sStr * 2));
                        sDef = (UInt16)((lvl * 2) + (sCon * 2));
                        sMatk = (UInt16)((lvl * 2) + (sInt * 2));
                        sMdef = (UInt16)((lvl * 2) + (sWis * 2));
                        sSpd = (UInt16)((lvl * 2) + (sAgi * 2));
                        maxHP = (UInt32)((lvl * 1) + (sCon * 5) + 180);
                        maxSP = (UInt16)((lvl * 1) + (sWis * 6) + 94);
                    } break;
                case 4: //wind
                    {
                        sAtk = (UInt16)((lvl * 2) + (sStr * 2));
                        sDef = (UInt16)((lvl * 2) + (sCon * 2));
                        sMatk = (UInt16)((lvl * 2) + (sInt * 2));
                        sMdef = (UInt16)((lvl * 2) + (sWis * 2));
                        sSpd = (UInt16)((lvl * 2) + (sAgi * 2));
                        maxHP = (UInt32)((lvl * 1) + (sCon * 5) + 180);
                        maxSP = (UInt16)((lvl * 1) + (sWis * 6) + 94);
                    } break;
            }
            switch (job)
            {
                case 1:
                    {
                    } break;
                case 2:
                    {
                    } break;
                case 3:
                    {
                    } break;
                case 4:
                    {
                    } break;
            }
        }
        public void CalcFullStats(cInvItem[] clothes) //this function sets the stas according to pts
        {
            sFullAtk = sAtk;
            sFullDef = sDef;
            sFullSpd = sSpd;
            sFullMdef = sMdef;
            sFullMatk = sMatk;
            fullHP = maxHP;
            fullSP = maxSP;
            hpPlus = 0;
            spPlus = 0;
            for (int n = 1; n < 6; n++)
            {
                if (clothes[n].ID != 0)
                {
                    if (clothes[n].itemtype.statType[0] > 0)
                        AddWeaponStat((byte)clothes[n].itemtype.statType[0], clothes[n].itemtype.statVal[0]);
                    if (clothes[n].itemtype.statType[1] > 0)
                        AddWeaponStat((byte)clothes[n].itemtype.statType[1], clothes[n].itemtype.statVal[1]);
                }
            }
        }
        
        public void Send8_1(cInvItem[] clothes, bool levelup = false) //this function sets the stas according to pts
        {

            if (levelup)
            {
                Send_1(36,totalExp,0);
                Send_1(35,own.level,0);
                Send_1(37,(uint)(own.level-1),0);
                Send_1(38,sPoints,0);
            }
            for (int n = 1; n < 6; n++)
            {
                if (clothes[n].ID != 0)
                {
                    if (clothes[n].itemtype.statType[0] > 0)
                    {
                        byte stat = GetStatTypeFromWeapon((byte)clothes[n].itemtype.statType[0]);
                        UInt32 value = GetStatByType(stat, false);
                        if (value > 0) Send_1(stat, value, 0);
                    }
                    if (clothes[n].itemtype.statType[1] > 0)
                    {
                        byte stat = GetStatTypeFromWeapon((byte)clothes[n].itemtype.statType[1]);
                        UInt32 value = GetStatByType(stat, false);
                        if (value > 0) Send_1(stat, value, 0);
                    }
                }
            }
            if (curHP > fullHP) { Send_1(25, fullHP, 0); curHP = fullHP; }
            if (curSP > fullSP) { Send_1(26, fullSP, 0); curSP = fullSP; }

            Send_1(207, hpPlus, 0); //a plus to hp
            Send_1(25, curHP, 0);
            Send_1(208, spPlus, 0); //a plus to sp
            Send_1(26, curSP, 0);
            Send_1(210, 3, 0);
            Send_1(41, sFullAtk, 0);
            Send_1(211, 0, 0);
            Send_1(42, sFullDef, 0);
            Send_1(214, 3, 0);
            Send_1(45, sFullSpd, 0);
            Send_1(215, 0, 0);
            Send_1(43, sFullMatk, 0);
            Send_1(216, 0, 0);
            Send_1(44, sFullMdef, 0);

        }
        
        void AddWeaponStat(byte weaponStat, UInt32 value)
        {
            switch (weaponStat)
            {
                case 210: sFullAtk = (UInt16)((Int16)(sFullAtk) + value); break;
                case 213: sFullDef = (UInt16)((Int16)(sFullDef) + value); break;
                case 214: sFullSpd = (UInt16)((Int16)(sFullSpd) + value); break;
                case 232: sFullMdef = (UInt16)((Int16)(sFullMdef) + value); break;
                case 233: sFullMatk = (UInt16)((Int16)(sFullMatk) + value); break;
                case 209: hpPlus = (UInt16)value; break;
                case 208: spPlus = (UInt16)value; break;
            }
        }
        byte GetStatTypeFromWeapon(byte weaponStat)
        {
            byte statType = 0;
            switch (weaponStat)
            {
                case 210: statType = 41; break;
                case 213: statType = 42; break;
                case 214: statType = 45; break;
                case 232: statType = 44; break;
                case 233: statType = 43; break;
                //case 209: statType = 25; break;
                //case 208: statType = 26; break;
            }
            return statType;
        }
        UInt32 GetStatByType(byte statType, bool full)
        {
            UInt32 value = 0;
            if (full)
            {
                switch (statType)
                {
                    case 41: value = sFullAtk; break;
                    case 42: value = sFullDef; break;
                    case 45: value = sFullSpd; break;
                    case 44: value = sFullMdef; break;
                    case 43: value = sFullMatk; break;
                    //case 25: value = fullHP; break;
                    //case 26: value = fullSP; break;
                }
            }
            else
            {
                switch (statType)
                {
                    case 41: value = sAtk; break;
                    case 42: value = sDef; break;
                    case 45: value = sSpd; break;
                    case 44: value = sMdef; break;
                    case 43: value = sMatk; break;
                    case 25: value = maxHP; break;
                    case 26: value = maxSP; break;
                }
            }
            return value;
        }
        
        public void FillHP()
        {
            curHP = fullHP;
        }
        public void FillSP()
        {
            curSP = fullSP;
        }

        public void SetBy9_1(cRecvPacket p)
        {
            sStr = p.GetByte(15);
            sCon = p.GetByte(16);
            sInt = p.GetByte(17);
            sWis = p.GetByte(18);
            sAgi = p.GetByte(19);
            byte body = p.GetByte(2);
            byte head = p.GetByte(4);

            switch (body)
            {
                case 1:
                    {
                        //Rocco
                        sInt += 2;
                        sAgi++;
                    } break;
                case 2:
                    {
                        if (head == 0)
                        {
                            //Nina
                            sInt++;
                            sWis++;
                            sAgi++;
                        }
                        else
                        {
                            //Betty
                            sInt++;
                            sAgi += 2;
                        }
                    } break;
                case 3:
                    {
                        if (head == 0)
                        {
                            //Daniel
                            sStr++;
                            sCon += 2;
                        }
                        else if (head == 1)
                        {
                            //Sid             010
                            sStr += 2;
                            sCon++;
                        }
                        else if (head == 2)
                        {
                            //More            020
                            sWis += 2;
                            sAgi++;
                        }
                        else
                        {
                            //Kurogane        030
                            sStr++;
                            sCon++;
                            sInt++;
                        }
                    } break;
                case 4:
                    {
                        if (head == 0)
                        {
                            //Iris            000
                            sStr++;
                            sCon += 2;
                        }
                        else if (head == 1)
                        {
                            //Lique           010
                            sStr += 2;
                            sAgi++;
                        }
                        else if (head == 2)
                        {
                            //Vanessa         020
                            sInt += 3;
                        }
                        else if (head == 3)
                        {
                            //Breillat        030	
                        }
                        else if (head == 4)
                        {
                            //Jessica         040
                            sInt++;
                            sWis += 2;
                        }
                        else if (head == 5)
                        {
                            //Konnotsuroko    050
                            sInt += 2;
                            sWis++;
                        }
                        else if (head == 6)
                        {
                            //Maria           060
                            sInt += 2;
                            sAgi++;
                        }
                        else
                        {
                            //Karin           070
                            sInt++;
                            sWis++;
                            sAgi++;
                        }
                    } break;
            }
        }

        public void AddtoStat(byte statType, byte ammt,bool Sub=false)
        {
            if (sPoints == 0) return;
            SPoints -= ammt;
            
            switch (statType)
            {
                case 27://int
                    {
                        sInt += ammt;
                        CalcBaseStats((byte)own.element, own.level, own.rebirth, (byte)own.job);
                        CalcFullStats(own.eq.clothes);
                        Send_1(27, sInt, 0);
                        Send_1(43, sFullMatk, 0);
                        Send_1(38, SPoints, 0);
                    }break;
                case 28://str
                    {
                        sStr += ammt;
                        CalcBaseStats((byte)own.element, own.level, own.rebirth, (byte)own.job);
                        CalcFullStats(own.eq.clothes);
                        Send_1(28, sStr, 0);
                        Send_1(41, sFullAtk,0);
                        Send_1(38, SPoints, 0);
                    }break;
                case 29://con
                    {
                        sCon += ammt;
                        CalcBaseStats((byte)own.element, own.level, own.rebirth, (byte)own.job);
                        CalcFullStats(own.eq.clothes);
                        Send_1(29, sCon, 0);
                        Send_1(42, sFullDef, 0);
                        Send_1(205, fullHP, 0);
                        Send_1(38, SPoints, 0);
                        Send_1(205, fullHP, 0);
                    }break;
                case 30://agi
                    {
                        sAgi += ammt;
                        CalcBaseStats((byte)own.element, own.level, own.rebirth, (byte)own.job);
                        CalcFullStats(own.eq.clothes);
                        Send_1(30, sAgi, 0);
                        Send_1(45, sFullSpd, 0);
                        Send_1(38, SPoints, 0);
                    }break;
                case 33://wis
                    {
                        sWis += ammt;
                        CalcBaseStats((byte)own.element, own.level, own.rebirth, (byte)own.job);
                        CalcFullStats(own.eq.clothes);
                        Send_1(33, sWis, 0);
                        Send_1(44, sFullMdef, 0);
                        Send_1(206, fullSP, 0);
                        Send_1(38, SPoints, 0);
                    }break;
            }
        }
        public void RemfromStat(byte statType, byte ammt)
        {
        }

        public void Send_1(byte stat, UInt32 ammt, UInt32 skill)
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(8, 1);
            p.AddByte(stat);
            p.AddByte(1);
            p.AddDWord(ammt);
            p.AddDWord(skill);
            p.SetSize();
            p.character = own;
            p.Send();
        }

    }
}

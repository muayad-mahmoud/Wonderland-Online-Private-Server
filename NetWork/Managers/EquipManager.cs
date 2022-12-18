using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.Managers
{
    public class cEquipManager
    {
        cCharacter Owner;
        cGlobals globals;
        public cInvItem[] clothes = new cInvItem[7];

        public cEquipManager(cGlobals src, cCharacter ow)
        {
            Owner = ow;
            globals = src;
            for (int n = 0; n < 7; n++) clothes[n] = new cInvItem(globals);

        }
        public byte[] GetFullClothes()
        {
            int at = 0;
            byte[] data = new byte[12];
            for (int n = 1; n < 7; n++)
                if (clothes[n].ID != 0)
                { globals.SetWord(clothes[n].ID, ref data, at); at += 2; }

            return data;
        }
        public byte[] GetWearingClothes()
        {
            byte[] ret = null;

            byte ammt = CountClothes();
            if (ammt > 0)
            {
                byte[] data = GetFullClothes();
                ret = new byte[ammt * 2];
                Array.Copy(data, ret, ammt * 2);
            }
            return ret;
        }
        public byte CountClothes()
        {
            byte ammt = 0;
            for (int n = 1; n < 7; n++)
                if (clothes[n].ID != 0) ammt++;
            return ammt;
        }
        byte[] Get_23_11()
        {
            byte[] data = null;
            int ammt = CountClothes();
            if (ammt > 0)
            {
                data = new byte[ammt * 19];
                int at = 0;
                for (int n = 1; n < 7; n++)
                {
                    if (clothes[n].ID != 0)
                    {
                        globals.SetWord(clothes[n].ID, ref data, at); at += 2;
                        data[at] = (byte)clothes[n].damage; at++;
                        for (int z = 0; z < 16; z++)
                        { data[at] = 0; at++; }
                    }
                }
            }
            return data;
        }
        public cInvItem RemoveEQ(byte clothesSlot,byte dst = 0)
        {
            cInvItem i = new cInvItem(globals);
            i.CopyFrom(clothes[clothesSlot]);
            clothes[clothesSlot].Clear();
            int dstCell;
            return i;
        }
        public void logClothes()
        {
            string s = "";
            for (int n = 1; n < 7; n++)
                s += n.ToString("0") + ") " + clothes[n].ID.ToString("00000") + " " + clothes[n].ammt.ToString("00") + "\r\n";
            globals.Log(s);
        }
        public bool WearEQ(byte index)
        {
            bool ret = false;
            cInvItem i = new cInvItem(globals);
            var red = globals.NumbertoMatrix(index);
            i.CopyFrom(Owner.inv.mainInv[red[0]][red[1]]);
            Owner.inv.mainInv[red[0]][red[1]].Clear();
            if (i.ID > 0)
            {
                var retrem = SetEQ(i.itemtype.EquipPos, i);
                if (retrem.ID > 0)
                    Owner.inv.PlaceItem(retrem,retrem.ammt,index);
                Owner.stats.CalcFullStats(clothes);
                Owner.Send_8_1();//send ac8
                Send_17(index, index);
                Owner.map.Send_5_2(i.itemtype, Owner);
                ret = true;
            }
            
            return ret;
        }
        public bool unWearEQ(byte src,byte dst)
        {
            bool ret = false;
            cInvItem i = new cInvItem(globals);
            if (clothes[src].ID > 0)
            {
                i.CopyFrom(clothes[src]);//copy from clothes
                if (Owner.inv.PlaceItem(i, i.ammt, dst))
                {
                    RemoveEQ(src);
                    Send_16(src, dst);
                    Owner.stats.CalcFullStats(clothes);
                    Owner.Send_8_1();
                    Owner.map.Send_5_1(i.itemtype, Owner);
                    ret = true;
                }
                else
                    SetEQ(src, i);
            }

            return ret;
        }
        public cInvItem SetEQ(byte clothesSlot, cInvItem eq)
        { cInvItem i = new cInvItem(globals);
            try
            {
               
                if (clothes[clothesSlot].ID != 0)
                    i = RemoveEQ(clothesSlot);
                clothes[clothesSlot].CopyFrom(eq);
            }
            catch { }return i;
        }

        public void Send_EQS() //items worn while logging in
        {
            byte[] data = Get_23_11();
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 11);
            p.AddArray(data);
            p.SetSize();
            p.character = Owner;
            p.Send();
        }
        void Send_16(byte src, byte dst) //deequipping
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 16);
            p.AddByte(src);
            p.AddByte(dst);
            p.SetSize();
            p.character = Owner;
            p.Send();
        }

        void Send_17(byte src, byte dst) //equipping
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 17);
            p.AddByte(src);
            p.AddByte(dst);
            p.SetSize();
            p.character = Owner;
            p.Send();
        }

    }
}

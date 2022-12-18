using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    public delegate void RiceballEvent(UInt16 e, cCharacter k);
    public class cRiceBall
    {
        //public event PacketRawOut OutPacket;
        public event RiceballEvent UpdateMapRiceball;
        cCharacter own;
        cGlobals globals;
        public bool active;
        public UInt16 id;
        public UInt32 time;

        public cRiceBall(cGlobals g,cCharacter ty)
        {
            globals = g;
            own = ty;
            active = false;
            id = 0;
            time = 0;
        }

        public void Activate(cCharacter target)
        {
            if (id > 0)
            {
                if (!CheckForStop())
                {
                    active = true;
                    UpdateMapRiceball(id, target);// g.ac5.Send_5(id, target);
                    Send_207(1, 1, target);
                }
            }
        }

        public void Deactivate(cCharacter target)
        {
            if (id > 0)
            {
                if (!CheckForStop())
                {
                    active = false;
                    own.map.UpdateRiceBall(id, target); //g.ac5.Send_5(0, target);
                    Send_207(1, 2, target);
                }
            }
        }

        public void Start(byte invCell, cInvItem i, cCharacter target)
        {
            active = true;
            id = i.itemtype.NpcID; //will be set from i.item.npcID
            //id = 19064;
            time = 60;
            target.inv.RemoveInv(invCell, i.ammt);
            Send_9(invCell, i, target); //remove the item from inventory
            own.map.UpdateRiceBall(id, target); //g.ac5.Send_5(id, target);
            Send_207(1, 1, target);
            Send_15(target);
        }
        public void GMRiceBall(UInt16 npcid, cCharacter target)
        {
            active = true;
            id = npcid;
            time = 6000;
            //target.inv.RemoveInv(invCell, i);
            //g.ac23.Send_9(invCell, i, target); //remove the item from inventory
            UpdateMapRiceball(id, target); //g.ac5.Send_5(id, target);
            Send_207(1, 1, target);
            Send_15(target);
        }

        //AC 23
        public void Send_9(byte src, cInvItem i, cCharacter target) //remove item from inv
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 9);
            p.AddByte(src);
            p.AddByte(i.ammt);
            p.SetSize();
            p.character = own;
            p.Send();
        }
        public void Send_15(cCharacter target) //??? is used int equipping rice ball
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 15);
            p.SetSize();
            p.character = own;
            p.Send();
        }
        public void Send_207(byte b1, byte b2, cCharacter target) //??? something to do with equiping rice ball
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 207);
            p.AddByte(b1);
            p.AddByte(b2);
            p.SetSize();
            p.character = own;
            p.Send();
        }
        public void Send_207(byte b1, UInt16 minutes, cCharacter target) //??? something to do with equiping rice ball
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 207);
            p.AddByte(b1);
            p.AddWord(minutes);
            p.SetSize();
            p.character = own;
            p.Send();
        }


        public void Stop()
        {
            if (active)
            {
            }

            time = 0;
            id = 0;
            active = false;

        }

        public bool CheckForStop()
        {
            bool ret = false;
            double passed = 0;
            //calc passeed time
            //do a time -= passed;
            if (time < 1)
            {
                time = 0;
                id = 0;
                active = false;
                ret = true;
            }
            return ret;
        }



    }
}

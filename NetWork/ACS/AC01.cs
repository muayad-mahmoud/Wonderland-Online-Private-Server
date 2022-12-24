using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_1 : cAC
    {
        //public cRecvPacket rp;

        public cAC_1(cGlobals globals) : base(globals)
        {
            this.g = globals;
            //rp = null;
        }
        public void SwitchBoard()
        {
            //rp=g.packet;
        }
        //singlas client to enter char creation screen
        public void Send_1(UInt32 id, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(1, 1);
            p.AddDWord(id);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_3(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(1, 3);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_6() //disconnect
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(1, 6);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_9()
        {
            //server name plus a few extra data
            cSendPacket p = new cSendPacket(g);
            p.Header(1,9);
            p.AddByte(135);
            p.AddByte(3);
            p.AddByte(1);
            p.AddArray("My Server".ToCharArray());
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_11()//???
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(1, 11);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }


    }
}

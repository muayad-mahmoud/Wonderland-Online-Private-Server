using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_31:cAC
    {
        public cAC_31(cGlobals g)
        {
            this.g = g;
        }

        public void Send_7()//open npc record
        {
            cSendPacket d = new cSendPacket(g);
            d.Header(31);
            d.AddByte(7);
            d.SetSize();
            d.character = g.packet.character;
            d.Send();
            g.ac20.Send_9();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_26 : cAC
    {
        public cAC_26(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Send_4(UInt32 gold)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(26, 4);
            p.AddDWord(gold);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_10(UInt32 gold)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(26, 10);
            p.AddDWord(gold);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

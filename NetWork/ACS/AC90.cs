using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_90 : cAC
    {
        public cAC_90(cGlobals globals) : base (globals)
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
        public void Send_1(UInt16 value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(90, 1);
            p.AddWord(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

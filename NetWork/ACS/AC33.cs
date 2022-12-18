using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_33 : cAC
    {
        public cAC_33(cGlobals globals) : base (globals)
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
        public void Send_2(byte v1,byte v2, byte v3, byte v4)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(33, 2);
            p.AddByte(v1);
            p.AddByte(v2);
            p.AddByte(v3);
            p.AddByte(v4);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

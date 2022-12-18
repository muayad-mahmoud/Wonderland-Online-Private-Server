using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_53 : cAC
    {
        public cAC_53(cGlobals globals) : base (globals)
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
        /*public void Send_5(cFighter f, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(53, 5);
            p.AddByte(f.x);
            p.AddByte(f.y);
            p.character = target;
            p.SetSize();
            p.Send();
        }*/

    }
}

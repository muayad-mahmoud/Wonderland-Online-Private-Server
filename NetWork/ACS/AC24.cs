using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_24 : cAC
    {
        public cAC_24(cGlobals globals)
            : base(globals)
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
        public void Send_5(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(25, 5);
            p.AddByte(value);
            p.AddWord(0);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
            
        }
    }
}


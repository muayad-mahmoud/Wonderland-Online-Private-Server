using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_25 :cAC
    {
        public cAC_25(cGlobals globals)
            : base(globals)
        {
            this.g = globals;
        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.Log(str);
                    } break;

            }
        }
        public void Send_44(byte val,double v)//time related
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(25, 44);
            p.AddByte(val);
            p.AddDouble(v);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
    }
}

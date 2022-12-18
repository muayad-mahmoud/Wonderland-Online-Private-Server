using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_70 : cAC
    {
        public cAC_70(cGlobals globals)
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
        public void Send_1(byte value,string name,UInt32 id)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(70, 1);
            p.AddByte(value);
            p.AddString(name);
            p.AddDWord(id);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
            
        }
    }
}


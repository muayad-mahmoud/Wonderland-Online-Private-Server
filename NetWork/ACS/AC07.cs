using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_7 : cAC
    {
        public cAC_7(cGlobals globals) : base (globals)
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
        public void Send(UInt32 id, UInt16 map,UInt16 x,UInt16 y, cCharacter target)
        {
            cSendPacket sp = new cSendPacket(g);
            sp.Header(7);
            sp.AddDWord(id);
            sp.AddWord(map);
            sp.AddWord(x);
            sp.AddWord(y);
            sp.SetSize();
            sp.character = target;
            sp.Send();
        }

    }
}

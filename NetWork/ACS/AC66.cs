using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_66 : cAC
    {
        public cAC_66(cGlobals globals)
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
    }
}


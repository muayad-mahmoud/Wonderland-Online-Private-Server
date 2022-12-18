using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_89 : cAC
    {
        public cAC_89(cGlobals g)
        {
            this.g = g;
        }
            public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 0: Recv_0(); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.Log(str);
                    } break;
 
            }
        }
            void Recv_0()
            {
                g.ac90.Send_1(1536);
                //cAC_20.Send_11(h);
                //cAC_20.Send_10(h);

            }
        
    }
}

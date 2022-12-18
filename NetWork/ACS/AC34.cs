using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_34 : cAC
    {
        public cAC_34(cGlobals globals)
            : base(globals)
        {
            this.g = globals;
        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 1: Recv_1(); break;
                //case 2: Recv_2(packet, t); break;
                //case 3: Recv_3(packet, t); break;
                //case 4: Recv_4(packet, t); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.Log(str);
                    } break;
            }
        }
        public void Recv_1()//Request Points?
        {
            cSendPacket i = new cSendPacket(g);
            i.Header(54);
            i.AddArray(new byte[] { 190, 2, 2, 191, 2, 2, 189, 2, 2, 89, 2, 2, 90, 2, 2, 91, 2, 2 });
            i.SetSize();
            i.character = g.packet.character;
            i.Send();
            g.ac35.Send_4(g.packet.character.IM);
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_29 :cAC
    {
        public cAC_29(cGlobals g)
        {
            this.g = g;
        }

        public void SwitchBoard()
        {
            g.Log("Got here");
            switch (g.packet.b)
            {
                
                //case 6: break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;
            }
        }
        public void Send_6()//props keeper
        {
            cSendPacket d = new cSendPacket(g);
            d.Header(29);
            d.AddByte(6);
            d.SetSize();
            d.character = g.packet.character;
            d.Send();
            g.ac20.Send_9();
        }
    }
}

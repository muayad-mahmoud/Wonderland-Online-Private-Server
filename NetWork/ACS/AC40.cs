using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_40 : cAC
    {
        public cAC_40(cGlobals globals) : base (globals)
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
        public void Send_1(cCharacter c) //TODO send sidebar items
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(40, 1);
            p.AddByte(2); p.AddWord(11085); p.AddByte(1); p.AddByte(1); //skill at 1,1
            p.AddByte(2); p.AddWord(15085); p.AddByte(3); p.AddByte(8); //skill at 3,8
            //2, 77, 43, 1, 1, 2, 237, 58, 3, 8
            p.SetSize();
            p.character = c;
            p.Send();
        }

    }
}

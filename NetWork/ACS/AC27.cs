using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_27:cAC
    {
        public cAC_27(cGlobals g)
        {
            this.g = g;
        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 1: Recv_1(); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;
            }
        }

        public void Recv_1()
        {
            //buying from props keeper
            int ptr = 2;
            int amt_buying = g.packet.GetByte(ptr); ptr++;
            for (int a = 0; a < amt_buying; a++)
            {
                byte slot = g.packet.GetByte(ptr); ptr++;
                byte ammt = g.packet.GetByte(ptr); ptr++;
                g.packet.character.talkingto.Interact(DataExt.NpcEntries.Interaction_Type.Buying, 0, slot, ammt);
            }

        }
        public void Send_3()//related to props shopkeeper selling panel
        {
            cSendPacket k = new  cSendPacket(g);
            k.Header(27);
            k.AddByte(3);
            k.SetSize();
            k.character = g.packet.character;
            k.Send();
        }
    }
}

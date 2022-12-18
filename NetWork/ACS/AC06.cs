using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_6 : cAC
    {
        public cAC_6(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            //rp=g.packet;
            switch (g.packet.b)
            {
                case 1:
                    {
                        Recv_1();
                    } break;
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
            cCharacter c = g.packet.character;
            byte direction = g.packet.data[2];
            c.x = g.packet.GetWord(3);
            c.y = g.packet.GetWord(5);
            //WORD unknown = p->GetWord(7);
            c.UpdateWalking(direction);
        }


        public void Send_1(cCharacter charWalking,byte direction, cCharacter charSend) //walking
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(6, 1);
            p.AddDWord(charWalking.characterID);
            p.AddByte(direction);
            p.AddWord(charWalking.x);
            p.AddWord(charWalking.y);
            p.SetSize();
            p.character = charSend;
            p.Send();
        }
        public void Send_2(byte val) //walking
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(6, 2);
            p.AddByte(val);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

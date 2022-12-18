using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork.Managers;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_51 : cAC
    {
        public cAC_51(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 1:
                    {
                       // Recv_1();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        /*
        public void Recv_1()
        {
            string str = "";
            int i = 3;
            while (i < g.packet.data.Length)
            {
                str += (char)g.packet.data[i];
                i++;
            }
            g.packet.character.SetNick(str);
            g.gCharacterManager.NickNameChanged(g.packet.character);
        }*/
        public void Send_1(cFighter t, byte skill, ushort amt, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(51, 1);
            p.AddByte(t.x); p.AddByte(t.y);
            p.AddByte(skill);
            p.AddWord(amt);
            p.AddWord(0);
            p.SetSize();
            p.character = t.character;
            p.Send();
        }

    }
}

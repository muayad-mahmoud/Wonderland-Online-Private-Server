using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_8 : cAC
    {
        public cAC_8(cGlobals globals) : base (globals)
        {

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
        public void Send_1(byte stat, UInt32 ammt, UInt32 skill, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(8, 1);
            p.AddByte(stat);
            p.AddByte(1);
            p.AddDWord(ammt);
            p.AddDWord(skill);
            p.SetSize();
            p.character = target;
            if (stat != 35)
                p.Send();
            else
                target.map.SendtoCharacters(p);
        }
        public void Send_3(uint srcid, byte stat, UInt32 ammt, UInt32 skill, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(8, 3);
            p.AddDWord(srcid);
            p.AddByte(stat);
            p.AddByte(1);
            p.AddDWord(ammt);
            p.AddDWord(skill);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        void Recv_1()
        {
            int max = g.packet.GetByte(3);
            int ptr = 4;
            for (int a = 0; a < max; a++)
            {
                g.packet.character.stats.AddtoStat(g.packet.GetByte(ptr), (byte)g.packet.GetDWord(ptr+1)); ptr += 5;
            }
        }
    }
}

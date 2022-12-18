using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_32 : cAC
    {
        public cAC_32(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 1:
                    {
                        Recv_1();
                    } break;
                case 2:
                    {
                        Recv_2();
                    }break;
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
            byte emotebubble = g.packet.data[2];
            //g.gMapManager.EmoteBubble(g.packet.character, emotebubble);
        }
        public void Recv_2()
        {
            byte emote = g.packet.data[2];
            //g.gMapManager.Emote(g.packet.character, emote);
        }
        public void Send_1(List<cCharacter> cList, cCharacter target, byte value)
        {
            foreach (cCharacter c in cList)
            {
                cSendPacket p = new cSendPacket(g);
                p.Header(32, 1);
                p.AddDWord(target.characterID);
                p.AddByte(value);
                p.character = c;
                p.SetSize();
                p.Send();
            }
        }
        public void Send_2(UInt32 id, byte value, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(32, 2);
            p.AddDWord(id);
            p.AddByte(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_2(List<cCharacter> cList, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(32, 2);
            foreach (cCharacter c in cList)
            {
                p.AddDWord(c.characterID);
                p.AddByte(c.emote);
            }
            p.SetSize();
            p.character = target;
            p.Send();
        }

    }
}

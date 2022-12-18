using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork.Managers;
using PServer_v2.DataLoaders;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_50 : cAC
    {
        public cAC_50(cGlobals globals) : base (globals)
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
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Recv_1() //recieve an attack command
        {
            cCharacter c = g.packet.character;
            if (c.battle.active)
            {
                BattleCMD tmp = new BattleCMD();
                tmp.src = c.battle.FindFighter(g.packet.GetByte(2), g.packet.GetByte(3));
                tmp.dst = c.battle.FindFighter(g.packet.GetByte(4), g.packet.GetByte(5));
                tmp.skill = g.gskillManager.GetSkillbyID((ushort)g.packet.GetDWord(6));
                tmp.unknownbyte = g.packet.GetByte(8);
                tmp.unknownbyte2 = g.packet.GetByte(9);
                c.battle.PCAttack(tmp, c);
            }
        }
        public void Send_1(cFighter src,ushort skill,cFighter dst,bool miss,uint dmg, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(50, 1);
            p.AddWord(17);
            p.AddByte(src.x); p.AddByte(src.y);
            p.AddWord(skill);
            p.AddByte(0); p.AddByte(1);
            p.AddByte(dst.x); p.AddByte(dst.y);
            p.AddByte(1);
            p.AddByte(0);
            p.AddByte(1);
            if (!miss)
                p.AddByte(25);
            else
                p.AddByte(0);
            //Second part is damage calculation
            p.AddDWord(dmg);
            p.AddByte(1);
            p.character = target;
            p.SetSize();
            p.Send();
        }
        public void Send_6(cFighter f, byte val, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(50, 6);
            p.AddByte(f.x); p.AddByte(f.y);
            p.AddByte(val);
            p.SetSize();
            p.character = target;
            p.Send();
        }

    }
}

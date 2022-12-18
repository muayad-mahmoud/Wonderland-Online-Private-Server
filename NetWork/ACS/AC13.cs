using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_13 : cAC
    {
        public cAC_13(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 1: g.packet.character.Party.JoinTeam(g.gCharacterManager.getByID(g.packet.GetDWord(2))); break;//party request
                case 3: Recv_3(); break;
                case 4: g.gCharacterManager.getByID(g.packet.GetDWord(2)).Party.MemberLeave(g.packet.character); break;
                case 7: g.packet.character.Party.InvitetoTeam(g.gCharacterManager.getByID(g.packet.GetDWord(2))); break;
                case 8: Recv_8(); break;
                case 9: g.packet.character.Party.KickMember(g.gCharacterManager.getByID(g.packet.GetDWord(2))); break;
                case 10: Recv_10(); break;
                case 15: g.packet.character.Party.LeaderChanged(g.packet.GetByte(2),
                    g.gCharacterManager.getByID(g.packet.GetDWord(3)),
                    g.gCharacterManager.getByID(g.packet.GetDWord(7))); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Send_4(UInt32 id, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(13, 4);
            p.AddDWord(id);
            p.SetSize();
            p.character = target;
            p.Send();
        }

        void Recv_3()
        {
            //1 accept,3 no response
            var src = g.gCharacterManager.getByID(g.packet.GetDWord(3));
            switch (g.packet.GetByte(2))
            {
                case 1: g.packet.character.Party.AcceptMember(g.packet.GetByte(2), src, false); break;//accept
                case 3: Send_3(g.packet.character, g.packet.data[2], src); break;//no response
            }

        }
        void Recv_8()
        {
            //1 accept,3 no response
            var src = g.gCharacterManager.getByID(g.packet.GetDWord(3));
            switch (g.packet.GetByte(2))
            {
                case 1: src.Party.AcceptMember(g.packet.GetByte(2), g.packet.character, true); break;//accept
                case 3: Send_3(src, g.packet.data[2], g.packet.character); break;//no response
            }

        }
        void Recv_10()
        {
            switch (g.packet.GetByte(2))
            {
                case 1: g.packet.character.Party.AppointLeader(g.packet.GetByte(2), 
                    g.gCharacterManager.getByID(g.packet.GetDWord(3))); break;
                case 3: g.packet.character.Party.MakeLeader(); break;
            }
        }
        void Send_3(cCharacter src, byte value, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(13, 3);
            p.AddByte(value);
            p.AddDWord(src.characterID);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_6(cCharacter target)
        {
            //TODO
        }

    }
}

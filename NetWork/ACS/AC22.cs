using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_22
    {
        static cAC_22()
        {
        }
        public static void SwitchBoard(cRecvPacket packet, cCharacter t)
        {
            switch (packet.b)
            {
                default:
                    {
                        string str = "";
                        str += "Packet code: " + packet.a + ", " + packet.b + " [unhandled]\r\n";
                        Form1.Log(str);
                    } break;

            }
        }
        public static void Send_4(List<NpcEntries> npclist,cCharacter target)
        {
            int ct = 0;
            cSendPacket j = new cSendPacket();
            j.Header(22,4);
            foreach(NpcEntries h in npclist)
            {
                ct++;
                j.AddWord(h.clickId);
                j.AddWord(h.unknownword2);
                j.AddWord((ushort)h.x);
                j.AddWord((ushort)h.y);
                j.AddWord(h.unknownbyte1);
                j.AddDWord(0);
            }
            j.SetSize();
            cServer.Send(j, target);
        }
    }
}

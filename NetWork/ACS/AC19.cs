using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_19
    {
        static cAC_19()
        {
        }
        public static void SwitchBoard(cRecvPacket packet, cCharacter t)
        {
            switch (packet.b)
            {
                case 1: Recv_1(packet, t); break;
                case 2: Recv_2(packet, t); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + packet.a + ", " + packet.b + " [unhandled]\r\n";
                        Form1.Log(str);
                    } break;

            }
        }
        static void Recv_1(cRecvPacket r, cCharacter e)
        {
            var id = r.GetDWord(2);
            if (e.pets.GetByID((ushort)id).state == Status.RidingPet)
            {
                cAC_15.Send_17(e);
            }
            Send_1(e.pets.GetByID((ushort)id),e);
        }
        static void Recv_2(cRecvPacket r, cCharacter e)
        {
            Send_2(e);
            Send_7(e);
        }
        
        static void Send_2(cCharacter e)
        {
            cSendPacket i = new cSendPacket();
            i.Header(19, 2);
            i.SetSize();
            cServer.Send(i, e);
        }
        static void Send_7(cCharacter src)
        {
            cSendPacket i = new cSendPacket();
            i.Header(19,7);
            i.AddDWord(src.characterID);
            i.SetSize();
            Form1.cMapManager.GetMapByID(src.map).SendtoCharactersEx(i, src);
        }
        
        static void Send_1(cPet h,cCharacter e)
        {
            cSendPacket k = new cSendPacket();
            k.Header(19, 1);
            k.AddDWord(h.petID);
            k.SetSize();
            h.state = Status.Battle;
            cServer.Send(k, e);
            cAC_15.Send_4(h, e);
        }
    }
}

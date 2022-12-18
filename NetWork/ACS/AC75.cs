using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_75 : cAC
    {
        public cAC_75(cGlobals globals)
            : base(globals)
        {
            this.g = globals;
        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
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
        public  void Recv_1()
        {
            int ptr = 0;
            int ammtbuying = g.packet.GetByte(2); ptr += 3;
            for (int a = 0; a < ammtbuying; a++)
            {
                g.gImMall_Manager.BuyImItem((ushort)g.packet.GetWord(ptr), g.packet.GetByte(ptr + 3), g.packet.GetByte(ptr + 2), (byte)g.packet.GetWord(ptr + 4)
                        , g.packet.character);
                ptr += 6;
            }
        }
        public void Send_1(byte[] data)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(75, 1);
            p.AddArray(data);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
            
        }
        public void Send_7(byte value)
        {
            cSendPacket sp = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            //PackSend->Clear();
            sp.Header(75, 7);//PackSend->Header(63,2);
            sp.AddByte(value);//PackSend->AddDWord(id);
            sp.SetSize();//PackSend->SetSize();
            sp.character = g.packet.character;//PackSend->Character = pArg->Packet->Character;
            sp.Send();//pArg->SQueue->EnqueuePacket(PackSend);
        }
        public void Send_8(UInt16 value)
        {
            cSendPacket sp = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            //PackSend->Clear();
            sp.Header(75, 8);//PackSend->Header(63,2);
            sp.AddWord(value);//PackSend->AddDWord(id);
            sp.SetSize();//PackSend->SetSize();
            sp.character = g.packet.character;//PackSend->Character = pArg->Packet->Character;
            sp.Send();//pArg->SQueue->EnqueuePacket(PackSend);
        }

    }
}


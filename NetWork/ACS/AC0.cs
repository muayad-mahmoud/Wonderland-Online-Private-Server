using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_0 : cAC
    {
        public cAC_0(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            //rp=g.packet;
            Recv_0();
        }

        public void Recv_0()
        {
            //a connection request was recieved

            //sends the server info
            g.ac1.Send_9(); //server name
            g.ac54.Send();  //other server info
        }
        public void Send_19()
        {
            cSendPacket sp = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            //PackSend->Clear();
            sp.Header(0, 19);//PackSend->Header(63,2);
            sp.SetSize();//PackSend->SetSize();
            sp.character = g.packet.character;//PackSend->Character = pArg->Packet->Character;
            sp.Send();//pArg->SQueue->EnqueuePacket(PackSend);
            sp.disconnect = true;
        }
    }
}

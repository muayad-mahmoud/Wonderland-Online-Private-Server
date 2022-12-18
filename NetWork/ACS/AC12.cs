using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_12 : cAC
    {
        public cAC_12(cGlobals globals)
        {
            this.g = globals;
        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 1: //must be an acknowledgemnet of the ac12 send
                    {
                        Recv_1();
                    }break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }

        public void Send(UInt32 id,cCharacter target,WarpInfo warp)
        {
            cSendPacket sp = new cSendPacket(g);
            sp.Header(12);
            sp.AddDWord(id);
            sp.AddWord(warp.mapID);
            sp.AddWord((ushort)warp.x);
            sp.AddWord((ushort)warp.y);
            sp.AddWord(warp.clickID);
            sp.AddByte(0);
            sp.SetSize();
            sp.character = target;
            sp.Send();
        }
        public void Recv_1()
        {
           // if (g.packet.character.warping)
                //g.packet.character.map.CompleteClientWarp(g.packet.character);
        }
    }
}

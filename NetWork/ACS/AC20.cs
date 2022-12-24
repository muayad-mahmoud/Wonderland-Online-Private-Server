using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_20 : cAC
    {
        public cAC_20(cGlobals globals)
        {
            this.g = globals;
        }
        public void SwitchBoard()
        {
            
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 1:
                    {
                        Recv_1(); // clicked on npc
                    }break;
                case 6:
                    {
                        Recv_6(); //client requesting more info
                    }break;
                case 8: Recv_8(); break;//warp request given
                case 9: Recv_9(); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

 
            }
        }

        public void Recv_1() //TODO interact with object
        {
            //this is just to keep it from locking
           var npc = g.packet.character.map.GetNpc(g.packet.GetByte(2));
            if (npc == null)
                Send_8(g.packet.character);
            else
            {
                g.packet.character.talking = true;
                g.packet.character.talkingto = npc;

                try
                {
                    npc.Interact(NpcEntries.Interaction_Type.Talking);
                }
                catch { Send_8(g.packet.character); }
            }
                
        }

        public void Recv_6()
        {
            if (g.packet.character.DatatoSend.Count > 0)
            {
                if (g.packet.character.DatatoSend.Count == 1)
                {
                    var f = g.packet.character.DatatoSend.Dequeue();
                    f.character = g.packet.character;
                    f.Send();

                    g.ac20.Send_8(g.packet.character);
                    
                    if (g.packet.character.warping)
                    { g.ac5.Send_4(); g.packet.character.warping = false; }
                }
                else
                {
                    var f = g.packet.character.DatatoSend.Dequeue();
                    f.character = g.packet.character;
                    f.Send();
                }
            }
            else 
            {
                g.ac20.Send_8(g.packet.character);
                if (g.packet.character.talking)
                {
                    g.packet.character.talking = false;
                    g.packet.character.talkingto = null;
                }
                if (g.packet.character.warping2)
                { g.ac5.Send_4(); g.packet.character.warping2 = false; }
            }
        }
        public void Send_7(cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(20, 7);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        public void Send_8(cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(20, 8);
            p.SetSize();
            p.character = t;
            p.Send();           
        }
        public void Send_9()//open storage?
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(20, 9);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Recv_8()
        {
            UInt16 warpID = g.packet.GetWord(2);
            if (g.packet.character.map.MapID != 11094)
                g.packet.character.map.WarpRequest( WarpType.regular,g.packet.character,(int)warpID);
            else
                g.packet.character.map.WarpRequest(WarpType.regular,g.packet.character,0,
                    g.packet.character.lastMap);
        }
        public void Recv_9()
        {
            if (g.packet.data.Length > 2)
            {
                byte answerID = g.packet.GetByte(2);
                try
                {
                    g.packet.character.talkingto.Interact(NpcEntries.Interaction_Type.Answering, answerID);
                }
                catch { Send_8(g.packet.character); }
            }
        }
        public void Send_33(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(20, 33);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }


        public void DoWarp(cWarp warp)
        {
            if (warp == null)
            {
                g.ac20.Send_8(g.packet.character);
                return;
            }
            g.packet.character.map.WarpRequest( WarpType.regular,g.packet.character,0,warp);
        }
    }
}


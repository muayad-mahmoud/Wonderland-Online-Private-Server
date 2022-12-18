using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_35 : cAC
    {
        public cAC_35(cGlobals globals) : base(globals)
        {
            
        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 2:  //delete character
                    {
                        Recv_2();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Recv_2()
        {
            string pw = g.packet.GetString(14);
            byte slot = g.packet.GetByte(2);
            cUser u = g.gUserManager.GetListByID(g.packet.character.userID);
            cCharacter c;
            if (u != null)
            {
                if (slot == 1)
                {
                    c = g.gCharacterManager.GetDBByID(u.userID);
                }
                else
                {
                    c = g.gCharacterManager.GetDBByID(u.player2ID);
                }

                if (c != null)
                {
                    if (string.Compare(pw, c.password) == 0)
                    {
                        //delete the character from the dtabase
                        g.gCharacterManager.DeleteCharacter(c);
                        if (slot == 1) { u.charname1 = ""; g.gUserManager.UpdatePlayerID(u, 0, 1); }
                        else { u.charname2 = ""; g.gUserManager.UpdatePlayerID(u, 0, 2); }
                        g.ac24.Send_5(53);
                        g.ac24.Send_5(52);
                        g.ac24.Send_5(54);
                        g.ac24.Send_5(183);
                        g.ac20.Send_8(c);
                        Send_2(1, slot);
                    }
                    else
                    {
                        Send_2(3);
                    }
                }
                else Send_2(3);
            }
            else
            {
                Send_2(3);
            }
        }
        public void Send_2(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(35, 2);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_2(byte value1, byte value2)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(35, 2);
            p.AddByte(value1);
            p.AddByte(value2);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_11() 
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(35,11);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_4(int IM)
        {
            cSendPacket im = new cSendPacket(g);
            im.Header(35, 4);
            im.AddDWord((uint)IM);
            im.AddDWord(0);
            im.AddDWord(0);
            im.AddDWord(0);
            im.SetSize();
            im.character = g.packet.character;
            im.Send();
        }

    }
}

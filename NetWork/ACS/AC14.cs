using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_14 : cAC
    {
        public cAC_14(cGlobals globals)
            : base(globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 1: Recv_1(); break;
                case 2: Recv_2(); break;
                case 3: Recv_3(); break;
                case 4: Recv_4(); break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Recv_1()//someone sent mail
        {
            switch (g.packet.GetByte(2))
            {
                case 1:
                    {
                        string str = "";
                        for (int a = 7; a < g.packet.data.Length; a++)
                        {
                            str += (char)g.packet.data[a];
                        }
                        cCharacter re = g.gCharacterManager.getByID(g.packet.GetDWord(3));
                        if (re == null)
                        {
                            //cCharacter offlinecharacter = g.gCharacterManager.GetDBByID(g.packet.GetDWord(3));

                            //offlinecharacter.Mail.Recvfrom(g.packet.character, str);
                        }
                        else
                            g.packet.character.Mail.SendTo(re, str);
                    } break;
            }
        }
        public void Recv_2()//request to add friend
        {
            cCharacter trgt = g.gCharacterManager.getByID(g.packet.GetDWord(2));
            Send_2(g.packet.character.characterID, trgt);
        }
        public void Recv_3()//they accept/didnt answer/refuse
        {

            cCharacter tmp = g.gCharacterManager.getByID(g.packet.GetDWord(2));
            Send_3(g.packet.character.characterID, tmp, g.packet.GetByte(6));
            Send_3(tmp.characterID, g.packet.character, g.packet.GetByte(6));
            switch (g.packet.GetByte(6))
            {
                case 1:
                    {
                        tmp.Friends.AddFriend(g.packet.character);
                        g.packet.character.Friends.AddFriend(tmp);
                    } break;
            }

        }
        public void Recv_4()//request to delete
        {
            cCharacter tmp = g.gCharacterManager.getByID(g.packet.GetDWord(2));
            if (tmp != null)
            {
                tmp.Friends.RemoveFriend(g.packet.character);
                g.packet.character.Friends.RemoveFriend(tmp);
            }
        }
        public void Send_2(uint ID, cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(14, 2);
            p.AddDWord(ID);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        public void Send_3(uint ID, cCharacter t, byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(14, 3);
            p.AddDWord(ID);
            p.AddByte(value);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        
        public void Send_13(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(14, 13);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}


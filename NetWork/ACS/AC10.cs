﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_10 : cAC
    {
        public cAC_10(cGlobals globals) : base (globals)
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
        public void Recv_1()
        {
            string str = "";
            int i = 3;
            while (i < g.packet.data.Length)
            {
                str += (char)g.packet.data[i];
                i++;
            }
            g.packet.character.SetNick(str);
            g.gCharacterManager.NickNameChanged(g.packet.character);
        }
        public void Send_1(cCharacter sender, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(10, 1);
            p.AddDWord(sender.characterID);
            p.AddString(sender.nickname);
            p.character = target;
            p.SetSize();
            p.Send();
        }
        public void Send_3(UInt32 id, byte value, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(10, 3);
            p.AddDWord(id);
            p.AddByte(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }

    }
}

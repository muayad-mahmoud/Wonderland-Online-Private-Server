using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_9 : cAC
    {
        public cAC_9(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 1:
                    {
                        Recv_1();
                    } break;
                case 2:
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

        public void Recv_1()
        {
            cCharacter c = g.packet.character;
            if (c.logging)
            {
                string password = g.packet.GetString(20);
                if ((password.Length < 6) || (password.Length > 14))
                {
                    //make sure no save of data by setting values to 0
                    c.characterID = 0;
                    c.Disconnect();
                    return;
                }
                c.element = (Element)g.packet.GetByte(14);
                c.color1 = g.packet.GetDWord(6);
                c.color2 = g.packet.GetDWord(10);
                
                g.packet.character.body = (BodyStyle)g.packet.GetByte(2);
                g.packet.character.head = g.packet.GetByte(4);
                c.stats.SetBy9_1(g.packet);
                c.stats.CalcBaseStats((byte)c.element, 1, 0, 0);
                c.SetNoobClothes();
                c.stats.CalcFullStats(c.eq.clothes);
                c.stats.FillHP(); c.stats.FillSP();

                g.packet.character.stats.TotalExp = 6;
                g.packet.character.map.MapID = 11016; //ship map 10017;
                g.packet.character.x = 500; // ship x 1042;
                g.packet.character.y = 1000; //ship y 1075;
                g.packet.character.gold = 0;
                g.packet.character.rebirth = 0;
                g.packet.character.job = 0;
                g.packet.character.level = 1;
                g.packet.character.password = password;
                g.packet.character.state = 1;

                //create a character id for this new character
                g.packet.character.characterID = g.packet.character.userID + (byte)(g.packet.character.slot - 1);

                
                cUser u = g.gUserManager.GetListByID(g.packet.character.userID);
                if (u == null)
                {
                    g.packet.character.characterID = 0;
                    g.packet.character.Disconnect();//Disconnect;
                }
                else
                {
                    if (g.packet.character.slot == 1)
                    {
                        u.playerID = g.packet.character.characterID;
                        g.packet.character.name = u.charname1;
                    }
                    else
                    {
                        u.player2ID = g.packet.character.characterID;
                        g.packet.character.name = u.charname2;
                    }
                    if (g.gCharacterManager.WriteNewCharacter(g.packet.character))
                    {
                        if (g.gUserManager.UpdatePlayerID(u,g.packet.character.characterID,g.packet.character.slot))
                        {
                            g.packet.character.creating = false;
                            g.ac63.NormalLog(g.packet.character);
                        }
                    }
                }
            }

        }

        public void Recv_2() //a name was recived for char in char creation
        {
            if (g.packet.character.creating)
            {
                int nameLen = g.packet.data.Length - 2;
                if ((nameLen < 4) || (nameLen > 14))
                {
                    Send_3(1);
                }
                else
                {
                    string name = g.packet.GetStringRaw(2,nameLen);
                    if (g.gUserManager.isDuplicateName(name,g.packet.character.userID))
                    {
                        Send_3(1);
                    }
                    else if (g.gCharacterManager.isDuplicateName(name))
                    {
                        Send_3(1);
                    }
                    else
                    {
                        cUser u = g.gUserManager.GetListByID(g.packet.character.userID);
                        if (u == null)
                        {
                            Send_3(1);
                        }
                        else
                        {
                            if (g.packet.character.slot == 1) u.charname1 = name;
                            else u.charname2 = name;
                            Send_3(0);
                        }

                    }
                }
            }		

        }
        public void Send_3(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(9, 3);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }


    }
}

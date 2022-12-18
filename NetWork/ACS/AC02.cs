using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_2 : cAC
    {
        public cAC_2(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 2:
                    {
                        Recv_2();
                    } break;
                case 3:
                    {
                        Recv_3();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;
            }
        }

        public void Recv_2() //local
        {
            string str = "";
            int textLen = g.packet.data.Length - 2;
            str = g.packet.GetStringRaw(2, textLen);

            cCharacter c = g.packet.character;
            if (g.gUserManager.GetListByID(c.userID).SubIM(0))
            {

                string[] words = str.Split(' ');
                if (words.Length >= 1)
                {
                        switch (words[0])
                        {
                            case ":warp":
                                {
                                    cWarp w = new cWarp();
                                    UInt16 value = 0;
                                    try { value = UInt16.Parse(words[1]); }
                                    catch { }
                                    w.x = 500; w.y = 500; w.mapTo = value; w.id = 0;
                                    g.packet.character.map.WarpRequest(WarpType.SpecialWarp,g.packet.character,0,w);
                                    g.ac20.DoWarp(w);
                                    
                                } break;
                            case ":item":
                                {
                                    byte ammt = 1;
                                    int ct = words.Length;
                                    UInt16 itemid = 0;
                                    if (ct > 1)
                                    {

                                        try { itemid = UInt16.Parse(words[1]); }
                                        catch { }
                                        if (ct > 2)
                                        {
                                            int tmp;
                                            try { ammt = byte.Parse(words[2]); }
                                            catch { }
                                            tmp = ammt;
                                            for (int a = 0; a < tmp; a++)
                                            {
                                                if (g.gUserManager.GetListByID(c.userID).SubIM(200))
                                                    ammt = (byte)(a+1);
                                            }
                                        }
                                        cInvItem i = new cInvItem(g);
                                        i.ammt = ammt;
                                        i.ID = itemid;


                                        c.inv.RecieveItem(i);
                                    }
                                } break;
                            case ":npc":
                                {
                                    UInt16 npcid = 0; int ct = words.Length;
                                    if (ct > 1)
                                    {
                                        try { npcid = UInt16.Parse(words[1]); }
                                        catch { }
                                        c.riceBall.GMRiceBall(npcid, c);
                                        g.ac5.Send_5(npcid, c, c);

                                        /* {
                   cInvItem i = new cInvItem(g);
                   i = c.GetInventoryItem(cell); i.ammt = 1;
                   c.riceBall.Start(cell, i, c);
                   }*/
                                    }

                                } break;
                            case ":fight":
                                {
                                    //cBattle b = new cBattle(g);
                                    //b.Test(c);
                                } break;
                            case ":next":
                                {
                                    //cBattle b = c.battle;
                                    //if (b.active) b.StartRound();
                                } break;
                            case ":end":
                                {
                                    //cBattle b = new cBattle(g);
                                    //b.TestExit(c);
                                } break;
                            default:
                                {
                                    g.packet.character.map.LocalChat(g.packet.character, str);
                                } break;
                        }
                }
            }

            else g.packet.character.map.LocalChat(g.packet.character, str);

                //g.logList.Enqueue("[Local] <" + g.packet.character.name + "> " + str + "\r\n");
            
        }
        public void Send_2(cCharacter sender,cCharacter target,string text) //send a local chat
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(2, 2);
            p.AddDWord(sender.characterID);
            p.AddArray(text.ToCharArray());
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Recv_3() //whisper
        {
             var targetID = g.gCharacterManager.getByID(g.packet.GetDWord(2));
            string text = g.packet.GetStringRaw(6, g.packet.data.Length - 6);
            g.packet.character.Whisper(text, targetID);
        }
    }
}

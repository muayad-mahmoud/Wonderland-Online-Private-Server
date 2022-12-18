using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_23 : cAC
    {
        public cAC_23(cGlobals globals)
            : base(globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 2: //a player requests to pick up an item
                    {
                        Recv_2();
                    }break;
                case 3: //a player requests to drop an item
                    {
                        Recv_3();
                    } break;
                case 10:
                    {
                        Recv_10();
                    } break;
                case 11:
                    {
                        Recv_11();
                    }break;
                case 12:
                    {
                        Recv_12();
                    }break;
                case 15: Recv_15(); break;//use item
                case 54:
                    {
                        Recv_54();
                    } break;
                case 114:
                    {
                        Recv_114(); //rice ball was used
                    }break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Recv_2() //player requests to get item from map
        {
            UInt16 itemIndex = g.packet.GetWord(2);
            cCharacter c = g.packet.character;
            c.inv.GetItemMap(itemIndex);

        }
        public void Send_2(UInt16 itemIndex,cCharacter target,bool animate) //remove an item from the map
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 2);
            p.AddWord(itemIndex);
            if (animate)
                p.AddByte(1);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Recv_3() //player requests to drop item on map
        {
            byte srcCell = g.packet.data[2];
            byte ammt = g.packet.data[3];
            byte uk = g.packet.data[4];
            cCharacter c = g.packet.character;
            c.inv.DropItemMap(srcCell, ammt);
        }
        public void Send_3(cGroundItem item, cCharacter target) //drop item on map
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 3);
            p.AddWord(item.id);
            p.AddWord(item.x);
            p.AddWord(item.y);
            p.AddDWord(0);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_4(byte[] data,cCharacter target) //send items on map to user
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 4);
            p.AddArray(data);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Recv_10() //item moved in inventory
        {
            byte src = g.packet.data[2];
            byte ammt = g.packet.data[3];
            byte dst = g.packet.data[4];

            if ((src > 0) && (src < 51))
            {
                if ((dst > 0) && (dst < 51))
                {
                    if ((ammt > 0) && (ammt < 51))
                    {
                        cCharacter c = g.packet.character;
                        c.inv.MoveItem(src, ammt, dst, c);
                    }
                }
            }
        }
        public void Recv_11() //item selected to wear
        {
            byte loc = g.packet.data[2];
            if ((loc > 0) && (loc < 51))
            {
                //do any checks here to make sure we can wear item
                cCharacter c = g.packet.character;
                c.eq.WearEQ(loc);
            }
        }
        public void Recv_12() //item selected to remove
        {
            byte loc = g.packet.data[2];
            if ((loc > 0) && (loc < 7))
            {
                cCharacter c = g.packet.character;
                c.eq.unWearEQ(loc, g.packet.data[3]);
            }
        }
        public void Recv_15()
        {
            g.packet.character.inv.UseItem(g.packet.GetByte(2), g.packet.GetByte(3));
        }
        public void Send_15(cCharacter target) //??? is used int equipping rice ball
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 15);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_32(UInt32 id,cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 32);
            p.AddDWord(id);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        public void Recv_54()
        {
            Send_122(g.packet.character.characterID, g.packet.character);
        }
        public void Send_57(string text, cCharacter target) //sends sytem prompt message
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 57);
            p.AddByte(152);
            p.AddArray(text.ToCharArray());
            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_74(UInt32 id, byte value, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 74);
            p.AddDWord(id);
            p.AddByte(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }//warping
        public void Send_76(UInt32 id,cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            p.Header(23, 76);//PackSend->Header(63,2);
            p.AddDWord(id);//PackSend->AddDWord(id);
            p.SetSize();//PackSend->SetSize();
            p.character = target;//PackSend->Character = pArg->Packet->Character;
            p.Send();//pArg->SQueue->EnqueuePacket(PackSend);
        }//warping
        public void Send_102(cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 102);
            p.SetSize();
            p.character = t;
            p.Send();
            p = new cSendPacket(g);
            p.Header(20, 8);
            p.SetSize();
            p.character = t;
            p.Send();
        }//warping
        public void Send_112(UInt32 id,cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 112);
            p.AddDWord(id);
            p.SetSize();
            p.character = t;
            p.Send();
        }//warping

        public void Recv_114() //riceball was used
        {
            cCharacter c = g.packet.character;

            switch (g.packet.data[2])
            {
                case 1: //new riceball being used
                    {
                        byte cell = g.packet.data[3];
                        if ((cell > 0) && (cell < 51))
                        {
                            cInvItem i = new cInvItem(g);
                            i = c.inv.GetInventoryItem(cell); i.ammt = 1;
                            c.riceBall.Start(cell, i, c);
                        }
                    } break;
                case 2: //rice ball icon clicked while active
                    {
                        g.ac23.Send_207(2, (UInt16)c.riceBall.time,c);
                    } break;
                case 3: //request to change into riceball
                    {
                        c.riceBall.Activate(c);
                    } break;
                case 4: //request to change back normal
                    {
                        c.riceBall.Deactivate(c);
                    }break;
            }

        }

        public void Send_122(UInt32 id, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            p.Header(23, 122);//PackSend->Header(63,2);
            p.AddDWord(id);//PackSend->AddDWord(id);
            p.SetSize();//PackSend->SetSize();
            p.character = target;//PackSend->Character = pArg->Packet->Character;
            p.Send();//pArg->SQueue->EnqueuePacket(PackSend);
        }
        public void Send_132(UInt32 id,cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 132);
            p.AddDWord(id);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        public void Send_138(cCharacter t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 138);
            p.SetSize();
            p.character = t;
            p.Send();
        }


        public void Send_207(byte b1, byte b2,cCharacter target) //??? something to do with equiping rice ball
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 207);
            p.AddByte(b1);
            p.AddByte(b2);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_207(byte b1, UInt16 minutes, cCharacter target) //??? something to do with equiping rice ball
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 207);
            p.AddByte(b1);
            p.AddWord(minutes);
            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_106(byte val,UInt32 val2)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 106);
            p.AddByte(val);
            p.AddDWord(val2);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_140(byte val,Double t)//time related
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 140);
            p.AddByte(val);
            p.AddDouble(t);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_160(byte t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 160);
            p.AddByte(t);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_162(byte t)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(23, 162);
            p.AddByte(t);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_204(UInt16 val1)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(23, 204);
            f.AddWord(val1);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_208(byte val1, byte val2, UInt32 t)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(23, 204);
            f.AddByte(val1);
            f.AddByte(val2);
            f.AddDWord(t);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }

    }
}


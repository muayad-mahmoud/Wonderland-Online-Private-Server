using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_5 : cAC
    {
        public cAC_5(cGlobals globals)
            : base(globals)
        {

        }
        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 7:
                    {
                        Recv_7(); //??? not sure what it means, but server returns character's id
                    } break;
                case 17: //warp to sb, or carnie, or recorded spot
                    {
                        if (g.packet.character.state>0)
                            Recv_17();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;
            }
        }
        public void Send_0(cCharacter src,cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 0);
            p.AddDWord(src.characterID);
            byte[] data = src.Get_5_0_Data();
            if (data != null) p.AddArray(data);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_1(UInt32 id, UInt16 itemID, cCharacter target) //update other players on map removing clothing
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 1);
            p.AddDWord(id);
            p.AddWord(itemID);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_2(UInt32 id, UInt16 itemID, cCharacter target) //update other players on map equipped clothing
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 2);
            p.AddDWord(id);
            p.AddWord(itemID);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_3(cCharacter character) //logging in player info
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 3);
            p.AddByte((byte)character.element);
            p.AddDWord(character.stats.CurHP);
            p.AddWord(character.stats.CurSP);
            p.AddWord(character.stats.Str); //base str
            p.AddWord(character.stats.Con); //base con
            p.AddWord(character.stats.Int); //base int
            p.AddWord(character.stats.Wis); //base wis
            p.AddWord(character.stats.Agi); //base agi
            p.AddByte(character.level); //lvl
            p.AddDWord((uint)character.stats.TotalExp); //exp ???
            p.AddDWord((UInt32)(character.level - 1)); //lvl -1 ???
            p.AddDWord(character.stats.MaxHP); //max hp
            p.AddWord(character.stats.MaxSP); //max sp
            
            //-------------- 7 DWords
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(100);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            
            //--------------- Skills
            p.AddArray(character.skills.GetSkillData());
            //p.AddWord(1); //ammt of skills
            //p.AddWord(188); p.AddWord(1); p.AddWord(0); p.AddByte(0); //skill data
            //--------------- table with rebirth and job
            p.AddWord(0); p.AddWord(0);
            p.AddByte(character.rebirth);p.AddByte((byte)character.job); p.AddByte((byte)character.stats.Potential);

            p.SetSize();
            p.character = g.packet.character;
            p.Send();

            //player data
            //244, 68, 106, 0, 5, 3, 
            //1, 
            //134, 1, 0, 0, 
            //15, 0,  7, 0,  30, 0,  0, 0,   0, 0,   1, 0, 
            //11, 
            //22, 24, 0, 0, 
            //10, 0, 0, 0, 
            //134, 1, 0, 0,   105, 0, 
            //0, 0, 0, 0,     0, 0, 0, 0,    0, 0, 0, 0, 
            //0, 0, 0, 0,     0, 0, 0, 0,    0, 0, 0, 0, 
            //0, 0, 0, 0,     
            //5, 0,    
            //1, 0, 5, 24, 1, 0, 0, 
            //2, 0, 1, 0, 0,0, 0, 
            //16,0, 1, 0, 0, 0,0, 
            //35,0, 1, 0, 0, 0, 0, 
            //188, 0, 1,0, 0, 0, 0, 
            //0, 0, 0, 0, 0, 0, 0, 
        }

        public void Send_4()
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 4);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_5(UInt16 id, cCharacter src,cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 5);
            p.AddDWord(src.characterID);
            p.AddWord(id);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Recv_7()
        {
            byte value = g.packet.data[2];
            Send_8(g.packet.character.characterID, value,g.packet.character);
        }
        public void Send_8(UInt32 id, byte value, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 8);
            p.AddDWord(id);
            p.AddByte(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_11(UInt16 wVal, UInt32 dwVal)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 11);
            p.AddWord(wVal);
            p.AddDWord(dwVal);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_15(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 15);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

        public void Recv_17()
        {
            byte warpSlot = g.packet.data[2];
            switch (warpSlot)
            {
                
                case 4: //out of jail
                    {
                        if (g.packet.character.state == 1)
                        {
                            cWarp w = new cWarp();
                            w.id = 0; w.x = 302; w.y = 395; w.mapTo = 11020;
                            g.ac20.DoWarp(w);
                        }
                    } break;

                case 5: //jail
                    {
                        if (g.packet.character.state == 2)
                        {
                            cWarp w = new cWarp();
                            w.id = 0; w.x = 462; w.y = 335; w.mapTo = 11020;
                            g.ac20.DoWarp(w);
                        }
                    }break;
                case 6: //general warp request
                    {
                        cWarp w = new cWarp();
                        w.id = 0;
                        w.mapTo = g.packet.GetWord(3);
                        w.x = g.packet.GetWord(5);
                        w.y = g.packet.GetWord(7);
                        g.ac20.DoWarp(w);
                    }break;
                default:
                    g.packet.character.Spawnto(warpSlot);break;
            }
        }

        public void Send_14(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 14);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_16(byte value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 16);
            p.AddByte(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

        public void Send_13(byte value, UInt16 wVal)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 13);
            p.AddByte(value);
            p.AddWord(wVal);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_24(byte Val, UInt16 wVal)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 24);
            p.AddByte(Val);
            p.AddWord(wVal);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send_21(byte wVal)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(5, 21);
            p.AddByte(wVal);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }


        public void SpawnStartersBeach()
        {
            cSendPacket sp = new cSendPacket(g);
            sp.Header(0x17, 0x20);
            sp.AddDWord(g.packet.character.characterID);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x70);
            sp.AddDWord(g.packet.character.characterID);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x84);
            sp.AddDWord(g.packet.character.characterID);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x0c);
            sp.AddDWord(g.packet.character.characterID);
            sp.AddWord(0x2b08);
            sp.AddWord(0x01f4);
            sp.AddWord(0x03e8);
            sp.AddWord(0);
            sp.AddByte(0);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x05, 0x00);
            sp.AddDWord(g.packet.character.characterID);
            sp.AddWord(21172); // From here on = equipment [Fur Coat (M)]
            sp.AddWord(12099); // [Beast Fork +9]
            sp.AddWord(0x62f9); // knight manteau
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x4f);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x8a);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();


            sp = new cSendPacket(g);
            sp.Header(0x17, 0x7A);
            sp.AddDWord(g.packet.character.characterID);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x16, 0x04);
            sp.AddByte(0x01);
            sp.AddByte(0x00);
            sp.AddByte(0xff);
            sp.AddByte(0x00);
            sp.AddWord(0x02d5);
            sp.AddWord(0x0184);
            sp.AddWord(0x0101);
            sp.AddDWord(0);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x04);
            sp.AddWord(0x0301);
            sp.AddByte(0x01);
            sp.AddByte(0x00);
            sp.AddWord(0xa06a); //coconut
            sp.AddWord(0x0000);
            sp.AddWord(0x05ae);
            sp.AddWord(0x0367);
            sp.AddByte(0xa0);
            sp.AddByte(0x00);
            sp.AddWord(0x0000);
            sp.AddWord(0x0203);
            sp.AddByte(0x00);
            sp.AddWord(0xa06a); // coconut
            sp.AddWord(0x0000);
            sp.AddWord(0x01ce);
            sp.AddWord(0x021a);
            sp.AddByte(0xaa);
            sp.AddWord(0x0000);
            sp.AddByte(0x00);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x4c);
            sp.AddDWord(g.packet.character.characterID);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x17, 0x66);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();
/*
 * Ride Pet
 * 
            sp = new cSendPacket(g);
            sp.Header(0x0f, 0x10);
            sp.AddByte(0x01);
            sp.AddDWord(g.packet.character.characterID);
            sp.AddWord(17562); // La Tim
            sp.AddWord(0x0000);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();
 */

            sp = new cSendPacket(g);
            sp.Header(0x14, 0x08);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();

            sp = new cSendPacket(g);
            sp.Header(0x05, 0x04);
            sp.SetSize();
            sp.character = g.packet.character;
            sp.Send();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using PServer_v2.NetWork.Managers;
using PServer_v2.NetWork.DataExt;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_11 : cAC
    {
        public cAC_11(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 2:                   
                    {
                        Recv_2(); //a pk was initiated
                    }break;

                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Send_0(UInt16 value, cFighter f, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 0);
            p.AddDWord(f.character.characterID);
            p.AddWord(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_1(cFighter f,cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 1);
            p.AddByte(f.x); p.AddByte(f.y);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_1(byte value,cFighter f, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 1);
            p.AddByte(f.x); p.AddByte(f.y);
            p.AddByte(value);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Recv_2() //pk
        {
            cCharacter c = g.packet.character;
            byte pkType = g.packet.data[2]; //pk type
            UInt32 targetID = g.packet.GetDWord(3); //id of player that was attacked
            UInt16 clickID = g.packet.GetWord(7); //npc, or pc's index (not sure if used on pc pks)

            

            switch (pkType)
            {
                case 2: //pk against other pc
                    {
                        cCharacter t = g.gCharacterManager.getByID(targetID);
                        if (t != null)
                        {
                            if (t.map == c.map)
                            {
                                //also check to see if both chars have their pk turned on TODO
                                cMap m = g.gMapManager.GetMapByID(c.map.MapID);
                                if (m != null)
                                {
                                    m.StartPK(c, t);
                                }

                            }
                        }
                    } break;
                case 3: //pk againts npc
                    {
                        PServer_v2.DataLoaders.Npc target = g.gNpcManager.GetNpcbyID((ushort)targetID);
                        cFighter f = new cFighter(g);
                        f.character = null;
                        f.SetFrom(target);
                        f.pet = false;
                        f.player = false;
                        f.ready = false;
                        f.ukval1 = 3; 
                        f.type = pkType;
                        f.clickID = clickID;

                        cMap m = g.gMapManager.GetMapByID(c.map.MapID);
                        if (m != null)
                        {
                            m.StartPKNpc(c,f);
                        }

                    } break;
            }
            //get the character file for target
        }

        public void Send_4(byte bval1,UInt32 id, UInt16 wval, byte bval2, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 4);
            p.AddByte(bval1);
            p.AddDWord(id);
            p.AddWord(wval);
            p.AddByte(bval2);
            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_5(List<cFighter> flist, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 5);
            foreach (cFighter f in flist)
            {
                p.AddByte(f.ukval1);//p.AddByte(3); 
                p.AddByte(f.type);
                p.AddDWord(f.id);
                p.AddWord(f.clickID); p.AddDWord(f.ownerID);
                p.AddByte(f.x); p.AddByte(f.y);
                p.AddDWord(f.maxhp); p.AddWord(f.maxsp);
                p.AddDWord(f.curhp); p.AddWord(f.cursp);
                p.AddByte(f.lvl);
                p.AddByte((byte)f.element);
                p.AddByte(f.rebirth); p.AddByte((byte)f.job);
            }
            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_10(byte val, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 10);
            p.AddByte(val);

            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_12(byte val, cCharacter target)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(11, 10);
            p.AddByte(val);

            p.SetSize();
            p.character = target;
            p.Send();
        }

        public void Send_250(UInt16 background, List<cFighter> flist,cCharacter target)
        {
            //this is sent to the player entering combat, and lists all other players already in
            if (flist.Count > 0)
            {
                cSendPacket p = new cSendPacket(g);
                p.Header(11, 250);
                p.AddWord(background);
                foreach (cFighter f in flist)
                {
                    p.AddByte(f.ukval1);
                    p.AddByte(f.type); 
                    p.AddDWord(f.id);
                    p.AddWord(f.clickID); p.AddDWord(f.ownerID);
                    p.AddByte(f.x); p.AddByte(f.y);
                    p.AddDWord(f.maxhp); p.AddWord(f.maxsp);
                    p.AddDWord(f.curhp); p.AddWord(f.cursp);
                    p.AddByte(f.lvl);
                    p.AddByte((byte)f.element); p.AddByte(f.rebirth); p.AddByte((byte)f.job);
                }
                p.SetSize();
                p.character = target;
                p.Send();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork.Managers;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_15 : cAC
    {
        public cAC_15(cGlobals globals)
            : base(globals)
        {
            this.g = globals;
        }

        public void SwitchBoard()
        {
            switch (g.packet.b)
            {
                case 7: Recv_7(); break;
                case 9: Recv_9(); break;
                case 8: Recv_8(); break;
                case 10: Recv_10(); break;
                case 11: Recv_11(); break;
                case 12: Recv_12(); break;
                case 14: Recv_14();break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.Log(str);
                    } break;

            }
        }
        public void Send_10(byte val,ushort ID)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 10);
            f.AddByte(val);
            f.AddWord(ID);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_15(uint id, ushort tool)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 15);
            f.AddDWord(id);
            f.AddWord(tool);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_11(byte val, ushort ID)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 11);
            f.AddByte(val);
            f.AddWord(ID);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_13(byte val)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 13);
            f.AddByte(val);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_4(cPet j, cCharacter t)
        {
            cSendPacket l = new cSendPacket(g);
            l.Header(15, 4);
            l.AddDWord(t.characterID);
            l.AddDWord(j.petID);
            l.AddWord(256);
            l.AddString(j.name);
            l.SetSize();
            g.gMapManager.GetMapByID(t.map.MapID).SendtoCharactersEx(l, t);
        }
        public void Send_19(byte val1,byte val2=0,byte val3=0)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 19);
            f.AddByte(val1);
            if (val2 != 0)
                f.AddByte(val2);
            if (val3 != 0)
                f.AddByte(val3);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Send_18(byte slot, uint id, ushort tool, uint x, uint y)
        {
            cSendPacket f = new cSendPacket(g);
            f.Header(15, 18);
            f.AddByte(slot);
            f.AddDWord(id);
            f.AddWord(tool);
            f.AddDWord(x);
            f.AddDWord(y);
            f.SetSize();
            f.character = g.packet.character;
            f.Send();
        }
        public void Recv_11()
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetDWord(3);
            cPet j = g.packet.character.pets.GetByID((ushort)id);
            if (j != null)
            {
                g.packet.character.pets.RidePet(j,slot);
                
            }
        }
        public void Recv_12()
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetDWord(3);
            cPet j = g.packet.character.pets.GetByID((ushort)id);
            if (j.state == PetStatus.RidingPet)
            {
                if (j != null)
                {
                    g.packet.character.pets.UnRidePet(j);

                }
            }
            else
            {
            }
        }
        public void Recv_10()//put in
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetWord(3);
            
            g.packet.character.vechile.LeaveVech(slot, g.packet.character.vechile.GetVechilebyID((ushort)id));
            
        }
        public void Recv_9() //open vech
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetWord(3);
            var car = g.packet.character.vechile.GetVechilebyID((ushort)id);
            g.packet.character.vechile.PullOut(car);
        }
        public void Recv_7() //ride vech
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetWord(3);
            g.packet.character.vechile.RideVech(g.packet.character.vechile.GetVechilebyID((ushort)id));
        }
        public void Recv_8()//unride
        {
            byte slot = g.packet.GetByte(2);
            var id = g.packet.GetWord(3);
            g.packet.character.vechile.LeaveVech(slot,g.packet.character.vechile.GetVechilebyID((ushort)id));
        
        }
        public void Recv_14() //auto open
        {
            var id = g.packet.GetWord(3);
            var car = g.packet.character.vechile.GetVechilebyID((ushort)id);
            g.packet.character.vechile.AutoOpen(car);
        }

    }
}

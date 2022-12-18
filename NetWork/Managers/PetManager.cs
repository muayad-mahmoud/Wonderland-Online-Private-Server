using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.Managers
{
    public enum eAC8_2_Skill
    {
        CHP = 25,
        CSP = 26,
        DEF = 42,
        MATK = 43,
        MDEF = 44,
        SPD = 45,
        ATK = 46,
        v207 = 207,
        v208 = 208,
        v210 = 210,
        v211 = 211,
        v214 = 214,
        v215 = 215,
        v216 = 216,
    }
    public enum PetStatus
    {
        Stored,
        Resting,
        RidingPet,
        Battle,
    }
    public class cPet
    {
        public cInvItem[] clothes = new cInvItem[7];
        public UInt16 petID;
        public UInt32 ownerID;
        public string name;
        public PetStatus state;
        public UInt32 atk;
        public UInt32 def;
        public UInt32 matk;
        public UInt32 mdef;
        public UInt32 spd;
        public UInt32 curHP;
        public UInt32 curSP;

        public void Clear()
        {
            petID = 0; ownerID = 0; name = "";
            atk = 0;
            def = 0;
            matk = 0;
            mdef = 0;
            spd = 0;
            curHP = 0;
            curSP = 0;
        }
        public UInt32 GetSkill(eAC8_2_Skill skill)
        {
            UInt32 value = 0;
            switch (skill)
            {
                case eAC8_2_Skill.ATK:
                    {
                        value = atk;
                    } break;
                case eAC8_2_Skill.CHP:
                    {
                        value = curHP;
                    } break;
                case eAC8_2_Skill.CSP:
                    {
                        value = curSP;
                    } break;
                case eAC8_2_Skill.DEF:
                    {
                        value = def;
                    } break;
                case eAC8_2_Skill.MATK:
                    {
                        value = matk;
                    } break;
                case eAC8_2_Skill.MDEF:
                    {
                        value = mdef;
                    } break;
                case eAC8_2_Skill.SPD:
                    {
                        value = spd;
                    } break;
                case eAC8_2_Skill.v207:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v208:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v210:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v211:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v214:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v215:
                    {
                        value = 0;
                    } break;
                case eAC8_2_Skill.v216:
                    {
                        value = 0;
                    } break;
            }
            return value;
        }
    }
    public class cPetList
    {
        cGlobals globals;
        cCharacter y;
        public cPet[] myPets = new cPet[20];
        public UInt16[] groupPets = { 0, 0, 0, 0, 0 };

        public cPetList(cCharacter i,cGlobals g)
        {
            globals = g;
            y = i;
            foreach (cPet p in myPets) if (p != null) p.Clear();
        }
        public bool Load()
        {
            return true;
        }
        public bool Save()
        {
            return true;
        }
        public cPet GetPetinBattleMode()
        {
            foreach (cPet e in myPets)
            {
                if (e != null)
                    if (e.state == PetStatus.Battle && inParty(e.petID))
                        return e;
            }
            return null;
        }
        public cPet GetByID(UInt16 id)
        {
            foreach (cPet p in myPets)
                if (p.petID == id) return p;
            return null;
        }
        bool inParty(UInt16 ID)
        {
            foreach (UInt16 id in groupPets)
            {
                if (id == ID)
                    return true;
            }
            return false;
        }
        public bool SendAC8Packet(byte petSlot, cSendPacket p, eAC8_2_Skill skill)
        {
            if ((petSlot < 1) || (petSlot > 4)) return false;
            if (groupPets[petSlot - 1] == 0) return false;
            cPet pet = GetByID(groupPets[petSlot - 1]);
            if (pet == null) return false;

            p.Header(8, 2); p.AddByte(4);
            p.AddByte(petSlot); p.AddByte(0);
            p.AddByte((byte)skill); p.AddByte(1);
            p.AddDWord(pet.GetSkill(skill));
            p.AddDWord(0);
            p.SetSize();

            return true;
        }
        public bool SendAC15_8Packet()
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(15, 8);

            for (int n = 1; n <= 4; n++)
            {
                if (groupPets[n] != 0)
                {

                }
            }
            p.SetSize();
            p.character = globals.packet.character;
            if (p.index > 7)
            {
                p.Send();
            }
            return true;
        }
        public void RidePet(cPet i,byte slot)
        {
            cSendPacket f = new cSendPacket(globals);
            f.Header(15, 16);
            f.AddByte((byte)slot);
            f.AddDWord(y.characterID);
            f.AddDWord(groupPets[slot]);
            f.SetSize();
            globals.gMapManager.GetMapByID(y.map.MapID).SendtoCharacters(f);
            y.Send_8_1();
        }
        public void UnRidePet(cPet i)
        {
            cSendPacket f = new cSendPacket(globals);
            f.Header(15, 17);
            f.AddDWord(y.characterID);
            f.SetSize();
            globals.gMapManager.GetMapByID(y.map.MapID).SendtoCharacters(f);
            y.Send_8_1();

        }

    }
}

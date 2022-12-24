using System;
using System.Collections.Generic;
using System.Linq;
using PServer_v2.NetWork.DataExt;
using System.Text;
using PServer_v2.DataLoaders;

namespace PServer_v2.NetWork.Managers
{
    public enum eFighterType
    {
        spectator = 0,
        mobside = 1,
        pcside = 2
    }
    public enum eFighterState
    {
        NotSealed,
        Sealed,
        Dead,
    }
    public struct BattleCMD
    {
        public cFighter src;
        public cFighter dst;
        public SkillItem skill;
        public byte unknownbyte;
        public byte unknownbyte2;
    }
    public class cFighter
    {
        public bool alive
        {
            get
            { if (curhp > 0)return true; else return false; }
        }
        public cCharacter character = null;
        public eFighterType placement;
        public UInt32 id = 0;
        public UInt32 ownerID = 0;
        public UInt32 maxhp;
        public UInt16 maxsp;
        public UInt32 curhp;
        public UInt16 cursp;
        public UInt16 Atk;
        public UInt16 Matk;
        public UInt16 Def;
        public UInt16 Mdef;
        public UInt16 Spd;
        public eFighterState State;
        public int Expearned;
        public byte x;
        public byte y;
        public bool player = true;
        public bool pet = false;
        public string name;
        public byte lvl;
        public Element element;
        public byte rebirth;
        public RebornJob job;
        public byte type;
        public UInt16 clickID;
        public bool actionDone;
        public bool Watching;
        public bool starter;

        public bool ready;

        public byte ukval1;
        cGlobals globals;
        public cFighter(cGlobals g)
        {
            globals = g;
        }
        public void SetFrom(cCharacter c)
        {
            character = c;
            id = c.characterID;
            ownerID = 0;
            maxhp = c.stats.MaxHP;
            maxsp = c.stats.MaxSP;
            curhp = c.stats.CurHP;
            cursp = c.stats.CurSP;
            Atk = c.stats.sFullAtk;
            Def = c.stats.sFullDef;
            Mdef = c.stats.sFullMdef;
            Matk = c.stats.sFullMatk;
            Spd = c.stats.sFullSpd;
            player = true;
            pet = false;
            string.Copy(c.name);
            lvl = c.level;
            element = c.element;
            rebirth = c.rebirth;
            job = c.job;
            clickID = 0;
            type = 2;
            ready = false;
            actionDone = false;

        }
        public void SetFrom(Npc c)
        {
            character = null;
            id = c.NpcID;
            ownerID = 0;
            maxhp = c.HP;
            maxsp = (ushort)c.SP;
            curhp = c.HP;
            cursp = (ushort)c.SP;
            player = true;
            pet = false;
            string.Copy(c.NpcName);
            lvl = c.Level;
            element = (Element)c.Element;
            clickID = 0;
            ready = false;
            actionDone = false;

        }
        public void SetFrom(cPet p)
        {
            Npc r = globals.gNpcManager.GetNpcbyID(p.petID);
            type = r.Type;
        }
        //public void SetFrom(cNpc m)
        //{
        //}

    }


    public class cBattle
    {

        public enum eBattleType
        {
            pk = 2,
            normal,
            quest,
        }


        public bool active = false;
        public bool delete = false;
        public bool allready = false;
        int fightercount;
        TimeSpan chk;
        public TimeSpan rdEnd;
        bool rdStart = false;
        public UInt16 background = 140;
        public eBattleType type;
        public cFighter startedby;
        List<cFighter> leftside = new List<cFighter>(8);
        List<cFighter> rightside = new List<cFighter>(8);
        public Queue<BattleCMD> datacal = new Queue<BattleCMD>();
        cGlobals globals;

        public cBattle(cGlobals g)
        {
            globals = g;
        }

        public int FighterCnt
        {
            get
            {
                return leftside.Count + rightside.Count;
            }
        }
        public bool RdyforUpdate
        {
            get
            {
                if (rdStart ||TimeSpan.Compare(globals.UpTime.Elapsed, rdEnd) > 0 || allready)                    
                {
                    if (chk == null || chk < globals.UpTime.Elapsed )
                    {
                        chk = globals.UpTime.Elapsed + new TimeSpan(0, 0, 1);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        public void Process()
        {
            if (rdStart)
            {
                StartRound();
            }
            else if (datacal.Count > 0)
            {
                Calculate();
            }
            else if (!Checkforlife(true) && !Checkforlife(false))//check if sides have ppl alive/etc
            {
                EndBattle();
            }
            else
                rdStart = true;

        }
        //Watching Battle
        bool Checkforlife(bool left)
        {
            int ct = 0;
            if(left)
                foreach(cFighter j in leftside.Where(h => h.alive))
                    ct++;
            else
                foreach(cFighter j in rightside.Where(h => h.alive))
                    ct++;
            if (ct == 0)
                return false;
            else
                return true;

        }

        void SpectateRside(cFighter fighter)
        {
        }
        void SpectateLside(cFighter fighter)
        {
        }

        int GetRSide_fightercnt()
        {
            return rightside.Count;
        }
        int GetLSide_fightercnt()
        {
            return leftside.Count;
        }

        byte[] GetRSidePlace(bool pet, cFighter own)
        {
            byte x = 4;
            byte y = 2;
            if (!pet)
            {
                switch (rightside.Count())
                {
                    case 0: break;
                    case 1: y += 1; break;
                    case 2: y -= 2; break;
                    case 3: y += 3; break;
                }
                return new byte[] { x, y };
            }
            else
            {
                return new byte[] { (byte)(x - 1), own.y };
            }
        }
        byte[] GetLSidePlace(bool pet, cFighter own)
        {
            byte x = 1;
            byte y = 2;
            if (!pet)
            {
                switch (leftside.Count())
                {
                    case 0: break;
                    case 1: y += 1; break;
                    case 2: y -= 2; break;
                    case 3: y += 3; break;
                }
                return new byte[] { x, y };
            }
            else
            {
                return new byte[] { (byte)(x + 1), own.y };
            }
        }
        //Joining Battle
        void JoinRSide(cFighter fighter)
        {
        }
        void JoinLSide(cFighter fighter)
        {
        }
        //Entering Battle
        public void AddFightertoRside(cFighter fighter)
        {
            fighter.character.Send_8_1();

            if (fighter.player)
            {
                cSendPacket p = new cSendPacket(globals);
                p.Header(11, 4);
                p.AddByte(2);
                p.AddDWord(fighter.character.characterID);
                p.AddWord(0);
                p.AddByte(2);
                p.SetSize();
                (globals.gMapManager.GetMapByID(fighter.character.map.MapID)).SendtoCharactersEx(p, fighter.character);
                var place = GetRSidePlace(fighter.pet, fighter);
                fighter.x = place[0];
                fighter.y = place[1];
                fighter.character.battle = this;
                if (fighter.character.pets.GetPetinBattleMode() != null)
                {
                    cFighter pet = new cFighter(globals);
                    pet.SetFrom(fighter.character.pets.GetPetinBattleMode());//finish later
                    pet.actionDone = false;
                    pet.ukval1 = 5;
                    pet.pet = true;
                    pet.x = GetRSidePlace(pet.pet, fighter)[0];
                    pet.y = GetRSidePlace(pet.pet, fighter)[1];
                    pet.character = fighter.character;
                    pet.ownerID = fighter.character.characterID;
                    pet.clickID = fighter.clickID;//not the right one
                    rightside.Add(fighter);
                    //Send_11_5(pet);
                }
            }
            else
            {
                fighter.x = GetRSidePlace(fighter.pet, fighter)[0];
                fighter.y = GetRSidePlace(fighter.pet, fighter)[1];
            }
            rightside.Add(fighter);
        }
        public void AddFightertoLside(cFighter fighter)
        {
            if (fighter.player)
            {
                fighter.character.Send_8_1();
                cSendPacket p = new cSendPacket(globals);
                p.Header(11, 4);
                p.AddByte(2);
                p.AddDWord(fighter.character.characterID);
                p.AddWord(0);
                p.AddByte(5);
                p.SetSize();
                (globals.gMapManager.GetMapByID(fighter.character.map.MapID)).SendtoCharactersEx(p, fighter.character);
                var place = GetLSidePlace(fighter.pet, fighter);
                fighter.x = place[0];
                fighter.y = place[1];
                fighter.character.battle = this;
                if (fighter.character.pets.GetPetinBattleMode() != null)
                {
                    cFighter pet = new cFighter(globals);
                    pet.SetFrom(fighter.character.pets.GetPetinBattleMode());//finish later
                    pet.actionDone = false;
                    pet.ukval1 = 5;
                    pet.pet = true;
                    pet.x = GetLSidePlace(pet.pet, fighter)[0];
                    pet.y = GetLSidePlace(pet.pet, fighter)[1];
                    pet.character = fighter.character;
                    pet.ownerID = fighter.character.characterID;
                    pet.clickID = fighter.clickID;//not the right one
                    leftside.Add(fighter);
                    //Send_11_5(pet);
                }
            }
            else
            {
                fighter.x = GetLSidePlace(fighter.pet, fighter)[0];
                fighter.y = GetLSidePlace(fighter.pet, fighter)[1];
            }
            leftside.Add(fighter);
        }
        //Leaving Battle
        public void RemFighter(cFighter fighter)
        {
            if (fighter.pet)
            {
                globals.ac11.Send_1(fighter, FindFighterbyID((int)fighter.ownerID).character);
            }
            if (fighter.player && !fighter.Watching)
            {
                if (fighter.player && this.startedby != fighter)
                    globals.ac11.Send_12(2, fighter.character);
                else
                    globals.ac11.Send_12(1, fighter.character);                
            }
            if (fighter.player)
            {
                globals.ac11.Send_12(2, fighter.character);
                //send an 8,1 for exp to the target
                cSendPacket p = new cSendPacket(globals);
                p.Header(11, 0);
                p.AddDWord(fighter.character.characterID);
                p.AddWord(0);
                p.SetSize();
                (globals.gMapManager.GetMapByID(fighter.character.map.MapID)).SendtoCharacters(p);
                if (!fighter.Watching)
                    globals.ac11.Send_1(0, fighter, fighter.character);
            }
            else
            {
            }
            switch (fighter.placement)
            {
                case eFighterType.pcside: rightside.Remove(fighter); break;
                case eFighterType.mobside: leftside.Remove(fighter); break;
            }
            fightercount--;
        }        

        //Handles sending Battle Info after sides populated
        public void Send_BattleInfo()
        {
            cFighter[] re = new cFighter[leftside.Count + rightside.Count];
            leftside.CopyTo(re);
            rightside.CopyTo(re, leftside.Count);
            foreach (cFighter r in re)
                if (r != null)
                    if (r.player)
                    {
                        globals.gServer.Multipkt_Request(r.character);
                        r.character.Send_8_1();
                        Send_11_250(r, r.starter, r.character);
                        globals.ac11.Send_10(1, r.character);
                        Send_11_5(r.character);
                        globals.gServer.SendCombinepkt(r.character);
                    }
        }
        //Send StartRd info
        public void StartRound()
        {
            allready = false;
            rdStart = false;
            this.rdEnd = (globals.UpTime.Elapsed + new TimeSpan(0, 0, 20));
            cFighter[] re = new cFighter[leftside.Count + rightside.Count];
            leftside.CopyTo(re);
            rightside.CopyTo(re, leftside.Count);
            foreach (cFighter f in re)
            {
                if (f.pet)
                {
                    f.actionDone = false;
                }
                else if (f.player)
                {
                    globals.gServer.Multipkt_Request(f.character);
                    f.actionDone = false;
                    foreach (cFighter ft in rightside.Where(z => !z.Watching))
                    {
                        globals.ac51.Send_1(ft, 25, (ushort)ft.curhp, f.character);
                        globals.ac51.Send_1(ft, 26, (ushort)ft.cursp, f.character);
                    }
                    foreach (cFighter lt in leftside.Where(z => !z.Watching))
                        globals.ac51.Send_1(lt, 25, (ushort)lt.curhp, f.character);
                    globals.ac52.Send_1(f.character);
                    globals.gServer.SendCombinepkt(f.character);
                }
                else
                {
                    BattleCMD h = new BattleCMD();
                    h.src = f;
                    h.skill = globals.gskillManager.GetSkillbyID(10001);
                    if (f.placement == eFighterType.mobside)
                        h.dst = rightside.First();
                    else
                        h.dst = leftside.First();
                    h.unknownbyte = 3;
                    datacal.Enqueue(h);
                    //enqueue it
                    f.actionDone = true;
                }
            }
        }
        //End BattleRd for all players
        public void EndBattle()
        {
            cFighter[] re = new cFighter[leftside.Count + rightside.Count];
            leftside.CopyTo(re);
            rightside.CopyTo(re, leftside.Count);
            foreach (cFighter f in re)
            {
                if (f.player)
                    f.character.inbattle = false;
                    RemFighter(f);
                    
            }
            this.delete = true;
        }

        #region Test tools
        public void Test(cCharacter target)
        {
            // Send:
            //244, 68, 9, 0, 11, 2, 2, 196, 242, 28, 0, 0, 0,          
            target.Send_8_1();

            cSendPacket p = new cSendPacket(globals);
            p.index = 0;
            p.AddArray(new byte[] { 244, 68, 34, 0, 11, 250, 140, 0, 2, 2 });
            p.AddDWord(target.characterID);
            p.AddArray(new byte[] { 0, 0, 0, 0, 0, 0, 4, 2, 134, 1, 0, 0, 105, 0, 134, 1, 0, 0, 75, 0, 11, 1, 0, 0 });

            p.AddArray(new byte[] { 244, 68, 3, 0, 11, 10, 1 });
            p.AddArray(new byte[] { 244, 68, 32, 0, 11, 5, 5, 2 });
            p.AddDWord(target.characterID); //other id
            p.AddArray(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2, 93, 1, 0, 0, 108, 0, 93, 1, 0, 0, 108, 0, 14, 3, 0, 0 });

            p.AddArray(new byte[] { 244, 68, 10, 0, 11, 4, 2 });
            p.AddDWord(target.characterID); //other id
            p.AddArray(new byte[] { 0, 0, 5 });
            p.character = target;
            p.Send();

            TestNext(target);
        }
        public void TestNext(cCharacter target)
        {
            //cAC_51.Send_1(new byte[] { 4, 2, 25, 93, 1, 0, 0 }, target);
            //cAC_51.Send_1(new byte[] { 1, 2, 25, 134, 1, 0, 0 }, target);
            //.Send_1(new byte[] { 1, 2, 26, 75, 0, 0, 0 }, target);
            globals.ac52.Send_1(target);
        }
        public void TestExit(cCharacter target, byte[] data)
        {
            //byte x = data[2]; byte y = data[3];
            //cFighter attacker = FindFighter(x, y);

            //g.ac41.Send_1(1, target);
        }
        #endregion

        public cFighter FindFighterbyID(int ID)
        {
            cFighter f = null;
            cFighter[] re = new cFighter[leftside.Count + rightside.Count];
            leftside.CopyTo(re);
            rightside.CopyTo(re, leftside.Count);
            foreach (cFighter r in re)
            {
                if (r.id == ID)
                {
                    f = r;
                }
            }
            return f;
        }
        public cFighter FindFighter(byte x, byte y)
        {
            foreach (cFighter t in rightside)
                if (t.x == x && t.y == y)
                    return t;
            foreach (cFighter t in leftside)
                if (t.x == x && t.y == y)
                    return t;
            return null;
        }

        public void PCAttack(BattleCMD data, cCharacter target)
        {
            cFighter f = data.src;//player doing attack
            cFighter t = data.dst; //target of attack
            SkillItem skill = data.skill; //skill used
            //calc the attck and stuff in here

            //add to queue
            data.unknownbyte = 1;
            datacal.Enqueue(data);
            f.actionDone = true;
            cSendPacket p = new cSendPacket(globals);
            p.Header(53, 5);
            p.AddByte(f.x);
            p.AddByte(f.y);
            p.SetSize();
            foreach (cFighter g in rightside.Where(c => c.player))
            {
                p.character = g.character;
                p.Send();
            }
            foreach (cFighter g in leftside.Where(c => c.player))
            {
                p.character = g.character;
                p.Send();
            }

            bool eveyready = true;

            foreach (cFighter j in rightside)
                if (j.pet || j.player && !j.Watching)
                    if (j.actionDone == false)
                    {
                        eveyready = false; break;
                    }
            foreach (cFighter j in leftside)
                if (j.pet || j.player && !j.Watching)
                    if (j.actionDone == false)
                    {
                        eveyready = false; break;
                    }
            if (eveyready)
            {
                allready = true;
            }
        }

        bool inSpdRange(eFighterType side)
        {
            bool ret = false;
            switch (side)
            {
                #region right side
                case eFighterType.pcside:
                    {
                        var e = this.rightside.OrderBy(r => r.character.stats.sFullSpd).ToList();

                        for (int a = 0; a < 8; a++)
                        {
                            if (a == 8) { }
                            else
                            {
                                if (e[a].player)
                                {
                                    if (e[a + 1].player)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                    else if (e[a + 1].pet)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                }
                                else if (e[a].pet)
                                {
                                    if (e[a + 1].player)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                    else if (e[a + 1].pet)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                }

                            }
                        }
                    } break;
                #endregion

                #region leftside

                case eFighterType.mobside:
                    {
                        var e = this.leftside.OrderBy(r => r.character.stats.sFullSpd).ToList();

                        for (int a = 0; a < 8; a++)
                        {
                            if (a == 8) { }
                            else
                            {
                                if (e[a].player)
                                {
                                    if (e[a + 1].player)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                    else if (e[a + 1].pet)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                }
                                else if (e[a].pet)
                                {
                                    if (e[a + 1].player)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                    else if (e[a + 1].pet)
                                    {
                                        if (e[a].character.stats.sFullSpd.CompareTo(e[a + 1].character.stats.sFullSpd - 100) >= 0)
                                            ret = true;
                                        else
                                            ret = false;
                                    }
                                }
                                else
                                {
                                }

                            }
                        }
                    } break;
                #endregion
            }
            return ret;
        }
        int AverageSpd(eFighterType side)
        {
            switch (side)
            {
                case eFighterType.mobside:
                    {
                        var totalspd = 0;
                        //foreach(cFighter u in leftside)
                        //totalspd += u.
                    } break;
                case eFighterType.pcside:
                    {
                    } break;
            }
            return 0;
        }
        eFighterType Fastestside()
        {
            foreach (cFighter t in leftside)
            {

            }
            return eFighterType.spectator;
        }
        double GetDamage(int atk_matk, int SkillPower, int Def_mdef, float elementCorr)
        {
            return (atk_matk * 1.4 + SkillPower - Def_mdef * 0.98) * elementCorr;
        }
        double GetElementCorrection(Element hitter, Element target)
        {
            switch (hitter)
            {
                case Element.Fire:
                    {
                        switch (target)
                        {
                            case Element.noElement: return 1.3;
                            case Element.Fire: return 1.3;
                            case Element.Earth: return 1.2;
                            case Element.Water: return 0.7;
                            case Element.Wind: return 1.6;
                        }
                    } break;
                case Element.Earth:
                    {
                        switch (target)
                        {
                            case Element.noElement: return 1.0;
                            case Element.Fire: return 1.0;
                            case Element.Earth: return 0.9;
                            case Element.Water: return 1.3;
                            case Element.Wind: return 0.6;
                        }
                    } break;
                case Element.Water:
                    {
                        switch (target)
                        {
                            case Element.noElement: return 0.9;
                            case Element.Fire: return 1.4;
                            case Element.Earth: return 0.5;
                            case Element.Water: return 0.9;
                            case Element.Wind: return 0.9;
                        }
                    } break;
                case Element.Wind:
                    {
                        switch (target)
                        {
                            case Element.noElement: return 1.0;
                            case Element.Fire: return 0.7;
                            case Element.Earth: return 1.3;
                            case Element.Water: return 1.0;
                            case Element.Wind: return 1.0;
                        }
                    } break;
            }
            return 0;
        }
        List<cFighter> fightersbyspd()
        {
            List<cFighter> tmp = new List<cFighter>();
            tmp.AddRange(rightside);
            tmp.AddRange(leftside);
            return tmp.OrderBy(r => r.Spd).ToList();
        }

        public void Calculate()
        {
            bool racombo = false;
            bool lacombo = false;
            //19 per attacker
            //order by spd
            //need to reorder based on speed
            //Second part is the Attk/miss
            var e = datacal.Dequeue();
            if (e.skill.Name == "Flee")
                switch (e.src.placement)
                {
                    case eFighterType.mobside:
                        {
                            if (leftside.Count - 1 <= 0)
                                EndBattle();
                            else
                                RemFighter(e.src);
                        } break;
                    case eFighterType.pcside:
                        {
                            if (rightside.Count - 1 <= 0)
                                EndBattle();
                            else
                                RemFighter(e.src);
                        } break;
                }
            else
            {
                Send_Attack(e.src, e.skill.SkillID, e.dst, false, 10);
            }

        }

        #region Quick Send/GEt
        void Send_11_5(cCharacter fighter)
        {
            List<cFighter> flist = new List<cFighter>();
            switch (this.type)
            {
                case eBattleType.pk:
                    {
                        foreach (cFighter e in rightside)
                            if (startedby.character != fighter)
                            {
                                if (!e.player)
                                    flist.Add(e);
                            }
                            else
                            {
                                if (e != startedby && e.placement != startedby.placement)
                                    flist.Add(e);

                            }
                        foreach (cFighter e in leftside)
                            if (startedby.character != fighter)
                            {
                                if (!e.player)
                                    flist.Add(e);
                            }
                            else
                                if (e != startedby && e.placement != startedby.placement)
                                    flist.Add(e);

                        if (flist.Count > 0)
                            globals.ac11.Send_5(flist, fighter);
                    } break;
                    case eBattleType.normal:
                    {
                        foreach (cFighter e in rightside)
                            if (startedby.character != fighter)
                            {
                                if (!e.player)
                                    flist.Add(e);
                            }
                            else
                            {
                                if (e != startedby && e.placement != startedby.placement)
                                    flist.Add(e);

                            }
                        foreach (cFighter e in leftside)
                            if (startedby.character != fighter)
                            {
                                if (!e.player)
                                    flist.Add(e);
                            }
                            else
                                if (e != startedby && e.placement != startedby.placement)
                                    flist.Add(e);

                        if (flist.Count > 0)
                            globals.ac11.Send_5(flist, fighter);
                    }break;
            }
        }
        void Send_11_250(cFighter fighter, bool starter, cCharacter target)
        {
            List<cFighter> flist = new List<cFighter>();
            if (!starter)
            {
                flist.Add(fighter);
                foreach (cFighter e in rightside)
                    if (e.player && e.character != target)
                        flist.Add(e);
                foreach (cFighter e in leftside)
                {
                    if (e.player && e.character != target)
                    {
                        flist.Add(e);
                    }
                    else if(e.placement == eFighterType.mobside)
                    {
                        flist.Add(e);
                    }
                    
                }
                    
                
                
                        
                    
                globals.ac11.Send_250(background, flist, target);
            }
            else
            {
                switch (fighter.placement)
                {
                    case eFighterType.pcside:
                        {
                            flist.Add(fighter);
                            foreach (cFighter e in rightside)
                                if (e.player && e != fighter)
                                    flist.Add(e);
                        } break;
                    case eFighterType.mobside:
                        {
                            flist.Add(fighter);
                            foreach (cFighter e in leftside)
                                if (e.player && e != fighter)
                                    flist.Add(e);
                        } break;
                }
                globals.ac11.Send_250(background, flist, target);
            }
        }
        void Send_Attack(cFighter src, ushort skill, cFighter dst, bool miss, uint Damg)
        {
            cFighter[] re = new cFighter[leftside.Count + rightside.Count];
            leftside.CopyTo(re);
            rightside.CopyTo(re, leftside.Count);
            foreach (cFighter rt in re)
            {
                if (rt.player)
                {
                    globals.gServer.Multipkt_Request(rt.character);
                    globals.ac50.Send_6(src, 0, rt.character);
                    globals.ac50.Send_1(src, skill, dst, miss, Damg, rt.character);
                    globals.gServer.SendCombinepkt(rt.character);
                }
            }
        }
        #endregion
    }


    //====================================================
    //===================================================

    public class cBattleSystem
    {
        List<cBattle> battleList = new List<cBattle>();
        cGlobals globals;

        public cBattleSystem(cGlobals g) //figure why need global
        {
            globals = g;
        }
        public cBattle StartPK(cCharacter starter, cCharacter enemy) //TODO these should be lists of players
        {
            cBattle battle = new cBattle(globals);

            battle.type = cBattle.eBattleType.pk;
            //starter team
            cFighter f = new cFighter(globals);
            f.SetFrom(starter);
            f.ukval1 = 2; f.type = 2;
            f.placement = eFighterType.pcside;
            battle.startedby = f;
            f.starter = true;
            battle.AddFightertoRside(f);
            if (starter.Party.Count > 0)
                foreach (cCharacter d in starter.Party.TeamMembers)
                {
                    cFighter fe = new cFighter(globals);
                    fe.SetFrom(d);
                    fe.ukval1 = 3;
                    fe.placement = eFighterType.pcside;
                    battle.AddFightertoRside(fe);
                }

            f = new cFighter(globals);
            f.SetFrom(enemy);
            f.placement = eFighterType.mobside;
            f.ukval1 = 5; f.type = 2;
            battle.AddFightertoLside(f);
            if (starter.Party.Count > 0)
                foreach (cCharacter d in enemy.Party.TeamMembers)
                {
                    cFighter fe = new cFighter(globals);
                    fe.SetFrom(d);
                    fe.ukval1 = 3;
                    fe.placement = eFighterType.pcside;
                    battle.AddFightertoLside(fe);
                }
            battle.Send_BattleInfo();
            battle.StartRound();
            battle.active = true;
            battleList.Add(battle);
            return battle;
        }
        public cBattle StartPKNpc(cCharacter starter, cFighter enemy) //TODO these should be lists of players
        {
            starter.inbattle = true;
            globals.Log("Battle Started \r\n");
            cBattle battle = new cBattle(globals);
            battle.type = cBattle.eBattleType.pk;

            cFighter f = new cFighter(globals);
            f.SetFrom(starter);
            f.clickID = enemy.clickID;
            f.ukval1 = 3;
            f.placement = eFighterType.pcside;
            battle.startedby = f;
            f.starter = true;
            battle.AddFightertoRside(f);
            if (starter.Party.Count > 0)
            {
                foreach (cCharacter d in starter.Party.TeamMembers)
                {
                    cFighter fe = new cFighter(globals);
                    fe.SetFrom(d);
                    fe.clickID = enemy.clickID;
                    fe.ukval1 = 3;
                    fe.placement = eFighterType.pcside;
                    battle.AddFightertoRside(fe);
                }
            }
            enemy.placement = eFighterType.mobside;
            battle.AddFightertoLside(enemy);
            battle.Send_BattleInfo();
            battle.StartRound();
            battle.active = true;
            battleList.Add(battle);

            return battle;
        }
        public cBattle StartBattleAmbush(cCharacter starter, cFighter enemy) //TODO these should be lists of players
        {
            starter.inbattle = true;
            globals.Log("Battle Started \r\n");
            cBattle battle = new cBattle(globals);
            battle.type = cBattle.eBattleType.normal;

            cFighter f = new cFighter(globals);
            f.SetFrom(starter);
            f.clickID = enemy.clickID;
            f.ukval1 = 1;
            f.placement = eFighterType.pcside;
            battle.startedby = enemy;
            f.starter = false;
            battle.AddFightertoRside(f);
            if (starter.Party.Count > 0)
            {
                foreach (cCharacter d in starter.Party.TeamMembers)
                {
                    cFighter fe = new cFighter(globals);
                    fe.SetFrom(d);
                    fe.clickID = enemy.clickID;
                    fe.ukval1 = 1;
                    fe.placement = eFighterType.pcside;
                    battle.AddFightertoRside(fe);
                }
            }
            enemy.placement = eFighterType.mobside;
            battle.AddFightertoLside(enemy);
            battle.Send_BattleInfo();
            battle.StartRound();
            battle.active = true;
            battleList.Add(battle);

            return battle;
        }
        


        public void Process()
        {
            Queue<cBattle> remove = new Queue<cBattle>();
            //process battle information
            //need some function in here to check time, so we know how long has elapsed since last call
            try
            {
                foreach (cBattle g in battleList)
                {
                    if (g.active)
                    {
                        if (g.RdyforUpdate)
                            g.Process();
                    }
                    if (g.delete)
                    {
                        //erase battle
                        remove.Enqueue(g);
                    }
                }
            }
            catch { }
            while (remove.Count > 0)
            {
                battleList.Remove(remove.Dequeue());
            }
        }
    }
}

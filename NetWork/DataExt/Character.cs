using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PServer_v2.NetWork.Managers;
using PServer_v2.NetWork.ACS;

namespace PServer_v2.NetWork.DataExt
{
    public class cCharacter
    {
        #region Character Enums
        enum Hairstyle_SmallM
        {
            Rocco,
        }
        enum HairStyle_SmallF
        {
            Nina,
            Betty,
        }
        enum HairStyle_BigM
        {
            Daniel,
            Sid,
            More,
            Kurogane,
        }
        enum Hairstyle_BigF
        {
            Iris,
            Lique,
            Vanessa,
            Breillat,
            Jessica,
            Konnotsuroko,
            Maria,
            Karin,
        }
        public enum characterState
        {
            Normal,
            Trading,
            Battle,
            Interacting,
            Warping,
            Logging,
            Dced,
        }
        #endregion

        
        cGlobals globals;
        public cSkills skills;
        //public GmSystem GMTools;
        public cRiceBall riceBall;
        public cMailManager Mail;
        public cCharacterFriend Friends;
        public TeamManager Party;
        public cInventory inv;
        public cEquipManager eq;
        public TransportationManager vechile;
        public TentManager myTent;
        public cBattle battle; //reference to a battle going on
        public cPetList pets;

        public bool inbattle;       
        public bool inCarnie;
        public bool inMap;
        public bool creating;
        public bool QueueMode;
        public bool MultiMode;
        public cSendPacket Combinepkt;
        public bool GM;
        public bool connected;
        public bool warping;
        public bool warping2;
        public byte state; //if the char is created or not  0:not made, 1:created, 2:jailed
        public byte slot;
        public bool saved; //tells if this chars files have been saved at disconnect or not
        public byte emote;
        public bool sitting;
        public bool inparty;

        public Queue<cSendPacket> DatatoSend = new Queue<cSendPacket>();
        // character data file items
        public string name; //character name
        public string nickname; //character nickname
        public string password; //password to delete character

        public byte head;
        public BodyStyle body;
        public Element element;
        public byte rebirth;
        public RebornJob job;
        public inGameState charState = inGameState.Normal;
        public UInt16 x;
        public UInt16 y;
        public cMap map;
        public Int32 IM { get { return globals.gUserManager.ImPoints(characterID); } }
        public UInt32 gold;

        public cStats stats;
        public bool talking;
        public NpcEntries talkingto;
        public byte level;
        public UInt32 color1;
        public UInt32 color2;


        public UInt16 tentMap;

        public UInt32 userID;
        public UInt32 characterID;
        public cClient client;
        public bool logging;        

        public cWarp lastMap;
        public cWarp recordMap;
        public cWarp gpsMap;

        public cCharacter() { }
        public cCharacter(cGlobals src)
        {
            globals = src;
            Clear();
            skills = new cSkills(globals, this);
            pets = new cPetList(this,globals);
            Mail = new cMailManager(this,globals);
            Friends = new cCharacterFriend(this,globals);
            Party = new TeamManager(this,globals);
            vechile = new TransportationManager(this,globals);
            myTent = new TentManager(this,globals);
        }
        public void Disconnect()
        {
            if (client != null)
                client.Disconnect();
            Friends.Dced();
        }
        void Clear()
        {
            battle = new cBattle(globals);
            riceBall = new cRiceBall(globals,this);
            warping = false;
            connected = false;
            logging = true;
            creating = false;
            slot = 1;
            emote = 0;

            userID = 0; //account id
            characterID = 0; //character id
            name = ""; //character name
            nickname = ""; //character nickname
            password = ""; //password to delete character

            head = 0;
            body = 0;
            element = 0;
            rebirth = 0;
            job = 0;
            x = 0;
            y = 0;
            map = new cMap(globals);
            map.MapID = 0;
            gold = 0;

            stats = new cStats(this,globals);
            level = 0;
            color1 = 0;
            color2 = 0;
            if (inv == null) inv = new cInventory(globals, this);
            if(eq == null) eq = new cEquipManager(globals,this);
            state = 0;
            inCarnie = false;
            saved = true;
            tentMap = 0;
            lastMap = new cWarp();
            gpsMap = new cWarp();
            recordMap = new cWarp();

        }

        #region DataBase Tools
        public void SetFromDB(DataTable character,bool noload = false)
        {
            if (character.Rows.Count != 1) return;
            DataRow r = character.Rows[0];
            Int64 v;
            v = (Int64.Parse(r["characterID"].ToString())); characterID = (UInt32)v;
            state = (byte)(Int64.Parse(r["state"].ToString()));
            name = (string)r["name"]; name = name.Trim();
            nickname = (string)r["nickname"]; nickname = nickname.Trim();
            password = (string)r["password"]; password = password.Trim();
            map.MapID = (UInt16)(Int64.Parse(r["map"].ToString()));
            x = (UInt16)(Int64.Parse(r["x"].ToString()));
            y = (UInt16)(Int64.Parse(r["y"].ToString()));
            body = (BodyStyle)(Int64.Parse(r["body"].ToString()));
            head = (byte)(Int64.Parse(r["head"].ToString()));
            
            color1 = (UInt32)(Int64.Parse(r["colors1"].ToString()));
            color2 = (UInt32)(Int64.Parse(r["colors2"].ToString()));
            gold = (UInt32)(Int64.Parse(r["gold"].ToString()));
            level = (byte)(Int64.Parse(r["level"].ToString()));
            element = (Element)Int32.Parse((r["element"]).ToString());
            SetFlags((string)r["flags"]);
            SetWarp(lastMap, (string)r["lastMap"]);
            SetWarp(recordMap, (string)r["recordSpot"]);
            SetWarp(gpsMap, (string)r["gpsSpot"]);
            rebirth = (byte)(Int64.Parse(r["rebirth"].ToString()));
            job = (RebornJob)Int32.Parse(r["job"].ToString());
            if (!noload)
            {
                stats.LoadStats((string)r["stats"], (UInt32)(Int64.Parse(r["curHP"].ToString())), (UInt16)(Int64.Parse(r["curSP"].ToString())));
                skills.LoadSkills((string)r["skills"]);
                //load inv
                inv.Load(characterID);
                Friends.LoadFriends(r["friends"].ToString());
                Mail.LoadMail((string)r["mail"]);
                pets.Load();
                stats.CalcBaseStats((byte)element, level, rebirth, (byte)job);
                stats.CalcFullStats(eq.clothes);
                if (globals.gUserManager.gmLvl(characterID) > 0)
                {
                    GM = true;
                    //GMTools = new GmSystem(this);
                }
            }
        }
        void SetFlags(string s)
        {
            string[] words = s.Split(' ');
            state = byte.Parse(words[0]);
            inCarnie = false; if (byte.Parse(words[1]) == 1) inCarnie = true;
            inMap = false; if (byte.Parse(words[2]) == 1) inMap = true;
        }
        void SetWarp(cWarp w, string s)
        {
            string[] words = s.Split(' ');
            w.mapTo = UInt16.Parse(words[0]);
            w.x = UInt16.Parse(words[1]);
            w.y = UInt16.Parse(words[2]);
        }
        public void SetName(string name)
        {
            this.name = string.Copy(name);
        }
        public void SetPass(string password)
        {
            this.password = string.Copy(password);
        }
        public void SetNick(string nickname)
        {
            this.nickname = string.Copy(nickname);
        }
        #endregion        

        public void Login()
        {
            globals.packet.character.Send_3_Me();
            globals.Interface(1);
            globals.gCharacterManager.SendAC4();
            globals.ac33.Send_2(2, 2, 1, 127); //system stats, like pk , battleview
            //---------------------------Character Final Info
            inv.Send_Inventory();
            eq.Send_EQS();
            globals.ac26.Send_4(gold);//244, 68, 6, 0, 26, 4, 70, 78, 0, 0,  //gold
            globals.ac5.Send_3(this);
            globals.gMapManager.GetMapByID(map.MapID).LogintoMap(this);
                
        }
        public void ExpGain(uint ammt)
        {
            if ((stats.CurExp + ammt) >= stats.maxExp)
            {
                stats.TotalExp += ammt;// +(uint)stats.CalcMaxExp(rebirth, level - 1);
                level++;
                stats.SPoints += 3;
                stats.CalcBaseStats((byte)element, level, rebirth, (byte)job);
                stats.CalcFullStats(eq.clothes);
                Send_8_1(true);
            }
            else
            {
                stats.TotalExp += ammt;
                globals.ac8.Send_1(36, stats.TotalExp, 0, this);
            }
        }
        public void UpdateWalking(byte direction)
        {
            sitting = false;
            emote = 0;
            for (int a = 0; a < Party.Count; a++)
            {
                Party.TeamMembers[a].sitting = false;
                Party.TeamMembers[a].emote = 0;
            }
            if (vechile.inVechile)
            {
                vechile.VechWalk();
            }
            cSendPacket p = new cSendPacket(globals);
            p.Header(6, 1);
            p.AddDWord(characterID);
            p.AddByte(direction);
            p.AddWord(x);
            p.AddWord(y);
            p.SetSize();
            map.SendtoCharactersEx(p, this);
        }
        public void Whisper(string text,cCharacter target)
        {
            if (target != null)
            {
                cSendPacket p = new cSendPacket(globals);
                p.Header(2, 3);
                p.AddDWord(characterID);
                p.AddArray(text.ToCharArray());
                p.SetSize();
                p.character = target;
                p.Send();
                p = new cSendPacket(globals);
                p.Header(2, 3);
                p.AddDWord(characterID);
                p.AddArray(text.ToCharArray());
                p.SetSize();
                p.character = this;
                p.Send();
            }
        }
        public void SetNoobClothes()
        {
            cItem[] y = new cItem[1];
            switch (body)
            {
                case BodyStyle.Big_Female:
                    {
                        Hairstyle_BigF g = new Hairstyle_BigF();
                        switch (Enum.GetName(g.GetType(), (Hairstyle_BigF)head))
                        {
                            case "Iris":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22005),
                                       globals.gItemManager.GetItemByID(21006),globals.gItemManager.GetItemByID(23001),
                                       globals.gItemManager.GetItemByID(24006)};
                                } break;
                            case "Lique":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22009),
                                       globals.gItemManager.GetItemByID(21014),globals.gItemManager.GetItemByID(18002),
                                       globals.gItemManager.GetItemByID(24014)};
                                } break;
                            case "Maria":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22006),
                                       globals.gItemManager.GetItemByID(21011),globals.gItemManager.GetItemByID(10004),
                                       globals.gItemManager.GetItemByID(24011)};
                                } break;
                            case "Vanessa":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21015),
                                       globals.gItemManager.GetItemByID(24008)};
                                } break;
                            case "Breillat":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22007),
                                       globals.gItemManager.GetItemByID(21009),globals.gItemManager.GetItemByID(10002),
                                       globals.gItemManager.GetItemByID(24009)};
                                } break;
                            case "Karin":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21015),
                                        globals.gItemManager.GetItemByID(22008),
                                       globals.gItemManager.GetItemByID(24015)};
                                } break;
                            case "Konnotsuroko":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(24013),
                                       globals.gItemManager.GetItemByID(21013),};
                                } break;
                            case "Jessica":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22002),
                                       globals.gItemManager.GetItemByID(21010),globals.gItemManager.GetItemByID(10003),
                                       globals.gItemManager.GetItemByID(24010)};
                                } break;
                        }
                    } break;
                case BodyStyle.Big_Male:
                    {
                        HairStyle_BigM g = new HairStyle_BigM();
                        switch (Enum.GetName(g.GetType(), (HairStyle_BigM)head))
                        {
                            case "Daniel":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21004),
                                       globals.gItemManager.GetItemByID(24004)};
                                } break;
                            case "Sid":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21005),
                                       globals.gItemManager.GetItemByID(24005)};
                                } break;
                            case "More":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21012),
                                       globals.gItemManager.GetItemByID(24012)};
                                } break;
                            case "Kurogane":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22009),
                                       globals.gItemManager.GetItemByID(21014),globals.gItemManager.GetItemByID(18002),
                                       globals.gItemManager.GetItemByID(24014)};
                                } break;
                        }
                    } break;
                case BodyStyle.Small_Female:
                    {
                        HairStyle_SmallF g = new HairStyle_SmallF();
                        switch (Enum.GetName(g.GetType(), (HairStyle_SmallF)head))
                        {
                            case "Nina":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22003),
                                       globals.gItemManager.GetItemByID(21002),globals.gItemManager.GetItemByID(24002)};
                                } break;
                            case "Betty":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(22001),
                                       globals.gItemManager.GetItemByID(21003),globals.gItemManager.GetItemByID(24003)};
                                } break;
                        }
                    } break;
                case BodyStyle.Small_Male:
                    {
                        Hairstyle_SmallM g = new Hairstyle_SmallM();
                        switch (Enum.GetName(g.GetType(), (Hairstyle_SmallM)head))
                        {
                            case "Rocco":
                                {
                                    y = new cItem[]{ globals.gItemManager.GetItemByID(21001),
                                       globals.gItemManager.GetItemByID(24001)};
                                } break;
                        }

                    } break;
            }
            foreach (cItem u in y)
            {
                cInvItem g = new cInvItem(globals);
                g.ammt = 1;
                g.damage = 0;
                g.ID = u.ItemID;
                eq.SetEQ((byte)u.EquipPos, g);
            }

        }
        public void NpcTalk(bool init = false)
        {
            cSendPacket h = DatatoSend.Dequeue();
            if (init)
                globals.ac6.Send_2(1);
            h.character = this;
            h.Send();
        }

        #region Data Quick Send/GET
        public void Spawnto(byte dst,ushort mapTo = 0,ushort x = 0,ushort y = 0)
        {
            switch (dst)
            {
                case 1: //spawn to starter beach
                    {
                        if (state == 1)
                            if (map.MapID != 11016)
                            {
                                cWarp w = new cWarp();
                                w.id = 0; w.x = 500; w.y = 1000; w.mapTo = 11016;
                                globals.ac20.DoWarp(w);
                            }
                    } break;
                case 2: //spawn recorded spot
                    break;
                case 3: //spawn carnie
                    {
                        if (state == 1)
                            if (map.MapID != 11094)
                            {
                                lastMap.mapTo = map.MapID;
                                lastMap.x = x;
                                lastMap.y = y;
                                lastMap.id = 0;
                                cWarp w = new cWarp();
                                w.id = 0; w.x = 1062; w.y = 835; w.mapTo = 11094;
                                globals.ac20.DoWarp(w);
                            }
                    } break;
                case 4: // Special Warp
                    {
                        cWarp w = new cWarp();
                        w.id = 0; w.x = x; w.y = y; w.mapTo = mapTo;
                        globals.ac20.DoWarp(w);
                    }break;
            }
        }
        public void Send_8_1(bool levelup = false)
        {
            stats.Send8_1(eq.clothes, levelup);
        }
        public void Send_8_3(cCharacter t)
        {
            //stats to send for join team
            globals.ac8.Send_3(this.characterID, 25, this.stats.CurHP, 0, t);
            globals.ac8.Send_3(this.characterID, 29, this.stats.Con, 0, t);
            globals.ac8.Send_3(this.characterID, 207, 0, 0, t);
            globals.ac8.Send_3(this.characterID, 48, 0, 0, t);
            globals.ac8.Send_3(this.characterID, 55, 0, 0, t);
            globals.ac8.Send_3(this.characterID, 26, this.stats.CurSP, 0, t);
            globals.ac8.Send_3(this.characterID, 33, this.stats.Wis, 0, t);
            globals.ac8.Send_3(this.characterID, 208, 0, 0, t);
            globals.ac8.Send_3(this.characterID, 49, 0, 0, t);

        }
        public void Send_3_They(cCharacter target)
        {
            cCharacter c = this;
            cSendPacket p = new cSendPacket(globals);
            p.Header(3);
            p.AddDWord(c.characterID);
            p.AddByte((byte)c.body); //body style
            p.AddByte((byte)c.element); //element
            p.AddByte(c.level); //level
            p.AddWord(c.map.MapID); //map id
            p.AddWord(c.x); //x
            p.AddWord(c.y); //y
            p.AddByte(0); p.AddByte(c.head); p.AddByte(0);
            p.AddDWord(c.color1); //colors
            p.AddDWord(c.color2); //colors
            p.AddByte(c.eq.CountClothes());//clothesAmmt); // ammt of clothes
            p.AddArray(c.eq.GetWearingClothes());
            p.AddDWord(0); p.AddByte(0); //??
            p.AddByte(c.rebirth); //is rebirth
            p.AddByte((byte)c.job); //rb class
            p.AddString(c.name);//(BYTE*)c.name,c.nameLen); //name
            p.AddString(c.nickname);//(BYTE*)c.nick,c.nickLen); //nickname
            p.AddByte(255); //??
            p.SetSize();
            p.character = target;
            p.Send();

        }
        public void Send_3_Me()
        {
            cCharacter c = this;
            cSendPacket p = new cSendPacket(globals);
            p.Header(3);
            p.AddDWord(c.characterID);
            p.AddByte((byte)c.body);
            p.AddWord(c.map.MapID);
            p.AddWord(c.x);
            p.AddWord(c.y);
            p.AddByte(0); p.AddByte(c.head); p.AddByte(0);
            p.AddDWord(c.color1); p.AddDWord(c.color2);
            p.AddByte(c.eq.CountClothes());//clothesAmmt); // ammt of clothes
            p.AddArray(c.eq.GetWearingClothes());
            p.AddDWord(0);
            p.AddString(c.name);
            p.AddString(c.nickname);
            p.AddDWord(0);
            p.SetSize();
            p.character = this;
            p.Send();

            //244, 68, 43, 0, 3, 
            //138, 152, 26, 0, 
            //4, 
            //0, 49, //map
            //226, 1, 219, 1, 0, 1, 0, 28, 175, 125, 26, 28,         
            //175, 125, 26, 1, 28, 133, 0, 0, 0, 0, 
            //7, 111, 109, 110, 105, 98,    
            //117, 115, 0, 0, 0, 0, 0, 
        }
        public byte[] Get_63_1_Data(cGlobals src)
        {
            cSendPacket temp = new cSendPacket(src);
            temp.index = 0;

            if (state > 0)
            {
                temp.AddByte(slot);// data[at] = slot; at++;//PackSend->AddByte(1);
                temp.AddString(name);// data[at] = nameLen; at++;
                //memcpy(data + at, name, nameLen); at += nameLen;//	PackSend->AddString(tmp1.Name,tmp1.NameLen);// 006 083 097 109 109 117 115 		// name 
                temp.AddByte(level);// data[at] = level; at++;//	PackSend->AddByte(tmp1.level);					// lvl 
                temp.AddByte((byte)element);// data[at] = element; at++;//	PackSend->AddByte(3);  					// element
                temp.AddDWord(stats.MaxHP);// putDWord(maxHP, data + at); at += 4;//	PackSend->AddDWord(tmp1.maxHP); 			// max hp
                temp.AddDWord(stats.CurHP);// putDWord(curHP, data + at); at += 4;//	PackSend->AddDWord(tmp1.curHP); 			// cur hp
                temp.AddDWord(stats.MaxSP);// putDWord(maxSP, data + at); at += 4;//	PackSend->AddDWord(tmp1.maxSP); 			// max sp
                temp.AddDWord(stats.CurSP);// putDWord(curSP, data + at); at += 4;//	PackSend->AddDWord(tmp1.curSP); 			// cur sp
                temp.AddDWord(stats.TotalExp);// putDWord(experience, data + at); at += 4;//	PackSend->AddDWord(tmp1.exp);			// exp
                temp.AddDWord(gold);// putDWord(gold, data + at); at += 4;//	PackSend->AddDWord(tmp1.gold); 			// gold
                temp.AddByte((byte)body);// data[at] = body; at++;//	PackSend->AddByte(tmp1.body); 					// body style
                temp.AddByte(0); temp.AddByte(head); temp.AddByte(0);// data[at] = 0; data[at + 1] = head; data[at + 2] = 0; at += 3;//	PackSend->AddArray(tmp1.hair,3);// hair style
                temp.AddDWord(color1);// putDWord(color1, data + at); at += 4;//	PackSend->AddDWord(tmp1.colors1);
                temp.AddDWord(color2);// putDWord(color2, data + at); at += 4;//	PackSend->AddDWord(tmp1.colors2);
                temp.AddByte(rebirth); temp.AddByte((byte)job);// data[at] = rebirth; data[at + 1] = job; at += 2;//PackSend->AddByte(tmp1.rebirth);PackSend->AddByte(tmp1.rebirthJob); 				// rebirth flag, job skill
                temp.AddArray(eq.GetFullClothes());// at = inv.Get_63_1_Data(data, at);//memcpy(data+at,(BYTE*)clothing,12);at+=12;//	PackSend->AddArray(tmp1.clothes+1,12);
            }
            if (temp.index < 1) return null;
            byte[] data = new byte[temp.index];
            Array.Copy(temp.data.ToArray(), 0, data, 0, temp.index);
            return data;
        }
        public byte[] Get_5_0_Data()
        {
            return eq.GetWearingClothes();
            //return null;
        }
        #endregion
        
        //----------------------Extra
        
        
    }
}

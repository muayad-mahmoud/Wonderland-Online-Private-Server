using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PServer_v2.NetWork.ACS;
using PServer_v2.DataLoaders;
using PServer_v2.NetWork.Managers;

namespace PServer_v2.NetWork.DataExt
{
    public class cWarp
    {
        public byte entry = 0;
        public UInt16 id = 0;
        public UInt16 mapTo = 0;
        public UInt16 x = 0;
        public UInt16 y = 0;
        public byte unkval1 = 0;
        public byte neededtopass = 0;
        public byte unkval2 = 0;
    }
    public enum WarpType
    {
        Login,
        regular,
        Questwarp,
        ToolWarp,
        SpecialWarp,
    }

    public class cFlag
    {
        cCharacter src;
        int Event;

    }

    public class cMap
    {
        List<cCharacter> charList = new List<cCharacter>();
        cGroundItem[] groundItem = new cGroundItem[256];
        bool dataloaded = false;
        bool loaded = false;
        GroundNodeInfo FloorData;
        public SceneInfo sceneinfo;
        public MapData mapData;
        object cMapLock = new object();        
        public string name;
        bool CivilianArea = false;
        bool Tentsallowed = true;
        bool battlesallowed = true;
        cGlobals globals;

        UInt16 mapID; public UInt16 MapID { get { return mapID; } set { mapID = value; } }
        public List<cCharacter> Characters { get { return charList; } }
        public int CharCnt { get { return charList.Count; } }
        //-----------------InMap Managers
        cBattleSystem battleMng; 

        public cMap(cGlobals src)
        {
            globals = src;
            battleMng = new cBattleSystem(globals);
            mapID = 0;
            name = "";
            for (int y = 0; y < groundItem.Length; y++)
                groundItem[y] = new cGroundItem(globals);
        }
        ~cMap()
        {
            loaded = false;
        }

        public void MapProcess()
        {
            try
            {
                //this will process all activities of a map until server closes
                for (int a = 0; a < mapData.Npclist.Count; a++)
                    if (mapData.Npclist[a].NeedUpdate(globals.UpTime.Elapsed))
                        //check for character place
                        //get list of all players
                        
                        mapData.Npclist[a].Update(Characters, globals);
                //look at all battle maps
                battleMng.Process();
                //update ground items
                for (int a = 0; a < mapData.ItemAreas.Count; a++)
                    if (mapData.ItemAreas[a].Drop(globals.UpTime.Elapsed))
                        PutItem(mapData.ItemAreas[a]);
                for (int a = 0; a < groundItem.Length; a++)
                    if (groundItem[a].Remove(globals.UpTime.Elapsed))
                        for (int b = 0; b < charList.Count; b++)
                            RemGItem((ushort)a, charList[b], false);
            }
            catch { }
        }

        public string Log()
        {
            string str = "";
            str += name + " " + mapID.ToString("00000");
            return str;
        }
        public void LocalChat(cCharacter sender, string text)
        {
            for(int a= 0;a<charList.Count;a++)
            {
                if (charList[a] != sender)
                {
                    cSendPacket chat = new cSendPacket(globals);
                    chat.Header(2, 2);
                    chat.AddDWord(sender.characterID);
                    chat.AddString(text);
                    chat.SetSize();
                    chat.character = charList[a];
                    chat.Send();
                }
            }
        }
        public void Emote(cCharacter sender, byte emote)
        {
            sender.emote = emote;
            foreach (cCharacter c in Characters)
            {
                if (c != sender)
                    globals.ac32.Send_2(sender.characterID, emote, c);
            }
        }
        public void EmoteBubble(cCharacter sender, byte emotebubble)
        {
            List<cCharacter> clist = new List<cCharacter>();
            foreach (cCharacter character in Characters)
                if (character != sender) clist.Add(character);

            globals.ac32.Send_1(clist, sender, emotebubble);
        }
        public void Load()
        {
            FloorData = globals.gGroundData.GetNodeBySceneName(mapID.ToString());
            mapData.Entry_Points = globals.gEveManager.LoadEntryEntries(mapData);
            mapData.Npclist = globals.gEveManager.LoadNpcEntries(mapData);
            mapData.MiningAreas = globals.gEveManager.LoadMiningEntries(mapData);
            mapData.ItemAreas = globals.gEveManager.LoadItemEntries(mapData);
            mapData.WarpLoc = globals.gEveManager.LoadWarpEntries(mapData);
            mapData.InteractiveInfo = globals.gEveManager.LoadInteractiveEntries(mapData);
            mapData.Events = globals.gEveManager.LoadEventEntries(mapData);
            mapData.Group = globals.gEveManager.LoadGroupEntries(mapData);
            mapData.ExtGroup = globals.gEveManager.LoadgroupExtEntries(mapData);
            mapData.PreEvents = globals.gEveManager.LoadpreEventEntries(mapData);
            mapData.ExtBattleInfo = globals.gEveManager.LoadBattleEntries(mapData);
            SetItems();
            dataloaded = true;
        }
        
        #region Npc Section

        public NpcEntries GetNpc(int to)
        {
            return mapData.Npclist[to - 1];
        }
        public void SendNpcs(cCharacter c)
        {
            int ct = 0;
            cSendPacket j = new cSendPacket(globals);
            if (mapData.Npclist == null)
                mapData.Npclist = globals.gEveManager.LoadNpcEntries(globals.gEveManager.GetbyID(mapID));
            else
            {
                j = new cSendPacket(globals);
                j.Header(22, 4);
                foreach (NpcEntries h in mapData.Npclist)
                {
                    ct++;
                    j.AddWord(h.clickId);
                    if (h.unknownbyte3 != 5)
                        j.AddWord(h.unknownword2);
                    else
                        j.AddWord(0);
                    j.AddWord((ushort)h.x);
                    j.AddWord((ushort)h.y);
                    j.AddWord(h.unknownbyte4);
                    j.AddDWord(0);
                }
                j.SetSize();
                j.character = c;
                j.Send();
            }

        }
        #endregion
        
        #region Item Section

        public void SetItems()
        {
            foreach (ItemsinMapEntries j in mapData.ItemAreas)
                PutItem(j);
        }

        public void SendItems(cCharacter target = null)
        {
            bool bfound = false;
            int unk = 0;
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 4);
            for (int a = 0; a < groundItem.Length; a++)
            {
                if (groundItem[a].id > 0)
                {
                    if (groundItem[a].reserved)
                    {
                        p.AddByte(3); bfound = true;
                        unk = groundItem[a].Info.unknownword1;
                    }

                    if (!bfound)
                        p.AddByte(1);
                    p.AddWord((ushort)a);
                    p.AddDWord((ushort)groundItem[a].id);
                    p.AddWord((ushort)groundItem[a].x);
                    p.AddWord((ushort)groundItem[a].y);
                    p.AddDWord((uint)unk);
                }
            }
            p.SetSize();
            if (p.index > 7)
            {
                p.character = target;
                p.Send();
            }
        }
        public cGroundItem GetGroundItem(UInt16 itemIndex, cCharacter src, bool remove)
        {
            cGroundItem gItem = new cGroundItem(globals);
            if (groundItem[itemIndex].id > 0)
            {
                gItem.CopyFrom(groundItem[itemIndex]);
                if (remove)
                {
                    foreach (ItemsinMapEntries o in mapData.ItemAreas)
                    {
                        if (groundItem[itemIndex].entryid == o.clickID && groundItem[itemIndex].reserved)
                        {
                            o.pickedup = true;
                            o.dropin = new TimeSpan(0, 2, 0) + globals.UpTime.Elapsed;
                        }
                    }
                    RemoveGroundItem(itemIndex, src);

                }
            }
            return gItem;
        }
        void RemoveGroundItem(UInt16 itemIndex, cCharacter src)
        {
            groundItem[itemIndex].Clear();
            foreach (cCharacter c in charList)
            {
                if (c.characterID == src.characterID)
                    RemGItem(itemIndex, c, true);
                else
                    RemGItem(itemIndex, c, false);
            }
        }
        public int countItems
        {
            get
            {
                int ammt = 0;
                for (int n = 1; n < 256; n++)
                {
                    if (groundItem[n].id != 0) ammt++;
                }
                return ammt;
            }
        }
        int GetEmptyGroundSlot
        {
            get
            {
                int loc = 0;
                for (int n = 1; n < 256; n++)
                    if (groundItem[n].id == 0) { loc = n; break; }
                return loc;
            }
        }
        public void PutItem(ItemsinMapEntries g)
        {
            cGroundItem gi = new cGroundItem(globals);
            gi.CopyFrom(g);
            if ((countItems + 1) <= 255)
            {
                for (int ct = 0; ct < 1; ct++)
                {
                    int loc = GetEmptyGroundSlot;
                    if (loc != 0)
                    {
                        gi.reserved = true;
                        gi.Info = g;
                        groundItem[loc] = gi;
                        foreach (cCharacter c in charList)
                            DropGItem(gi, c);
                    }
                }
            }
            g.pickedup = false;
            g.dropin = new TimeSpan(0, 0, 0);
        }
        public bool DropItem(cInvItem i, cCharacter src)
        {
            bool ret = false;

            if ((countItems + i.ammt) <= 255)
            {
                for (int ct = 0; ct < i.ammt; ct++)
                {
                    cGroundItem gi = new cGroundItem(globals);
                    gi.CopyFrom(i);
                    gi.x = (ushort)(src.x + new Random().Next(-10, 10));
                    gi.y = (ushort)(src.y + new Random().Next(-10, 10));
                    gi.lifetime = 1000;
                    int loc = GetEmptyGroundSlot;
                    if (loc != 0)
                    {
                        groundItem[loc] = gi;
                        groundItem[loc].Removein = globals.UpTime.Elapsed.Duration() + new TimeSpan(0, 2, 0);
                        foreach (cCharacter c in charList)
                            DropGItem(gi, c);
                        ret = true;
                    }

                }
            }

            return ret;
        }
        public void RemGItem(UInt16 itemIndex, cCharacter target, bool animate)
        {
            if (target.warping) return;
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 2);
            p.AddWord(itemIndex);
            if (animate)
                p.AddByte(1);
            p.SetSize();
            p.character = target;
            p.Send();
            groundItem[itemIndex].Clear();
        }
        public void DropGItem(cGroundItem item, cCharacter target)
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 3);
            p.AddWord(item.id);
            p.AddWord(item.x);
            p.AddWord(item.y);
            p.AddDWord(0);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        #endregion
        
        #region Warping Section
        public void LogintoMap(cCharacter c)
        {
            AddFromLogging(c);
        }
        public void WarpRequest(WarpType typeofwarp,cCharacter t,int Entry=0, cWarp i = null)
        {
            try
            {
                globals.packet.character.warping = true;
                switch (typeofwarp)
                {
                    case WarpType.regular:
                        {
                            var exitpoint = mapData.Entry_Points[Entry - 1];
                            var WarpID = (int)mapData.Events[exitpoint.unknownbytearray1[0] - 1].SubEntry[0].SubEntry[0].dialog2;
                            if (WarpID == 0)
                            {
                                globals.ac20.Send_8(t); return;
                            }
                            globals.gServer.Multipkt_Request(t);
                            var map = Phase1Warp(t, mapData.WarpLoc[WarpID - 1]);
                            globals.gServer.Queue_Request(t);
                            map.Phase2Warp(t);
                            globals.packet.character.DatatoSend.Enqueue(globals.gServer.GenerateQueuepkt(t));
                            globals.gServer.SendCombinepkt(t);
                        } break;
                    case WarpType.SpecialWarp:
                        {
                            WarpInfo f = new WarpInfo();
                            f.clickID = i.id;
                            f.mapID = i.mapTo;
                            f.x = i.x;
                            f.y = i.y;
                            globals.gServer.Multipkt_Request(t);
                            var map = Phase1Warp(t, f,false);
                            globals.gServer.Queue_Request(t);
                            map.Phase2Warp(t);
                            globals.packet.character.DatatoSend.Enqueue(globals.gServer.GenerateQueuepkt(t));
                            globals.gServer.SendCombinepkt(t);
                        } break;
                    case WarpType.Questwarp:
                        {
                            var exitpoint = mapData.Entry_Points[Entry - 1];
                            var WarpID = (int)mapData.Events[exitpoint.unknownbytearray1[0] - 1].SubEntry[0].SubEntry[0].dialog2;
                            globals.gServer.Queue_Request(t);
                           
                            var map = Phase1Warp(t, mapData.WarpLoc[WarpID - 1]);
                            globals.packet.character.DatatoSend.Enqueue(globals.gServer.GenerateQueuepkt(t));
                            globals.gServer.Queue_Request(t);
                            map.Phase2Warp(t);
                            globals.packet.character.DatatoSend.Enqueue(globals.gServer.GenerateQueuepkt(t));
                        } break;
                    case WarpType.ToolWarp:
                        {
                            t.inv.RemoveInv((byte)Entry, 1);
                        } break;
                    case WarpType.Login: Phase2Warp(t); break;
                }
            }
            catch { }
        }
        cMap Phase1Warp(cCharacter t, WarpInfo f,bool g20_7 = true)
        {
            if (g20_7)
                globals.ac20.Send_7(t);
            globals.ac23.Send_32(t.characterID, t);
            globals.ac23.Send_112(t.characterID, t);
            globals.ac23.Send_132(t.characterID, t);
            RemoveByWarp(t, f);
            globals.packet.character.warping = true;
            t.x = (ushort)f.x;
            t.y = (ushort)f.y;
            globals.ac12.Send(t.characterID, t, f);
            globals.ac5.Send_0(t, t);
            var mapto = globals.gMapManager.GetMapByID(f.mapID);
            mapto.AddFromWarp(t, f);
            return mapto;
        }
        void Phase2Warp(cCharacter t)
        {
            cSendPacket tmp = new cSendPacket(globals);
            globals.ac23.Send_138(t);
            globals.ac6.Send_2(1);

            for (int a = 0; a < charList.Count; a++)
                globals.ac23.Send_122(charList[a].characterID, t);

            for (int a = 0; a < charList.Count; a++)
            {
                if (charList[a] != t)
                {
                    globals.ac10.Send_3(charList[a].characterID, 255, t);

                    if (charList[a].riceBall.id > 0)
                    {
                        if (charList[a].riceBall.active) globals.ac5.Send_5(charList[a].riceBall.id, charList[a], t);
                    }
                    if (t.riceBall.id > 0)
                    {
                        if (t.riceBall.active) globals.ac5.Send_5(t.riceBall.id, t, charList[a]);
                    }
                    
                    if (t.Party.PartyLeader && t.Party.hasParty && charList[a] != t)
                    {
                        cSendPacket fg = t.Party.Get_13_6();
                        fg.character = charList[a];
                        fg.Send();
                    }
                    //if (character.characterID != t.characterID)
                    //g.ac23.Send_74(character.characterID, 0, c); //TODO find out what this does
                    //AC 15,4 //possibly pet info for players on map with pets
                    if (charList[a].Party.PartyLeader && charList[a].Party.hasParty)
                    {
                        cSendPacket fg = charList[a].Party.Get_13_6();
                        fg.character = t;
                        fg.Send();
                    }
                    //if (character.characterID != c.characterID)
                    //cAC_11.Send_4(2, character.characterID, 0, 0, c);
                    //15_4

                }
            }
            
            SendNpcs(t);
            SendItems(t);
            Send_32_2(t);
            //23_76
            for (int a = 0; a < charList.Count; a++)
                globals.ac23.Send_76(charList[a].characterID, t);
            //39_9
            //cSendPacket gh = new cSendPacket(globals);
            //gh.AddArray(new byte[] { 244, 68, 5, 0, 22, 6, 1, 0, 1, 244, 68, 5, 0, 22, 6, 21, 0, 1, 244, 68, 5, 0, 22, 6, 22, 0, 1, 244, 68, 5, 0, 22, 6, 23, 0, 1, 244, 68, 5, 0, 22, 6, 24, 0, 1, });
            // cServer.Send(gh, t);
            /*tmp = new cSendPacket(globals);
            tmp.Header(6, 2);
            tmp.AddByte(1);
            tmp.SetSize();
            tmp.character = t;
            tmp.Send();
            for (int a = 0; a < 1; a++)
            {
                gh = new cSendPacket(globals);
                gh.AddArray(new byte[] { 244, 68, 2, 0, 20, 11, 244, 68, 2, 0, 20, 10 });
                t.DatatoSend.Enqueue(gh);
            }
            for (int a = 0; a < 1; a++)
            {
                gh = new cSendPacket(globals);
                gh.AddArray(new byte[] { 244, 68, 2, 0, 20, 10 });
                t.DatatoSend.Enqueue(gh);
            }*/
            globals.ac23.Send_102(t);
            globals.ac20.Send_8(t);
        }

        void AddFromLogging(cCharacter c)
        {
            for (int a = 0; a < charList.Count; a++)
            {
                if (charList[a] == c) return;
            }
            c.map = this;

            if (!dataloaded)
                Load();
            charList.Add(c);

            for (int a = 0; a < charList.Count; a++)
                if (charList[a].characterID != c.characterID)
                {
                    if (!c.logging)
                    {
                        globals.ac5.Send_0(c, charList[a]);
                        globals.ac10.Send_3(c.characterID, 255, charList[a]); //maybe guild info???
                        globals.ac23.Send_122(c.characterID, charList[a]);
                    }

                    //send to me
                    globals.ac7.Send(charList[a].characterID, charList[a].map.MapID, charList[a].x, charList[a].y, c);
                    globals.ac5.Send_0(charList[a], c);
                }
            //Send_32_2(c);
        }
        void AddFromWarp(cCharacter c, WarpInfo warp)
        {
            bool duplicate = false;
            for (int a = 0; a < charList.Count; a++)
                if (charList[a] == c) duplicate = true;
            if (!duplicate)
            {
                if (!dataloaded)
                    Load();
                c.map = this;
                charList.Add(c);
            }
            else
                return;
            //in order
            for (int a = 0; a < charList.Count; a++)//sending to others
                if (charList[a].characterID != c.characterID)
                {
                    globals.ac12.Send(c.characterID, charList[a], warp);
                    globals.ac5.Send_0(c, charList[a]);
                    
                }
            if (c.vechile.inVechile)
                        c.vechile.RideVech(c.vechile.Car, true);
            //send to me
            for (int a = 0; a < charList.Count; a++)
                if (charList[a].characterID != c.characterID)
                {
                    globals.ac7.Send(charList[a].characterID, charList[a].map.MapID, charList[a].x, charList[a].y, c);
                    globals.ac5.Send_0(charList[a], c);
                }

            //15_11 prob not lol
        }
            
        public void RemoveByDC(cCharacter c)
        {
            charList.Remove(c);
        }
        public void RemoveByWarp(cCharacter c, WarpInfo warp)
        {
            charList.Remove(c);
            for (int a = 0; a < charList.Count; a++)
                globals.ac12.Send(c.characterID, charList[a], warp);
        }

        #endregion

        #region Quest


        #endregion

        #region Quick Data Send Function
        public void Send_32_2(cCharacter c)
        {
            List<cCharacter> clist = new List<cCharacter>();
            for (int a = 0; a < Characters.Count; a++)
                if (Characters[a] != c && (Characters[a].emote != 0)) clist.Add(Characters[a]);
            if (clist.Count > 0)
                globals.ac32.Send_2(clist, c);
        }
        public void Send_5_1(cItem i, cCharacter src)
        {
            for (int a = 0; a < Characters.Count; a++)
                if (Characters[a] != src) globals.ac5.Send_1(src.characterID, i.ItemID, Characters[a]);
        }
        public void Send_5_2(cItem i, cCharacter src)
        {
            for (int a = 0; a < Characters.Count; a++)
                if (Characters[a] != src) globals.ac5.Send_2(src.characterID, i.ItemID, Characters[a]);
        }
        public void SendtoCharacters(cSendPacket p)
        {
            for (int a = 0; a < Characters.Count; a++)
            {
                p.character = Characters[a];
                p.Send();
            }
        }
        public void DSendtoCharacters(cSendPacket p)
        {
            for (int a = 0; a < Characters.Count; a++)
            {
                p.character = Characters[a];
                p.Send();
            }
        }
        public void SendtoCharactersEx(cSendPacket p, cCharacter y)
        {
            for (int a = 0; a < Characters.Count; a++)
                if (Characters[a] != y)
                {
                    p.character = Characters[a];
                    p.Send();
                }
        }
        #endregion

        public void UpdateRiceBall(UInt16 npcID, cCharacter src)
        {
            foreach (cCharacter c in Characters)
            {
                if (src.riceBall.active) globals.ac5.Send_5(npcID, src, c);
                else globals.ac5.Send_5(0, src, c);
            }
        }

        //battle
        public void StartPK(cCharacter starter, cCharacter enemy) //TODO these should be lists of players
        {
            battleMng.StartPK(starter, enemy);
        }
        public void StartPKNpc(cCharacter starter, cFighter enemy) //TODO these should be lists of players
        {
            battleMng.StartPKNpc(starter, enemy);
        }
        public void StartAmbush(cCharacter starter, cFighter enemy)
        {
            battleMng.StartBattleAmbush(starter, enemy);
        }
        
    }
}

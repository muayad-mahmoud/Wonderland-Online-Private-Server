using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using System.Diagnostics;
using PServer_v2.NetWork;
using System.IO;

namespace PServer_v2.DataLoaders
{
    public class EveManager
    {
        byte[] eveData;
        cGlobals globals;
        List<MapData> Maps = new List<MapData>();
        public EveManager(cGlobals g)
        {
            globals = g;
        }
        public bool LoadFile(string filename)
        {
            globals.Log("Loading Eve.EMG.....");
            try
                       {
            Maps.Clear();
            if (!File.Exists(filename)) return false;
            byte[] data = File.ReadAllBytes(filename);
            eveData = data;
            ReadData(data);
            globals.Log("done loading Eve");
            return true;

            }
            catch { globals.Log("failed"); }
            return false;

        }

        public UInt16 GetSceneIDbyMapID(UInt16 id)
        {
            foreach (MapData r in Maps)
                if (r.mapID == id)
                    return r.sceneID;
            return 0;
        }
        public MapData GetbyID(ushort ID)
        {
            foreach (MapData r in Maps)
            {
                if (r.mapID == ID)
                    return r;
            }
            return null;
        }
        void ReadData(byte[] d)
        {
            int ptr = 0;
            // header is 12 bytes
            //entry length is important to know when to stop 
            ptr += 8;
            uint entrylen = globals.GetDWord(d, ptr); ptr += 4;
            // Now Process by categories
            //MapData <---better to be call since houses all info within that map
            Load_MapEntries(ref ptr, d, entrylen);
            //after the long processing we now enter the second stage of the long processing
            //going throug each map... to pull the req offset data
            Load_ScenceData(ref ptr, d, (uint)d.Length);
            //after that we enter the third Stage here well will seek out the category infos
            //n inject the data we need into the maps within the Mapmanager
            //to avoid using EveManager as the ref to speed up the server
            //since we have the offest values we will go straight to it to pull certain data
            //tricky as hell i kno
            Load_FinalData();

        }
        void Load_MapEntries(ref int ptr, byte[] d, uint len)
        {
            globals.Log("Stage 1\r\nReading Map Entries.....");
            //used to load MapEntries
            //File.Delete("LogOutput.txt");
            for (int a = 0; a < len; a++)
            {
                MapData tmp = new MapData();
                tmp.mapID = globals.GetWord(d, ptr); ptr += 2;
                tmp.sceneID = globals.GetWord(d, ptr); ptr += 2;
                tmp.dataptr = globals.GetDWord(d, ptr); ptr += 4;
                tmp.datalen = globals.GetWord(d, ptr); ptr += 2;
                Maps.Add(tmp);


                StreamWriter te = File.AppendText("LogOutput.txt");
                te.WriteLine("Map->"+tmp.mapID.ToString()+" Scence ID-"+tmp.sceneID.ToString()+"  dataptr-"+tmp.dataptr.ToString()+"  datalen"+tmp.datalen.ToString());
                te.Close();
            }
            globals.Log("done");
        }
        void Load_ScenceData(ref int ptr2, byte[] d, uint len)
        {
            globals.Log("Stage2\r\nReading Scene Data Category Offset Entries.....");
            //used to load Scence Data Entries

            foreach (MapData scen in Maps)
            {
                int count = 0;
                int ptr = (int)scen.dataptr;
                //find the Map that the data belongs to
                UInt16 id = globals.GetWord(d, ptr); ptr += 2;///skipping        
                if (scen == null)
                {
                }
                scen.unknownword = globals.GetWord(d, ptr); ptr += 2;
                ptr = ((int)(scen.datalen + scen.dataptr) - 44);
                // ptr += ptr2;
                #region Offsets to Data
                categoryoffset j = new categoryoffset();
                j.NPC = globals.GetDWord(d, ptr); ptr += 4;
                j.Entry = globals.GetDWord(d, ptr); ptr += 4;
                j.Mining = globals.GetDWord(d, ptr); ptr += 4;
                j.Items = globals.GetDWord(d, ptr); ptr += 4;
                j.Events = globals.GetDWord(d, ptr); ptr += 4;
                j.Groups = globals.GetDWord(d, ptr); ptr += 4;
                j.Warp = globals.GetDWord(d, ptr); ptr += 4;
                j.Interactiveinfo = globals.GetDWord(d, ptr); ptr += 4;
                j.Battleinfo = globals.GetDWord(d, ptr); ptr += 4;
                j.PreEvent = globals.GetDWord(d, ptr); ptr += 4;
                j.groupext = globals.GetDWord(d, ptr); ptr += 4;
                scen.offsetlist = j;
                #endregion
                if (scen.datalen == 1028)
                {
                }
                if (ptr2 > scen.datalen + scen.dataptr)
                {
                }
                if (ptr < scen.datalen + scen.dataptr)
                {
                    globals.Log("Map" + scen.mapID.ToString() + " data mistmach... counted length-"
                    + count.ToString() + " lower than req" + scen.datalen);
                    ptr2 += scen.datalen;
                }
                else if (count > scen.datalen)
                {
                    globals.Log("Map" + scen.mapID.ToString() + " data mistmach... counted length-"
                    + count.ToString() + " greater than req" + scen.datalen);
                    ptr2 += scen.datalen;
                }
                ptr2 += scen.datalen;
                if (ptr2 > 4760262)
                    break;
            }
            globals.Log("done");
            globals.Log("Data Read");
            globals.Log("Map Data found -" + globals.gEveManager.Maps.Count.ToString());
        }
        void Load_FinalData()
        {
            Stopwatch timer = new Stopwatch();
            globals.Log("Stage 3..Loading Data into Map...");
            timer.Reset();
            timer.Start();
            //this will be the loader
            //Entry n Warp for Now
            int ct = 0;
            foreach (MapData r in Maps)
            {
                ct++;
                try
                {                    
                    cMap map = new cMap(globals);
                    map.MapID = r.mapID;
                    if (r.mapID == 11016)
                    {
                    }
                    try
                    {
                        map.sceneinfo = globals.gSceneManager.GetSceneByID(r.sceneID);
                        map.name = map.sceneinfo.Name;
                        map.mapData = r;
                    }
                    catch { }
                    
                    //map.EntryPoints = LoadEntryEntries(r);
                    //map.Npclist = LoadNpcEntries(r);
                    //map.VacumSpots = LoadMiningEntries(r);
                    //map.ItemSpot = LoadItemEntries(r);
                    //map.WarpPointList = LoadWarpEntries(r);
                    //map.DataSafe.Interaction = LoadInteractiveEntries(r);
                    //map.EventList = LoadEventEntries(r);
                    //map.DataSafe.group = LoadGroupEntries(r);
                    //map.DataSafe.groupext = LoadgroupExtEntries(r);
                    //map.DataSafe.preevent = LoadpreEventEntries(r);
                    //map.DataSafe.battleinfo = LoadBattleEntries(r);
                    //map.SetItems();
                    globals.gMapManager.mapList.Add(map);
                }
                catch { }
            }

            timer.Elapsed.ToString();
            timer.Stop();
            globals.Log("Done");
        }
        public List<NpcEntries> LoadNpcEntries(MapData y)
        {
            try
            {
                byte[] d = eveData;
                var ptr = (int)y.offsetlist.NPC + (int)y.dataptr;
                List<NpcEntries> map = new List<NpcEntries>();
                UInt16 elen = globals.GetWord(d, ptr); ptr += 2;
                for (int a = 0; a < elen; a++)
                {
                    NpcEntries tmp = new NpcEntries();
                    tmp.globals = globals;
                    tmp.location = globals.gMapManager.GetMapByID(y.mapID);                    
                    tmp.clickId = globals.GetWord(d, ptr); ptr += 2;
                    tmp.Name = "";
                    for (int ap = 1; ap < d[ptr] + 1; ap++)
                        tmp.Name += (char)d[ap + ptr]; ptr += 20;
                    tmp.unknownbyte1 = d[ptr]; ptr++;
                    tmp.x = globals.GetDWord(d, ptr); ptr += 4;
                    tmp.y = globals.GetDWord(d, ptr); ptr += 4;
                    int blen = d[ptr]; ptr++;
                    for (int c = 0; c < blen; c++)
                    {
                        tmp.Events.Add(d[ptr]); ptr++;
                    }
                    blen = d[ptr]; ptr++;
                    for (int c = 0; c < blen; c++)
                    {
                        tmp.unknownbytearray2.Add(d[ptr]); ptr++;
                    }
                    tmp.unknownbyte2 = d[ptr]; ptr++;
                    tmp.npcId = globals.GetDWord(d, ptr); ptr += 4;
                    tmp.myinfo = globals.gNpcManager.GetNpcbyID((ushort)tmp.npcId);
                    tmp.unknownbyte3 = d[ptr]; ptr++;
                    tmp.unknownbyte4 = d[ptr]; ptr++;
                    tmp.unknownbyte5 = d[ptr]; ptr++;
                    blen = d[ptr]; ptr++;
                    for (int c = 0; c < blen; c++)
                    {
                        npcWalkStep r = new npcWalkStep();
                        r.x = globals.GetDWord(d, ptr); ptr += 4;
                        r.y = globals.GetDWord(d, ptr); ptr += 4;
                        r.z = globals.GetDWord(d, ptr); ptr += 4;
                        tmp.walksteps.Add(r);
                    }
                    tmp.unknownbyte6 = d[ptr]; ptr++;
                    tmp.unknownbyte7 = d[ptr]; ptr++;
                    tmp.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                    tmp.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                    tmp.unknownbyte8 = d[ptr]; ptr++;
                    tmp.unknownbyte9 = d[ptr]; ptr++;
                    tmp.unknownbyte10 = d[ptr]; ptr++;
                    blen = d[ptr]; ptr++;
                    for (int c = 0; c < blen; c++)
                    {
                        walkpattern r = new walkpattern();
                        r.unknownbyte1 = d[ptr]; ptr++;
                        r.unknownbyte2 = d[ptr]; ptr++;
                        r.unknownbyte3 = d[ptr]; ptr++;
                        r.steps_needed = d[ptr]; ptr++;
                        r.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                        r.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                        for (int dd = 0; dd < 10; dd++)
                        {
                            npcWalkStep st = new npcWalkStep();
                            st.x = globals.GetDWord(d, ptr); ptr += 4;
                            st.y = globals.GetDWord(d, ptr); ptr += 4;
                            r.walksteps.Add(st);
                        }
                        tmp.walkpatterns.Add(r);
                    }
                    tmp.unknownword1 = globals.GetWord(d, ptr); ptr += 2;
                    tmp.unknownword2 = globals.GetWord(d, ptr); ptr += 2;
                    tmp.unknownword3 = globals.GetWord(d, ptr); ptr += 2;
                    tmp.unknownword4 = globals.GetWord(d, ptr); ptr += 2;

                    map.Add(tmp);
                }
                return map;
            }
            catch { }
            return null;
        }
        public List<Entry_Exit_Point_Entries> LoadEntryEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Entry + (int)r.dataptr;
            List<Entry_Exit_Point_Entries> map = new List<Entry_Exit_Point_Entries>();
            #region Entry/Exit Points
            //Entry/Exit Points
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                Entry_Exit_Point_Entries tmp = new Entry_Exit_Point_Entries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.x = globals.GetDWord(d, ptr); ptr += 4;
                tmp.y = globals.GetDWord(d, ptr); ptr += 4;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray1.Add(d[ptr]); ptr++;
                }
                blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray2.Add(d[ptr]); ptr++;
                }
                tmp.unknownbyte2 = d[ptr]; ptr++;
                tmp.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                tmp.unknowndword3 = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknowndword4 = globals.GetDWord(d, ptr); ptr += 4;

                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<MiningAreaEntries> LoadMiningEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Mining + (int)r.dataptr;
            List<MiningAreaEntries> map = new List<MiningAreaEntries>();
            #region Mining Area

            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                MiningAreaEntries tmp = new MiningAreaEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.x = globals.GetDWord(d, ptr); ptr += 4;
                tmp.y = globals.GetDWord(d, ptr); ptr += 4;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray1.Add(d[ptr]); ptr++;
                }
                blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray2.Add(d[ptr]); ptr++;
                }
                tmp.unknownbyte2 = d[ptr]; ptr++;
                blen = d[ptr]; ptr++;
                tmp.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                tmp.unknownbyte4 = d[ptr]; ptr += 2;

                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<ItemsinMapEntries> LoadItemEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Items + (int)r.dataptr;
            List<ItemsinMapEntries> map = new List<ItemsinMapEntries>();
            #region Items in Map
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                ItemsinMapEntries tmp = new ItemsinMapEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                int len = d[ptr]; ptr++;
                for (int ap = 0; ap < 19; ap++)
                {
                    tmp.Name += (char)d[ptr + ap];
                } ptr += 19;
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.x = globals.GetDWord(d, ptr); ptr += 4;
                tmp.y = globals.GetDWord(d, ptr); ptr += 4;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray1.Add(d[ptr]); ptr++;
                }
                blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    tmp.unknownbytearray2.Add(d[ptr]); ptr++;
                }
                tmp.unknownbyte2 = d[ptr]; ptr++;
                tmp.itemID = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                tmp.unknownbyte4 = d[ptr]; ptr++;
                tmp.unknownbyte5 = d[ptr]; ptr++;
                tmp.unknownword1 = globals.GetWord(d, ptr); ptr += 2;
                tmp.unknownword2 = globals.GetWord(d, ptr); ptr += 2;
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<EventsinMapEntries> LoadEventEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Events + (int)r.dataptr;
            List<EventsinMapEntries> map = new List<EventsinMapEntries>();
            #region Events in Map
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                EventsinMapEntries tmp = new EventsinMapEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.Name = "";
                byte[] test = new byte[20];
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; test[ap] = d[ptr]; ptr++;
                }
                //tmp.Name = Convert.ToBase64String(test);
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    EventSubEntry u = new EventSubEntry();
                    u.subIndex = d[ptr]; ptr++;
                    u.unknownbyte1 = d[ptr]; ptr++;
                    u.unknownword1 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknownword2 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknownword3 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknownword4 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknownword5 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknownword6 = globals.GetWord(d, ptr); ptr += 2;
                    u.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                    u.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                    int blen2 = d[ptr]; ptr++;
                    for (int cs = 0; cs < blen2; cs++)
                    {
                        EventSubSubEntry ur = new EventSubSubEntry();
                        ur.subsubIndex = d[ptr]; ptr++;
                        ur.DialogPtr = d[ptr]; ptr++;
                        ur.dialog1 = globals.GetWord(d, ptr); ptr += 2;
                        ur.dialog2 = globals.GetWord(d, ptr); ptr += 2;
                        ur.dialog3 = globals.GetWord(d, ptr); ptr += 2;
                        ur.dialog4 = globals.GetWord(d, ptr); ptr += 2;
                        ur.unknowndword1 = globals.GetDWord(d, ptr); ptr += 4;
                        ur.unknowndword2 = globals.GetDWord(d, ptr); ptr += 4;
                        ur.unknowndword3 = globals.GetDWord(d, ptr); ptr += 4;
                        u.SubEntry.Add(ur);
                    }
                    tmp.SubEntry.Add(u);
                }
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<GroupEntries> LoadGroupEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Groups + (int)r.dataptr;
            List<GroupEntries> map = new List<GroupEntries>();
            #region Groups
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                GroupEntries tmp = new GroupEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.unknownbyte2 = d[ptr]; ptr++;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                tmp.unknownword1 = globals.GetWord(d, ptr); ptr += 2;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    GroupSubEntry y = new GroupSubEntry();
                    y.unknownbyte1 = d[ptr]; ptr++;
                    y.unknownbyte2 = d[ptr]; ptr++;
                    y.unknownbyte3 = d[ptr]; ptr++;
                    tmp.subentry.Add(y);
                }
                tmp.unknownbyte4 = d[ptr]; ptr++;
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<WarpInfo> LoadWarpEntries(MapData r)
        {
            if (r.mapID == 11016)
            {
            }
            var d = eveData;
            var ptr = (int)r.offsetlist.Warp + (int)r.dataptr;
            List<WarpInfo> map = new List<WarpInfo>();
            #region Warp Info
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                WarpInfo tmp = new WarpInfo();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.mapID = globals.GetWord(d, ptr); ptr += 2;
                tmp.x = globals.GetDWord(d, ptr); ptr += 4;
                tmp.y = globals.GetDWord(d, ptr); ptr += 4;
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.neededtopass = d[ptr]; ptr++;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<InteractiveInfoEntries> LoadInteractiveEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Interactiveinfo + (int)r.dataptr;
            List<InteractiveInfoEntries> map = new List<InteractiveInfoEntries>();
            #region Interactive info
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                InteractiveInfoEntries tmp = new InteractiveInfoEntries();
                tmp.entryID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.unknownbyte2 = d[ptr]; ptr++;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    InteractiveInfoSubEntry y = new InteractiveInfoSubEntry();
                    y.unknownbyte1 = d[ptr]; ptr++;
                    y.unknownbyte2 = d[ptr]; ptr++;
                    y.unknownbyte3 = d[ptr]; ptr++;
                    y.unknownbyte4 = d[ptr]; ptr++;
                    y.unknownbyte5 = d[ptr]; ptr++;
                    tmp.subentry.Add(y);
                }
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<BattleInfoEntries> LoadBattleEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.Battleinfo + (int)r.dataptr;
            List<BattleInfoEntries> map = new List<BattleInfoEntries>();
            #region BattleInfomation
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                BattleInfoEntries tmp = new BattleInfoEntries();
                tmp.entryID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.unknownbyte2 = d[ptr]; ptr++;
                tmp.unknownbyte3 = d[ptr]; ptr++;
                tmp.unknownbyte4 = d[ptr]; ptr++;
                tmp.unknownbyte5 = d[ptr]; ptr++;
                tmp.unknownbyte6 = d[ptr]; ptr++;
                tmp.unknownbyte7 = d[ptr]; ptr++;
                tmp.unknownbyte8 = d[ptr]; ptr++;
                tmp.unknownbyte9 = d[ptr]; ptr++;
                tmp.unknownbyte10 = d[ptr]; ptr++;
                tmp.unknownbyte11 = d[ptr]; ptr++;
                tmp.unknownbyte12 = d[ptr]; ptr++;
                tmp.unknownbyte13 = d[ptr]; ptr++;
                tmp.unknownbyte14 = d[ptr]; ptr++;
                tmp.unknownbyte15 = d[ptr]; ptr++;
                tmp.unknownbyte16 = d[ptr]; ptr++;
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    BattleInfoSubEntry y = new BattleInfoSubEntry();
                    y.unknownbyte1 = d[ptr]; ptr++;
                    y.unknownbyte2 = d[ptr]; ptr++;
                    y.unknownbyte3 = d[ptr]; ptr++;
                    tmp.subentry1.Add(y);
                }
                blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    BattleInfoSubEntry y = new BattleInfoSubEntry();
                    y.unknownbyte1 = d[ptr]; ptr++;
                    y.unknownbyte2 = d[ptr]; ptr++;
                    y.unknownbyte3 = d[ptr]; ptr++;
                    tmp.subentry2.Add(y);
                }
                tmp.unknownbyte17 = d[ptr]; ptr++;
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<preEventEntries> LoadpreEventEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.PreEvent + (int)r.dataptr;
            List<preEventEntries> map = new List<preEventEntries>();
            #region Pre-Event
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                preEventEntries tmp = new preEventEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                int blen = d[ptr]; ptr++;
                for (int c = 0; c < blen; c++)
                {
                    preEventSubEntry y = new preEventSubEntry();
                    y.subIndex = d[ptr]; ptr++;
                    for (int ga = 0; ga < 21; ga++)
                    {
                        y.unknown.Add(d[ptr]); ptr++;
                    }
                    int blen2 = d[ptr]; ptr++;
                    for (int cl = 0; cl < blen2; cl++)
                    {
                        preEventSubSubEntry fy = new preEventSubSubEntry();
                        fy.subIndex = d[ptr]; ptr++;
                        for (int ga = 0; ga < 21; ga++)
                        {
                            y.unknown.Add(d[ptr]); ptr++;
                        }
                        y.subentry2.Add(fy);
                    }
                    tmp.subentry1.Add(y);
                }
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        public List<ExtgroupEntries> LoadgroupExtEntries(MapData r)
        {
            var d = eveData;
            var ptr = (int)r.offsetlist.groupext + (int)r.dataptr;
            List<ExtgroupEntries> map = new List<ExtgroupEntries>();
            #region Ext Group Info
            var elen = globals.GetWord(d, ptr); ptr += 2;
            for (int a = 0; a < elen; a++)
            {
                ExtgroupEntries tmp = new ExtgroupEntries();
                tmp.clickID = globals.GetWord(d, ptr); ptr += 2;
                tmp.Name = "";
                for (int ap = 0; ap < 20; ap++)
                {
                    tmp.Name += (char)d[ptr]; ptr++;
                }
                tmp.unknownbyte1 = d[ptr]; ptr++;
                tmp.unknownword1 = globals.GetWord(d, ptr); ptr += 2;
                tmp.unknownword2 = globals.GetWord(d, ptr); ptr += 2;
                int blen = globals.GetWord(d, ptr); ptr += 2;
                for (int c = 0; c < blen; c++)
                {
                    extGroupSubEntry e = new extGroupSubEntry();
                    e.Index = d[ptr]; ptr++;
                    e.unknownbyte1 = d[ptr]; ptr++;
                    e.unknownbyte2 = d[ptr]; ptr++;
                    tmp.subEntry.Add(e);
                }
                map.Add(tmp);
            }
            #endregion
            return map;
        }
        MapData GetMapDatabySceneID(UInt16 id)
        {
            foreach (MapData d in Maps)
            {
                if (d.sceneID == id)
                    return d;
            }
            return null;
        }
        MapData GetMapDatabyPtr(UInt32 id)
        {
            foreach (MapData d in Maps)
            {
                if (d.dataptr == id)
                    return d;
            }
            return null;
        }

    }
}

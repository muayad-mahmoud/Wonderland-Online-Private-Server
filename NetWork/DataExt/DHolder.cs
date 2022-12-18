using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    public class DBManager
    {

        public List<InteractiveInfoEntries> Interaction = new List<InteractiveInfoEntries>();

        public List<GroupEntries> group = new List<GroupEntries>();
        public List<ExtgroupEntries> groupext = new List<ExtgroupEntries>();
        public List<BattleInfoEntries> battleinfo = new List<BattleInfoEntries>();
        public List<preEventEntries> preevent = new List<preEventEntries>();
        cMap Owner;
        public DBManager(cMap src)
        {
            Owner = src;
        }


        /*public WarpInfo GetWarpInfo(int Entry_Exit)
        {
            try
            {
                var enterfrom = EntryPoints[Entry_Exit - 1];
                for (int a = 0; a < EventList.Count; a++)
                {
                    if (EventList[a].clickID == enterfrom.unknownbytearray1[0])
                    {
                        for (int b = 0; b < WarpPointList.Count; b++)
                        {
                            //if(WarpPointList[b].clickID == 
                        }
                        //var eventreg = EventList[];
                    }
                }
                //return WarpPointList[EventList[(int)(.unknownbytearray1[0])].SubEntry[0].SubEntry[0].unknownbyte1];
            }
            catch { }
            return null;
        }*/

    }
}

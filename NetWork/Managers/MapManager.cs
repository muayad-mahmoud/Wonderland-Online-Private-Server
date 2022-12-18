using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.Managers
{
    public class cMapManager
    {
        public List<cMap> mapList = new List<cMap>();
        cGlobals globals;

        public cMapManager(cGlobals src)
        {
            globals = src;
        }

        #region Database/Ext tools
        /*public bool LoadMaps()
        {
            bool bRetVal = false;
            globals.Log("Loading maps...\r\n");
            lock (thisLock)
            {
                try
                {
                    DataTable table;
                    String query = "select * from maps";
                    table = globals.WloDatabase.GetDataTable(query);
                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow r in table.Rows)
                        {
                            cMap m = new cMap(globals);
                            m.SetFromDB(r);
                            mapList.Add(m);
                            //g.Log(m.Log() + "\r\n");
                        }
                        bRetVal = true;
                    }
                    globals.Log("Done.\r\n\r\n");
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            }
            return bRetVal;

        }*/
        /*public void SaveMaps()
        {
            foreach (cMap f in mapList)
            {
                if (f.UPdated)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d.Add("mapID", f.MapID.ToString());
                    d.Add("name", f.name);
                    string s = "";
                    foreach (cWarp p in f.WarpPoints)
                    {
                        s += p.entry + " " + p.id + " " + p.mapTo + " " + p.x.ToString() + " " + p.y.ToString() + " " + p.doorinfo.ToString() + " ";
                    }
                    d.Add("warps", s);
                    try
                    {
                        globals.WloDatabase.Update("maps", d, "mapID = " + f.MapID);
                    }
                    catch (SQLiteException r)
                    {
                        if (r.ErrorCode == SQLiteErrorCode.NotFound)
                        {
                            globals.WloDatabase.Insert("maps", d);
                        }
                    }
                    f.UPdated = false;
                }
            }

        }*/
        /*public void LoadScene(List<SceneInfo> src)
        {
            foreach (cMap r in mapList)
            {
                r.sceneinfo = SceneDataFile.GetSceneByID(EveManager.GetSceneIDbyMapID(r.MapID));
            }
        }*/
        public void ListMaps()
        {
                foreach (cMap m in mapList)
                {
                    globals.Log(m.Log() + "\r\n");
                }
        }
        public void SetListBox(System.Windows.Forms.ListBox lb)
        {
                foreach (cMap m in mapList)
                {
                    lb.Items.Add(m.Log());
                }
        }
        #endregion

        public void RemoveByDC(cCharacter c)
        {
            c.map.RemoveByDC(c);
        }
        public cMap GetMapByID(UInt16 id)
        {
                cMap map = null;
                for (int a = 0; a < mapList.Count; a++)
                {
                    if (mapList[a].MapID == id)
                    {
                        map = mapList[a];
                        return mapList[a];
                    }
                }
                return map;
        }
        public cMap GetMapByName(string id)
        {
            cMap map = null;
                for (int a = 0; a < mapList.Count; a++)
                {
                    if (mapList[a].name == id)
                    {
                        map = mapList[a];
                        break;
                    }
                }
            return map;
        }


    }
}

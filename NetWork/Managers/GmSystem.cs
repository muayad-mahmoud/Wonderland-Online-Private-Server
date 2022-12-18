using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace P_Server.Managers
{
    public class GmSystem
    {
       /*cCharacter src;
        public GmSystem(cCharacter c)
        {
            src = c;
        }

        public void EndBattle()
        {
            
        }

        bool CheckPerm(cCharacter c)
        {
            if (c.GM)
            {
                return true;
            }
            return false;
        }

        public bool ParseCMD(string[] words)
        {
            
            if (words[0][0].ToString() == ":")
            {
                try
                {
                    if (words.Length >= 1)
                    {
                        switch (words[0])
                        {
                            case ":warp":
                                {
                                    UInt16 value = 0;
                                    try { value = UInt16.Parse(words[1]); }
                                    catch { }
                                    cWarp w = new cWarp();
                                    w.x = 500; w.y = 500; w.mapTo = value; w.id = 0;
                                    cAC_20.DoWarp(w, src);
                                } break;
                            case ":item":
                                {
                                    byte ammt = 1;
                                    int ct = words.Length;
                                    UInt16 itemid = 0;
                                    if (ct > 1)
                                    {

                                        try { itemid = UInt16.Parse(words[1]); }
                                        catch { }
                                        if (ct > 2)
                                        {
                                            try { ammt = byte.Parse(words[2]); }
                                            catch { }
                                        }
                                        cInvItem i = new cInvItem();
                                        i.ammt = ammt;
                                        i.ID = itemid;


                                        if (src.inv.RecieveItem(i))
                                        {
                                            i.ammt = ammt;
                                        }

                                    }
                                } break;
                            case ":npc":
                                {
                                    UInt16 npcid = 0; int ct = words.Length;
                                    if (ct > 1)
                                    {
                                        try { npcid = UInt16.Parse(words[1]); }
                                        catch { }
                                        src.riceBall.GMRiceBall(npcid, src);

                                        // g.ac5.Send_5(npcid, c, c);

                                        //                       {
                                        //cInvItem i = new cInvItem(g);
                                        //i = c.GetInventoryItem(cell); i.ammt = 1;
                                        //c.riceBall.Start(cell, i, c);
                                        //}//
                                    }

                                } break;
                            case ":fight":
                                {
                                    cBattle b = new cBattle();
                                    b.Test(src);
                                } break;
                            case ":next":
                                {
                                    cBattle b = src.battle;
                                    if (b.active) b.StartRound();
                                } break;
                            case ":end":
                                {
                                    // t.GMTools.EndBattle();
                                    cBattle b = src.battle;
                                    b.EndBattle();

                                } break;
                            case ":WarpList":
                                {
                                    // create list of warp entries to send to GM
                                    foreach (WarpEntries it in Form1.cMapManager.GetMapByID(src.map).DataSafe.WarpPointList)
                                    {
                                        cAC_2.Send_3(100, string.Format("DoorInfo#-> {0}, WarpTo-> {1}",
                                            it.clickID, Form1.cMapManager.GetMapByID(it.mapID).name), src);
                                    }
                                } break;
                            case ":WarpPoints":
                                {

                                    foreach (cWarp it in Form1.cMapManager.GetMapByID(src.map).WarpPoints)
                                    {
                                        cAC_2.Send_3(100, string.Format("Doorinfo#-> {0}, WarpToMap-> {1}, WarpClickID#-> {2}",
                                             it.entry, it.mapTo, it.id), src);
                                    }
                                    if (Form1.cMapManager.GetMapByID(src.map).WarpPoints.Count == 0)
                                        cAC_2.Send_3(100, "No Doors for" + Form1.cMapManager.GetMapByID(src.map).name, src);

                                } break;
                            case ":UpdateWarpID":
                                {
                                    if (Form1.cMapManager.GetMapByID(src.map).UpdateWarp_clickID(Int32.Parse(words[1]), UInt16.Parse(words[2])))
                                    {
                                        cAC_2.Send_3(100, "Action Success", src);
                                    }
                                    else
                                    {
                                        cAC_2.Send_3(100, "Action Fail", src);
                                    }

                                } break;
                            case ":AddWarp":
                                {
                                    var e = Form1.cMapManager.GetMapByID(src.map).GetWarpEntry(ushort.Parse(words[1]));


                                    if (Form1.cMapManager.GetMapByID(src.map).AddWarpPoint(e, UInt16.Parse(words[2])))
                                    {
                                        cAC_2.Send_3(100, "Action Success", src);
                                    }
                                    else
                                    {
                                        cAC_2.Send_3(100, "Action Fail", src);
                                    }

                                } break;
                            case ":RemWarp":
                                {
                                    if (Form1.cMapManager.GetMapByID(src.map).DelWarpPoint(Int32.Parse(words[1])))
                                    {
                                        cAC_2.Send_3(100, "Action Success", src);
                                    }
                                    else
                                    {
                                        cAC_2.Send_3(100, "Action Fail", src);
                                    }

                                } break;
                        }
                    }
                }
                catch { cAC_2.Send_3(100, "Action Fail", src); return false; }
            }
            else
                Form1.cMapManager.LocalChat(src, string.Join(" ", words));
            return true;
        }*/
    }
}

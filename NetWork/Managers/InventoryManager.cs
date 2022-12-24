using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork.ACS;

namespace PServer_v2.NetWork.Managers
{
    public class cInventory
    {
        cCharacter own;
        cGlobals globals;
        public cInvItem[][] mainInv = new cInvItem[11][];
        bool storage;
        public cInventory(cGlobals d, cCharacter y, bool st)
        {
            own = y;
            globals = d;
            storage = st;
            for (int n = 0; n < 11; n++)
            {
                mainInv[n] = new cInvItem[6];
                for (int n1 = 0; n1 < 6; n1++) mainInv[n][n1] = new cInvItem(globals);
            }
        }

        public byte ItemSpaceleft
        {
            get
            {
                int ammt = 0;
                for (int a = 1; a < 11; a++)
                {
                    for (int b = 1; b < 6; b++)
                    {
                        if (mainInv[a][b].ID == 0)
                            ammt += 50;
                        else if (mainInv[a][b].ID > 0)
                            if (mainInv[a][b].itemtype.Stackable)
                                ammt += 50 - mainInv[a][b].ammt;
                    }
                }
                return (byte)ammt;
            }
        }
        public byte Slotsleft
        {
            get
            {
                int ammt = 0;
                for (int a = 1; a < 11; a++)
                {
                    for (int b = 1; b < 6; b++)
                    {
                        if (mainInv[a][b].ID == 0)
                            ammt++;
                    }
                }
                return (byte)ammt;
            }
        }
        public byte CountMainInv()
        {
            byte ammt = 0;
            for (int n = 1; n < 11; n++)
                for (int b = 1; b < 6; b++)
                    if (mainInv[n][b].ID != 0) ammt++;
            return ammt;
        }
        byte[] Get_23_5()
        {
            byte[] data = null;
            int ammt = CountMainInv();
            if (ammt > 0)
            {
                data = new byte[ammt * 29];
                int at = 0;
                for (int n = 1; n < 11; n++)
                    for (int b = 1; b < 6; b++)
                        if (mainInv[n][b].ID != 0)
                        {
                            data[at] = (byte)globals.MatrixtoNumber(n, b); at++;
                            globals.SetWord(mainInv[n][b].ID, ref data, at); at += 2;
                            data[at] = (byte)mainInv[n][b].ammt; at++;
                            for (int z = 0; z < 25; z++)
                            { data[at] = 0; at++; }
                        }
            }
            return data;
        }
        
        public void logInv()
        {
            string s = "";
            for (int n = 1; n < 11; n++)
                for (int b = 1; b < 6; b++)
                    s += n.ToString("00") + ") " + mainInv[n][b].ID.ToString("00000") + " " + mainInv[n][b].ammt.ToString("00") + "\r\n";
            globals.Log(s);
        }
        public void DropItemMap(byte src, byte ammt)
        {
            var srcCell = globals.NumbertoMatrix(src);
            //check to see if I even have that ammt in my inventory
            if ((src > 0) && (src < 51))
            {
                if (mainInv[srcCell[0]][srcCell[1]].ammt >= ammt)
                {
                    cInvItem i = new cInvItem(globals);
                    i.CopyFrom(mainInv[srcCell[0]][srcCell[1]]);
                    i.ammt = ammt;
                    //do a drop item on map
                    if (own.map.DropItem(i, own))
                    {
                        // if (i.itemtype.ItemType == 39)
                        //own.vechile.Rem(src);
                        RemoveInv(src, i.ammt);

                    }
                }
            }
        }
        public cInvItem GetInventoryItem(byte scell)
        {
            var cell = globals.NumbertoMatrix(scell);
            cInvItem i = new cInvItem(globals);
            i.CopyFrom(mainInv[cell[0]][cell[1]]);
            return i;
        }
        public void GetItemMap(UInt16 itemIndex)
        {
            //check to see if item is on map
            cGroundItem gi = own.map.GetGroundItem(itemIndex, own, false);
            if (gi == null)
                return;
            if (gi.id > 0)
            {
                cInvItem i = new cInvItem(globals);
                i.CopyFrom(gi);
                byte ammt = i.ammt;
                if (PlaceItem(i, i.ammt))
                {
                    own.map.GetGroundItem(itemIndex, own, true);
                    i.ammt = ammt;
                    Send_6(own, i);
                }
            }

        }
       

        #region DataBase tools
        public bool Load(UInt32 id)
        {
            try
            {
                if(storage)
                { 
                    globals.Log("loading storage");
                    DataRow r = GetDBData(id,true);
                        if (r != null)
                {
                    string heading = "";
                    for (int a = 1; a < 51; a++)
                    {
                        heading = "inv" + a;
                        string istr = (string)r[heading]; istr = istr.Trim();
                        if (istr.Length > 0)
                        
                        {
                            var n = globals.NumbertoMatrix(a);
                            string[] words = istr.Split(' ');
                            int ct = words.Length;
                            if (ct > 0) mainInv[n[0]][n[1]].ID = UInt16.Parse(words[0]);
                            if (ct > 1) mainInv[n[0]][n[1]].ammt = byte.Parse(words[1]);
                            if (ct > 2) mainInv[n[0]][n[1]].damage = byte.Parse(words[2]);
                            if (ct > 3) mainInv[n[0]][n[1]].parent = byte.Parse(words[3]);
                            if (mainInv[n[0]][n[1]].ID > 0)
                                mainInv[n[0]][n[1]].itemtype = globals.gItemManager.GetItemByID(mainInv[n[0]][n[1]].ID);
                            if (mainInv[n[0]][n[1]].itemtype.Veichle)
                                own.vechile.Add(mainInv[n[0]][n[1]].ID, a);
                        }

                    }
                    for (int n = 1; n < 7; n++)
                    {
                        heading = "wear" + n;
                        string istr = (string)r[heading]; istr = istr.Trim();
                        if (istr.Length > 0)
                        {
                            string[] words = istr.Split(' ');
                            int ct = words.Length;
                            if (ct > 0) own.eq.clothes[n].ID = UInt16.Parse(words[0]);
                            if (ct > 1) own.eq.clothes[n].ammt = byte.Parse(words[1]);
                            if (ct > 2) own.eq.clothes[n].damage = byte.Parse(words[2]);
                            if (ct > 3) own.eq.clothes[n].parent = byte.Parse(words[3]);
                        }

                    }

                    return true;
                }
                }
                else
                {
                    DataRow r = GetDBData(id,false);
                if (r != null)
                {
                    string heading = "";
                    for (int a = 1; a < 51; a++)
                    {
                        heading = "inv" + a;
                        string istr = (string)r[heading]; istr = istr.Trim();
                        if (istr.Length > 0)
                        
                        {
                            var n = globals.NumbertoMatrix(a);
                            string[] words = istr.Split(' ');
                            int ct = words.Length;
                            if (ct > 0) mainInv[n[0]][n[1]].ID = UInt16.Parse(words[0]);
                            if (ct > 1) mainInv[n[0]][n[1]].ammt = byte.Parse(words[1]);
                            if (ct > 2) mainInv[n[0]][n[1]].damage = byte.Parse(words[2]);
                            if (ct > 3) mainInv[n[0]][n[1]].parent = byte.Parse(words[3]);
                            if (mainInv[n[0]][n[1]].ID > 0)
                                mainInv[n[0]][n[1]].itemtype = globals.gItemManager.GetItemByID(mainInv[n[0]][n[1]].ID);
                            if (mainInv[n[0]][n[1]].itemtype.Veichle)
                                own.vechile.Add(mainInv[n[0]][n[1]].ID, a);
                        }

                    }
                    for (int n = 1; n < 7; n++)
                    {
                        heading = "wear" + n;
                        string istr = (string)r[heading]; istr = istr.Trim();
                        if (istr.Length > 0)
                        {
                            string[] words = istr.Split(' ');
                            int ct = words.Length;
                            if (ct > 0) own.eq.clothes[n].ID = UInt16.Parse(words[0]);
                            if (ct > 1) own.eq.clothes[n].ammt = byte.Parse(words[1]);
                            if (ct > 2) own.eq.clothes[n].damage = byte.Parse(words[2]);
                            if (ct > 3) own.eq.clothes[n].parent = byte.Parse(words[3]);
                        }

                    }

                    return true;
                }
                }
            }
            catch { }
            return false;
        }
        public bool Save(UInt32 id)
        {
            if (this.storage)
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
            string where = "invID = " + id;
            string heading = "";
            for (int a = 1; a < 51; a++)
            {
                var n = globals.NumbertoMatrix(a);
                cInvItem i = mainInv[n[0]][n[1]]; //i.Clear();
                heading = "inv" + a;
                string v = "";
                v += i.ID + " " + i.ammt + " " + i.damage + " " + i.parent;
                d.Add(heading, v);
            }
            
            return globals.WloDatabase.Update("Storage", d, where);
            }
            else
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
            string where = "invID = " + id;
            string heading = "";
            for (int a = 1; a < 51; a++)
            {
                var n = globals.NumbertoMatrix(a);
                cInvItem i = mainInv[n[0]][n[1]]; //i.Clear();
                heading = "inv" + a;
                string v = "";
                v += i.ID + " " + i.ammt + " " + i.damage + " " + i.parent;
                d.Add(heading, v);
            }
            for (int n = 1; n < 7; n++)
            {
                cInvItem i = own.eq.clothes[n]; //i.Clear();
                heading = "wear" + n;
                string v = "";
                v += i.ID + " " + i.ammt + " " + i.damage + " " + i.parent;
                d.Add(heading, v);
            }
            return globals.WloDatabase.Update("inventory", d, where);
            }
            
        }
        public bool Delete(UInt32 id)
        {
            bool retVal = false;
            //lock (thisLock)
            {
                try
                {
                    string query = "DELETE FROM inventory WHERE invID = " + id + ";";
                    if (globals.WloDatabase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            }
            return retVal;

        }
        public bool Create(UInt32 id)
        {
            bool retVal = false;
            //lock (thisLock)
            {
                try
                {
                    string query = "INSERT INTO inventory " +
                        "(invID";

                    for (int a = 1; a < 51; a++)
                    {
                        var n = globals.NumbertoMatrix(a);
                        cInvItem i = mainInv[n[0]][n[1]];
                        query += ", inv" + a;
                        //string v = "";
                        //v += i.id + " " + i.ammt + " " + i.damage + " " + (i.parent +1) + " " + i.locked;
                        //d.Add(heading, v);
                    }
                    for (int n = 1; n < 7; n++)
                    {
                        cInvItem i = own.eq.clothes[n];
                        query += ", wear" + n;
                        //string v = "";
                        //v += i.id + " " + i.ammt + " " + i.damage + " " + (i.parent +1) + " " + i.locked;
                        //d.Add(heading, v);
                    }

                    query += ") VALUES (" + id;
                    for (int a = 1; a < 51; a++)
                    {
                        var n = globals.NumbertoMatrix(a);
                        cInvItem i = mainInv[n[0]][n[1]];
                        //query += ", inv" + (n + 1);
                        //string v = "";
                        query += ",'" + i.ID + " " + i.ammt + " " + i.damage + " " + i.parent + "'";
                        //d.Add(heading, v);
                    }
                    for (int n = 1; n < 7; n++)
                    {
                        cInvItem i = own.eq.clothes[n];
                        //query += ", wear" + (n + 1);
                        //string v = "";
                        query += ",'" + i.ID + " " + i.ammt + " " + i.damage + " " + i.parent + "'";
                        //d.Add(heading, v);
                    }

                    query += ");";
                    if (globals.WloDatabase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            }
            return retVal;
        }

        public DataRow GetDBData(UInt32 id, bool storage)
        {
            DataRow bRetVal = null;
            //lock (thisLock)
            {
                if (storage)
                {
                    try
                    {
                        
                    DataTable table;
                    String query = "select * from Storage where invID = " + id + "";
                    table = globals.WloDatabase.GetDataTable(query, true);
                    if (table.Rows.Count > 0)
                        bRetVal = table.Rows[0];
                    }
                    catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
                }
                else
                {
                    try
                {

                    DataTable table;
                    String query = "select * from inventory where invID = " + id + "";
                    table = globals.WloDatabase.GetDataTable(query, true);
                    if (table.Rows.Count > 0)
                        bRetVal = table.Rows[0];
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
                }
                
            }
            return bRetVal;
        }
        #endregion

        //finds first non full cell with matching item id, returns cell location
        public byte FindFirstNonFull(UInt16 id)
        {
            byte cell = 0;
            for (int n = 1; n < 11; n++)
                for (int b = 1; b < 6; b++)
                    if ((mainInv[n][b].ID == id) && (mainInv[n][b].ammt < 50) && mainInv[n][b].parent == 0)
                    {
                        if (mainInv[n][b].itemtype == null || mainInv[n][b].itemtype.Stackable)
                            cell = (byte)globals.MatrixtoNumber(n, b); break;
                    }

            return cell;
        }
        public byte FindEmptySlot(int h, int w)
        {
            byte cell = 0;
            try
            {

                for (int n = 1; n < 10; n++)
                    for (int b = 1; b < 5; b++)
                    {
                        if ((mainInv[n][b].ID == 0) && (mainInv[n][b].ammt == 0))
                        {
                            if (((n + h) < 12) && ((b + w) < 7))
                            {
                                bool good = true;
                                for (int s = 0; s < h; s++)
                                    for (int y = 0; y < w; y++)
                                    {
                                        if ((mainInv[n + s][b + y].ID != 0))
                                        {
                                            good = false; break;
                                        }
                                    }
                                if (good)
                                {
                                    cell = (byte)globals.MatrixtoNumber(n, b);
                                    return cell;
                                }
                            }
                        }

                    }

            }
            catch { }
            return cell;
        }
        //places the item in the designated cell, returns how many items were placed
        public void UseItem(byte slot, byte ammt)
        {
            //Get item first
            var n = globals.NumbertoMatrix(slot);
            cInvItem i = mainInv[n[0]][n[1]];
            if (i.ID > 0)
            {
                switch (mainInv[n[0]][n[1]].itemtype.ItemType)
                {
                }
                mainInv[n[0]][n[1]].ammt -= 1;
                Send_9(slot, ammt, own);
            }
            Send_15();
        }
        public bool PlaceItem(cInvItem i, byte ammt, byte cell = 0)
        {
            if (ammt > ItemSpaceleft)
                return false;

            byte numPlaced = 0;
            while (ammt > 0)
            {
                if (cell == 0)
                {
                    cell = FindFirstNonFull(i.ID);
                    if (cell == 0)
                    {
                        cell = FindEmptySlot(i.itemtype.InvHeight, i.itemtype.InvWidth);
                        if (cell == 0)
                            return false;
                    }
                }
                var n = globals.NumbertoMatrix(cell);
                if ((mainInv[n[0]][n[1]].ID == i.ID) || (mainInv[n[0]][n[1]].ID == 0))
                {
                    int mast = cell;
                    int avail = 50 - mainInv[n[0]][n[1]].ammt;
                    if (avail > ammt)
                        avail = ammt;
                    ammt -= (byte)avail;
                    for (byte h = 0; h < i.itemtype.InvHeight; h++)
                        for (byte w = 0; w < i.itemtype.InvWidth; w++)
                        {
                            if (w == 0 && h == 0)
                            {
                                mainInv[n[0] + h][n[1] + w].itemtype = i.itemtype;
                                mainInv[n[0] + h][n[1] + w].ammt += (byte)avail;
                                if (mainInv[n[0] + h][n[1] + w].ID == 0) mainInv[n[0] + h][n[1] + w].ID = i.ID;
                            }
                            else
                                mainInv[n[0] + h][n[1] + w].parent = (byte)mast;
                        }
                    numPlaced += (byte)avail;

                    if (i.itemtype.ItemType == 39)
                        own.vechile.Add(i.ID, mast);
                }
                else { return false; }
            }

            return true;
        }
        public bool MoveItem(byte src, byte ammt, byte dst, cCharacter target)
        {
            bool ret = false;

            //get src item
            var sslot = globals.NumbertoMatrix(src);
            var dslot = globals.NumbertoMatrix(dst);
            cInvItem si = new cInvItem(globals);
            si.CopyFrom(mainInv[sslot[0]][sslot[1]]); si.ammt = ammt;
            if ((si.ID != 0) && (si.parent == 0) && (src != dst))
            {
                if ((mainInv[dslot[0]][dslot[1]].ID == si.ID) || (mainInv[dslot[0]][dslot[1]].ID == 0))
                {
                    if (si.itemtype.Stackable)
                    {
                        if ((50 - mainInv[dslot[0]][dslot[1]].ammt) >= ammt)
                        {
                            RemoveInv(src, si.ammt, true);
                            if (mainInv[dslot[0]][dslot[1]].ID == 0)
                            {
                                mainInv[dslot[0]][dslot[1]].CopyFrom(si);
                            }
                            else
                            {
                                mainInv[dslot[0]][dslot[1]].ammt += ammt;
                            }
                            ret = true;
                        }
                        else
                        {
                            if (mainInv[dslot[0]][dslot[1]].ID == 0)
                            {
                                int ammttoput = 50 - mainInv[dslot[0]][dslot[1]].ammt;
                                ret = true;
                                mainInv[dslot[0]][dslot[1]].ammt += (byte)ammttoput;
                                RemoveInv(src, si.ammt, true);
                            }
                        }
                    }
                    else
                    {
                        if (mainInv[dslot[0]][dslot[1]].ammt == 0)
                        {
                            ret = true;
                            mainInv[dslot[0]][dslot[1]].CopyFrom(si);
                            RemoveInv(src, si.ammt, true);
                        }
                    }
                }
            }
            if (ret) Send_10(src, ammt, dst, target);
            return ret;
        }
        public bool RecieveItem(cInvItem[] i, int goldlost = 0)
        {
            bool ret = false;
            for (int a = 0; a < i.Length; a++)
            {
                if (i[a] != null)
                {
                    if (i[a].itemtype != null)
                    {
                        if (PlaceItem(i[a], i[a].ammt))
                            ret = true;
                    }
                }
            }
            if (ret)
            {
                for (int a = 0; a < i.Length; a++)
                {
                    if (i[a] != null)
                    {
                        if (goldlost > 0)
                        {
                            cSendPacket j = new cSendPacket(globals);
                            j.Header(26, 2);
                            j.AddDWord((uint)goldlost);
                            j.SetSize();
                            j.character = own;
                            j.Send();
                        }
                        Send_6(own, i[a]);
                    }
                }
            }
            return ret;
        }
        public bool RecieveItem(cInvItem i, int goldlost = 0)
        {
            bool ret = false;
            if (i != null)
            {
                if (i.itemtype != null)
                {
                    if (PlaceItem(i, i.ammt))
                    {
                        if (goldlost > 0)
                        {
                            cSendPacket j = new cSendPacket(globals);
                            j.Header(26, 2);
                            j.AddDWord((uint)goldlost);
                            j.SetSize();
                            j.character = own;
                            j.Send();
                        }
                        Send_6(own, i);
                    }
                }
            }
            return ret;
        }
        public void RemoveInv(byte invSlot, byte amt, bool blkrem = false)
        {
            var s = globals.NumbertoMatrix(invSlot);
            if (mainInv[s[0]][s[1]].ID > 0)
            {
                mainInv[s[0]][s[1]].ammt -= amt;
                if (mainInv[s[0]][s[1]].ammt < 1)
                    mainInv[s[0]][s[1]].Clear();
            }
            for (int a = 1; a < 11; a++)
                for (int b = 1; b < 6; b++)
                    if (mainInv[a][b].parent == invSlot)
                        mainInv[a][b].Clear();
            if (!blkrem)
                Send_9(invSlot, amt, own);
        }


        public void Send_15(byte slot = 0, byte ammt = 0)
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 10);
            if (slot != 0)
            {
                p.AddByte(slot);
                p.AddByte(ammt);
                p.AddWord(0);
            }
            p.SetSize();
            p.character = own;
            p.Send();
        }//use inventory
        public void Send_9(byte src, byte i, cCharacter target) //remove item from inv
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 9);
            p.AddByte(src);
            p.AddByte(i);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_10(byte src, byte ammt, byte dst, cCharacter target) //return of item moved in inv
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 10);
            p.AddByte(src);
            p.AddByte(ammt);
            p.AddByte(dst);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public void Send_6(cCharacter target, cInvItem item) //send an item to a player
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 6);
            p.AddWord(item.ID);
            p.AddByte(item.ammt);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddDWord(0);
            p.AddWord(0);
            p.SetSize();
            p.character = target;
            p.Send();
        }
        public bool putIteminStorage(cInvItem givenItem)
        {
            
            foreach(var itemArray in own.storage.mainInv)
            {
                foreach(var item in itemArray)
                {
                    if(item.ID == givenItem.ID)
                    {
                        //check if stackable
                        if (item.itemtype.Stackable)
                        {
                            //check if ammount is good
                            if( item.ammt + givenItem.ammt <= 50)
                            {
                                item.ammt += givenItem.ammt;
                                globals.Log("Done1");
                                return true;
                            }
                            else
                            {
                                int needed = 50 - item.ammt;
                                int difference = givenItem.ammt - needed;
                                item.ammt = (byte)50;
                                PlaceItem(item, (byte)difference);
                                globals.Log("Done2");
                                return true;
                            }
                        }
                    }
                }
            }
            globals.Log("Done3");
            PlaceItem(givenItem,givenItem.ammt);
            return true;
        }
        public void Send_Inventory() //user's primary inventory
        {
            byte[] data = own.inv.Get_23_5();
            cSendPacket p = new cSendPacket(globals);
            p.Header(23, 5);
            p.AddArray(data);
            p.SetSize();
            p.character = own; ;
            p.Send();
        }
        public void Send_Storage()
        {
            byte[] data = own.storage.Get_23_5();
            cSendPacket p = new cSendPacket(globals);
            p.Header(30,1);
            p.AddArray(data);
            p.SetSize();
            p.character = own;
            p.Send();
        }
    }
}

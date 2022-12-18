using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel;
using PServer_v2.NetWork.DataExt;
using System.Windows.Forms;

namespace PServer_v2.NetWork.Managers
{
    public class ImMallItem
    {
        cGlobals globals;
        public UInt16 id;
        public cItem item;
        public byte entryId;
        public byte ammt;
        public byte state;
        public byte perc;
        public byte Tab;
        public UInt16 Price;
        public string name;

        #region Prop
        public string Entry { get { return entryId.ToString(); } }
        public string Name { get { return name; } }
        public string ID { get { return id.ToString(); } }
        public string Ammt { get { return ammt.ToString(); } }
        public string ORGPrice { get { return Price.ToString(); } }
        public string PercentREM { get { return perc.ToString(); } }
        public string State { get { return state.ToString(); } }
        public string Group { get { return Tab.ToString(); } }
        #endregion

        public ImMallItem(cGlobals k)
        {
            globals = k;
        }
        public void setFromDB(System.Data.DataRow r)
        {
            if (r["ItemID"].ToString() != "")
            {
                id = (UInt16.Parse(r["ItemID"].ToString()));
                ammt = (byte.Parse(r["Ammt"].ToString())); ;
                state = (byte.Parse(r["Type"].ToString())); ;
                if (globals.gItemManager.GetItemByID(id) != null)
                    name = (globals.gItemManager.GetItemByID(id)).Name;
                perc = (byte.Parse(r["Pert"].ToString())); ;
                if (perc == 0) perc = (byte)new Random().Next(1, 100);
                Price = (ushort.Parse(r["OrigPrice"].ToString())); ;
                Tab = (byte.Parse(r["Tab"].ToString())); ;
            }
        }
        public void setFromGrid(DataGridViewCellCollection r)
        {
            try
            {
                id = (UInt16.Parse(r[1].Value.ToString()));
                entryId = (byte.Parse(r[0].Value.ToString()));
                ammt = (byte.Parse(r[2].Value.ToString()));
                state = (byte.Parse(r[5].Value.ToString()));
                if (globals.gItemManager.GetItemByID(id) != null)
                    name = globals.gItemManager.GetItemByID(id).Name;
                perc = (byte.Parse(r[4].Value.ToString()));
                Price = (ushort.Parse(r[3].Value.ToString()));
                Tab = (byte.Parse(r[6].Value.ToString()));
            }
            catch { }
        }
        public void copyFrom(ImMallItem r)
        {
            id = r.id;
            ammt = r.ammt;
            state = r.state;
            name = r.name;
            perc = r.perc;
            Price = r.Price;
            Tab = r.Tab;
        }
        public void copyFrom(cItem r)
        {
            id = r.ItemID;
            ammt = 1;
            state = 0;
            name = r.Name;
            item = r;
            perc = 0;
            Price = (ushort)((int)(r.ItemRank * new Random().Next(1,5) + ((int)r.BuyingPrice * 2)+new Random().Next(1,50)));
            if (r.ItemType > 2 && r.ItemType < 7 || r.ItemType > 23 && r.ItemType < 29)
            {
                Tab = 1;
            }
            if (r.ItemType > 0 && r.ItemType < 3 || r.ItemType > 13 && r.ItemType < 16)
            {
                Tab = 2;
            }
            if (r.ItemType == 48)
            {
                Tab = 5;
                Price = (ushort)(Price * 3);
            }
            if (r.ItemType == 32)
            {
                Tab = 4;
            }
        }
        public void Discount(int by)
        {
            if (item == null)
                item = globals.gItemManager.GetItemByID(id);
            if (item.ItemType > 2 && item.ItemType < 7 || item.ItemType > 23 && item.ItemType < 29)
            {
                perc = (byte)new Random().Next(by -5, by);
            }
            if (item.ItemType > 0 && item.ItemType < 3 || item.ItemType > 13 && item.ItemType < 16)
            {
                perc = (byte)new Random().Next(by - 7, by);
            }
            if (item.ItemType == 48)
            {
                perc = (byte)new Random().Next(by - 3, by);
            }
            if (item.ItemType == 32)
            {
                perc = (byte)new Random().Next(by - 10, by);
            }
            else
                perc = (byte)new Random().Next(by - 15, by);
        }
        public byte[] GetByteForm()
        {
            cSendPacket u = new cSendPacket(globals);
            u.AddWord(id);
            u.AddByte(ammt);
            u.AddWord(Price);
            u.AddByte(perc);
            u.AddByte(state);
            u.AddByte(Tab);
            u.AddByte(entryId);
            u.AddByte(0);

            return u.data.ToArray();
        }
    }
    public class ImMall_Manager
    {
        SQLiteCommandBuilder sqlb;
        SQLiteDataAdapter imDB;
        cGlobals globals;
        System.Data.DataSet ImMall;
        BindingList<ImMallItem> IMlist = new BindingList<ImMallItem>();
        public BindingList<ImMallItem> HOTlist = new BindingList<ImMallItem>();
        public BindingList<ImMallItem> Armorylist = new BindingList<ImMallItem>();
        public BindingList<ImMallItem> Weaponrylist = new BindingList<ImMallItem>();
        public BindingList<ImMallItem> Grocerylist = new BindingList<ImMallItem>();
        public BindingList<ImMallItem> Furniturelist = new BindingList<ImMallItem>();


        public ImMall_Manager(cGlobals k)
        {
            globals = k;
            Init();
        }

        public ImMallItem GetbyId(ushort id)
        {
            IMlist.Clear();
            foreach (ImMallItem t in globals.gImMall_Manager.Weaponrylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Armorylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.HOTlist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Grocerylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Furniturelist)
            {
                IMlist.Add(t);
            }
            try
            {
                foreach (ImMallItem l in IMlist)
                {
                    if (l.id == id)
                        return l;
                }
                return null;
            }
            catch { return null; }
        }
        public ImMallItem GetbyEntry(ushort id)
        {
            IMlist.Clear();
            foreach (ImMallItem t in globals.gImMall_Manager.Weaponrylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Armorylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.HOTlist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Grocerylist)
            {
                IMlist.Add(t);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Furniturelist)
            {
                IMlist.Add(t);
            }
            try
            {
                foreach (ImMallItem t in IMlist)
                {
                    if (t.entryId == id)
                        return t;
                }
                return null;
            }
            catch { return null; }
        }
        public int Count { get { return IMlist.Count; } }
        public byte[] Get_75IM
        {
            get
            {
                int ent = 0;
                List<byte> tmp = new List<byte>();
                foreach (ImMallItem t in globals.gImMall_Manager.Weaponrylist)
                {
                    ent++;
                    t.Tab = 1;
                    t.entryId = (byte)ent;
                    GetbyId(t.id).entryId = t.entryId;
                    tmp.AddRange(t.GetByteForm());
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Armorylist)
                {
                    ent++;
                    t.Tab = 2;
                    t.entryId = (byte)ent;
                    tmp.AddRange(t.GetByteForm());
                    GetbyId(t.id).entryId = t.entryId;
                }
                foreach (ImMallItem t in globals.gImMall_Manager.HOTlist)
                {
                        ent++;
                        t.Tab = 3;
                        t.entryId = (byte)ent;
                        tmp.AddRange(t.GetByteForm());
                        GetbyId(t.id).entryId = t.entryId;                    
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Grocerylist)
                {
                    
                        ent++;
                        t.Tab = 4;
                        t.entryId = (byte)ent;
                        tmp.AddRange(t.GetByteForm());
                        GetbyId(t.id).entryId = t.entryId;
                    
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Furniturelist)
                {
                    ent++;
                    t.Tab = 5;
                    t.entryId = (byte)ent;
                    tmp.AddRange(t.GetByteForm());
                    GetbyId(t.id).entryId = t.entryId;
                }
                cSendPacket f = new cSendPacket(globals);
                f.AddWord((ushort)ent);
                f.AddArray(tmp.ToArray());
                return f.data.ToArray();

            }
        }
        void Init()
        {
            globals.IM_db.AutoGenerateColumns = true;
            string sql = "select* from ImMall";
            imDB = new SQLiteDataAdapter(sql, globals.WloDatabase.dbConnection);
            ImMall = new System.Data.DataSet();
            imDB.Fill(ImMall, "ImMall");
            sqlb = new SQLiteCommandBuilder(imDB);
            imDB.InsertCommand = sqlb.GetInsertCommand();
            imDB.UpdateCommand = sqlb.GetUpdateCommand();
            imDB.DeleteCommand = sqlb.GetDeleteCommand();
            foreach (System.Data.DataRow i in ImMall.Tables[0].Rows)
            {
                ImMallItem uo = new ImMallItem(globals);
                uo.setFromDB(i);
                switch (uo.Tab)
                {
                    case 1: Weaponrylist.Add(uo); break;
                    case 2: Armorylist.Add(uo); break;
                    case 3: HOTlist.Add(uo); break;
                    case 4: Grocerylist.Add(uo); break;
                    case 5: Furniturelist.Add(uo); break;
                }
                switch (uo.state)
                {
                    case 0: break;
                    default: { if (!HOTlist.Contains(uo))HOTlist.Add(uo); break; }
                }
                IMlist.Add(uo);
                System.Threading.Thread.Sleep(1);
            }
            
            //globals.IM_db.DataSource = ImMall.Tables[0];
            //globals.IM_db.Columns[0].ReadOnly = true;
            globals.hotdb.DataSource = HOTlist;
            globals.armdb.DataSource = Armorylist;
            globals.weapdb.DataSource = Weaponrylist;
            globals.grodb.DataSource = Grocerylist;
            globals.furdb.DataSource = Furniturelist;
        }
        public void DiscountALL(int by)
        {
            foreach (ImMallItem t in globals.gImMall_Manager.Weaponrylist)
            {
                t.Discount(by);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Armorylist)
            {
                t.Discount(by);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.HOTlist)
            {
                t.Discount(by);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Grocerylist)
            {
                t.Discount(by);
            }
            foreach (ImMallItem t in globals.gImMall_Manager.Furniturelist)
            {
                t.Discount(by);
            }
        }
        public void Delete(ushort ID)
        {
            string s = "ItemID =" + ID;
            globals.WloDatabase.Delete("ImMall", s);
            var r = this.GetbyId(ID);
            IMlist.Remove(r);
            switch(r.Tab)
            {
                case 1: Weaponrylist.Remove(r); break;
                case 2: Armorylist.Remove(r); break;
                case 3: HOTlist.Remove(r); break;
                case 4: Grocerylist.Remove(r); break;
                case 5: Furniturelist.Remove(r); break;
            }
        }
        public void Add(ImMallItem item)
        {
            for (int d = 0; d < IMlist.Count; d++)
            {
                if (IMlist[d] == item) return;
            }
            //IMlist.Add(item);
            switch(item.Tab)
            {
                case 1: Weaponrylist.Add(item); break;
                case 2: Armorylist.Add(item); break;
                case 3: HOTlist.Add(item); break;
                case 4: Grocerylist.Add(item); break;
                case 5: Furniturelist.Add(item); break;
            }
        }
        public void Update(ImMallItem item)
        {
            bool bfound = false;
            foreach (ImMallItem i in IMlist.Where(c => c.id == item.id))
            {
                bfound = true;
                i.copyFrom(item);

            }
            if (!bfound)
            {
                IMlist.Add(item);
            }
        }
        public void Update()
        {
            try
            {
                imDB.Update(ImMall.Tables[0]);
            }
            catch { MessageBox.Show("EntryID not unique"); }
        }
        public void Save()
        {
            foreach (ImMallItem t in globals.gImMall_Manager.Weaponrylist)
            {
                IMlist.Add(t);
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Armorylist)
                {
                    IMlist.Add(t);
                }
                foreach (ImMallItem t in globals.gImMall_Manager.HOTlist)
                {
                    IMlist.Add(t);
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Grocerylist)
                {
                    IMlist.Add(t);              
                }
                foreach (ImMallItem t in globals.gImMall_Manager.Furniturelist)
                {IMlist.Add(t);
                }
            foreach (ImMallItem k in IMlist)
            {
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                tmp.Add("ItemID", k.id.ToString());
                tmp.Add("Ammt", k.ammt.ToString());
                tmp.Add("OrigPrice", k.Price.ToString());
                tmp.Add("Pert", k.perc.ToString());
                tmp.Add("Type", k.state.ToString());
                tmp.Add("Tab", k.Tab.ToString());
                var e = globals.WloDatabase.GetDataTable("select* from ImMall where ItemID =" + k.id,true);
                if (e != null)
                {
                    //globals.WloDatabase.Update("ImMall", tmp, "ItemID =" + k.id);
                }
                else
                {

                    globals.WloDatabase.Insert("ImMall", tmp);
                }
            }
        }
        public void PushIM()
        {
            foreach (cCharacter u in globals.gCharacterManager.getCharList())
            {
                Send_2(Get_75IM, u);
            }
        }
        public void Randomize()
        {
            globals.gImMall_Manager.Furniturelist.Clear();
        Retry:
            var list = globals.gItemManager.GetbyItemType(new Random().Next(1, 90)+2);
            if (list.Count == 0) goto Retry;
            var list2 = list.Skip(new Random().Next(1, list.Count)+1);
            int amt3 = 32;
            foreach (cItem fs in list2)
            {
                if (amt3 == 0) break;
                amt3--;
                ImMallItem f = new ImMallItem(globals);
                f.copyFrom(fs);
                f.state = 0;
                f.Tab = (byte)5;
                if (f.Tab == 0) throw new Exception();
                globals.gImMall_Manager.Furniturelist.Add(f);
            }
        }
        public void BuyImItem(ushort ItemID, byte ammt, byte tab, byte entry, cCharacter t)
        {
            if (ItemID == 0) return;
            try
            {
                globals.gServer.Multipkt_Request(t);
                cSendPacket u = new cSendPacket(globals);
                u.Header(75, 4);
                u.AddWord(ItemID);
                u.AddByte(tab);
                u.AddByte(ammt);
                u.AddWord(entry);
                u.AddByte(1);
                u.SetSize();
                u.character = t;
                u.Send();
                int cost = ((GetbyEntry(entry).Price * GetbyEntry(entry).perc) / 100);
                globals.gUserManager.GetListByID(t.characterID).IM -= cost;
                if (globals.gUserManager.GetListByID(t.characterID).IM < 0)
                    globals.gUserManager.GetListByID(t.characterID).IM = 0;
                globals.gUserManager.UpdatePlayerIM(globals.gUserManager.GetListByID(t.characterID));
                u = new cSendPacket(globals);
                u.Header(75, 3);
                u.AddDWord((uint)t.IM);
                u.AddDWord((uint)cost);
                u.AddWord(ItemID);
                u.AddByte(ammt);
                u.SetSize();
                u.character = t;
                u.Send();
                cInvItem h = new cInvItem(globals);
                h.ammt = ammt;
                h.ID = ItemID;
                h.itemtype = globals.gItemManager.GetItemByID(ItemID);
                t.inv.PlaceItem(h, ammt);
                t.inv.Send_6(t, h);
                globals.gServer.SendCombinepkt(t);
            }
            catch { }
        }

        void Send_2(byte[] data, cCharacter target)
        {

            cSendPacket p = new cSendPacket(globals);
            p.Header(75, 2);
            p.AddArray(data);
            p.SetSize();
            p.character = target;
            p.Send();

        }
    }
}

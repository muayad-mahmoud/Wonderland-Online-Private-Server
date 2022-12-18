using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork;
using System.IO;

namespace PServer_v2.DataLoaders
{
    public class cItemManager
    {
        public List<cItem> itemList = new List<cItem>();
        cGlobals globals;
        public cItemManager(cGlobals src)
        {
            globals = src;
        }

        public cItem GetItemByID(UInt16 id)
        {
            cItem ret = new cItem();
            for(int a = 0;a<itemList.Count;a++)
            {
                if (itemList[a].ItemID == id)
                {
                    ret = itemList[a];
                    break;
                }
                else if (id == 0)
                {
                    ret.Name = "";
                    ret.ItemID = 0;
                }
            }

            return ret;
        }
        public List<cItem> GetbyItemType(int type)
        {
            List<cItem> tmp = new List<cItem>();
            for (int a = 0; a < itemList.Count; a++)
                if (itemList[a].ItemType == type)
                    tmp.Add(itemList[a]);
            return tmp;
        }
        public cItem GetItemByName(string Name)
        {
            for (int a = 0; a < itemList.Count; a++)
            {
                if (string.Compare(Name.ToLower(), itemList[a].Name.ToLower()) == 0)
                {
                    return itemList[a];
                }
            }
            return null;
        }
        public bool isWearable(UInt16 id)
        {
            bool ret = false;
            cItem i = GetItemByID(id);
            if (i != null)
            {
                if (i.wearAt != eWearSlot.none) ret = true;
            }
            return ret;
        }
        public bool isStackable(UInt16 id)
        {
            bool r = false;
            cItem i = GetItemByID(id);
            if (i != null)
            {
                r = i.Stackable;
            }
            return r;
        }
        public bool LoadItems(string path)
        {
            globals.Log("Loading items...\r\n");
            try
            {
                itemList.Clear();
                if (!File.Exists(path)) return false;
                byte[] data = File.ReadAllBytes(path);
                int max = data.Length / 457;
                int at = 0;
                for (int n = 0; n < max; n++)
                {
                    cItem i = new cItem();
                    i.mydata = new byte[457];
                    Array.Copy(data, at, i.mydata, 0, 457);
                    i.Load();
                    itemList.Add(i);
                    at += 457;
                }
                globals.Log("Done.\r\n\r\n");
                return true;
            }
            catch (Exception fail)
            {
                globals.Log("failed.\r\n");
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                System.Windows.Forms.MessageBox.Show(error);
            }
            return false;
        }
        public string GetItemTypeName(byte type)
        {
            string ret = "<unknown>";
            switch (type)
            {
                case 1: ret = "Flower"; break;
                case 2: ret = "Grass"; break;
                case 3: ret = "Leaf"; break;
                case 4: ret = "Wood"; break;
                case 5: ret = "Rice"; break;
                case 6: ret = "Wheat"; break;
                case 7: ret = "Vegetable"; break;
                case 8: ret = "Fruit"; break;
                case 9: ret = "Meat"; break;
                case 10: ret = "Egg"; break;
                case 11: ret = "Bean"; break;
                case 12: ret = "Dairy Product"; break;
                case 13: ret = "Fish & Shellfish"; break;
                case 14: ret = "Herbal Medicine"; break;
                case 15: ret = "Gold"; break;
                case 16: ret = "Silver"; break;
                case 17: ret = "Copper"; break;
                case 18: ret = "Iron"; break;
                case 19: ret = "Pure Iron"; break;
                case 20: ret = "Tin"; break;
                case 21: ret = "Lead"; break;
                case 22: ret = "Aluminum"; break;
                case 23: ret = "Silicon"; break;
                case 24: ret = "Sulfur"; break;
                case 25: ret = "Saltpeter"; break;
                case 26: ret = "Coal"; break;
                case 27: ret = "Steel"; break;
                case 28: ret = "Jade"; break;
                case 29: ret = "Corundum"; break;
                case 30: ret = "Diamond"; break;
                case 31: ret = "Quartz"; break;
                case 33: ret = "Platinum"; break;
                case 34: ret = "Rock"; break;
                case 35: ret = "Magnet"; break;
                case 36: ret = "Mercury"; break;
                case 37: ret = "Titanium"; break;
                case 39: ret = "Veichles"; break;
                case 42: ret = "Clay"; break;
                case 43: ret = "Red Clay"; break;
                case 45: ret = "Black Clay"; break;
                case 46: ret = "White & Yellow Clay"; break;
                case 47: ret = "Grey Clay"; break;
                case 48: ret = "Dry Clay"; break;
                case 49: ret = "Skin"; break;
                case 50: ret = "Feather"; break;
                case 51: ret = "Canine"; break;
                case 52: ret = "Hard Tissue"; break;
                case 53: ret = "Beehive"; break;
                case 54: ret = "Internal Organ"; break;
                case 55: ret = "Feces"; break;
                case 56: ret = "Fluid"; break;
                case 57: ret = "Liquor"; break;
                case 58: ret = "Nylon"; break;
                case 59: ret = "Crude"; break;
                case 60: ret = "Vegetable Oil"; break;
                case 61: ret = "Oil"; break;
                case 62: ret = "Seed"; break;
                case 63: ret = "Gas"; break;
                case 64: ret = "Gum"; break;
                case 65: ret = "Tissue"; break;
                case 66: ret = "Crystal"; break;
                case 67: ret = "Magic"; break;
                case 68: ret = "Venom"; break;
                case 69: ret = "Fur"; break;
                case 71: ret = "Chemical"; break;
                case 72: ret = "NPC Item"; break;
                case 73: ret = "NPC Equipment"; break;
                case 101: ret = "Water1"; break;
                case 102: ret = "Water2"; break;
            }
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork;

namespace PServer_v2.Other
{
    public abstract class ItemTemplate
    {
        public cGlobals globals;
        public cItem itemtype;
        public UInt16 id; public UInt16 ID { get { return id; } set { SetID(value); } }
        public byte ammt;
        public byte damage;
        public byte parent;
        public bool locked;

        public void Clear()
        {
            id = 0;
            itemtype = globals.gItemManager.GetItemByID(0); ;
            ammt = 0;
            damage = 0;
            parent = 0;
            locked = false;
        }
        public void CopyFrom(ItemTemplate i)
        {
            if (i != null)
            {
                id = i.id;
                itemtype = i.itemtype;
                ammt = i.ammt;
                damage = i.damage;
                parent = i.parent;
                locked = i.locked;
            }
        }
        void SetID(UInt16 v)
        {
            itemtype = globals.gItemManager.GetItemByID(v);
            if (itemtype != null) id = v;
        }
    }
}

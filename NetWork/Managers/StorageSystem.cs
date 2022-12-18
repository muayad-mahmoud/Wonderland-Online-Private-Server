using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.Managers
{
    public class CharacterStorage
    {
        ushort ID;
        cInvItem[] storage = new cInvItem[51];

        public cInvItem GetItem(int at)
        {
            return null;
        }
        public void PutItem(int at)
        {
        }
        public void MoveTo(int src,int dst)
        {
        }
    }

    public class StorageSystem
    {
        cGlobals g;

        public StorageSystem(cGlobals g)
        {
            this.g = g;
        }


        public CharacterStorage AccessmyStorage(cCharacterManager t)
        {
            return null;
        }

        public bool Create(cCharacter t)
        {
            return false;
        }
        public bool Delete(ushort ID)
        {
            return false;
        }
    }
}

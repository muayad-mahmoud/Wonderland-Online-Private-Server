using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
   public  class TentManager
    {
       List<cCharacter> charlist = new List<cCharacter>();
       cInvItem[] Warehouse = new cInvItem[50];
       cCharacter own;
       cGlobals globals;
       public TentManager(cCharacter r, cGlobals g)
       {
           own = r;
           globals = g;
       }

    }
}

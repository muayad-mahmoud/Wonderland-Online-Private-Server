using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public abstract class cAC
    {
        public cGlobals g;
        //public cRecvPacket rp;

        public cAC()
        {
        }
        public cAC(cGlobals globals)
        {
            this.g = globals;
        }
    }
}

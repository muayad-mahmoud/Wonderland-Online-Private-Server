using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_54:cAC
    {

        public cAC_54(cGlobals globals):base(globals)
        {
        }

        public void Send()
        {
            //some sort of server info given to client
            cSendPacket p = new cSendPacket(g);
            p.Header(54);
            p.AddArray(
            new byte[] {
                101,0,3,77,4,1,29,37,1,
                102,0,2,103,0,2,106,0,2,
                201,0,2,202,0,2,203,0,2,
                204,0,2,45,1,2,46,1,2,47,
                1,2,104,0,2,105,0,2,146,1,
                1,148,1,1,145,1,2,245,1,2,
                246,1,2,234,3,1,233,3,2,
                247,1,2,147,1,1,78,4,1,235,3,1,
                79,4,1,33,3,2,34,3,1,35,3,1,133,
                3,1,135,3,1,134,3,1,107,0,2 });
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }
        public void Send2()
        {
            //some sort of server info given to client
            cSendPacket p = new cSendPacket(g);
            p.Header(54);
            p.AddArray(
            new byte[] {
                89,2,2,90,2,1,91,2,1,189,2,2,190,2,1,191,2,1});
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

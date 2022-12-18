using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_62 : cAC
    {
        public cAC_62(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }
        public void Send_4(UInt32 id) //tent items
        {
            byte[] data = 
            {1, 0, 161, 148, 43, 0,     
0, 0, 39, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3,                    
0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 2, 0, 167, 148, 30, 0, 0, 0, 39, 0, 0,               
0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
3, 0, 170, 148, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                 
0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 118, 152, 40,                
0, 0, 0, 38, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 5, 0, 139, 148, 39, 0, 0, 0, 39, 0,               
0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 6, 0, 126, 148, 37, 0, 0, 0, 38, 0, 0, 0, 1, 0, 0,               
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 0, 0,                    
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 148, 148,                 
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,                     
0, 0, 0, 0, 0, 0, 0};
            cSendPacket p = new cSendPacket(g);
            p.Header(62, 4);
            p.AddDWord(id);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

        public void Send_53(UInt16 value)
        {
            cSendPacket p = new cSendPacket(g);
            p.Header(62, 53);
            p.AddWord(value);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
        }

    }
}

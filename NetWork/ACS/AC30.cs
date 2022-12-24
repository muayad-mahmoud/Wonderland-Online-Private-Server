using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_30 : cAC
    {
        public cAC_30(cGlobals g)
        {
            this.g = g;
        }
         public void SwitchBoard()
        {
            
            switch (g.packet.b)
            {
                
                case 2:
                    {
                        //request put item in prop keeper
                        int length = g.packet.data.Length;
                        g.Log(length.ToString());
                        byte location = g.packet.GetByte(2);
                        DataExt.cInvItem item = g.packet.character.inv.GetInventoryItem(location);
                        
                        if (g.packet.character.storage.putIteminStorage(item))
                        {
                            g.packet.character.inv.RemoveInv(location,item.ammt);
                            g.packet.character.storage.Save(g.packet.character.characterID);
                            g.packet.character.storage.Send_Storage();
                        }
                        
                       
                    }
                    break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;
            }
        }
    }
}

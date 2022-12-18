using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_69:cAC
    {
        public cAC_69(cGlobals g)
        {
            this.g = g;
        }
        public void SwitchBoard()
        {
            cWarp w = new cWarp();
            w.id = 0; 
            if (!g.packet.character.vechile.inVechile) return;
            switch (g.packet.b)
            {
                case 1://bcck to Earth
                    {
                        if (g.packet.character.map.MapID == 11016) return;
                        g.packet.character.vechile.Car.UseFuel(300);
                        w.x = 962; w.y = 735; w.mapTo = 11016;
                        g.packet.character.map.WarpRequest(WarpType.SpecialWarp,g.packet.character,0,w);
                    }break;
                case 2://moon
                    {
                        if (g.packet.character.map.MapID == 11163) return;
                        g.packet.character.vechile.Car.UseFuel(300);
                        w.x = 850; w.y = 700; w.mapTo = 11163;
                        g.packet.character.map.WarpRequest(WarpType.SpecialWarp, g.packet.character, 0, w);
                    }break;
                case 3://mars
                    {
                        if (g.packet.character.map.MapID == 11164) return;
                        g.packet.character.vechile.Car.UseFuel(300);
                        w.x = 782; w.y = 1295; w.mapTo = 11164;
                        g.packet.character.map.WarpRequest(WarpType.SpecialWarp, g.packet.character, 0, w);
                    }break;
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

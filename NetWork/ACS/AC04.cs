using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_4 : cAC
    {
        public cAC_4(cGlobals globals) : base (globals)
        {

        }
        public void SwitchBoard()
        {
            //rp=g.packet;
        }

        public void Send_4(cCharacter c)
        {
            cSendPacket p = new cSendPacket(g);
			p.Header(4);
			p.AddDWord(c.characterID);
			p.AddByte((byte)c.body); //body style
			p.AddByte((byte)c.element); //element
			p.AddByte(c.level); //level
			p.AddWord(c.map.MapID); //map id
			p.AddWord(c.x); //x
			p.AddWord(c.y); //y
			p.AddByte(0);p.AddByte(c.head);p.AddByte(0);
			p.AddDWord(c.color1); //colors
			p.AddDWord(c.color2); //colors
			p.AddByte(c.eq.CountClothes());//clothesAmmt); // ammt of clothes
            p.AddArray(c.eq.GetWearingClothes());
			p.AddDWord(0); p.AddByte(0); //??
			p.AddByte(c.rebirth); //is rebirth
			p.AddByte((byte)c.job); //rb class
			p.AddString(c.name);//(BYTE*)c.name,c.nameLen); //name
			p.AddString(c.nickname);//(BYTE*)c.nick,c.nickLen); //nickname
			p.AddByte(255); //??
            p.SetSize();
			p.character = g.packet.character;
            p.Send();
        }

    }
}

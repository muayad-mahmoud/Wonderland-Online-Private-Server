using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PServer_v2.NetWork.DataExt
{
    public class cUser
    {
        public UInt32 userID;
        public UInt32 playerID;
        public UInt32 player2ID;
        public string username;
        public string password;
        Int32 Im;
        //in server use
        cGlobals globals;
        public Int32 IM { get { return Im; } set { Im = value; } }
        public bool created1;
        public bool created2;
        public string charname1;
        public string charname2;
        public UInt32 fileIndex;
        public byte GMLvl;

        public cUser(cGlobals g)
        {
            globals = g;
            userID = 0;
            username = "";
            password = "";
            IM = 0;
            fileIndex = 0;
            created1 = false;
            created2 = false;
            charname1 = "";
            charname2 = "";
            GMLvl = 0;
        }
        public void SetFromDB(DataTable user)
        {
            if (user.Rows.Count != 1) return;
            DataRow r = user.Rows[0];
            userID = (UInt32)(Int32.Parse(r["ID"].ToString()));
            playerID = (UInt32.Parse(r["character1ID"].ToString()));// v;
            player2ID = (UInt32.Parse(r["character2ID"].ToString()));// v;
            username = (string)r["Username"];
            password = (string)r["Password"];
            GMLvl = (byte)(Int32.Parse(r["GMLevel"].ToString()));
            IM = (Int32.Parse(r["IM"].ToString()));
        }
        public void PushIM()
        {
            cSendPacket im = new cSendPacket(globals);
            im.Header(35, 4);
            im.AddDWord((uint)IM);
            im.AddDWord(0);
            im.AddDWord(0);
            im.AddDWord(0);
            im.SetSize();
            if (globals.gCharacterManager.IsOnline((ushort)player2ID))
                im.character = globals.gCharacterManager.getByID(player2ID);
            if (globals.gCharacterManager.IsOnline((ushort)playerID))
                im.character = globals.gCharacterManager.getByID(playerID);
            im.Send();
        }
        public bool SubIM(int by)
        {
            if (IM > by)
            {
                Im -= by;
                if (IM < 0)
                    Im = 0;
                PushIM();
                return true;                
            }
            else
                return false;
        }


    }
}

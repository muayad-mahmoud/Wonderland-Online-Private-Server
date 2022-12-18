using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PServer_v2.NetWork.DataExt
{
    public class cFriend : cCharacter
    {

        public cFriend(uint id,string Name,byte lvl,byte reb,byte Job,byte elem,byte Body,byte Head,uint col1,uint col2,string nick)
        {
            characterID = id; name = Name; level = lvl; rebirth = reb; job = (RebornJob)Job; head = Head;
            
            element = (Element)elem; body = (BodyStyle)Body; color1 = col1; color2 = col2; nickname = nick;
        }
    }
    public class cCharacterFriend
    {
        List<cFriend> myFriends = new List<cFriend>();
        cCharacter Owner;
        cGlobals globals;
        public cCharacterFriend(cCharacter r,cGlobals g)
        {
            Owner = r; globals = g;
        }

        public bool LoadFriends(string str)
        {

            foreach (string y in str.Split('&'))
            {
                if (y.Length > 0 && y != "none")
                {
                    string[] f = y.Split(' ');
                    cFriend tmp = new cFriend(uint.Parse(f[0]), f[1], byte.Parse(f[2]), byte.Parse(f[3]),
                        byte.Parse(f[4]), byte.Parse(f[5]), byte.Parse(f[6]), byte.Parse(f[7]), uint.Parse(f[8]),
                        uint.Parse(f[9]), f[10]);
                    myFriends.Add(tmp);
                }
            }
            return true; ;
        }
        public string GetFriends_string()
        {
            int ct = 0;
            string query = "";
            foreach (cCharacter h in myFriends)
            {
                ct++;
                query += h.characterID.ToString() + " " + h.name + " " + h.level.ToString() + " " + h.rebirth.ToString() + " " +
                    ((byte)h.job).ToString() + " " + ((byte)h.element).ToString() + " " + ((byte)h.body).ToString() + " " +
                    h.head.ToString() + " " + h.color1.ToString() + " " + h.color2.ToString() + " " + h.nickname;
                    query += "&";
            }
            if (query == "")
                query+= "none";
            return query;
        }
        public void Login()
        {
            foreach (cCharacter f in myFriends)
                if (globals.gCharacterManager.IsOnline((ushort)f.characterID))
                {
                    Send_9(Owner.characterID, globals.gCharacterManager.getByID((ushort)f.characterID), 0);
                    Send_9(f.characterID, Owner, 0);
                }
        }
        public void Dced()
        {
            foreach (cCharacter f in myFriends)
            {
                Send_9(Owner.characterID, globals.gCharacterManager.getByID((ushort)f.characterID), 3);
            }
        }
        public bool ContainsFriendID(uint id)
        {
            for (int a = 0; a < myFriends.Count; a++)
                if (myFriends[a].characterID == id)
                    return true;
                
                    return false;
        }
        public void AddFriend(cCharacter src)
        {
            myFriends.Add(new cFriend(src.characterID,src.name,src.level,src.rebirth,(byte)src.job,(byte)src.element,(byte)src.body,
                src.head,src.color1,src.color2,src.nickname));
            Send_9(src.characterID, Owner,0);
        }
        public void RemoveFriend(cCharacter src)
        {
            for (int a = 0; a < myFriends.Count; a++)
                if (myFriends[a].characterID == src.characterID || myFriends[a].name == src.name)
                    myFriends.Remove(myFriends[a]);
            Send_4(src.characterID);
        }

        public void RecvList()
        {
            cSendPacket y = new cSendPacket(globals);
            y.Header(14, 5);
            y.AddArray(new byte[]{100, 0, 0, 0, 6, 71, 77, 164, 164, 164, 223, 200, 0,      
0, 0, 0, 0, 28, 175, 125, 26, 28, 175, 125, 26, 0, 0});
            foreach (cCharacter h in myFriends)
            {
                y.AddDWord(h.characterID);
                y.AddString(h.name);
                y.AddByte(h.level);
                y.AddByte(h.rebirth);
                y.AddByte((byte)h.job);
                y.AddByte((byte)h.element);
                y.AddByte((byte)h.body);
                y.AddByte(h.head);
                y.AddDWord(h.color1);
                y.AddDWord(h.color2);
                y.AddString(h.nickname);
                y.AddByte(0);
            }
            y.SetSize();
            y.character = Owner;
            y.Send();
        }
        
        void Send_9(uint ID, cCharacter t, byte value)
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(14, 9);
            p.AddDWord(ID);
            p.AddByte(value);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        void Send_4(uint ID)
        {
            cSendPacket p = new cSendPacket(globals);
            p.Header(14, 4);
            p.AddDWord(ID);
            p.SetSize();
            p.character = Owner;
            p.Send();
        }
        
    }
}

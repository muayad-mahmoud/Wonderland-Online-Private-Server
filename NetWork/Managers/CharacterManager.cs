using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PServer_v2.NetWork.DataExt;
using PServer_v2.NetWork;
using PServer_v2.NetWork.ACS;

namespace PServer_v2.NetWork.Managers
{
    public class cCharacterManager
    {

        public List<cCharacter> characterList = new List<cCharacter>();
        cGlobals globals;

        public cCharacterManager(cGlobals src)
        {
            globals = src;
        }

        public int GetCount()
        {
            int ammt = 0;
                ammt = characterList.Count;
            return ammt;
        }
        public void SetByClient(cCharacter character)
        {
                foreach (cCharacter c in characterList)
                {
                    if (c.client == character.client)
                    {

                    }
                }
        }
        public void Add(cCharacter character)
        {
                characterList.Add(character);
            globals.Interface(1);
        }
        public void Remove(cCharacter character)
        {
                characterList.Remove(character);
                if (character.characterID != 0)
                {
                    WriteCharacter(character); //write character to database  TODO 
                    character.characterID = 0;
                }
            globals.Interface(1);
        }
        public void Clear()
        {
            List<cCharacter> clist = getCharList();
            foreach (cCharacter c in clist)
            {
                //do not update other players about log off 
                c.logging = true;
                c.Disconnect();
            }
                characterList.Clear();
            globals.Interface(1);
        }

        public bool isDuplicateName(string name)
        {
            bool bRetVal = false;
                try
                {

                    DataTable table;
                    String query = "select * from characters where name = \"" + name + "\"";
                    table = globals.WloDatabase.GetDataTable(query,true);
                    if (table.Rows.Count > 0)
                        bRetVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            return bRetVal;
        }

        #region DataBase tools
        public bool DeleteCharacter(cCharacter c)
        {
            bool retVal = false;
                try
                {
                    string query = "DELETE FROM characters WHERE characterID = " + c.characterID + ";";
                    if (globals.WloDatabase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
                c.inv.Delete(c.characterID);
            //globals.WloDatabase.Delete("Mail", "id =" + c.characterID);
            //globals.WloDatabase.Delete("Friend", "characterID =" + c.characterID);
            return retVal;
        }
        public bool WriteNewCharacter(cCharacter c)
        {
            bool retVal = false;
                try
                {
                    string query = "INSERT INTO characters (characterID, name, nickname, password," +
                        "map, x, y,body, head, colors1, colors2, gold, level, exp, curHP, maxHP, curSP, maxSP, element," +
                        "flags, lastMap, recordSpot, gpsSpot, rebirth, job, stats,sidebar,skills,mail,friends" +

                        ") VALUES (" +
                        c.characterID + ",'"+ c.name + "','" + c.nickname +
                        "','" + c.password + "'," + c.map.MapID + "," + c.x + "," + c.y + "," +
                        (byte)c.body + "," + (byte)c.head + "," + c.color1 + "," + c.color2 + "," + c.gold + "," + c.level + "," +
                        c.stats.TotalExp + "," + c.stats.CurHP + "," + c.stats.CurSP + "," + 0 + "," + 0 + "," + (byte)c.element + "," +
                        GetFlagsString(c) + "," +
                        GetWarpString(c.lastMap) + "," +
                        GetWarpString(c.recordMap) + "," +
                        GetWarpString(c.gpsMap) + "," +
                        c.rebirth + "," +
                        (byte)c.job + "," +
                        c.stats.GetStatString() + "," +
                        "'none'"+",'"+
                        c.skills.SaveSkills()+"','"+
                        c.Mail.SaveMail()+"','"+
                        c.Friends.GetFriends_string()+"' "+
                        ");";
                    if (globals.WloDatabase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }

                c.inv.Create(c.characterID);
            return retVal;
        }
        string GetFlagsString(cCharacter c)
        {
            string s = "'";
            s += c.state + " ";
            if (c.inCarnie) s += "1 "; else s += "0 ";
            if (c.inMap) s += "1 "; else s += "0 ";

            s += "'";
            return s;
        }
        string GetWarpString(cWarp w)
        {
            string s = "'";
            s += w.mapTo + " " + w.x + " " + w.y + "'";
            return s;
        }
        public bool WriteCharacter(cCharacter c)
        {
            
            // ",   exp = 30, curHP = 1, maxHP = 2, curSP = 1, maxSP = 2, element = 1 WHERE rowid = 3;"
            bool retVal = false;
                try
                {
                    string query = "UPDATE characters SET " +
                        //"characterID = " + c.characterID + "," +
                        //"state = " + c.characterID + "," +
                        //"name = " + c.characterID + "," +
                        "nickname = '" + c.nickname + "', " +
                        //"password = " + c.characterID + "," +
                        "map = " + c.map.MapID + ", " +
                        "x = " + c.x + ", " +
                        "y = " + c.y + ", " +
                        //"body = " + c.characterID + "," +
                        //"head = " + c.characterID + "," +
                        "colors1 = " + c.color1 + ", " +
                        "colors2 = " + c.color2 + ", " +
                        "gold = " + c.gold + ", " +
                        "level = " + c.level + ", " +
                        "exp = " + c.stats.TotalExp + ", " +
                        "curHP = " + c.stats.CurHP + ", " +
                        "maxHP = " + 0 + ", " +
                        "curSP = " + c.stats.CurSP + ", " +
                        "maxSP = " + 0 + ", " +

                        "flags = " + GetFlagsString(c) + ", " +
                        "lastMap = " + GetWarpString(c.lastMap) + ", " +
                        "recordSpot = " + GetWarpString(c.recordMap) + ", " +
                        "gpsSpot = " + GetWarpString(c.gpsMap) + ", " +
                        "rebirth = " + c.rebirth + ", " +
                        "job = " + (byte)c.job + ", " +
                        "stats = " + c.stats.GetStatString() + "," +
                        "sidebar ='" + "" + "'," +
                        "skills ='" +c.skills.SaveSkills() + "'," +
                        "mail ='" +c.Mail.SaveMail()+"',"+
                        "friends ='"+c.Friends.GetFriends_string()+"'"+

                         " WHERE characterID = " + c.characterID + ";";


                    if (globals.WloDatabase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
                c.inv.Save(c.characterID);
            return retVal;
        }
        public cCharacter GetDBByID(UInt32 id,bool noload = false)
        {
            cCharacter retVal = null;
                try
                {

                    DataTable table;
                    String query = "select * from characters where characterID = \"" + id + "\"";
                    table = globals.WloDatabase.GetDataTable(query,true);

                    if (table == null)
                    {
                        retVal = null;
                    }
                    else
                    {
                        cCharacter c = new cCharacter(globals);
                        c.SetFromDB(table, noload);
                        retVal = c;
                    }
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    globals.Log("Im here \r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }


            return retVal;
        }
        public bool GetDBByID(cCharacter c, UInt32 id)
        {
            bool bRetVal = false;
                try
                {

                    DataTable table;
                    String query = "select * from characters where characterID = \"" + id + "\"";
                    table = globals.WloDatabase.GetDataTable(query,true);

                    if (table == null)
                    {
                        bRetVal = false;
                    }
                    else
                    {
                        c.SetFromDB(table);
                        bRetVal = true;
                    }
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            return bRetVal;
        }
        #endregion

        /*public void Redirect(byte[] data, System.Net.Sockets.TcpClient y, bool u)
        {
            OutPacket(data, y, u);
        }*/
        public cCharacter getByID(UInt32 id)
        {
                foreach (cCharacter c in characterList)
                    if (c.characterID == id) return c;
            return null;
        }
        public bool IsOnline(ushort id)
        {
            if (getByID(id) != null)
                return true;
            else
                return false;
        }
        public void outputCharList()
        {
                foreach (cCharacter c in characterList)
                {
                    if (c.characterID == 0)
                        globals.Log("(empty char)\r\n");
                    else globals.Log(c.name + "\r\n");
                }
        }
        public List<cCharacter> getCharList()
        {
            List<cCharacter> cList = new List<cCharacter>();
                foreach (cCharacter c in characterList)
                    cList.Add(c);
            return cList;
        }

        public void SendAC4()
        {

            byte[] data = {100, 0, 0, 0, 0, 1, 1, 35, 39, 210, 2, 227, 3, 0, 0, 0, 28, 175,            
                125, 26, 28, 175, 125, 26, 0, 0, 0, 0, 0, 0, 0, 0, 8, 67,           
                117, 112, 105, 100, 49, 48, 48, 0, 255};
            cSendPacket p = new cSendPacket(globals);
            p.Header(4);
            p.AddArray(data);
            p.SetSize();
            p.character = globals.packet.character;
            p.Send();
                foreach (cCharacter c in characterList)
                {
                    if ((!c.logging) && (c.characterID != globals.packet.character.characterID))
                    {
                        if (c.map.MapID == globals.packet.character.map.MapID)
                        {
                            globals.packet.character.Send_3_They(c);
                            globals.ac5.Send_0(globals.packet.character, c);
                            globals.ac10.Send_3(globals.packet.character.characterID, 255, c);
                            globals.ac5.Send_8(globals.packet.character.characterID, 0, c);
                            globals.ac23.Send_122(globals.packet.character.characterID, c);
                        }
                        else
                        {
                            globals.packet.character.Send_3_They(c);
                            globals.ac5.Send_0(globals.packet.character, c);
                            globals.ac5.Send_8(globals.packet.character.characterID, 0, c);
                        }
                        p = new cSendPacket(globals);
                        p.Header(4);
                        p.AddDWord(c.characterID);
                        p.AddByte((byte)c.body); //body style
                        p.AddByte((byte)c.element); //element
                        p.AddByte(c.level); //level
                        p.AddWord(c.map.MapID); //map id
                        p.AddWord(c.x); //x
                        p.AddWord(c.y); //y
                        p.AddByte(0); p.AddByte(c.head); p.AddByte(0);
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
                        p.character = globals.packet.character;
                        p.Send();
                    }
                }
            
        }
        public void NickNameChanged(cCharacter src)
        {
            foreach (cCharacter c in characterList)
            {
                globals.ac10.Send_1(src, c);
            }
        }
        public void Send1_1(cCharacter src) //a user disconnected
        {
                foreach (cCharacter c in characterList)
                {
                    if ((!c.logging) && (c.characterID != src.characterID))
                    {
                        if (c.map == src.map)
                        {
                            //src.Party.Leave();
                            globals.ac1.Send_1(src.characterID, c);
                        }
                        else
                        {
                            globals.ac1.Send_1(src.characterID, c);
                        }
                    }
                }
            
        }

    }
}

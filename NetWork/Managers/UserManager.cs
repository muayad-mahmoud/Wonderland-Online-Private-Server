using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.Managers
{
    public class cUserManager
    {
        public uint versionnum;
        public string itemdat;
        public List<cUser> userList = new List<cUser>();
        Object thisLock = new Object();
        cGlobals globals;

        public cUserManager(cGlobals src)
        {
            globals = src;
        }
        public void Disconnect(cUser user)
        {
            lock (thisLock)
            {

            }
        }
        public void Add(cUser user)
        {
            bool bAdd = true;
            lock (thisLock)
            {
                foreach (cUser u in userList)
                {
                    if ((user.userID == u.userID) || (user.userID == u.player2ID))
                    {
                        bAdd = false;
                    }
                }
                if (bAdd != false)
                {
                    userList.Add(user);
                }
            }
        }
        public void DisconnectUser(UInt32 id)
        {
            lock (thisLock)
            {
                cUser u = GetListByID(id);
                if (u == null) return;

                if (u.userID != 0)
                {
                    //TODO save user
                }

                userList.Remove(u);
                //check to see if any characters related to this user are disconected
                cCharacter c = globals.gCharacterManager.getByID(u.playerID);
                if (c != null)
                {
                    if (c.characterID != 0) globals.gCharacterManager.Remove(c);
                }
                c = globals.gCharacterManager.getByID(u.player2ID);
                if (c != null)
                {
                    if (c.characterID != 0) globals.gCharacterManager.Remove(c);
                }
            }

        }
        public cUser GetListByID(UInt32 id)
        {
            cUser retVal = null;
            lock (thisLock)
            {
                foreach (cUser u in userList)
                {
                    if (id == u.userID)
                    {
                        retVal = u;
                        break;
                    }
                }
            }
            return retVal;
        }
        public byte gmLvl(UInt32 userID)
        {
            byte retVal = 0;
            lock (thisLock)
            {
                foreach (cUser u in userList)
                {
                    if (userID == u.userID)
                    {
                        retVal = u.GMLvl;
                    }
                }
            }
            return retVal;
        }
        public int ImPoints(UInt32 userID)
        {
            int retVal = 0;
            lock (thisLock)
            {
                foreach (cUser u in userList)
                {
                    if (userID == u.userID)
                    {
                        retVal = u.IM;
                    }
                }
            }
            return retVal;
        }
   
        public cUser GetDBByUnPw(string username, string password)
        {
            cUser u = null;
            lock (thisLock)
            {
                try
                {
                    DataTable user;
                    String query = "select* from User where Username = \"" + username + "\"";
                    user = globals.UserDataBase.GetDataTable(query,true);
                    if (user.Rows.Count > 1)
                    {
                        //we have an error in the database, duplicate usernames
                        globals.Log("Database error, duplicate names (" + (string)user.Rows[0]["Username"] + ")\r\n");
                    }
                    else if (user.Rows.Count < 1)
                    {
                        globals.Log("User (" + username + ") not found.\r\n");
                    }

                    else
                    {
                        string pass = (string)user.Rows[0]["Password"];
                        if (string.Compare(pass, password) != 0)
                        {
                            globals.Log("Invalid password (" + password + ")\r\n");
                        }
                        else
                        {
                            u = new cUser(globals);
                            u.SetFromDB(user);
                        }
                    }
                }

                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }

            }
            return u;
        }
        public bool UpdatePlayerID(cUser user, UInt32 id, byte slot)
        {
            bool retVal = false;
            lock (thisLock)
            {
                try
                {
                    string query = "";
                    if (slot == 1)
                    {
                        user.playerID = id;
                        query = "UPDATE User SET character1ID = " + id +
                            " WHERE ID = " + user.userID + ";";
                    }
                    else
                    {
                        user.player2ID = id;
                        query = "UPDATE User SET character2ID = " + id +
                            " WHERE ID = " + user.userID + ";";
                    }
                    if (globals.UserDataBase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            }
            return retVal;
        }
        public bool UpdatePlayerIM(cUser user)
        {
            bool retVal = false;
            lock (thisLock)
            {
                try
                {
                    string query = "";
                    query = "UPDATE User SET IM = " + user.IM +
                        " WHERE ID = " + user.userID + ";";
                    if (globals.UserDataBase.ExecuteNonQuery(query) == 1)
                        retVal = true;
                }
                catch (Exception fail)
                {
                    globals.Log("failed.\r\n");
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    System.Windows.Forms.MessageBox.Show(error);
                }
            }
            return retVal;
        }

        /*public void CreateNewUser(cUser user)
        {
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("userID", user.userID.ToString());
            data.Add("username", user.username);
            data.Add("password", user.password);
            data.Add("character2ID", user.player2ID.ToString());
            data.Add("IM", user.IM.ToString());
            data.Add("gmLvl", user.GMLvl.ToString());
            try
            {
                db.Insert("users", data);
            }
            catch (Exception crap)
            {
                System.Windows.Forms.MessageBox.Show(crap.Message);
            }
        }*/
        public bool isDuplicateName(string name, UInt32 userID)
        {
            bool retVal = false;
            lock (thisLock)
            {
                foreach (cUser u in userList)
                {
                    if (userID != u.userID)
                    {
                        if (string.Compare(name, u.charname1) == 0) { retVal = true; break; }
                        else if (string.Compare(name, u.charname2) == 0) { retVal = true; break; }
                    }
                }
            }
            return retVal;
        }

    }
}

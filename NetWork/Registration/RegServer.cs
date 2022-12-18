using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using PServer_v2.DataBase;

namespace PServer_v2.NetWork.Registration
{
    public class RegServer
    {
        int port = 7425;
        cGlobals globals;
        public List<RegClient> clientList = new List<RegClient>();
        cListener Listener;
        JOBBER cManager;
        public RegServer(cGlobals src)
        {
             
            src.cRegServer = this;
            globals = src;cManager = new JOBBER(globals);
            Listener = new cListener(port, src);
            Listener.Start();

            
        }

        int GetRegUserCnt { get { return globals.UserDataBase.GetDataTable("User").Rows.Count; } }
        void SwitchBoard(cRecvPacket g)
        {
            switch (g.a)
            {
                case 0:
                    {
                        try
                        {
                            cSendPacket grr = new cSendPacket(globals);
                            grr.rclient = g.rclient;
                            grr.Header(2, 1);
                            grr.AddWord((ushort)(globals.gCharacterManager.characterList.Count));
                            grr.SetSize();
                            grr.Send();
                            grr = new cSendPacket(globals);
                            grr.rclient = g.rclient;
                            grr.Header(2, 2);
                            grr.AddWord((ushort)GetRegUserCnt);
                            grr.SetSize();
                            grr.Send();
                            grr = new cSendPacket(globals);
                            grr.rclient = g.rclient;
                            grr.Header(2, 4);
                            grr.AddWord((ushort)globals.Limit);
                            grr.SetSize();
                            grr.Send();
                            
                        }
                        catch { }
                    } break;
                case 1:
                    {
                        try
                        {
                            bool found = false;
                            var name = globals.GetString(g.data, 1);
                            var table = globals.UserDataBase.GetDataTable("User");
                            for (int a = 0; a < table.Rows.Count; a++)
                            {
                                if (table.Rows[a][1].ToString() == name)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                cSendPacket gi = new cSendPacket(globals);
                                gi.rclient = g.rclient;
                                gi.Header(1, 1);
                                gi.AddString(name);
                                gi.SetSize();
                                gi.Send();
                            }
                            else
                            {
                                cSendPacket h = new cSendPacket(globals);
                                h.rclient = g.rclient;
                                h.Header(1, 2);
                                h.SetSize();
                                h.Send();
                            }
                        }
                        catch
                        {
                            cSendPacket h = new cSendPacket(globals);
                            h.rclient = g.rclient;
                            h.Header(1, 2);
                            h.SetSize();
                            h.Send();
                        }
                    }break;
                case 2:
                    {
                        switch (g.b)
                        {
                            case 1:
                                {
                                    int n = 3 + g.GetByte(2);
                                    var user = globals.gUserManager.GetDBByUnPw(g.GetString(2), g.GetString(n));
                                    if (user != null)
                                    {
                                        cSendPacket t = new cSendPacket(globals);
                                        t.Header(2, 5);
                                        t.SetSize();
                                        t.rclient = g.rclient;
                                        t.Send();
                                    }
                                    else
                                    {
                                        cSendPacket t = new cSendPacket(globals);
                                        t.Header(2, 6);
                                        t.SetSize();
                                        t.rclient = g.rclient;
                                        t.Send();
                                    }
                                }break;
                            case 2:
                                {
                                    cSendPacket t = new cSendPacket(globals);
                                    t.Header(2, 6);
                                    t.SetSize();
                                    t.rclient = g.rclient;
                                    t.Send();
                                }break;
                        }
                    } break;
                case 3:
                    {
                        try{
                        var user = g.GetString(1);
                        var psw = g.GetString((g.data[1] + 2));
                            if(psw.Length < 6)throw new Exception("PassWord too Short");
                        cSendPacket f = new cSendPacket(globals);
                            f.Header(3,1);
                            f.rclient = g.rclient;
                            f.AddString(user);
                            f.AddString(psw);
                            var fg = NewUser(user,psw);
                            if (fg != 0)
                                f.AddDWord(fg);
                            else throw new Exception();
                            f.SetSize();
                            f.Send();
                            f = new cSendPacket(globals);
                            f.Header(2, 2);
                            f.AddWord((ushort)GetRegUserCnt);
                            f.SetSize();
                            for (int a = 0; a < clientList.Count; a++)
                            {
                                f.rclient = clientList[a];
                                f.Send();
                            }
                            cSendPacket grr = new cSendPacket(globals);
                            grr.rclient = g.rclient;
                            grr.Header(2, 1);
                            grr.AddWord((ushort)(globals.cRegServer.clientList.Count + globals.gCharacterManager.characterList.Count));
                            grr.SetSize();
                            for (int a = 0; a < clientList.Count; a++)
                            {
                                grr.rclient = clientList[a];
                                grr.Send();
                            }
                        }
                        catch(Exception r)
                        {
                            cSendPacket f = new cSendPacket(globals);
                            f.Header(3,2);
                            f.rclient = g.rclient;
                            f.AddString(r.Message);
                            f.SetSize();
                            f.Send();
                        }
                    } break;
                case 4:
                    {
                        switch (g.b)
                        {
                            case 1:
                                {
                                    try
                                    {
                                        var k = globals.UserDataBase.GetDataTable("User");
                                        foreach (System.Data.DataRow u in k.Rows)
                                        {
                                            if (u["Username"].ToString() == g.GetString(2))
                                            {
                                                cSendPacket l = new cSendPacket(globals);
                                                l.Header(2, 3);
                                                l.AddDWord(uint.Parse(u["ID"].ToString()));
                                                l.AddString((string)u["Username"]);
                                                l.AddString((string)u["Password"]);
                                                l.AddDWord(uint.Parse(u["character1ID"].ToString()));
                                                l.AddDWord(uint.Parse(u["character2ID"].ToString()));
                                                l.AddDWord(uint.Parse(u["IM"].ToString()));
                                                l.AddByte(byte.Parse(u["GMLevel"].ToString()));
                                                string str = "";
                                                switch (byte.Parse(u["GMLevel"].ToString()))
                                                {
                                                    case 0: str = "Regular"; break;
                                                    case 1: str = "Tester"; break;
                                                    case 2: str = "DevGroup"; break;
                                                    case 20: str = "Administrator"; break;
                                                }
                                                l.AddString(str);
                                                l.SetSize();
                                                l.rclient = g.rclient;
                                                l.Send();
                                            }
                                        }
                                    }
                                    catch { }
                                } break;
                            case 2:
                                {
                                    var re = globals.UserDataBase.GetDataTable("User");
                                    foreach (System.Data.DataRow b in re.Rows)
                                    {
                                        if (b["ID"].ToString() == g.GetDWord(2).ToString())
                                        {
                                            cSendPacket d = new cSendPacket(globals);
                                                d.Header(2, 7);
                                                try
                                                {
                                                    Dictionary<string, string> k = new Dictionary<string, string>();
                                                    k.Add("Password", g.GetString(6));
                                                    globals.UserDataBase.Update("User", k, "ID =" + g.GetDWord(2));

                                                    d.AddByte(1);
                                                }
                                                catch { d.AddByte(2); }
                                                finally { d.SetSize(); d.rclient = g.rclient; d.Send(); }
                                        }
                                    }
                                }break;
                            case 3:
                                {
                                    int ptr = 3;
                                    var id = g.GetDWord(ptr); ptr += 4;
                                    var user = g.GetString(ptr); ptr += g.GetByte(ptr)+1;
                                    var cha = g.GetString(ptr);
                                    var fg = globals.UserDataBase.GetDataTable("User");
                                    cSendPacket s = new cSendPacket(globals);
                                    s.Header(2, 8);
                                    
                                    foreach (System.Data.DataRow l in fg.Rows)
                                    {
                                        try
                                        {
                                            switch (g.GetByte(2))
                                            {
                                                case 1: if (l["Username"].ToString() == user && (uint.Parse(l["ID"].ToString()) == id))
                                                    {
                                                        s.AddByte(1);
                                                        s.AddString(l["Password"].ToString());
                                                        goto Next;
                                                    } break;
                                                case 2: break;
                                            }
                                        }
                                        catch { }
                                    }
                                Next:
                                    s.SetSize();
                                    s.rclient = g.rclient;
                                    s.Send();
                                }break;
                        }
                    }break;
                case 5:
                    {
                        switch (g.b)
                        {
                            case 1: cManager.ADD(g.GetByte(2), g.data, g.rclient); break;
                            case 2: cManager.Chng(g.GetByte(2), g.data, g.rclient); break;
                            case 3: cManager.Del(g.GetByte(2), g.data, g.rclient); break;
                            case 8: cManager.GetInfo(g.GetByte(2), g.rclient); break;
                        }
                    }break;
            }
        }
        public void Error(string t,RegClient h)
        {
            cSendPacket l = new cSendPacket(globals); l.Header(199); l.AddString(t);
            l.SetSize(); l.rclient = h;  l.Send();
        }
        public void PushLimit()
        {
            cSendPacket grr = new cSendPacket(globals);            
            grr.Header(2, 4);
            grr.AddWord((ushort)globals.Limit);
            grr.SetSize();
            for (int a = 0; a < clientList.Count; a++)
            {
                grr.rclient = clientList[a];
                grr.Send();
            }
        }
        public void PushpplOnline()
        {
            cSendPacket up = new cSendPacket(globals);
            up.Header(2, 1);
            up.AddWord((ushort)globals.gCharacterManager.characterList.Count);
            up.SetSize();
            for (int a = 0; a < clientList.Count; a++)
            {
                up.rclient = clientList[a];
                up.Send();
            }
        }
        public void Disconnect()
        {
            Listener.Stop();
            foreach (RegClient g in clientList)
                g.Disconnect();
        }
        public UInt16 NewUser(string user,string psw)
        {
            var db = globals.UserDataBase.GetDataTable("User");
            int ID = (10000+(db.Rows.Count*2)+1);     
            retry:
            foreach (System.Data.DataRow g in db.Rows)
            {
                if ((int.Parse(g["ID"].ToString()) == ID - 1||int.Parse(g["ID"].ToString()) == ID
                    || int.Parse(g["character1ID"].ToString()) == ID || 
                    int.Parse(g["character2ID"].ToString()) == ID))
                {
                    ID+=2; goto retry;
                }

            }
            Dictionary<string,string> f = new Dictionary<string,string>();
            f.Add("ID",ID.ToString());
            f.Add("Username",user);
            f.Add("Password",psw);
            f.Add("character1ID","0");
            f.Add("character2ID","0");
            f.Add("IM","0");
            f.Add("GMLevel","0");
            if (globals.UserDataBase.Insert("User", f))
            {
                return (ushort)ID;
            }
            else
            return 0;
        }
        public void DataRcv(object sender,DataRcv g)
        {
            SwitchBoard(g.g);
        }
        public void ClientConnected(RegClient r)
        {
            clientList.Add(r);
            
        }
        public void ClientDced(object sender, UserDC g)
        {
            clientList.Remove(g.g);
            cSendPacket up = new cSendPacket(globals);
            up.Header(2, 1);
            up.AddWord((ushort)globals.cRegServer.clientList.Count);
            up.SetSize();
            for (int a = 0; a < clientList.Count; a++)
            {
                up.rclient = clientList[a];
                up.Send();
            }
        }

    }

    public class cListener
    {
        private TcpListener listener;
        cGlobals globals;

        public cListener(int portNr, cGlobals t)
        {
            globals = t;
            //Same way as the kickstart
            this.listener = new TcpListener(IPAddress.Any, portNr);
            globals.Log("Registration Server started... waiting for clients.\r\n");
        }

        ~cListener()
        {
            this.Stop();
        }

        public void Start()
        {
            //Same as the kickstart
            this.listener.Start();
            this.ListenForNewClient();
        }

        public void Stop()
        {
            this.listener.Stop();
        }

        private void ListenForNewClient()
        {
            //Just as for the Read/BeginRead-methods in the TcpClient the BeginAcceptTcpClient
            //is the AcceptTcpClient()s asyncrounous brother.
            try
            {
                this.listener.BeginAcceptTcpClient(this.AcceptClient, null);
            }
            catch { }
        }

        private void AcceptClient(IAsyncResult ar)
        {
            //The EndAcceptTcpClient works similar to the EndRead on the TcpClient
            //but instead it returns the client that has connected

            RegClient myClient = new RegClient(globals);
            myClient.DataReceived += globals.cRegServer.DataRcv;
            myClient.UserDisconnected += globals.cRegServer.ClientDced;
            try
            {
                Socket client = this.listener.EndAcceptSocket(ar);
                myClient.Start(client);
                globals.cRegServer.clientList.Add(myClient);
            }
            catch { }
            this.ListenForNewClient();
        }
    }
}

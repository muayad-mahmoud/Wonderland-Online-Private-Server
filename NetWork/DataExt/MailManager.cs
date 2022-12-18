using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PServer_v2.NetWork.DataExt
{
    public class Mail
    {
        public uint id;
        public string message;
        public string type;
        public uint targetid;
        public void Load(string r)
        {
            var words = r.Split(' ');
            id = (UInt16)(Int64.Parse(words[0]));
            type = (words[2]);
            targetid = (UInt16)(Int64.Parse(words[3]));
            message = words[1];
        }

    }
    public class cMailManager
    {
        List<Mail> myMail = new List<Mail>();
        cCharacter own;
        cGlobals globals;
        public cMailManager(cCharacter owner,cGlobals g)
        {
            own = owner;
            globals = g;
        }

        public void LoadMail(string mail)
        {
            string[] query = mail.Split('&');
            foreach(string n in query)
            {
                if (n != "none")
                {
                    Mail tmp = new Mail();
                    tmp.Load(n);
                    myMail.Add(tmp);
                }
            }
        }
        public string SaveMail()
        {
            string query = "";
            for (int a = 0; a < myMail.Count; a++)
            {
                query += myMail[a].id + " " + myMail[a].message + " " + myMail[a].type + " " + myMail[a].targetid;
                if (a < myMail.Count)
                    query += "&";
            }
            if (query == "")
                query += "none";
            return query;
        }

        public void SendTo(cCharacter t,string msg)
        {
            Mail a = new Mail();
            a.message = msg;
            a.id = own.characterID;
            a.targetid = t.characterID;
            a.type = "send";
            myMail.Add(a);
            cSendPacket p = new cSendPacket(globals);
            p.Header(14, 1);
            p.AddDWord(own.characterID);
            p.AddDouble(globals.GetTime());
            for (int n = 0; n < msg.Length; n++)
                p.AddByte((byte)msg[n]);
            p.SetSize();
            p.character = t;
            p.Send();
            t.Mail.Recvfrom(own,a.message);
        }
        public void Recvfrom(cCharacter t,string msg)
        {
            Mail a = new Mail();
            a.message = msg;
            a.id = own.characterID;
            a.targetid = t.characterID;
            a.type = "Recv";
            myMail.Add(a);
        }
        public List<Mail> GetmyMail()
        {
            return myMail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PServer_v2.NetWork.Registration
{

    class JOBBER
    {
        cGlobals g;
        public JOBBER(cGlobals g)
        {
            this.g = g;
        }
        public void ADD(int a, byte[] data,RegClient y)
        {
            string str = "";
            switch(a)
            {
                case 1: str = "VechData";break;
            }
            ADD(a, str,data,y);
        }
        public void ADD(int a, string Table, byte[] Dat,RegClient h)
        {
            int ptr = 2;
            Dictionary<string, string> tmp = new Dictionary<string, string>(); 
            switch (a)
            {
                case 1:
                    {
                        try
                        {
                            cSendPacket k = new cSendPacket(g);
                            k.Header(5, 1);
                            tmp.Add("ID", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            tmp.Add("imcost", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            tmp.Add("goldcost", g.GetDWord(Dat, ptr).ToString()); k.AddDWord(g.GetDWord(Dat, ptr)); ptr += 4;
                            tmp.Add("health", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            g.WloDatabase.Insert(Table, tmp);
                            k.SetSize();
                            k.rclient = h;
                            k.Send();
                        }
                        catch (Exception y)
                        {
                            cSendPacket l = new cSendPacket(g); l.Header(199); l.AddString(y.Message);
                            l.SetSize();
                            l.rclient = h; l.Send();
                        }
                    }break;
            }
        }

        public void Chng(int a, byte[] data,RegClient k)
        {
            string str = "";
            switch (a)
            {
                case 1: str = "VechData"; break;
            }
            Chng(a, str, data,k);
        }
        public void Chng(int a, string Table, byte[] Dat, RegClient h)
        {
            int ptr = 2;
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            switch (a)
            {
                case 1:
                    {
                        try
                        {
                            cSendPacket k = new cSendPacket(g);
                            k.Header(5, 2);
                            tmp.Add("ID", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            tmp.Add("imcost", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            tmp.Add("goldcost", g.GetDWord(Dat, ptr).ToString()); k.AddDWord(g.GetDWord(Dat, ptr)); ptr += 4;
                            tmp.Add("health", g.GetWord(Dat, ptr).ToString()); k.AddWord(g.GetWord(Dat, ptr)); ptr += 2;
                            g.WloDatabase.Update(Table, tmp,"ID = "+g.GetWord(Dat, 2).ToString());
                            k.SetSize();
                            k.rclient = h;
                            k.Send();
                        }
                        catch (Exception y)
                        {
                            cSendPacket l = new cSendPacket(g); l.Header(199); l.AddString(y.Message);
                            l.SetSize();
                            l.rclient = h; l.Send();
                        }
                    } break;
            }
        }

        public void Del(int a, byte[] data, RegClient n)
        {
            string str = "";
            switch (a)
            {
                case 1: str = "VechData"; break;
            }
            try{
                cSendPacket k = new cSendPacket(g);
                k.Header(5, 2);
                k.AddWord(g.GetWord(data, 2));
                k.SetSize();
                k.rclient = n;
                k.Send();
            g.WloDatabase.Delete(str,"ID ="+g.GetWord(data,2));
            }
            catch (Exception y)
            {
                cSendPacket l = new cSendPacket(g); l.Header(199); l.AddString(y.Message);
                l.SetSize();
                l.rclient = n; l.Send();
            }
        }
        public void GetInfo(int a, RegClient k)
        {
            try
            {
                cSendPacket t = new cSendPacket(g);
                t.Header(5, 5);
                cSendPacket tr = new cSendPacket(g);
                tr.Header(5, 6);
                var dat = g.WloDatabase.GetDataTable("select* from Groups where ID =" + a, true);
                t.AddString(dat.Rows[0]["Job"].ToString());
                tr.AddString(dat.Rows[0]["Members"].ToString());
                t.SetSize();
                tr.SetSize();
                t.rclient = k;
                tr.rclient = k;
                t.Send();
                tr.Send();
            }
            catch (Exception r) { g.cRegServer.Error(r.Message, k); }
            finally
            {
                cSendPacket d = new cSendPacket(g); d.Header(5, 0); d.SetSize(); d.rclient = k; d.Send();
            }
        }
    }
}

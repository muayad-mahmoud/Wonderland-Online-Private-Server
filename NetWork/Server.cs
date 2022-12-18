using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PServer_v2.NetWork;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2
{
    //public delegate void PacketRecv(cRecvPacket f,cCharacter charat);
    //public delegate void PacketSend(cSendPacket pd,cCharacter p);
    public struct Combinepkt
    {
       public cCharacter t;
       public List<byte> data;
    }
    public struct QCombinepkt
    {
        public cCharacter t;
        public List<byte> data;
    }

    public class cServer 
    {

        //public event PacketRecv PlayerDataRecv;
        private cListener listener = null;
        public Queue<cSendPacket> sendList;
        public cGlobals globals;
        List<Combinepkt> multipktlist = new List<Combinepkt>();
        List<QCombinepkt> Qmultipktlist = new List<QCombinepkt>();
        public delegate void OutputPacketCallback(string text);

        public cServer(cGlobals src)
        {
            globals = src;
        }
        ~cServer()
        {
            Disconnect();
        }
        public void Connect(int port,cGlobals g)
        {
            globals = g;
            listener = new cListener(port,g);
            listener.userAdded += new UserEventDlg(listener_userAdded);
            listener.Start();
            sendList = new Queue<cSendPacket>();
        }
        public void Disconnect()
        {
            if (listener != null)
            {
                sendList.Clear();
                listener.Stop();
                //remove all players
                globals.gCharacterManager.Clear();
            }
        }

        //Someone has connected
        private void listener_userAdded(object sender, cClient client)
        {
            globals.cRegServer.PushpplOnline();
            string str = "Character connected (" + client.GetEndpoint() + ")\r\n";
            globals.Log(str);
        }

        //Someone has disconnected
        public void player_UserDisconnected(object sender, cClient client)
        {
            globals.cRegServer.PushpplOnline();
            try
            {
                cCharacter character = client.character;
                
                if (character != null)//send to all others that this character has disconnected
                {
                    globals.gMapManager.RemoveByDC(character);
                    if (!character.logging)
                    {
                        globals.gCharacterManager.Send1_1(character);                        
                    }
                    for (int a = 0; a < globals.gCharacterManager.characterList.Count; a++)
                        globals.ac13.Send_4(character.characterID, globals.gCharacterManager.characterList[a]);
                    string str = ""; str += "Character disconnected ()\r\n";
                    globals.Log(str);
                    character.Disconnect();
                    globals.gUserManager.DisconnectUser(character.userID);
                    globals.gCharacterManager.Remove(character);
                    

                }
            }
            catch { }
        }

        //Someone has sent data to the server
        public void player_DataReceived(cClient sender, byte[] data)
        {
            
            if (data.Length < 5)
            {
                //need to disconnect here
                return;
            }
            globals.Log("player_DataRecieved");
            while (sender.character == null) { globals.Log("CATCH\rn"); System.Threading.Thread.Sleep(5); }
            Encode(ref data);
            //check to see if its a valid packet
            
            
            int at = 0;
            while (at < data.Length)
            {

                if ((data[at] != 244) || (data[at+1] != 68))
                {
                    //need to disconnect
                    globals.Log("heading is not 244,68\r\n");
                    return;
                }
                cRecvPacket p = new cRecvPacket(globals);
                UInt16 len = (UInt16)(data[at+2] + (data[at+3] << 8));
                at += 4;
                if (len > 20024) len = (UInt16)20024;
                p.data = new byte[len];
                Array.Copy(data, at, p.data, 0, len);
                p.a = p.data[0]; p.b = 0;
                if (len > 1) p.b = p.data[1];
                globals.Log(string.Join(",",p.data));
                sender.recvList.Enqueue(p);
                at += len;
            }
        }

        private void Encode(ref byte[] data)
        {
            for (int n = 0; n < data.Length; n++)
            {
                data[n] = (byte)(173 ^ data[n]);
            }
        }

        

        
        //========================================
        // Specific Protocol Functions
        //========================================

        public void SendCombinepkt(cCharacter p)
        {
            if (p.Combinepkt != null)
            {
                p.MultiMode = false;
                p.Combinepkt.character = p;
                p.Combinepkt.Send();
                p.Combinepkt = null;
            }
        }
        public cSendPacket GenerateQueuepkt(cCharacter p)
        {
            cSendPacket g = new cSendPacket(globals);
            QCombinepkt t = new QCombinepkt();
            foreach (QCombinepkt i in Qmultipktlist)
            {
                if (i.t == p)
                {
                    p.QueueMode = false;
                    t = i;  
                    g.data = i.data;
                    g.character = i.t;
                    
                }
            }
            if (t.t != null)
                Qmultipktlist.Remove(t);
            return g;
        }
        public void Multipkt_Request(cCharacter p)
        {
            p.Combinepkt = new cSendPacket(globals);
            p.MultiMode = true;
        }
        public void Queue_Request(cCharacter p)
        {
            p.QueueMode = true;
            QCombinepkt u = new QCombinepkt();
            u.t = p;
            u.data = new List<byte>();
            Qmultipktlist.Add(u);
        }
        public void Send(cSendPacket pd)
        {
            if (pd.data[2] == 0)//packet error check
                return;
            
            if (pd.character.QueueMode)
            {
                for (int a = 0; a < Qmultipktlist.Count; a++)
                {
                    if (Qmultipktlist[a].t == pd.character)
                    {
                        Qmultipktlist[a].data.AddRange(pd.data);
                        break;
                    }
                }
            }
            else if (pd.character.MultiMode)
            {
                pd.character.Combinepkt.AddArray(pd.data.ToArray());
            }
            else
                sendList.Enqueue(pd);
        }
        public void Send(cSendPacket pd, PServer_v2.NetWork.Registration.RegClient p)
        {
                sendList.Enqueue(pd);
        }
        /*public void Send2(cSendPacket pd, cCharacter p)
        {
            if (p.client != null)
            {
                if (p.client.disconnected) return;
                
                p.client.sendList.Enqueue(pd.data.ToArray());
                //if setting p.disconnect on a packet, be sure to save that packet in the function setting disconnect
                if (pd.disconnect)
                {
                    p.userID = 0; p.characterID = 0;
                    p.Disconnect();
                }
            }
        }
        public void Send3(byte[] pd, cCharacter p)
        {
            if (p.client != null)
            {
                if (p.client.disconnected) return;

                p.client.sendList.Enqueue(pd);
            }
        }

        public void DirectSend(cSendPacket pd, cCharacter p)
        {
            var t = new System.Net.Sockets.SocketError();
            p.client.sendList.Enqueue(pd.data.ToArray());
        }*/

    }
}
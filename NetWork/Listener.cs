using System;
using System.Net;
using System.Net.Sockets;
using PServer_v2.NetWork;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2
{
    public class cListener
    {
        private TcpListener listener;
        cGlobals globals;
        //When we've received a User we send an event
        public event UserEventDlg userAdded;

        public cListener(int portNr, cGlobals t)
        {
            globals = t;
            //Same way as the kickstart
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            this.listener = new TcpListener(IPAddress.Any, portNr);
            globals.Log("Server started... waiting for clients.\r\n");
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
            while (globals.gCharacterManager.GetCount() >= globals.Limit)
            {
                System.Threading.Thread.Sleep(1);
            }
                cClient myClient = new cClient();
                myClient.DataReceived += new DataReceivedDlg(globals.gServer.player_DataReceived);
                myClient.UserDisconnected += new UserEventDlg(globals.gServer.player_UserDisconnected);
                cCharacter character = new cCharacter(globals);
                myClient.character = character;
                character.client = myClient;
                try
                {
                    Socket client = this.listener.EndAcceptSocket(ar);


                    character.client.Start(client);
                    globals.gCharacterManager.Add(character);

                    if (this.userAdded != null)
                        this.userAdded(this, character.client);
                }
                catch { }
            
            this.ListenForNewClient();
        }
    }
}
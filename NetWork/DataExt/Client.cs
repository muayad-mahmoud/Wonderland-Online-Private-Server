using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.DataExt
{
    public class cClient
    {
        object locker;
        public cCharacter character;
        public Socket client;
        private byte[] readBuffer;
        public Queue<cRecvPacket> recvList = new Queue<cRecvPacket>();
        public Queue<cSendPacket> sendList = new Queue<cSendPacket>();
        //Events to call when the client received data or got disconnected
        public event UserEventDlg UserDisconnected;
        public event DataReceivedDlg DataReceived;

        public bool disconnected = false;

        internal cClient(Socket client) //called from listener
        {
            Init();
            this.client = client;
            this.client.NoDelay = true;
            this.StartListening();
        }
        public cClient()
        {
            Init();
        }
        public void Start(Socket client)
        {
            this.client = client;
            this.client.NoDelay = true;
            this.StartListening();
        }
  
        ~cClient()
        {
            this.Disconnect();
        }

        private void Init()
        {
            locker = client;
            character = null;            
        }

        public void Disconnect()
        {
            if (!this.disconnected)
            {
                this.disconnected = true;
                if (client == null)
                    return;
                if (this.client.Connected)
                {
                    this.client.Close(500);
                }
                else
                    this.client.Close();

                //When we've disconnected we fire an event to tell the program that we've got disconnected.
                if (this.UserDisconnected != null)
                    this.UserDisconnected(null, this);
            }
        }

        private void StartListening()
        {
            if (!Ping())
            {
                Disconnect();
                return;
            }
            readBuffer = new byte[client.Available];
            this.client.BeginReceive(this.readBuffer, 0, readBuffer.Length,SocketFlags.None, this.StreamReceived, null);
        }

        private void StreamReceived(IAsyncResult ar)
        {
            //Note: This metod is not executed on the same thread as the one that called BeginRead
            //This means that you have to be careful when you modify data in the DataReceived-event
            int bytesRead = 0;
            try
            {
                //Lock the stream to prevent objects from other threads to acess it at the same time
                //Then call EndRead(ar) where ar is the IAsyncResult the TcpClient sent to us.
                //This will return the number of bytes that has been received
                    bytesRead = this.client.EndReceive(ar);
            }
            catch (Exception)
            { } //If we get an exception bytesread will remain 0 and we will disconnect.

            //If bytesRead is 0 we have lost connection to the server,          

            //Create a new buffer with just enough space to fit the received data
            byte[] data = new byte[bytesRead];

            //Copy the data from the readBuffer to data.
            //The reason why we do this instead of sending the entire readBuffer with the DataReceived-event is
            //that we want to start listening for new messages asap. But if we start listening before we call the
            //DataReceived-event and new data is received before the DataReceived-method has finished the 
            //readBuffer will change and the DataReceived-method will process completly corrupt data.
            for (int i = 0; i < bytesRead; i++)
                data[i] = readBuffer[i];

            //Start listening for new data
            this.StartListening();

            //Tell everyone that we've received data.
            if (this.DataReceived != null)
                this.DataReceived(this, data);
        }


        //public void SendData(byte[] b)
        public bool Ping()
        {
            return SocketConnected(this.client);
        }
        public bool SocketConnected(Socket s)
        {
            bool ret = false;
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                ret = false;
            else
            {
                ret = true;
            }
            return ret;
        }
        private void Encode(ref byte[] data)
        {
            for (int n = 0; n < data.Length; n++)
            {
                data[n] = (byte)(173 ^ data[n]);
            }
        }
        public void SendData(byte[] b)
        {
            //System.Threading.Thread.Sleep(500);
            int len = b.Length;
            try
            {
                Encode(ref b);
                //Lock the stream and send away data asyncrounously
                //We can register a callback-method to be called when the data has been received like we did with 
                //BeginRead but in this example there is no need to know that the data has
                //been sent so we set it to null.
                //
                //BeginWrite can throw an exception when the client has been disconnected or when you
                //pass a buffer with incorrect size, we only catch the ones that occur when you've got disconnected
                //lock (client.GetStream())
                //

                this.client.BeginSend(b, 0, b.Length, SocketFlags.None, null, client);

            }
            catch (System.IO.IOException)
            {
                this.Disconnect();
            }
            catch (InvalidOperationException)
            {
                this.Disconnect();
            }
            catch (Exception e)
            {
                this.Disconnect();
            }


        }

        /// <summary>
        /// Copies the data in the memorystream from position 0 to the memorystreams current position and
        /// then sends the data away.
        /// </summary>
        /// <param name="ms"></param>
        public void SendMemoryStream(System.IO.MemoryStream ms)
        {
            lock (ms)
            {
                //We will probably have to do this 100 times in our mainprogram to get a bytebuffer
                //from a memorystream so we can just as well put a method for it in here
                //Don't bother about this method untill you've read the packaging-section
                int bytesWritten = (int)ms.Position;
                byte[] result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
                //this.SendData(result);
            }
        }
        public EndPoint GetEndpoint()
        {
            return client.RemoteEndPoint;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace PServer_v2.NetWork.Registration
{
    
    public class RegClient
    {
        Thread Recv;
        cGlobals globals;
        Socket socket;
        bool dc = false;
        byte[] buffer;
        public event EventHandler<DataRcv> DataReceived;
        public event EventHandler<UserDC> UserDisconnected;
        public RegClient(cGlobals src)
        {
            globals = src;            
        }
        public void Start(Socket src)
        {
            buffer = new byte[globals.READBUFFERSIZE];
            Recv = new Thread(new  ParameterizedThreadStart(Recieve));
            Recv.Name = "RegClient-" + globals.cRegServer.clientList.Count + 1;
            Recv.Start(src);
        }

        void Recieve(object src)
        {
            socket = src as Socket;

            while (!dc)
            {
                if (!globals.SocketConnected(socket))
                {
                    Disconnect();
                    break;
                }
                if (socket.Available > 0)
                {
                    buffer = new byte[socket.Available];
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recved), socket);
                }
                Thread.Sleep(500);
            }
        }
        void Recved(IAsyncResult iar)
        {
            var cli = iar.AsyncState as Socket;
            if (buffer.Length > 0)
            {
                globals.Log("RegClient"+cli.RemoteEndPoint.ToString()+" _DataRecieved\r\n");
            //check to see if its a valid packet
            if (buffer.Length < 5)
            {
                //need to disconnect here
                globals.Log("data.length < 5\rn");
                return;
            }

            int at = 0;
            while (at < buffer.Length)
            {

                if ((buffer[at] != 244) || (buffer[at+1] != 68))
                {
                    //need to disconnect
                    globals.Log("heading is not 244,68\r\n");
                    return;
                }
                cRecvPacket p = new cRecvPacket(globals);
                p.rclient = this;
                UInt16 len = (UInt16)(buffer[at+2] + (buffer[at+3] << 8));
                at += 4;
                if (len > globals.READBUFFERSIZE) len = (UInt16)globals.READBUFFERSIZE;
                p.data = new byte[len];
                Array.Copy(buffer, at, p.data, 0, len);
                p.a = p.data[0]; p.b = 0;
                if (len > 1) p.b = p.data[1];
                globals.Log(string.Join(",",p.data));
                DataReceived(this,new DataRcv(p));
                at += len;
            }
        }
            }

        public void Send(cSendPacket e)
        {
            Thread.Sleep(5);
            lock (this)
            {
                try
                {
                    socket.BeginSend((e.data.ToArray<byte>()),0,e.data.ToArray().Length,SocketFlags.None,null,null);
                }
                catch (Exception r) { System.Windows.Forms.MessageBox.Show(r.Message); }
            }
        }
        public void Disconnect()
        {
            dc = true;
            socket.Shutdown(SocketShutdown.Both);
            UserDisconnected(this, new UserDC(this));
            socket.Close();            
        }
    }
}

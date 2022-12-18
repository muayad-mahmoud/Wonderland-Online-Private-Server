using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork
{
    public class DataRcv : EventArgs
    {
        public cRecvPacket g;
        public DataRcv(cRecvPacket src)
        {
            g = src;
        }
    }
    public class UserDC : EventArgs
    {
        public PServer_v2.NetWork.Registration.RegClient g;
        public UserDC(PServer_v2.NetWork.Registration.RegClient src)
        {
            g = src;
        }
    }

    public delegate void UserEventDlg(object sender, cClient player);
    public delegate void DataReceivedDlg(cClient sender, byte[] data);
}

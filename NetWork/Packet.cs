using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork
{
    public class cRecvPacket
    {
        public byte[] data;
        public byte a;
        public byte b;
        public cCharacter character;
        public Registration.RegClient rclient;
        public cGlobals globals;

        public cRecvPacket(cGlobals src)
        {
            globals = src;
        }
        public byte GetByte(int at)
        {
            return data[at];
        }
        public UInt16 GetWord(int at)
        {
            UInt16 v = (UInt16)(data[at] + (data[at + 1] << 8));
            return v;
        }
        public UInt32 GetDWord(int at)
        {
            UInt32 v = (UInt32)(data[at] + (data[at + 1] << 8) + (data[at + 2] << 16) + (data[at + 3] << 24));
            return v;
        }
        public UInt64 GetLong(int at)
        {
            UInt64 v = BitConverter.ToUInt64(data, at);
            return v;
        }
        public string GetString(int at)
        {
            int len = data[at];
            char[] c = new char[len];
            Array.Copy(data, at + 1, c, 0, len);
            string str = new string(c);
            return str;
        }
        public string GetStringRaw(int at, int len)
        {
            char[] c = new char[len];
            Array.Copy(data, at, c, 0, len);
            string str = new string(c);
            return str;
        }

    }
    public class cSendPacket
    {
        public cCharacter character;
        public Registration.RegClient rclient;
        public int index = 0;
        public List<byte> data;
        public bool disconnect;
        public cGlobals globals;
        public cSendPacket(cGlobals src)
        {
            globals = src;
            Clear();
        }
        void Clear()
        {
            index = 0;
            data = new List<byte>();
            disconnect = false;
        }
        public void Send()
        {
            if (rclient != null)
                globals.gServer.sendList.Enqueue(this);
            if (character != null)
                globals.gServer.Send(this);
        }
        public void Header(byte ac, byte subac)
        {
            if (index > 0) Clear();
            data.AddRange(new byte[] { 244, 68, 0, 0 }); index = 4;
            data.Add((byte)ac); index++;
            data.Add((byte)subac); index++;
        }
        public void Header(byte ac)
        {
            if (index > 0) Clear();
            data.AddRange(new byte[] { 244, 68, 0, 0 }); index = 4;
            data.Add((byte)ac); index++;
        }

        public void AddByte(byte v) { data.Add(v); index++; }
        public void AddWord(UInt16 v)
        {
            data.Add((byte)v);
            data.Add((byte)(v >> 8));
            index += 2;
        }
        public void AddDWord(UInt32 v)
        {
            data.Add((byte)v);
            data.Add((byte)(v >> 8));
            data.Add((byte)(v >> 16));
            data.Add((byte)(v >> 24));
            index += 4;
        }
        public void AddTime(long v)
        {
            var v100 = new DateTime(2013, 4, 4, 11, 59, 38).Ticks;
            v = 0;
            int cr1 = 0;
            int cr2 = 0;
            int cr3 = 0;
            int cr4 = 0;
            int cr5 = 0;
            int cr6 = 0;
            int cr7 = 0;
            int cr8 = 0;
            long v1 = 8500000000;
            long v2 = 620000000000;//420000000;
            long v3 = 35000000000000;//108000000000;
            long v4 = 9500000000000000;
            long v5 = 32200000;
            long v6 = 54500;
            long v7 = 247;
            long v8 = 1;
            while (v <= v4)
            {
                v -= v4;
                cr4++;
            }
            while (v <= v3)
            {
                cr3++;
                v -= v3;
            }
            while (v <= v2)
            {
                cr2++;
                v -= v2;
            }
            while (v <= v1)
            {
                cr1++;
                v -= v1;
            }
            while (v <= v5)
            {
                cr5++;
                v -= v5;
            }
            while (v <= v6)
            {
                cr6++;
                v -= v6;
            }
            while (v <= v7)
            {
                cr7++;
                v -= v7;
            }
            while (v <= v8)
            {
                cr8++;
                v -= v8;
            }

            data.Add((byte)cr1);
            data.Add((byte)cr2);
            data.Add((byte)cr3);
            data.Add((byte)cr4);

            index += 4;
        }
        public void AddDouble (double v)
        {
            var e = BitConverter.GetBytes(v);
            data.AddRange(e);
            index += e.Length;
        }
        public void AddString(string str)
        {
            data.Add((byte)str.Length); index++;
            for (int n = 0; n < str.Length; n++)
                data.Add((byte)str[n]);
            index += str.Length;
        }
        public void AddArray(byte[] a)
        {
            if (a != null)
            {
                for (int n = 0; n < a.Length; n++)
                    data.Add((byte)a[n]);
                index += a.Length;
            }
        }
        public void AddArray(char[] a)
        {
            if (a != null)
            {
                for (int n = 0; n < a.Length; n++)
                    data.Add((byte)a[n]);
                index += a.Length;
            }
        }
        public void SetSize()
        {
            UInt16 size = (UInt16)index;
            if (size < 4) size = 4;
            size -= 4;
            data[2] = (byte)size;
            data[3] = (byte)(size >> 8);
        }
        public int GetSize()
        {
            return (int)index;
        }
    }
}

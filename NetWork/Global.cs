using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.DataBase;
using System.Net.Sockets;
using PServer_v2.NetWork.Managers;
using PServer_v2.NetWork.DataExt;
using PServer_v2.DataLoaders;
using PServer_v2.NetWork.ACS;
using System.Diagnostics;

namespace PServer_v2.NetWork
{
    public class cGlobals
    {
        public Stopwatch UpTime;
        public int READBUFFERSIZE;
        public cUserManager gUserManager;
        public System.Windows.Forms.DataGridView IM_db;
        public System.Windows.Forms.DataGridView hotdb;
        public System.Windows.Forms.DataGridView armdb;
        public System.Windows.Forms.DataGridView weapdb;
        public System.Windows.Forms.DataGridView grodb;
        public System.Windows.Forms.DataGridView furdb;
        public String LoginMsg = "";
        public int Limit = 0;
        public cCharacterManager gCharacterManager;
        public cMapManager gMapManager;
        public GroundMMGDataFile gGroundData;
        public cSkillDat gskillManager;
        public NpcDat gNpcManager;
        public cItemManager gItemManager;
        public ImMall_Manager gImMall_Manager;
        public SceneDataFile gSceneManager;
        public EveManager gEveManager;
        public Registration.RegServer cRegServer;
        public Queue<string> logList;
        public Queue<int> interfaceList;
        public cServer gServer;
        public cRecvPacket packet;
        public cDatabase WloDatabase;
        public cDatabase UserDataBase;
        public char[] ItemDatSize = "2124136".ToCharArray();
        public UInt16 VersionNum = 1093;


        public cAC_0 ac0;
        public cAC_1 ac1; //server info at login
        public cAC_2 ac2;
        //public cAC_3 ac3; //my data setn alogin, other player data sent in game
        public cAC_4 ac4; //other player data sent in login
        public cAC_5 ac5; //char info?
        public cAC_6 ac6; //movement... walking
        public cAC_7 ac7; //same as an ac12 but coming to player entering warp, from others in map
        public cAC_8 ac8; //used to send updated stat information to player
        public cAC_9 ac9; //new char creation stuff
        public cAC_10 ac10; //???
        public cAC_11 ac11; //initiating battles, and other stuff
        public cAC_12 ac12; //warping
        public cAC_13 ac13; //??
        public cAC_14 ac14; //??
        public cAC_15 ac15; //??
        public cAC_20 ac20; //Init a warp
        //public cAC_22 ac22; //??
        public cAC_23 ac23; //items
        public cAC_24 ac24; //used in deleteing a char
        public cAC_25 ac25; //??
        public cAC_26 ac26; //sets gold???
        public cAC_27 ac27;//props shopkeep related
        public cAC_29 ac29; //props keep related
        //public cAC_30 ac30; //??
        public cAC_31 ac31; // npc record related
        public cAC_32 ac32; //social emotes
        public cAC_33 ac33; //??
        public cAC_34 ac34; //IM related(CHEAT PROTECTION)
        public cAC_35 ac35; //delete character
        //public cAC_37 ac37; //ip sent from client
        //public cAC_39 ac39; //???
        public cAC_40 ac40; //??
        public cAC_41 ac41; //??
        //public cAC_45 ac45; //??
        public cAC_50 ac50; //battles
        public cAC_51 ac51;// has to do with battles
        public cAC_52 ac52;// has to do with battles
        public cAC_53 ac53; //battles
        public cAC_54 ac54; //server info at login
        public cAC_62 ac62; //???
        public cAC_63 ac63; //logging in
        //public cAC_65 ac65; //tent packets
        public cAC_69 ac69; // tool warp
        public cAC_66 ac66; //??
        public cAC_70 ac70; //??
        public cAC_75 ac75; //item mall stuff
        //public cAC_89 ac89; //??
        public cAC_90 ac90; //??*/

        public void Log(string str)
        {
            logList.Enqueue(str);
        }
        public void Interface(int action)
        {
            interfaceList.Enqueue(action);
        }

        #region Extra Funtions
        public UInt32 GetDWord(byte[] data, int at)
        {
            UInt32 v = (UInt32)(data[at] + (data[at + 1] << 8) + (data[at + 2] << 16) + (data[at + 3] << 24));
            return v;
        }
        public UInt16 GetWord(byte[] data, int at)
        {
            UInt16 v = (UInt16)(data[at] + (data[at + 1] << 8));
            return v;
        }
        public string GetString(byte[] data, int at)
        {
            byte len = data[at]; at++;
            char[] c = new char[len];
            Array.Copy(data, at, c, 0, len);
            string str = new string(c);
            return str;
        }
        public void SetWord(UInt16 value, ref byte[] data, int at)
        {
            data[at] = (byte)value;
            data[at + 1] = (byte)(value >> 8);
        }
        public void SetWord(UInt32 value, ref byte[] data, int at)
        {
            data[at] = (byte)value;
            data[at + 1] = (byte)(value >> 8);
            data[at + 2] = (byte)(value >> 16);
            data[at + 3] = (byte)(value >> 24);
        }
       public  int MatrixtoNumber(int a, int b) { return ((a * 5) + (b - 5)); }
        public byte[] NumbertoMatrix(int a)
        {
            var s =0;
            if (a == 5 ||a == 10 ||a == 15 ||a == 20 ||a == 25 ||a == 30 ||a == 35 ||a == 40 ||a == 45 ||a == 50)
                s = (a/5);
            else
                s = 1 + (a / 5);
             var t=0;
             if (a == 5 || a == 10 || a == 15 || a == 20 || a == 25 || a == 30 || a == 35 || a == 40 || a == 45 || a == 50)
                 t = 5;
             else if (a > 5)
                 t = 5 - (((1+(a / 5)) * 5) - a);
             else
                 t = a;
            byte[] matrixloc = new byte[2];
            matrixloc[0] = (byte)(s);
            matrixloc[1] = (byte)(t);
            return matrixloc;
        }
        public bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
                return false;
            else
                return true;
        }

        public double GetTime()
        {
            //DateTime.UtcNow
            return DateTime.Now.ToOADate();
            //return (UInt64)date.Ticks;
        }
        #endregion
    }
}

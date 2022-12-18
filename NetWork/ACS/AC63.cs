using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.ACS
{
    public class cAC_63 : cAC
    {
        public cAC_63(cGlobals globals)
            : base(globals)
        {

        }
        public void SwitchBoard()
        {
            g.packet = g.packet;
            switch (g.packet.b)
            {
                case 0:
                    {
                        Recv_0();
                    } break;
                case 2:
                    {
                        Recv_2();
                    } break;
                case 3:
                    {
                        Recv_3();
                    }break;
                case 4:
                    {
                        Recv_4();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + g.packet.a + ", " + g.packet.b + " [unhandled]\r\n";
                        g.logList.Enqueue(str);
                    } break;

            }
        }

        public void Recv_0() //user moved from char selection back to enter name , pw
        {
            g.gUserManager.DisconnectUser(g.packet.character.userID);
            //g.gCharacterManager.Remove(g.packet.character);
        }
        public void Send_1(cUser user)
        {
            user.charname1 = "";
            user.charname2 = "";
            cCharacter c1 = g.gCharacterManager.GetDBByID(user.playerID);
            cCharacter c2 = g.gCharacterManager.GetDBByID(user.player2ID);
            byte[] data1 = null;
            byte[] data2 = null;
            if (c1 != null) { c1.slot = 1; data1 = c1.Get_63_1_Data(g); user.charname1 = c1.name; }
            if (c2 != null) { c2.slot = 2; data2 = c2.Get_63_1_Data(g); user.charname2 = c2.name; }

            cSendPacket p = new cSendPacket(g);
            p.Header(63, 1);
            if (data1 != null) p.AddArray(data1);
            if (data2 != null) p.AddArray(data2);
            p.SetSize();
            p.character = g.packet.character;
            p.Send();
            
        }
        public void Send_2(UInt32 value)
        {
            cSendPacket sp = new cSendPacket(g);// PSENDPACKET PackSend = new SENDPACKET;
            //PackSend->Clear();
            sp.Header(63, 2);//PackSend->Header(63,2);
            sp.AddDWord(value);//PackSend->AddDWord(id);
            sp.SetSize();//PackSend->SetSize();
            sp.character = g.packet.character;//PackSend->Character = pArg->Packet->Character;
            sp.Send();//pArg->SQueue->EnqueuePacket(PackSend);
        }

        public void Recv_2() //selected a character
        {
            byte charNum = g.packet.data[2];
            if ((charNum < 1) || (charNum > 2))
            {
                Send_2(0);
                g.ac1.Send_6();
                return;
            }
            cUser u = g.gUserManager.GetListByID(g.packet.character.userID);
            if (u==null) 
             {
                Send_2(0);
                g.ac1.Send_6();
                return;
            }

            g.packet.character.slot = charNum;
            if (charNum == 1)
            {
                if (u.playerID == 0) // char is not created
                {
                    g.packet.character.creating = true;
                    g.ac1.Send_3(0); //go to create screen
                }
                else
                {
                    g.gCharacterManager.GetDBByID(g.packet.character, u.userID);
                    NormalLog(g.packet.character);
                }
            }
            else
            {
                if (u.player2ID == 0) //char is not created
                {
                    g.packet.character.creating = true;
                    g.ac1.Send_3(0); //go to create screen
                }
                else
                {
                    g.gCharacterManager.GetDBByID(g.packet.character, u.player2ID);
                    g.packet.character.inv.Load(g.packet.character.characterID);
                    NormalLog(g.packet.character);
                }
            }
        }

        public void Recv_3() //left char creation screen, release name for others to use
        {
	        if (g.packet.character.logging)
	        {
                cUser u = g.gUserManager.GetListByID(g.packet.character.userID);
                if (u == null)
                {
                    Send_2(0);
                    g.ac1.Send_6();
                    return;
                }
                if (g.packet.character.slot == 1) u.charname1 = "";
                else u.charname2 = "";
                g.packet.character.creating = false;
	        }
        }

        public void Recv_4() //user enteered username and password
        {
            int loginState = 0; //0-good login  1-bad un/pw  2-dup log 3-wrong version 4-need update
            cUser user = null;

            //sending username and password
            int at = 2;

            string name = g.packet.GetString(at); at += name.Length + 1;
            string password = g.packet.GetString(at); at += password.Length + 1;
            UInt16 version = g.packet.GetWord(at); at += 2;
            byte lcLen = g.packet.data[at]; at++;
            byte key = g.packet.data[at]; at++;
            char[] lCode = new char[20];
            Array.Copy(g.packet.data, at, lCode, 0, lcLen);
            for (int n = 0; n < lcLen; n++)
                lCode[n] = (char)((byte)lCode[n] ^ (byte)key);

            if ((name.Length < 4) || (name.Length > 14))
            {
                g.Log("1");
                loginState = 1;
                //bad name len
                //Send_2(0);//g.ac63.send_2();//Send_63_2(pArg,0);
                //g.ac1.Send_6();//Send_1_6(pArg);
                //return;
            }
            else if ((password.Length < 4) || (password.Length > 14))
            {
                g.Log("2");
                loginState = 1;
            }

            else if (version != g.VersionNum)
            { //bad aloign version
                loginState = 0;
            }
            else if ((lcLen < 2) || (lcLen > 15))
            { //bad login code length
                loginState = 0;
            }
            //check if correct value
            else if (lcLen != g.ItemDatSize.Length) //not correct version
            {
                loginState = 4;
            }
            else
            {
                for (int n = 0; n < lcLen; n++)
                {
                    if (lCode[n] != g.ItemDatSize[n]) //not correct version
                    {
                        loginState = 4;
                        break;
                    }
                }

            }


            //at this point we have no use, and only an empty character object
            //check to see if we are still in the clear for loggin ing
            if (loginState == 0)
            {
                //should have correct version if at this point
                //check username and password
                user = g.gUserManager.GetDBByUnPw(name, password);
                if (user == null)
                {
                    g.Log("3");
                    loginState = 1;
                }
                else //success
                {
                    //need to check if this user is already logged in
                    cUser temp = g.gUserManager.GetListByID(user.userID);
                    if (temp != null) //already logged in
                    {
                        loginState = 2;
                        //g.Log("Already logged in. (" + temp.username + ")\r\n");// << endl;
                        //Send_2(user.userID);
                        //user.userID = 0;
                        //user.playerID = 0;
                        //user.player2ID = 0;
                        //g.ac0.Send_19();
                        //return;
                    }
                }
            }

            // here we do the results of loginstate
            switch (loginState)
            {
                case 0:
                    {
                        g.Log("4");
                        g.packet.character.userID = user.userID;
                        g.gUserManager.Add(user);
                        Send_2(g.packet.character.userID);
                        g.ac35.Send_11();
                        Send_1(user);
                    } break;
                case 1:
                    {
                        g.Log("5");
                        Send_2(0);//g.ac63.send_2();//Send_63_2(pArg,0);
                        g.ac1.Send_6();//Send_1_6(pArg);
                    } break;
                case 2:
                    {
                        g.Log("6");
                        g.Log("Already logged in. (" + user.username + ")\r\n");
                        Send_2(user.userID);
                        user.userID = 0;
                        user.playerID = 0;
                        user.player2ID = 0;
                        g.ac0.Send_19();
                    } break;
                case 3:
                    {
                        g.Log("7");
                        Send_2(0);//g.ac63.send_2();//Send_63_2(pArg,0);
                        g.ac1.Send_6();//Send_1_6(pArg);
                    } break;
                case 4:
                    {
                        g.Log("8");
                        Send_2(0);//g.ac63.send_2();//Send_63_2(pArg,0);
                        g.ac1.Send_6();//Send_1_6(pArg);
                    } break;
            }





            /*
            cData2 dat = new cData2();
            cSendPacket p = new cSendPacket(g);
            p.AddArray(dat.data1);
            p.character = rp.character;
            p.Send();
            */
        }

        public void NormalLog(cCharacter c)
        {
            if (c.characterID == 0) return;
            //a connection request was recieved
            g.ac20.Send_8(c);
            g.ac24.Send_5(183);
            g.ac24.Send_5(53);
            g.ac24.Send_5(52);
            g.ac24.Send_5(54);
            g.ac70.Send_1(23, "Something", 194);
            g.ac20.Send_33(0);
            //--------------------------Player PreInfo
            c.Send_8_1();
            g.ac14.Send_13(3);            
            //------------------------Im Mall List
            byte[] mock = g.gImMall_Manager.Get_75IM.ToArray();
            g.ac75.Send_1(mock);
            g.ac75.Send_8(0);
            //------------------------
            g.packet.character.Login();
            //put me in my maps list
            //pets
            //-----------------------------------            
            g.ac40.Send_1(g.packet.character);   //sidebar         
            //---------Warp Info---------------------------------------------------
            g.packet.character.warping = true;

            g.packet.character.map.WarpRequest( WarpType.Login,g.packet.character);
            g.packet.character.logging = false;
            g.ac5.Send_15(0);//244, 68, 3, 0, 5, 15, 0, 
            g.ac62.Send_53(2); //244, 68, 4, 0, 62, 53, 2, 0, 
            g.ac5.Send_21(1);//244, 68, 3, 0, 5, 21, 1, 
            //g.ac5.Send_11(15085, 0);//244, 68, 8, 0, 5, 11, 237, 58, 0, 0, 0, 0,         

            //---------------------------------

            //g.ac62.Send_4(g.packet.character.characterID); //tent items
            g.packet.character.Friends.RecvList();
            //--------------------------------------
            g.ac5.Send_14(2);
            g.ac5.Send_16(2);
            var time =g.GetTime();
            g.ac23.Send_140(3,time);
            g.ac25.Send_44(2, time);
            //g.ac23.Send_106(1, 1);
            g.ac23.Send_160(3);
            g.ac75.Send_7(1);
            g.ac23.Send_57(g.LoginMsg, g.packet.character);
            for (byte a = 1; a < 10; a++)
                g.ac5.Send_13(a, 0);
            for (byte a = 1; a < 10; a++)
                g.ac5.Send_24(a, 0);
            g.ac23.Send_162(2);
            g.ac26.Send_10(0);
            g.ac23.Send_204(1);
            //g.ac23.Send_208(2, 3, 0);
            //g.ac23.Send_208(2, 4, 0);
            g.ac1.Send_11();
            g.ac15.Send_19(0);
            //--------------------------------
 
            g.packet.character.Friends.Login();
            g.ac54.Send2();
            g.ac35.Send_4(c.IM);// IM Points 
            g.ac90.Send_1(768);//244, 68, 4, 0, 90, 1, 0, 5,
               
            
            g.ac5.Send_4();
            g.packet.character.warping = false;
            //---------------------------------
        }
    }
}


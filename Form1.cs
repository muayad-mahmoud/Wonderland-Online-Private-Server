using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PServer_v2.NetWork;
using PServer_v2.NetWork.Managers;
using System.Diagnostics;
using PServer_v2.NetWork.ACS;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2
{
    public partial class Form1 : Form
    {
        TimeSpan IMupdate = new TimeSpan();
        TimeSpan PointGiving = new TimeSpan();
        TimeSpan Exp = new TimeSpan();
        System.Threading.Thread IMthr;
        cGlobals globals;
        public Form1()
        {            
            globals = new cGlobals();
            InitializeComponent();
            globals.UserDataBase = new DataBase.cDatabase("C:\\pServer\\data\\PServer.db");
            globals.logList = new Queue<string>();
            globals.interfaceList = new Queue<int>();
            globals.READBUFFERSIZE = 2024;
            globals.UpTime = new Stopwatch();            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            globals.UpTime.Restart();            
            globals.cRegServer = new NetWork.Registration.RegServer(globals);
            globals.gServer = new cServer(globals);
            globals.gGroundData = new DataLoaders.GroundMMGDataFile();
            globals.gGroundData.LoadGroundNodes();
            globals.gServer.sendList = new Queue<cSendPacket>();
            globals.gMapManager = new cMapManager(globals);
            globals.gUserManager = new cUserManager(globals);
            globals.WloDatabase = new DataBase.cDatabase("C:\\pServer\\data\\WonderlandPServer.s3db");
            globals.gCharacterManager = new cCharacterManager(globals);
            globals.gSceneManager = new DataLoaders.SceneDataFile(globals);
            globals.gEveManager = new DataLoaders.EveManager(globals);
            globals.gItemManager = new DataLoaders.cItemManager(globals);
            globals.IM_db = IMDB;
            globals.hotdb = HotIMView1;
            globals.armdb = armView;
            globals.weapdb = weapView;
            globals.grodb = grocView;
            globals.furdb = furnView;

            globals.ac0 = new cAC_0(globals);
            globals.ac1 = new cAC_1(globals);  //server info at login
            globals.ac2 = new cAC_2(globals);
            //globals.ac3 = new cAC_3(globals);  //my data setn alogin, other player data sent in game
            globals.ac4 = new cAC_4(globals);  //other player data sent in login
            globals.ac5 = new cAC_5(globals);  //[Client->Server] Spawn to Starter's Beach // [Server->Client] (character info)
            globals.ac6 = new cAC_6(globals);  //movement... walking
            globals.ac7 = new cAC_7(globals);
            globals.ac8 = new cAC_8(globals);  //used to send updated stat information to player
            globals.ac9 = new cAC_9(globals);  //new char creation stuff
            globals.ac10 = new cAC_10(globals);  //???
            globals.ac11 = new cAC_11(globals); //??
            globals.ac12 = new cAC_12(globals); //warping
            globals.ac13 = new cAC_13(globals); //??
            globals.ac14 = new cAC_14(globals); //??
            globals.ac15 = new cAC_15(globals); //??
            globals.ac20 = new cAC_20(globals); //Init a warp
            //globals.ac22; //??
            globals.ac23 = new cAC_23(globals); //items/Time/Warptools
            globals.ac24 = new cAC_24(globals); //used in deleteing a char
            globals.ac25 = new cAC_25(globals); //??
            globals.ac26 = new cAC_26(globals); //sets gold???
            globals.ac27 = new cAC_27(globals); // props shopkeeper related
            globals.ac29 = new cAC_29(globals); // props keeper related
            globals.ac30 = new cAC_30(globals); //??
            globals.ac31 = new cAC_31(globals);//npc record related
            globals.ac32 = new cAC_32(globals); //social emotes
            globals.ac33 = new cAC_33(globals);//public cAC_33 ac33; //??
            globals.ac34 = new cAC_34(globals);
            globals.ac35 = new cAC_35(globals); //delete character
            //globals.ac37; //ip sent from client
            //globals.ac39; //???
            globals.ac40 = new cAC_40(globals); //??
            globals.ac41 = new cAC_41(globals); //??
            //globals.ac45; //??
            globals.ac50 = new cAC_50(globals); //battles
            globals.ac51 = new cAC_51(globals); //has to do with battles
            globals.ac52 = new cAC_52(globals); //has to do with battles
            globals.ac53 = new cAC_53(globals); //battles
            globals.ac54 = new cAC_54(globals); //server info at login
            globals.ac62 = new cAC_62(globals);//public cAC_62 ac62; //???
            globals.ac63 = new cAC_63(globals); //logging in
            //public cAC_65 ac65; //tent packets
            globals.ac66 = new cAC_66(globals); //??
            globals.ac69 = new cAC_69(globals); // related to warpiing(using space craft)
            globals.ac70 = new cAC_70(globals); //??
            globals.ac75 = new cAC_75(globals); //item mall stuff
            //public cAC_89 ac89; //??
            globals.ac90 = new cAC_90(globals);//public cAC_90 ac90; //??

            globals.Log("Wonderland Online P-Server\r\nby...\r\nDragon\r\nSharky\r\n\r\n Version 2");
            

            try
            {
                globals.gItemManager.LoadItems("E:\\Wonderland Online-20210429T233513Z-001\\Wonderland Online\\data\\Item.dat");
                globals.gSceneManager.LoadScenes("E:\\Wonderland Online-20210429T233513Z-001\\Wonderland Online\\data\\SceneData.Dat");
                globals.gEveManager.LoadFile("E:\\Wonderland Online-20210429T233513Z-001\\Wonderland Online\\data\\eve.EMG");
                globals.gMapManager.SetListBox(listBox1);
                globals.gskillManager = new DataLoaders.cSkillDat(globals);
                globals.gskillManager.LoadSkills("E:\\Wonderland Online-20210429T233513Z-001\\Wonderland Online\\data\\Skill.Dat");
                globals.gNpcManager = new DataLoaders.NpcDat(globals);
                globals.gNpcManager.LoadNpc("E:\\Wonderland Online-20210429T233513Z-001\\Wonderland Online\\data\\Npc.Dat");
                globals.gImMall_Manager = new ImMall_Manager(globals);
            }
            catch (Exception t) { MessageBox.Show(t.Message); }
            globals.gServer.Connect(6414, globals);
            GuiUpdate.Start();
            SendTimer.Start();
            RecvTimer.Start();
            UpdateTimer.Start();
        }

        private void GuiUpdate_Tick(object sender, EventArgs e)
        {
            #region Randomizer for Im Mall
            if (checkBox2.Checked)
            {
                if (IMupdate < globals.UpTime.Elapsed)
                {
                    if (checkBox4.Checked)
                    {
                        globals.gImMall_Manager.Randomize();
                    }
                    globals.gImMall_Manager.DiscountALL(new Random().Next(45, 100));
                    PUSHIM(null, EventArgs.Empty);
                    IMupdate = globals.UpTime.Elapsed + new TimeSpan(0, 8, 0);
                }
            }
            #endregion

            #region ImPoint
            if (checkBox3.Checked)
            {
                if (PointGiving < globals.UpTime.Elapsed)
                {
                    try
                    {
                        for (int a = 0; a < globals.gCharacterManager.characterList.Count; a++)
                        {
                            var v = globals.gUserManager.GetListByID(globals.gCharacterManager.characterList[a].characterID);
                            if (globals.gCharacterManager.characterList[a].characterID != 0)
                            {
                                v.IM += (int)((numericUpDown7.Value * 5) * 50);
                                globals.gUserManager.UpdatePlayerIM(v);
                                v.PushIM();
                            }

                        }
                    }
                    catch { }
                    PointGiving = globals.UpTime.Elapsed + new TimeSpan(0, 5, 0);
                }
            }
            #endregion

            #region Exp
            if (globals.UpTime.Elapsed > Exp)   
            {
                if (numericUpDown8.Value != 0)
                {
                    for (int a = 0; a < globals.gCharacterManager.characterList.Count; a++)
                    {
                        var value = ((uint)numericUpDown8.Value * 1);// *globals.gCharacterManager.characterList[a].level;
                        globals.gCharacterManager.characterList[a].ExpGain(value);
                    }
                    Exp = globals.UpTime.Elapsed + new TimeSpan(0, (int)numericUpDown9.Value, 4);
                }
            }
            #endregion

            #region Out info
            if (globals.logList.Count > 0)
            {
                OutputBox.AppendText(globals.logList.Dequeue() + "\r\n");
            }
            #endregion

            #region Interface
            if (globals.interfaceList != null)
            {
                if (globals.interfaceList.Count > 0)
                {
                    int action = globals.interfaceList.Dequeue();
                    switch (action)
                    {
                        case 1: //refresh player list
                            {
                                UserListBox1.Items.Clear();
                                List<cCharacter> clist = globals.gCharacterManager.getCharList();
                                foreach (cCharacter c in clist)
                                {
                                    string str = c.name + " " + c.characterID;
                                    if (c.client != null) UserListBox1.Items.Add(str);
                                }
                            } break;
                        case 2: { button7.Enabled = true; button12.Enabled = true; button13.Enabled = true; } break;
                    }
                }

            }
            #endregion
            label6.Text = "System Uptime "+new DateTime(globals.UpTime.ElapsedTicks).ToString("HH:mm:ss");
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {

            if (globals.gServer.sendList.Count > 0)
            {
                var fe = globals.gServer.sendList.Dequeue();
                if (fe.rclient != null)
                    fe.rclient.Send(fe);
                if (fe.character != null)
                    if (fe.character.client != null)
                        fe.character.client.SendData(fe.data.ToArray());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {                
                globals.gCharacterManager.Clear();
                globals.gServer.Disconnect();
                globals.cRegServer.Disconnect();
            globals.gImMall_Manager.Save();
        }

        private void RecvTimer_Tick(object sender, EventArgs e)
        {
                foreach(cCharacter g in globals.gCharacterManager.characterList.ToArray())
                {
                    if (g.client.client != null )
                    {
                        if (g.client.recvList.Count > 0)
                        {
                            while (g.client.recvList.Count > 0)
                            {
                                cRecvPacket p = g.client.recvList.Dequeue();
                                p.character = g;

                                globals.packet = p;
                                SwitchBoard(globals);
                            }
                        }
                    }
                }
        }
        bool SwitchBoard(cGlobals globals)
        {
            cRecvPacket Packet = globals.packet;//Args->Packet = Packet;
            bool bRet = true;
            switch (Packet.a)
            {
                case 0:
                    {
                        globals.ac0.SwitchBoard();
                    }
                    break;
                case 2:
                    {
                        globals.ac2.SwitchBoard();
                    } break;
                case 5:
                    {
                        globals.ac5.SwitchBoard();
                    } break;
                case 6:
                    {
                        globals.ac6.SwitchBoard();
                    } break;
                case 8: globals.ac8.SwitchBoard(); break;
                case 9:
                    {
                        globals.ac9.SwitchBoard();
                    } break;
                case 10:
                    {
                        globals.ac10.SwitchBoard();
                    } break;
                case 11:
                    {
                        globals.ac11.SwitchBoard();
                    } break;
                case 12:
                    {
                        globals.ac12.SwitchBoard();
                    } break;
                case 13: globals.ac13.SwitchBoard(); break;
                case 14: globals.ac14.SwitchBoard(); break;
                case 15: globals.ac15.SwitchBoard(); break;
                case 20:
                    {
                        globals.ac20.SwitchBoard();
                    } break;
                case 23:
                    {
                        globals.ac23.SwitchBoard();
                    } break;
                case 24: globals.ac24.SwitchBoard(); break;
                case 27: globals.ac27.SwitchBoard(); break;
                case 29: globals.ac29.SwitchBoard(); break;
                    case 30: globals.ac30.SwitchBoard(); break;
                case 32:
                    {
                        globals.ac32.SwitchBoard();
                    } break;
                case 34: globals.ac34.SwitchBoard(); break;
                case 35:
                    {
                        globals.ac35.SwitchBoard();
                    } break;
                case 37:
                    {
                        //globals.ac37.SwitchBoard();
                    } break;
                case 39:
                    {
                        //globals.ac39.SwitchBoard();
                    } break;
                case 50:
                    {
                        globals.ac50.SwitchBoard();
                    } break;
                case 63: //related to logging in
                    {
                        globals.ac63.SwitchBoard();
                    } break;
                case 65: //related to tents
                    {
                        //globals.ac65.SwitchBoard();
                    } break;
                case 69: globals.ac69.SwitchBoard(); break;
                case 75: globals.ac75.SwitchBoard(); break;
                case 89:
                    {
                        //globals.ac89.SwitchBoard();
                    } break;
                default:
                    {
                        string str = "";
                        str += "Packet code: " + Packet.a + ", " + Packet.b + " [unhandled]\r\n";
                        globals.logList.Enqueue(str);
                    } break;
            }
            return bRet;
        }

        private void inputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string cmmd = cmdBox.Text;
                globals.Log(cmmd + "\r\n");
                cmdBox.Text = "";
                doCommand(cmmd);
            }
        }

        private void doCommand(string cmmd)
        {
            string[] word = cmmd.Split(' ');
            switch (word[0])
            {
                case "users":
                    {
                        foreach (cUser u in globals.gUserManager.userList)
                        {
                            globals.Log(u.username + "\r\n");
                        }
                    } break;
                case "characters":
                    {
                        globals.gCharacterManager.outputCharList();
                    } break;
                case "maps":
                    {
                        globals.gMapManager.ListMaps();
                    } break;
                case "give":
                    {
                        try
                        {
                            int ct = word.Length;
                            if (ct > 3)
                            {
                                UInt32 id = UInt32.Parse(word[1]);
                                UInt16 itemid = UInt16.Parse(word[2]);
                                byte ammt = byte.Parse(word[3]);
                                cInvItem i = new cInvItem(globals);
                                i.ammt = ammt;
                                i.ID = itemid;
                                cCharacter c = globals.gCharacterManager.getByID(id);
                                if (c != null)
                                {
                                    if (c.inv.RecieveItem(i))
                                    {
                                        i.ammt = ammt;
                                    }
                                }
                            }
                        }
                        catch { }
                    } break;
                case "npc":
                    {
                        int ct = word.Length;
                        if (ct > 1)
                        {
                            UInt32 id = UInt32.Parse(word[1]);
                            UInt16 npcid = UInt16.Parse(word[2]);
                            cCharacter c = globals.gCharacterManager.getByID(id);
                            if (c != null)
                            {
                                globals.ac5.Send_5(npcid, c, c);
                            }
                        }
                    } break;
                case "clothes":
                    {
                        int ct = word.Length;
                        if (ct > 1)
                        {
                            UInt32 id = UInt32.Parse(word[1]);
                            cCharacter c = globals.gCharacterManager.getByID(id);
                            if (c != null)
                            {
                                c.eq.logClothes();
                            }
                        }
                    } break;
                case "inv":
                    {
                        int ct = word.Length;
                        if (ct > 1)
                        {
                            UInt32 id = UInt32.Parse(word[1]);
                            cCharacter c = globals.gCharacterManager.getByID(id);
                            if (c != null)
                            {
                                c.inv.logInv();
                            }
                        }
                    } break;
                default:
                    {
                        globals.Log("unknown command '" + cmmd + "'\r\n");
                    } break;
            }
        }

        #region IM gridview
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                IMDB.AllowUserToAddRows = true;
                IMDB.AllowUserToDeleteRows = true;
                IMDB.ReadOnly = false;
            }
            else
            {
                IMDB.AllowUserToAddRows = false;
                IMDB.AllowUserToDeleteRows = false;
                IMDB.ReadOnly = true;
                UpdateIM();
            }
        }
        void UpdateIM()
        {
            for (int a = 0; a < dataGridView2.Rows.Count; a++)
            {
                ImMallItem g = new ImMallItem(globals);
                g.setFromGrid(dataGridView2.Rows[a].Cells);
                g.entryId = (byte)(a + 1);
                globals.gImMall_Manager.Update(g);
            }
        }
        void AddNewIM(object sender, EventArgs e)
        {
            try
            {
                if (radioButton9.Checked)
                {
                    ImMallItem f = new ImMallItem(globals);
                    f.copyFrom(globals.gItemManager.GetItemByName(textBox6.Text));
                    if (numericUpDown5.Value > 0)
                    {
                        f.state = (byte)numericUpDown5.Value;
                    }
                    if (numericUpDown6.Value > 0)
                    {
                        f.Tab = (byte)numericUpDown6.Value;
                    }
                    else
                        f.Tab = 3;
                    globals.gImMall_Manager.Add(f);
                }
                else if (radioButton10.Checked)
                {
                    ImMallItem f = new ImMallItem(globals);
                    f.copyFrom(globals.gItemManager.GetItemByID(ushort.Parse(textBox6.Text)));
                    if (numericUpDown5.Value > 0)
                    {
                        f.state = (byte)numericUpDown5.Value;
                    }
                    if (numericUpDown6.Value > 0)
                    {
                        f.Tab = (byte)numericUpDown6.Value;
                    }
                    globals.gImMall_Manager.Add(f);
                }
                else if (radioButton11.Checked)
                {
                    if (numericUpDown5.Value > 0)
                    {
                        foreach (cItem fs in globals.gItemManager.GetbyItemType((int)numericUpDown5.Value))
                        {
                            ImMallItem f = new ImMallItem(globals);
                            f.copyFrom(fs);
                            f.Tab = (byte)numericUpDown6.Value;
                            if (f.Tab == 0) throw new Exception();
                            globals.gImMall_Manager.Add(f);
                        }
                    }
                }
            }
            catch { MessageBox.Show("IM Add Failed"); }
        }
        void DeleteIM(object sender, EventArgs e)
        {
            try
            {
                int f = int.Parse(textBox7.Text);
                globals.gImMall_Manager.Delete((ushort)f);
            }
            catch { MessageBox.Show("Im Item not Found"); }
        }
        void PUSHIM(object sender,EventArgs e)
        {
            button7.Enabled = false;
            button12.Enabled = false;
            button13.Enabled = false;

            IMthr = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(IMPush));
            IMthr.Name = "IMUPDATE THREAD";
            IMthr.Start(globals);
        }


        void IMPush(object g)
        {
            var f = g as cGlobals;
            f.gImMall_Manager.PushIM();
            f.Interface(2);
        }
        private void IMDB_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                if (!IMDB.Rows[e.RowIndex].IsNewRow)
                {
                    ImMallItem g = new ImMallItem(globals);
                  //  g.entryId = (byte)(cImManager.Count + 1);
                    globals.gImMall_Manager.Add(g);

                }
            }
            catch { }
        }
        private void IMDB_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                if (IMDB.Rows.Count != 0)
                {

                    globals.gImMall_Manager.Delete(ushort.Parse(IMDB.Rows[e.RowIndex].Cells[0].Value.ToString()));
                }
            }
            catch { }
        }
        private void IMDB_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                for (int yi = 0; yi < IMDB.Rows[e.RowIndex].Cells.Count; yi++)
                {
                    if (IMDB.Rows[e.RowIndex].Cells[yi].Value.ToString() == "")
                        IMDB.Rows[e.RowIndex].Cells[yi].Value = 0;
                }
            }
            catch { }
        }
        #endregion

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            for (int a = 0; a < globals.gMapManager.mapList.Count; a++)
            {
                if (globals.gMapManager.mapList[a].Characters.Count > 0)
                {
                    globals.gMapManager.mapList[a].MapProcess();
                }
            }
            for (int a = 0; a < globals.gCharacterManager.characterList.Count; a++)
                if (!globals.gCharacterManager.characterList[a].client.client.Connected)
                    globals.gCharacterManager.Remove(globals.gCharacterManager.characterList[a]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            globals.LoginMsg = textBox1.Text;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            globals.Limit = (int)numericUpDown3.Value;
            globals.cRegServer.PushLimit();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            foreach (var t in UserListBox1.CheckedItems)
            {
                var fe = globals.gCharacterManager.getByID(UInt16.Parse((string)t.ToString().Split(' ')[1]));
                if (fe != null)
                {
                    if (radioButton5.Checked)
                    {
                        cInvItem item = new cInvItem(globals);
                        item.ammt = (byte)numericUpDown4.Value;
                        item.itemtype = globals.gItemManager.GetItemByID(ushort.Parse(textBox8.Text));
                        item.ID = ushort.Parse(textBox8.Text);
                        fe.inv.RecieveItem(item);
                    }
                    if (radioButton12.Checked)
                    {
                        uint ammt = (byte)numericUpDown4.Value;
                        fe.stats.Send_1(35, ammt, 0);
                        fe.stats.Send_1(37, ammt - 1, 0);
                    }
                    if (radioButton4.Checked)
                    {
                        uint ammt = (byte)numericUpDown4.Value;
                        globals.ac26.Send_4(ammt * 20);
                    }
                }

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}

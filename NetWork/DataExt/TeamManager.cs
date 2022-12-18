using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    public class TeamManager
    {
        List<cCharacter> myTeamMembers = new List<cCharacter>(3);
        bool leader = false;
        cCharacter own;
        cGlobals globals;
        public TeamManager(cCharacter t,cGlobals g)
        {
            own = t;
            globals = g;
        }


        public int Count { get { return myTeamMembers.Count; } }
        public bool PartyLeader { get { return leader; } }
        public bool hasParty { get { if (myTeamMembers.Count > 0)return true; else return false; } }
        public List<cCharacter> TeamMembers { get { return myTeamMembers; } }

        public cSendPacket Get_13_6()
        {
            cSendPacket f = new cSendPacket(globals);
            f.Header(13, 6);
            f.AddDWord(own.characterID);
            f.AddByte((byte)Count);
            foreach (cCharacter y in TeamMembers)
                f.AddDWord(y.characterID);
            f.SetSize();
            f.character = own;
            return f;
        }

        public void KickMember(cCharacter t)
        {
            MemberLeave(t);
        }
        public void MakeLeader()
        {
            cSendPacket f = new cSendPacket(globals);
            f.Header(13, 15);
            f.AddByte(3);
            f.AddDWord(own.characterID);
            foreach (cCharacter u in myTeamMembers.ToArray())
                if (u.Party.PartyLeader)
                {
                    f.AddDWord(u.characterID);
                }
            f.SetSize();
            leader = true;
            own.map.SendtoCharacters(f);

        }
        public void MemberLeave(cCharacter l)
        {
            if (l.Party.leader)
                EndTeam();
            else if (myTeamMembers.Contains(l))
            {
                Rem(l);
            }
            Rem(l);
            l.Party.Leave();
        }
        public void Leave()
        {
            globals.ac24.Send_5(53);
            globals.ac24.Send_5(54);
            globals.ac24.Send_5(183);
            cSendPacket p = new cSendPacket(globals);
            p.Header(13, 4);
            p.AddDWord(own.characterID);
            p.SetSize();
            own.map.SendtoCharacters(p);
            leader = false;
            myTeamMembers.Clear();
        }
        public void EndTeam()
        {
            //end party
            foreach (cCharacter s in myTeamMembers.ToArray())
                s.Party.Leave();
        }
        public void Add(cCharacter t) { myTeamMembers.Add(t); }
        public void Rem(cCharacter t) { myTeamMembers.Remove(t); }

        public void AcceptMember(byte value,cCharacter t,bool join)
        {
            cSendPacket p = new cSendPacket(globals);
            if (!join)
            {
                if (!leader && myTeamMembers.Count == 0)
                leader = true;
                p.Header(13, 3);

            }
            else
                p.Header(13, 10);
            p.AddByte(value);
            p.AddDWord(own.characterID);
            p.SetSize();
            p.character = t;
            p.Send();
            t.Party.Add(own);
            p = new cSendPacket(globals);
            p.Header(13, 5);
            p.AddDWord(own.characterID);
            p.AddDWord(t.characterID);
            p.SetSize();
            own.map.SendtoCharacters(p);

            own.Send_8_3(t);
            t.Send_8_3(own);

            foreach (cCharacter y in myTeamMembers.ToArray())
            {
                t.Send_8_3(y);
                y.Send_8_3(t);
                t.Party.Add(y);
            }
            Add(t);
        }
        public void JoinTeam(cCharacter t)
        {

            cSendPacket p = new cSendPacket(globals);
            p.Header(13, 1);
            p.AddDWord(own.characterID);
            p.SetSize();
            p.character = t;
            p.Send();
        }
        public void InvitetoTeam(cCharacter t)
        {
            cSendPacket f = new cSendPacket(globals);
            f.Header(13, 9);
            f.AddDWord(own.characterID);
            f.SetSize();
            f.character = t;
            f.Send();
        }
        public void AppointLeader(byte value, cCharacter to)
        {
            cSendPacket f = new cSendPacket(globals);

            f.Header(13, 15);
            f.AddByte(value);
            f.AddDWord(own.characterID);
            f.SetSize();
            f.character = to;
            f.Send();
        }
        public void LeaderChanged(byte value,cCharacter from, cCharacter to)
        {
            if (from == own)
                leader = false;
        }
    }
}

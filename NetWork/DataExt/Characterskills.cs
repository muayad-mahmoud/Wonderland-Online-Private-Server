using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.NetWork.DataExt
{
    public class Skill :SkillItem
    {
        public Skill(SkillItem g)
        {
            Name = g.Name;
            SkillID = g.SkillID;
        }
        public uint exp;
        public byte level;
    }
    public class cSkills
    {
        List<Skill> myskills = new List<Skill>();
        cCharacter owner;
        cGlobals globals;

        public cSkills(cGlobals h, cCharacter r)
        {
            globals = h; owner = r;
        }

        public void LoadSkills(string skill)
        {
            var words = skill.Split('&');
            foreach(string k in words)
            {
                var sec = k.Split(' ');
                if (sec[0] == "none") break;
            Skill newskill = new Skill( globals.gskillManager.GetSkillbyID(UInt16.Parse(sec[0])));
            newskill.level = byte.Parse(sec[1]);
            newskill.exp = byte.Parse(sec[2]);
            myskills.Add(newskill);
            }
        }
        public string SaveSkills()
        {
            string str = "";
            int ct = 0;
            foreach (Skill h in myskills)
            {
                ct++;
                string sk = "";
                sk += h.SkillID.ToString() + " ";
                sk += h.level.ToString() + " ";
                sk += h.exp.ToString() + " ";
                if (ct < myskills.Count)
                    sk += "&";
                str += sk;
            }
            if (str == "")
                str += "none";
            return str;
        }
        public void RecieveSkill(UInt16 skill)
        {
        }
        public byte[] GetSkillData()
        {
            cSendPacket data = new cSendPacket(globals);
            data.AddWord((ushort)myskills.Count);
            for (int a = 0; a < myskills.Count; a++)
            {
                data.AddWord(myskills[a].UnknownWord10);
                data.AddByte(myskills[a].level);
                data.AddDWord(myskills[a].exp);
            }
            return data.data.ToArray();
        }

        public void CheckforUpdate()
        {
        }
    }
}

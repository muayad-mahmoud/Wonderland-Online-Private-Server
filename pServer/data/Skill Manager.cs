using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using PServer_v2.NetWork;
using PServer_v2.NetWork.DataExt;

namespace PServer_v2.DataLoaders
{
    public class cSkillDat
    {
        cGlobals globals;
        List<SkillItem> skillList;

        public cSkillDat(cGlobals j)
        {
            globals = j;
            skillList = new List<SkillItem>();
        }

        public bool LoadSkills(string path)
        {
            globals.Log("loading skill dat.....");
            try
            {
                skillList.Clear();
                if (!File.Exists(path)) return false;
                byte[] data = File.ReadAllBytes(path);
                int max = data.Length / 148;
                for (int n = 0; n < max; n++)
                {
                    SkillItem i = new SkillItem();
                    i.Load(data, n * 148);
                    skillList.Add(i);
                }
            }
            catch { globals.Log("failed"); }
            globals.Log("done");
            return true;
        }
        public SkillItem GetSkillbyID(UInt16 ID)
        {
            SkillItem ret = new SkillItem();
            foreach (SkillItem i in skillList)
            {
                if (i.SkillID == ID)
                {
                    ret = i;
                    return ret;
                }
            }
            return null;
        }
        public SkillItem GetSkillbyName(string Name)
        {
            foreach (SkillItem i in skillList)
            {
                if (string.Compare(Name.ToLower(), i.Name.ToLower()) == 0)
                {
                    return i;
                }
            }
            return null;
        }
    }
}

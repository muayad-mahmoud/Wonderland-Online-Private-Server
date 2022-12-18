using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PServer_v2.NetWork.DataExt
{
    #region Character related
    public enum Element
    {
        noElement,
        Earth,
        Water,
        Fire,
        Wind,
    }
    public enum RebornJob
    {
        Killer = 1,
        Warrior,
        Knight,
        Wit,
        Priest,
        Seer,
    }
    public enum BodyStyle
    {
        none,
        Small_Male = 1,
        Small_Female,
        Big_Male,
        Big_Female,
    }
    public enum inGameState
    {
        Normal,
        Sitting,
        Emote,
        Battle,
        RidingNpc,
        RidingVech,
    }
#endregion

#region Item Related
    public enum eWearSlot
    {
        none = 0,
        head = 1,
        body = 2,
        hand = 3,
        arm = 4,
        feet = 5,
        special = 6

    }
    public enum eWeaponType
    {
        none = 0,
        sword = 1,
        spear = 2,
        noob = 3,
        bow = 4,
        wand = 5,
        claw = 6,
        axe = 7,
        club = 8,
        fan = 9,
        gun = 10


    }
#endregion
}

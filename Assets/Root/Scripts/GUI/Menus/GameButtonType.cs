using System.ComponentModel;
using System.Reflection;

// Ordered according to the portraits file
public enum GameButtonType {
    None,

    [Description ("Research _U_nholy Armor")]
    UnholyArmorResearch = 1,

    [Description ("Research _D_ark Vision")]
    DarkVisionResearch = 2,

    [Description ("Research _R_aising Dead")]
    RaiseDeadResearch = 3,
    
    [Description ("_R_ain of Fire")]
    RainOfFireResearch = 4,

    [Description ("_I_nvisibility")] 
    InvisibilityResearch = 5,

    [Description ("_F_ar Seeing")] 
    FarSeeingResearch = 6,

    [Description ("_H_ealing")] 
    HealingResearch = 7,

    [Description ("Upgrade S_h_ield Strength")]
    OrcUpgradeShield2 = 8,
    
    [Description ("Upgrade S_h_ield Strength")]
    OrcUpgradeShield1 = 9,

    [Description ("_S_top")]
    OrcShield = 10,

    [Description ("Upgrade S_h_ield Strength")] 
    HumanUpgradeShield2 = 11,

    [Description ("Upgrade S_h_ield Strength")] 
    HumanUpgradeShield1 = 12,

    [Description ("_S_top")]
    HumanShield = 13,

    [Description ("_B_reed Faster Horses")] 
    BreedFasterHorses2 = 14,

    [Description ("_B_reed Faster Horses")] 
    BreedFasterHorses1 = 15,

    [Description ("_U_pgrade Spear Strength")]
    UpgradeSpearStrength2 = 16,
    
    [Description ("_U_pgrade Spear Strength")]
    UpgradeSpearStrength1 = 17,

    [Description ("_A_ttack")]
    Spear = 18,

    [Description ("_U_pgrade Arrow Strength")] 
    UpgradeArrowStrength2 = 19,

    [Description ("_U_pgrade Arrow Strength")] 
    UpgradeArrowStrength1 = 20,

    [Description ("_A_ttack")]
    Arrow = 21,
    
    [Description ("_B_reed Faster Wolves")]
    BreedFasterWolves2 = 22,
    
    [Description ("_B_reed Faster Wolves")]
    BreedFasterWolves1 = 23,
    
    [Description ("Upgrade _A_xe Strength")]
    UpgradeAxeStrength2 = 24,
    
    [Description ("Upgrade _A_xe Strength")]
    UpgradeAxeStrength1 = 25,

    [Description ("_A_ttack")]
    Axe = 26,

    [Description ("Upgrade S_w_ord Strength")] 
    UpgradeSwordStrength2 = 27,

    [Description ("Upgrade S_w_ord Strength")] 
    UpgradeSwordStrength1 = 28,

    [Description ("_A_ttack")]
    Sword = 29,

    [Description ("_A_ttack")]
    Fireball = 30,

    [Description ("_A_ttack")]
    ShadowSpear = 31,
    
    [Description ("_A_ttack")]
    ElementalBlast = 32,
    
    [Description ("_A_ttack")]
    HolyLance = 33,
    
    Brigand = 34,
    Wounded = 35,
    Griselda = 36,
    Garona = 37,
    Medivh = 38,
    Lothar = 39,

    [Description ("Research M_a_jor Summoning")]
    WaterElementalResearch = 40,
    
    [Description ("Research M_a_jor Summoning")]
    DaemonResearch = 41,
    
    Skeleton = 42,
    OrcSkeleton = 43,
    
    [Description ("Research _M_inor Summoning")]
    ScorpionResearch = 44,
    
    FireElemental = 45,
    Slime = 46,
    
    [Description ("Research _M_inor Summoning")]
    SpiderResearch = 47,
    
    Ogre = 48,
    Placeholder = 49, //Not sure what this is

    [Description ("Build _R_oad")]
    BuildRoad = 50,

    [Description ("Build _W_all")]
    BuildWall = 51,

    [Description ("_ESC_ - Cancel")]
    Cancel = 52,

    [Description ("Return Goods to _T_own Hall")]
    ReturnGoods = 53,

    [Description ("Build _A_dvanced Structure")]
    BuildAdvancedStructure = 54,

    [Description ("Build _B_asic Structure")]
    BuildBasicStructure = 55,

    [Description ("_H_arvest lumber/Mine gold")]
    HarvestLumberMineGold = 56,

    [Description ("_R_epair")] 
    Repair = 57,

    [Description ("_M_ove")] 
    OrcMove = 58,

    [Description ("_M_ove")] 
    HumanMove = 59,
    
    BlackRock = 60,
    Stormwind = 61,
    GoldMine = 62,
    
    [Description ("Build T_e_mple")]
    OrcTemple = 63,

    [Description ("Build Ch_u_rch")] 
    HumanChurch = 64,
    
    [Description ("Build _B_lacksmith")]
    OrcBlacksmith = 65,

    [Description ("Build _B_lacksmith")] 
    HumanBlacksmith = 66,
    
    [Description ("Build _K_ennel")]
    OrcKennels = 67,

    [Description ("Build _S_table")] 
    HumanStables = 68,
    
    [Description ("Build _L_umber Mill")]
    OrcLumberMill = 69,

    [Description ("Build _L_umber Mill")] 
    HumanLumberMill = 70,
    
    [Description ("Rebuild the Town _H_all")]
    OrcTownHall = 71,

    [Description ("Rebuild the Town _H_all")] 
    HumanTownHall = 72,
    
    [Description ("Build _T_ower")]
    OrcTower = 73,

    [Description ("Build _T_ower")] 
    HumanTower = 74,
    
    [Description ("Build _B_arracks")]
    OrcBarracks = 75,

    [Description ("Build _B_arracks")] 
    HumanBarracks = 76,
    
    [Description ("Build _Farm_")]
    OrcFarm = 77,

    [Description ("Build _F_arm")] 
    HumanFarm = 78,
    
    [Description ("_T_rain Necrolyte")]
    Necrolyte = 79,
    
    [Description ("_T_rain Cleric")]
    Cleric = 80,
    
    [Description ("Train _S_pearman")]
    Spearman = 81,
    
    [Description ("Train _A_rcher")]
    Archer = 82,
    
    [Description ("Train _R_aider")]
    WolfRaider = 83,

    [Description ("Train _K_night")]
    Knight = 84,
    
    [Description ("Build Ca_t_apult")]
    OrcCatapult = 85,
    
    [Description ("Build Ca_t_apult")]
    HumanCatapult = 86,
    
    [Description ("Train _P_eon")]
    Peon = 87,

    [Description ("Train _P_easant")] 
    Peasant = 88,

    [Description ("_T_rain Warlock")]
    Warlock = 89,
    
    [Description ("_T_rain Conjurer")]
    Conjurer = 90,

    [Description ("Train Gr_u_nt")]
    Grunt = 91,
    
    [Description ("Train _F_ootman")]
    Footman = 92,

    [Description ("Research Cloud of _P_oison")]
    CloudOfPoisonResearch = 93,

    //An invalid GameButtonType state, to be used when there's no possible state the button can be, not even None
    Invalid = 1000,

    //From here onwards, the underlying type is determined by subtracting 1000 from its value
    //(example: Healing - 1000 = HealingResearch

    [Description ("_U_nholy Armor")]
    UnholyArmor = 1001,

    [Description ("_D_ark Vision")]
    DarkVision = 1002,

    [Description ("_R_aise Dead")]
    RaiseDead = 1003,
    
    [Description ("_R_ain of Fire")]
    RainOfFire = 1004,
    
    [Description ("_I_nvisibility")] 
    Invisibility = 1005,
    
    [Description ("_F_ar Seeing")] 
    FarSeeing = 1006,

    [Description ("_H_ealing")]
    Healing = 1007,

    [Description ("Summon _W_ater Elemental")]
    WaterElemental = 1040,
    
    [Description ("Summon _D_aemon")]
    Daemon = 1041,

    [Description ("Summon Sc_o_rpions")]
    Scorpion = 1044,
    
    [Description ("Summon Spide_r_s")]
    Spider = 1047,

    [Description ("Cloud of _P_oison")]
    CloudOfPoison = 1093
}

public static class ButtonTypeExtensions {
    
    public static string GetDescription (this GameButtonType type) {
        FieldInfo info = typeof (GameButtonType).GetField (type.ToString ());
        
        if (info != null) {
            object[] attributes = info.GetCustomAttributes (typeof (DescriptionAttribute), true);

            if (attributes != null && attributes.Length > 0) {
                return ((DescriptionAttribute) attributes[0]).Description;
            }
        }
        
        return type.ToString ();
    }    
}

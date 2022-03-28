using System.ComponentModel;

public static class Rank {

    private static int[] rankThresholds = new int[] {
        200,
        300,
        400,
        800,
        1600,
        2400,
        3200,
        4000,
        4800,
        5200,
        5600,
        6000,
        6400,
        6800,
        7200,
        9600,
        11400,
        11880,
        12000
    };

    private static int CalculateRankIndex (int score) {
        for (int i = 0; i < Rank.rankThresholds.Length; i ++)
            if (Rank.rankThresholds [i] > score)
                return i;

        return Rank.rankThresholds.Length - 1;
    }

    public static HumanRank GetHumanRank (int score) {
        return (HumanRank) Rank.CalculateRankIndex (score);
    }

    public static OrcRank GetOrcRank (int score) {
        return (OrcRank) Rank.CalculateRankIndex (score);
    }
}

public enum HumanRank {
    Slave,
    Peasant,
    Squire,
    Footman,
    Corporal,
    Sergeant,
    Lieutenant,
    Captain,
    Major,
    Knight,
    General,
    Brigadier,
    Marshall,
    Duke,
    [Description ("War Leader")] WarLeader,
    Demigod,
    God,
    Designer
}

public enum OrcRank {
    Slave,
    Peon,
    Rogue,
    Grunt,
    Slasher,
    Sergeant,
    Commander,
    Captain,
    Major,
    Raider,
    General,
    Master,
    Slayer,
    Dictator,
    [Description ("War Chief")] WarChief,
    Demigod,
    God,
    Designer
}

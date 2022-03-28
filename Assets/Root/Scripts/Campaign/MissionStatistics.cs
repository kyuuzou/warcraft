using UnityEngine;
using System.Collections;

public class MissionStatistics : MonoBehaviour {

    [SerializeField]
    private int score = 0;
    public int Score {
        get { return this.score; }
        set { this.score = value; }
    }

    [SerializeField]
    private int unitsYouDestroyed = 0;
    public int UnitsYouDestroyed {
        get { return this.unitsYouDestroyed; }
        set { this.unitsYouDestroyed = value; }
    }

    [SerializeField]
    private int unitsEnemyDestroyed = 0;
    public int UnitsEnemyDestroyed {
        get { return this.unitsEnemyDestroyed; }
        set { this.unitsEnemyDestroyed = value; }
    }

    [SerializeField]
    private int structuresYouDestroyed = 0;
    public int StructuresYouDestroyed {
        get { return this.structuresYouDestroyed; }
        set { this.structuresYouDestroyed = value; }
    }

    [SerializeField]
    private int structuresEnemyDestroyed = 0;
    public int StructuresEnemyDestroyed {
        get { return this.structuresEnemyDestroyed; }
        set { this.structuresEnemyDestroyed = value; }
    }

    [SerializeField]
    private int goldYouMined = 0;
    public int GoldYouMined {
        get { return this.goldYouMined; }
        set { this.goldYouMined = value; }
    }

    [SerializeField]
    private int goldEnemyMined = 0;
    public int GoldEnemyMined {
        get { return this.goldEnemyMined; }
        set { this.goldEnemyMined = value; }
    }

    [SerializeField]
    private int unitsYouTrained = 0;
    public int UnitsYouTrained {
        get { return this.unitsYouTrained; }
        set { this.unitsYouTrained = value; }
    }

    [SerializeField]
    private int unitsEnemyTrained = 0;
    public int UnitsEnemyTrained {
        get { return this.unitsEnemyTrained; }
        set { this.unitsEnemyTrained = value; }
    }

    [SerializeField]
    private int structuresYouBuilt = 0;
    public int StructuresYouBuilt {
        get { return this.structuresYouBuilt; }
        set { this.structuresYouBuilt = value; }
    }

    [SerializeField]
    private int structuresEnemyBuilt = 0;
    public int StructuresEnemyBuilt {
        get { return this.structuresEnemyBuilt; }
        set { this.structuresEnemyBuilt = value; }
    }

    [SerializeField]
    private int lumberYouHarvested = 0;
    public int LumberYouHarvested {
        get { return this.lumberYouHarvested; }
        set { this.lumberYouHarvested = value; }
    }

    [SerializeField]
    private int lumberEnemyHarvested = 0;
    public int LumberEnemyHarvested {
        get { return this.lumberEnemyHarvested; }
        set { this.lumberEnemyHarvested = value; }
    }

    public int CalculateScore () {
        return this.Score;
    }
}

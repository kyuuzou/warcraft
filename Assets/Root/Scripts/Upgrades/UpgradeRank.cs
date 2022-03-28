using System.Collections;
using UnityEngine;

[System.Serializable]
public class UpgradeRank {

    [SerializeField]
    private string name;

    [SerializeField]
    private int strength;
    public int Strength {
        get { return this.strength; }
    }

    [SerializeField]
    private int cost;
    public int Cost {
        get { return this.cost; }
    }

    [SerializeField]
    private int researchTime;
    public int ResearchTime {
        get { return this.researchTime; }
    }

    [SerializeField]
    private GameButtonType buttonType;
    public GameButtonType ButtonType {
        get { return this.buttonType; }
    }
}

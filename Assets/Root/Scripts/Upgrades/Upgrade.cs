using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : CustomScriptableObject {

    [SerializeField]
    private UpgradeIdentifier identifier;
    public UpgradeIdentifier Identifier {
        get { return this.identifier; }
    }

    [SerializeField]
    private UpgradeType type;
    public UpgradeType Type {
        get { return this.type; }
    }

    [SerializeField]
    private GameButtonType buttonType;
    public GameButtonType ButtonType {
        get { return this.buttonType; }
    }

    [SerializeField]
    private List<UnitType> units;

    [SerializeField]
    private UpgradeRank[] ranks;

    private int currentRankIndex;
    public UpgradeRank CurrentRank {
        get { return this.ranks[this.currentRankIndex]; }
    }

    public UpgradeRank NextRank {
        get {
            if (this.currentRankIndex + 1 < this.ranks.Length) {
                return this.ranks[this.currentRankIndex + 1];
            }

            return null;
        }
    }

    public bool AppliesTo (UnitType type) {
        return this.units.Contains (type);
    }

    public UpgradeRank GetRank (int rank) {
        if (rank < this.ranks.Length) {
            return this.ranks[rank];
        }

        Debug.LogError ("Upgrade does not have rank: " + rank);

        return null;
    }

    public override void Initialize () {
        base.Initialize ();

        this.currentRankIndex = 0;
    }

    public void SetRankIndex (int currentRankIndex) {
        this.currentRankIndex = Mathf.Clamp (currentRankIndex, 0, this.ranks.Length);
    }
}

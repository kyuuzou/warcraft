using UnityEngine;

public class Campaign : CustomScriptableObject {

    [SerializeField]
    private CampaignType type;
    public CampaignType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Level[] levels;

    public Level GetLevel(int index) {
        return this.levels[index];
    }
}

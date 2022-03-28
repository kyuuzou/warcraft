using System.Collections;
using UnityEngine;

public class FactionData : CustomScriptableObject {

    [SerializeField]
    private FactionIdentifier identifier;
    public FactionIdentifier Identifier {
        get { return this.identifier; }
    }

    [SerializeField]
    private new string name;
    public string Name {
        get { return this.name; }
    }

    [SerializeField]
    private int levelParserIndex;
    public int LevelParserIndex {
        get { return this.levelParserIndex; }
    }

    [SerializeField]
    private FactionType type;
    public FactionType Type {
        get { return this.type; }
    }

    [Header ("Diplomacy")]
    [SerializeField]
    private FactionStanding[] standings;

    public bool IsEnemy (FactionIdentifier identifier) {
        foreach (FactionStanding standing in this.standings) {
            if (standing.Identifier == identifier) {
                return standing.Relationship == FactionRelationship.Enemy;
            }
        }

        return false;
    }
}

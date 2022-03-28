using System.Collections;
using UnityEngine;

public class PlayerData : CustomScriptableObject {

    [SerializeField]
    private Color color;
    public Color Color {
        get { return this.color; }
    }

    [SerializeField]
    private FactionIdentifier[] factions;
    public FactionIdentifier[] Factions {
        get { return this.factions; }
    }

    [SerializeField]
    public bool humanPlayer;
    public bool HumanPlayer {
        get { return this.humanPlayer; }
    }
}

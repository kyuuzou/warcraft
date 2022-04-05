using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableSpriteData : ScriptableObject {

    [Header ("Animations")]

    [SerializeField]
    private MeshAnimation[] animations;
    public MeshAnimation[] Animations {
        get { return this.animations; }
    }

    [Header ("Sprite data")]

    [SerializeField]
    private Vector2Int tileSize = new Vector2Int(1, 1);
    public Vector2Int TileSize {
        get { return this.tileSize; }
    }
   
    [SerializeField]
    private int armourPoints;
    public int ArmourPoints {
        get { return this.armourPoints; }
    }

    [SerializeField]
    private int hitPoints;
    public int HitPoints {
        get { return this.hitPoints; }
    }

    [SerializeField]
    private int manaPoints;
    public int ManaPoints {
        get { return this.manaPoints; }
    }

    [SerializeField]
    private bool regainsMana = true;
    public bool RegainsMana {
        get { return this.regainsMana; }
    }

    [SerializeField]
    private int timeToBuild;
    public int TimeToBuild {
        get { return this.timeToBuild; }
    }

    [SerializeField]
    private int cost;
    public int Cost {
        get { return this.cost; }
    }

    [Header ("Context Menu data")]

    [SerializeField]
    private GameButtonType portrait;
    public GameButtonType Portrait {
        get { return this.portrait; }
    }

    [SerializeField]
    private string title;
    public string Title {
        get { return this.title; }
        set { this.title = value; }
    }

    [SerializeField]
    private ContextMenuNode rootMenuNode;
    public ContextMenuNode RootMenuNode {
        get { return this.rootMenuNode; }
    }

    [Header ("File data")]

    /// <summary>
    /// The string that represents this sprite on a file.
    /// </summary>
    [SerializeField]
    private string nameOnFile;
    public string NameOnFile {
        get { return this.nameOnFile; }
    }

    [Header ("Sprite Sound")]

    [SerializeField]
    private AudioIdentifier selectionSound;
    public AudioIdentifier SelectionSound {
        get { return this.selectionSound; }
    }

    [SerializeField]
    private AudioIdentifier consecutiveSelectionSound;
    public AudioIdentifier ConsecutiveSelectionSound {
        get { return this.consecutiveSelectionSound; }
    }
}

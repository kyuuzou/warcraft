using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level : CustomScriptableObject {

    [SerializeField]
    private FactionData[] factions;
    public FactionData[] Factions {
        get { return this.factions; }
    }

    [SerializeField]
    private TextAsset mapLayout;
    public TextAsset MapLayout {
        get { return this.mapLayout; }
    }
    
    [SerializeField]
    private TextAsset mapObjectives;
    public TextAsset MapObjectives {
        get { return this.mapObjectives; }
    }
    
    [SerializeField]
    private TextAsset mapPresentation;
    public TextAsset MapPresentation {
        get { return this.mapPresentation; }
    }

    [SerializeField]
    private MapType mapType;
    public MapType MapType {
        get { return this.mapType; }
    }

    [SerializeField]
    private PlayerData[] players;
    public PlayerData[] Players {
        get { return this.players; }
    }

    [SerializeField]
    private MissionRequirement[] requirements;
    public MissionRequirement[] Requirements {
        get { return this.requirements; }
    }

    [SerializeField]
    private int startCurrency;
    public int StartCurrency {
        get { return this.startCurrency; }
    }

    [SerializeField]
    private Vector2Int startPosition;
    public Vector2Int StartPosition {
        get { return this.startPosition; }
    }
}

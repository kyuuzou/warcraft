using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SceneObject {

    public PlayerData Data { get; private set; }

    public List<Faction> Factions { get; private set; }

    protected GameController GameController { get; private set; }

    protected override void Awake () {
        base.Awake ();

        this.GameController = ServiceLocator.Instance.GameController;
    }

    public void Initialize (PlayerData data) {
        this.InitializeExternals ();

        this.Data = data;

        this.InitializeFactions ();
    }

    private void InitializeFactions () {
        this.Factions = new List<Faction> ();

        foreach (FactionIdentifier identifier in this.Data.Factions) {
            Faction faction = this.GameController.GetFaction (identifier);
            
            faction.ControllingPlayer = this;
            this.Factions.Add (faction);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraitBuilder : UnitTrait, IDeathListener, IMovementListener, IUnitTraitBuilder {

    public UnitTraitDataBuilder Data { get; set; }

    private AudioManager audioManager;
    private ContextMenu contextMenu;
    private GameController gameController;
    private Map map;
    private SpawnFactory spawnFactory;

    private Building building;

    private static Dictionary<GameButtonType, BuildingType> buildingByButtonType;

    static UnitTraitBuilder () {
        UnitTraitBuilder.buildingByButtonType = new Dictionary<GameButtonType, BuildingType> () {
            { GameButtonType.HumanTownHall, BuildingType.HumanTownHall },
            { GameButtonType.OrcTownHall,   BuildingType.OrcTownHall }
        };
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Builder; }
    }

    public void ApproachingTarget () {
        this.Unit.Collider.enabled = false;
        this.Unit.Selection.SetVisible (false);
        
        if (this.Unit.Selected) {
            this.Unit.SetSelected (false);
            this.gameController.ClearSelection ();
            //this.statusWindow.SetVisible (false);
            this.contextMenu.SetVisible (false);
        }
    }

    public void Build (BuildingType type) {
        this.Activate ();

        int currency = this.gameController.GetGold ();

        BuildingData data = this.spawnFactory.GetData (type);

        if (currency < data.Cost) {
            return;
        }

        if (data.Unique && this.Unit.Faction.GetBuildings (type).Count > 0) {
            Debug.LogWarning ("Tried to build more than one unique building: " + type);
            return;
        }

        this.building = this.spawnFactory.SpawnBuilding (type, this.Unit.Faction);
        this.building.GetComponent<Renderer>().enabled = false;

        this.building.AddDeathListener (this);

        this.gameController.DecreaseGold (data.Cost);
        
        this.contextMenu.SetNode (this.contextMenu.CancelNode);

        //bool build = true;

        if (data.Rooted) {
            IntVector2 position = this.Unit.Faction.GetRootedPosition (type);

            if (position != null) {
                //build = false;

                this.building.SetUpConstructionSite (this.Unit, this.map.GetTile (position));
            }
        }

        /*
        if (build) {
            this.interactionHandler.SetMode (
                InteractionModeType.Building,
                new InteractionModeBuildingArgs (this.building, this.Unit)
            );
        }
        */
    }

    public override void Deactivate () {
        base.Deactivate ();

        if (GameFlags.building && this.building != null) {
            this.gameController.IncreaseGold (this.building.Data.Cost);

            this.building = null;
        }
    }

    public override void FilterButtons (ref List<GameButtonType> buttons) {
        for (int i = 0; i < buttons.Count; i ++) {
            if (UnitTraitBuilder.buildingByButtonType.ContainsKey (buttons[i])) {
                BuildingType type = UnitTraitBuilder.buildingByButtonType[buttons[i]];

                if (this.Unit.Faction.GetBuildings (type).Count > 0) {
                    buttons[i] = GameButtonType.None;
                }
            }
        }
    }

    public void Initialize (Unit unit, UnitTraitDataBuilder data) {
        base.Initialize (unit);
        
        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.audioManager = serviceLocator.AudioManager;
        this.contextMenu = serviceLocator.ContextMenu;
        this.gameController = serviceLocator.GameController;
        this.map = serviceLocator.Map;
        this.spawnFactory = serviceLocator.SpawnFactory;
    }
    
    public bool IsTileTraversable (MapTile tile) {
        if (tile.GetInhabitant<Building> () == this.building) {
            return true;
        }

        return tile.IsTraversable (this.Unit.GetTrait<IUnitTraitMoving> ().MovementType, this.Unit);
    }

    public void MoveToConstructionSite (Building building) {
        this.Activate ();

        this.building = building;

        this.Unit.Move (this.building, this, true, false);
    }

    public void OnDeathNotification (SpawnableSprite sprite) {
        if (sprite == this.building) {
            this.Deactivate ();
            this.Unit.Stop ();
        }
    }

    public void OnOrderAccepted () {
        
    }

    public void OnWorkComplete () {
        this.audioManager.Play (this.Data.WorkCompleteSound);
    }
    
    public void ReachedTarget () {
        if (this.Active) {
            this.Unit.Renderer.enabled = false;
            this.building.Build (this.Unit);
        } else {
            this.Unit.Collider.enabled = true;
        }

        this.Unit.Play (AnimationType.Idle);
    }

    public void ShowAdvancedStructures () {
        this.contextMenu.SetNode (this.Unit.Data.RootMenuNode.GetLinkedNode (GameButtonType.BuildAdvancedStructure));
    }
    
    public void ShowBasicStructures () {
        this.contextMenu.SetNode (this.Unit.Data.RootMenuNode.GetLinkedNode (GameButtonType.BuildBasicStructure));
    }

    public void TileChanged () {
        if (this.Unit.Tile == this.building.Tile) {
            return;
        }

        Building building = this.Unit.Tile.GetInhabitant<Building> ();

        if (building != null) {
            this.ApproachingTarget ();
            this.Unit.Stop ();
        }
    }
}

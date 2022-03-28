using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTraitTrainer : BuildingTrait, IBuildingTraitTrainer {

    public BuildingTraitDataTrainer Data { get; set; }

    private AudioManager audioManager;
    private ContextMenu contextMenu;
    private GameController gameController;
    private Map map;
    private MissionStatistics missionStatistics;
    private SpawnFactory spawnFactory;
    private StatusPane statusPane;

    private IEnumerator trainEnumerator = null;
    private bool training = false;
    private Unit unit;

    public override void Deactivate () {
        base.Deactivate ();

        if (this.training) {
            this.Building.StopCoroutine (this.trainEnumerator);

            this.gameController.IncreaseGold (this.unit.Data.Cost);

            this.Building.StatusBackgroundIndex = 8;
            this.statusPane.SetBackgroundIndex (this.Building.StatusBackgroundIndex);
        }
    }

    public void Initialize (Building building, BuildingTraitDataTrainer data) {
        base.Initialize (building);
        
        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.audioManager = serviceLocator.AudioManager;
        this.contextMenu = serviceLocator.ContextMenu;
        this.gameController = serviceLocator.GameController;
        this.map = serviceLocator.Map;
        this.missionStatistics = serviceLocator.MissionStatistics;
        this.spawnFactory = serviceLocator.SpawnFactory;
        this.statusPane = serviceLocator.StatusPane;
    }

    public void Train (UnitType type) {
        if (this.trainEnumerator != null) {
            this.Building.StopCoroutine (this.trainEnumerator);
        }

        this.trainEnumerator = this.TrainCoroutine (type);
        this.Building.StartCoroutine (this.trainEnumerator);
    }

    private IEnumerator TrainCoroutine (UnitType type) {
        this.training = true;

        this.unit = this.spawnFactory.SpawnUnit (type, this.Building.Faction);
        this.unit.Collider.enabled = false;
        this.unit.Renderer.enabled = false;

        this.gameController.DecreaseGold (this.unit.Data.Cost);

        if (this.Building.Selected) {
            this.contextMenu.SetNode (this.contextMenu.CancelNode);
        }

        this.trainEnumerator = this.Building.Progress (this.unit.Data.TimeToBuild);
        yield return this.Building.StartCoroutine (this.trainEnumerator);

        this.Building.StatusBackgroundIndex = 8;
        
        if (this.Building.Selected) {
            this.statusPane.SetBackgroundIndex (this.Building.StatusBackgroundIndex);
            this.contextMenu.SetNode (this.Building.Data.RootMenuNode);
        }
        
        this.training = false;
        
        this.unit.Initialize (this.Building.Tile);
        this.unit.ClaimTile (this.Building.Tile);
        this.map.AddUnit (this.unit);

        this.audioManager.Play (this.Data.TrainedSound);
        yield return this.Building.StartCoroutine (this.Building.EjectUnitCoroutine (this.unit));

        this.missionStatistics.Score += 20;
        this.missionStatistics.UnitsYouTrained ++;
    }
}

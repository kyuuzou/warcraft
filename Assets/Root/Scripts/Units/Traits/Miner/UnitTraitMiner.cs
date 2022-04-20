using System.Collections;
using UnityEngine;

public class UnitTraitMiner : UnitTrait, IUnitTraitMiner, IMovementListener {

    public UnitTraitDataMiner Data { get; set; }

    private Building minable;
    private Building townHall;

    private GameController gameController;
    private Map map;
    private MissionStatistics missionStatistics;

    private int carryingGoldLayer = 1;
    private bool goingToMine;

    private int gold = 0;
    private int Gold {
        get { return this.gold; }
        set {
            if (value == 0) {
                if (this.gold > 0) {
                    this.Unit.SetAnimatorLayer(0);
                }
            } else {
                //this.Unit.Lumber = 0;

                if (this.gold == 0) {
                    this.Unit.SetAnimatorLayer(this.carryingGoldLayer);
                }
            }

            this.gold = value;
        }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Miner; }
    }

    public void ApproachingTarget() {
        this.Unit.ApproachBuilding();
    }

    private IEnumerator DropGold() {
        this.goingToMine = true;

        this.gameController.IncreaseGold(this.Gold);
        this.Gold = 0;

        yield return new WaitForSeconds(1.0f);

        this.Unit.LeaveBuilding();

        if (this.minable.IsMinable()) {
            yield return this.Unit.StartCoroutine(this.townHall.EjectUnitCoroutine(this.Unit, false));

            this.Unit.Move(this.minable, this, true, false);
        } else {
            yield return this.Unit.StartCoroutine(this.townHall.EjectUnitCoroutine(this.Unit));
        }

        this.missionStatistics.Score++;
        this.missionStatistics.GoldYouMined += this.Gold;
    }

    private IEnumerator GatherGold() {
        this.goingToMine = false;

        this.Gold = this.minable.Mine(this.Unit.GetTrait<IUnitTraitMiner>());

        if (!this.minable.IsDead()) {
            yield return new WaitForSeconds(2.0f);
        }

        yield return this.Unit.StartCoroutine(this.minable.EjectUnitCoroutine(this.Unit, false));

        this.Unit.Move(this.townHall, this, true, false);
    }

    public void Initialize(Unit unit, UnitTraitDataMiner data) {
        base.Initialize(unit);

        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.map = serviceLocator.Map;
        this.missionStatistics = serviceLocator.MissionStatistics;
    }

    public bool IsTileTraversable(MapTile tile) {
        return true;
    }

    public void Mine(Building building) {
        if (building.IsMinable()) {
            this.minable = building;
            this.townHall = this.map.GetNearestBuilding(BuildingType.HumanTownHall, this.Unit.Tile);

            this.goingToMine = true;

            this.Unit.Move(building, this, true, false);
        }
    }

    public void OnOrderAccepted() {

    }

    public void ReachedTarget() {
        if (this.goingToMine && this.minable.Overlaps(this.Unit)) {
            this.Unit.EnterBuilding();
            this.Unit.StartCoroutine(this.GatherGold());
        } else if (!this.goingToMine && this.townHall.Overlaps(this.Unit)) {
            this.Unit.EnterBuilding();
            this.Unit.StartCoroutine(this.DropGold());
        } else {
            this.RecalculatePath();
        }
    }

    public void RecalculatePath() {
        if (this.goingToMine) {
            this.Unit.Move(this.minable, this, true, true);
        } else {
            this.ReturnToTownHall();
        }
    }

    private void ReturnToTownHall() {
        this.Unit.Move(this.townHall, this, true, true);
    }

    public void TileChanged() {

    }
}

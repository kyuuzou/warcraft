using System;
using System.Collections.Generic;
using UnityEngine;

public class Faction {

    public Player ControllingPlayer { get; set; }
    public FactionData Data { get; private set; }

    private List<Building> buildings;
    private List<Unit> units;

    private Dictionary<BuildingType, List<Building>> buildingsByType;
    private Dictionary<BuildingType, Vector2Int> rootedPositionsByType;
    private Dictionary<UnitTraitType, List<Unit>> unitsByRole;

    private Dictionary<GameButtonType, Upgrade> upgradeByButtonType;
    private Dictionary<UpgradeIdentifier, Upgrade> upgradeByIdentifier;
    private Dictionary<UpgradeType, List<Upgrade>> upgradeByUpgradeType;

    public Faction(FactionData data) {
        this.Data = data;

        this.upgradeByButtonType = new Dictionary<GameButtonType, Upgrade>();
        this.upgradeByIdentifier = new Dictionary<UpgradeIdentifier, Upgrade>();
        this.upgradeByUpgradeType = new Dictionary<UpgradeType, List<Upgrade>>();

        this.InitializeStructures();
    }

    public void AddBuilding(Building building) {
        this.buildings.Add(building);

        this.buildingsByType[building.Type].Add(building);

        this.AddRootedPosition(building);
    }

    public void AddRootedPosition(Building building) {
        if (!building.Data.Rooted) {
            return;
        }

        BuildingType type = building.Type;

        if (this.rootedPositionsByType.ContainsKey(type)) {
            return;
        }

        if (building.Pivot == null) {
            return;
        }

        this.rootedPositionsByType[type] = building.Pivot.MapPosition;
    }

    public void AddUnit(Unit unit) {
        this.units.Add(unit);
    }

    public void AddUpgrade(Upgrade upgrade, int rank) {
        if (!this.upgradeByUpgradeType.ContainsKey(upgrade.Type)) {
            this.upgradeByUpgradeType[upgrade.Type] = new List<Upgrade>();
        }

        bool newAddition = true;

        foreach (Upgrade existingUpgrade in this.upgradeByUpgradeType[upgrade.Type]) {
            if (upgrade.Identifier == existingUpgrade.Identifier) {
                existingUpgrade.SetRankIndex(rank);
                newAddition = false;
            }
        }

        if (newAddition) {
            this.upgradeByUpgradeType[upgrade.Type].Add(upgrade);
            upgrade.SetRankIndex(rank);

            this.upgradeByButtonType[upgrade.ButtonType] = upgrade;
            this.upgradeByIdentifier[upgrade.Identifier] = upgrade;
        }
    }

    public List<Building> GetBuildings() {
        return this.buildings;
    }

    public List<Building> GetBuildings(BuildingType type) {
        return this.buildingsByType[type];
    }

    public GameButtonType GetButton(GameButtonType type) {
        if (this.upgradeByButtonType.ContainsKey(type)) {
            Upgrade upgrade = this.upgradeByButtonType[type];

            if (upgrade.NextRank == null) {
                return GameButtonType.None;
            }

            return upgrade.NextRank.ButtonType;
        }

        return type;
    }

    public Vector2Int GetRootedPosition(BuildingType type) {
        if (this.rootedPositionsByType.ContainsKey(type)) {
            return this.rootedPositionsByType[type];
        }

        return Vector2Int.zero;
    }

    public List<Unit> GetUnits() {
        return this.units;
    }

    public List<Unit> GetUnits(UnitTraitType role) {
        return this.unitsByRole[role];
    }

    public UpgradeRank GetUpgrade(UpgradeType type, UnitType unitType) {
        if (this.upgradeByUpgradeType.ContainsKey(type)) {
            foreach (Upgrade upgrade in this.upgradeByUpgradeType[type]) {
                if (upgrade.AppliesTo(unitType)) {
                    return upgrade.CurrentRank;
                }
            }
        }

        return null;
    }

    public bool HasUpgrade(UpgradeIdentifier identifier) {
        return this.upgradeByIdentifier.ContainsKey(identifier);
    }

    private void InitializeStructures() {
        this.buildings = new List<Building>();
        this.rootedPositionsByType = new Dictionary<BuildingType, Vector2Int>();
        this.units = new List<Unit>();

        this.unitsByRole = new Dictionary<UnitTraitType, List<Unit>>();

        UnitTraitType[] roles = (UnitTraitType[])Enum.GetValues(typeof(UnitTraitType));

        foreach (UnitTraitType role in roles) {
            if (role != UnitTraitType.None) {
                this.unitsByRole.Add(role, new List<Unit>());
            }
        }

        this.buildingsByType = new Dictionary<BuildingType, List<Building>>();
        BuildingType[] types = (BuildingType[])Enum.GetValues(typeof(BuildingType));

        foreach (BuildingType type in types) {
            if (type != BuildingType.None) {
                this.buildingsByType.Add(type, new List<Building>());
            }
        }
    }

    public bool IsEnemy(Faction faction) {
        return this.Data.IsEnemy(faction.Data.Identifier);
    }

    public void RemoveBuilding(Building building) {
        this.buildings.Remove(building);

        this.buildingsByType[building.Type].Remove(building);
    }

    public void RemoveUnit(Unit unit) {
        this.units.Remove(unit);
    }
}

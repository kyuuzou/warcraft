using System.Collections.Generic;
using UnityEngine;

public class UnitTraitSpellcaster : UnitTrait, IUnitTraitSpellcaster, IMovementListener {

    public UnitTraitDataSpellcaster Data { get; private set; }

    private Dictionary<SpellType, Spell> spellByType;

    private SpellType currentSpell;
    private IMovementDestination currentTarget;
    private MapTile currentTargetTile;
    private MapTile lastTargetTile;
    private Map map;

    private static Dictionary<GameButtonType, UpgradeIdentifier> identifierByButtonType;

    static UnitTraitSpellcaster() {
        UnitTraitSpellcaster.identifierByButtonType = new (){
            { GameButtonType.CloudOfPoison,     UpgradeIdentifier.CloudOfPoison },
            { GameButtonType.Daemon,            UpgradeIdentifier.OrcMajorSummoning },
            { GameButtonType.DarkVision,        UpgradeIdentifier.DarkVision },
            { GameButtonType.FarSeeing,         UpgradeIdentifier.FarSeeing },
            { GameButtonType.Healing,           UpgradeIdentifier.Healing },
            { GameButtonType.Invisibility,      UpgradeIdentifier.Invisibility },
            { GameButtonType.RainOfFire,        UpgradeIdentifier.RainOfFire },
            { GameButtonType.Scorpion,          UpgradeIdentifier.HumanMinorSummoning },
            { GameButtonType.Spider,            UpgradeIdentifier.OrcMinorSummoning },
            { GameButtonType.UnholyArmor,       UpgradeIdentifier.UnholyArmor },
            { GameButtonType.WaterElemental,    UpgradeIdentifier.HumanMajorSummoning },
        };
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Spellcaster; }
    }

    public void ApproachingTarget() {

    }

    private bool BeforeCast(SpellType spellType, IMovementDestination target, MapTile targetTile) {
        this.currentSpell = spellType;
        this.currentTarget = target;
        this.currentTargetTile = targetTile;

        if (this.spellByType.ContainsKey(spellType)) {
            return true;
        }

        Debug.LogWarning(string.Format("{0} does not have the spell: {1}", this.Unit.Type, spellType));
        return false;
    }

    private void Cast(SpellType spellType, IMovementDestination target, MapTile currentTile) {
        if (target is Unit) {
            this.Cast(spellType, (Unit)target, currentTile);
        } else if (target is Building) {
            this.Cast(spellType, (Building)target, currentTile);
        } else {
            this.Cast(spellType, (MapTile)target, currentTile);
        }
    }

    public void Cast(SpellType spellType, Building building, MapTile tile) {
        if (this.BeforeCast(spellType, building, tile)) {
            if (this.IsTargetInRange()) {
                this.spellByType[spellType].Cast(this.Unit, building, tile);
            } else {
                this.RecalculatePath();
            }
        }
    }

    public void Cast(SpellType spellType, MapTile tile) {
        if (this.BeforeCast(spellType, tile, tile)) {
            if (this.IsTargetInRange()) {
                this.spellByType[spellType].Cast(this.Unit, tile);
            } else {
                this.RecalculatePath();
            }
        }
    }

    public void Cast(SpellType spellType, Unit unit, MapTile tile) {
        if (this.BeforeCast(spellType, unit, tile)) {
            if (this.IsTargetInRange()) {
                this.spellByType[spellType].Cast(this.Unit, unit, tile);
            } else {
                this.RecalculatePath();
            }
        }
    }

    public override void FilterButtons(ref List<GameButtonType> buttons) {
        for (int i = 0; i < buttons.Count; i++) {
            if (UnitTraitSpellcaster.identifierByButtonType.ContainsKey(buttons[i])) {
                UpgradeIdentifier upgradeIdentifier = UnitTraitSpellcaster.identifierByButtonType[buttons[i]];

                if (!this.Unit.Faction.HasUpgrade(upgradeIdentifier)) {
                    buttons[i] = GameButtonType.None;
                }
            }
        }
    }

    public void Initialize(Unit unit, UnitTraitDataSpellcaster data) {
        base.Initialize(unit);

        this.Data = data;

        this.map = ServiceLocator.Instance.Map;

        this.spellByType = new Dictionary<SpellType, Spell>();

        foreach (Spell spell in this.Data.Spells) {
            this.spellByType[spell.Type] = spell;
            spell.Initialize();
        }
    }

    private bool IsTargetInRange() {
        int range = this.spellByType[this.currentSpell].Range;

        if (range == 0) {
            return true;
        }

        MapTile tile = this.Unit.GetRealTile();

        Vector2Int destination = this.map.FindClosestBoundary(tile, this.currentTarget);

        int distance = tile.MapPosition.EstimateDistance(destination);

        return (range >= distance - 1);
    }

    public bool IsTileTraversable(MapTile tile) {
        return tile.IsTraversable(this.Unit.GetTrait<IUnitTraitMoving>().MovementType, this.Unit);
    }

    public bool MayCast(SpellType type) {
        if (this.spellByType.ContainsKey(type)) {
            Spell spell = this.spellByType[type];

            return spell.ManaCost <= this.Unit.CurrentManaPoints;
        }

        return false;
    }

    public void OnOrderAccepted() {

    }

    public void ReachedTarget() {
        if (this.IsTargetInRange()) {
            this.Unit.Stop();
            this.Cast(this.currentSpell, this.currentTarget, this.currentTargetTile);
        } else {
            this.RecalculatePath();
        }
    }

    public void RecalculatePath() {
        this.Unit.Move(this.currentTarget, this, false, true);
        this.lastTargetTile = this.currentTarget.Pivot;
    }

    public bool RequiresTarget(SpellType type) {
        return this.spellByType[type].RequireTarget;
    }

    public void TileChanged() {
        if (this.IsTargetInRange()) {
            this.Unit.Stop();
            this.Cast(this.currentSpell, this.currentTarget, this.currentTargetTile);
        } else {
            if (this.currentTarget.Pivot == this.lastTargetTile) {
                // do nothing
            } else {
                this.RecalculatePath();
                this.lastTargetTile = this.currentTarget.Pivot;
            }
        }
    }
}

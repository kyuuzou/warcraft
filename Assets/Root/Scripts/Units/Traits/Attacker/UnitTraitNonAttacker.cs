using System.Collections.Generic;
using UnityEngine;

public class UnitTraitNonAttacker : UnitTrait, IUnitTraitAttacker {

    public UnitTraitDataNonAttacker Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public Color RangeColor {
        get { return Color.white; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public void Attack(SpawnableSprite target) {
        Debug.Log("Trying to attack without an attacker trait: " + this.Unit.Type);
    }

    public void Attack(MapTile target) {
        Debug.Log("Trying to attack without an attacker trait: " + this.Unit.Type);
    }

    public void AttackAfterCooldown() {

    }

    public List<MapTile> GetExtendedTilesInRange() {
        return new List<MapTile>();
    }

    public List<MapTile> GetTilesInRange() {
        return new List<MapTile>();
    }

    public void Initialize(Unit unit, UnitTraitDataNonAttacker data) {
        base.Initialize(unit);

        this.Data = data;
    }

    public void ManualUpdate() {

    }

    public void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {

    }

    public void OnAttack() {

    }

    public void OnOrderAccepted() {

    }
}

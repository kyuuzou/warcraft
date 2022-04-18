using System;
using UnityEngine;

public class InteractionModeAttacking : InteractionMode {

    private Unit unit;

    public InteractionModeAttacking() : base() {

    }

    private void ClickBuilding(Collider2D collider) {
        Building building = collider.GetComponent<Building>();

        this.unit.Attack(building);
        this.unit.PressCancel();

        InteractionHandler.Instance.SetMode(InteractionModeType.Regular);
    }

    private void ClickGround(Collider2D collider) {
        InteractionHandler.Instance.SetMode(InteractionModeType.Regular);
    }

    private void ClickUnit(Collider2D collider) {
        Unit unit = collider.GetComponent<Unit>();

        if (!unit.IsDead()) {
            this.unit.Attack(unit);
            this.unit.PressCancel();

            InteractionHandler.Instance.SetMode(InteractionModeType.Regular);
        }
    }

    public override void DisableMode() {

    }

    public override void EnableMode(InteractionModeArgs args = null) {
        if (args == null) {
            return;
        }

        InteractionModeAttackingArgs modeArgs = (InteractionModeAttackingArgs)args;
        this.unit = modeArgs.GetUnit();
    }

    protected override RaycastHit2D HandleClick() {
        RaycastHit2D hit = base.HandleClick();
        Collider2D collider = hit.collider;

        if (collider == null) {
            return hit;
        }

        switch (collider.tag) {
            case "Building":
                this.ClickBuilding(collider);
                break;

            case "Ground":
                this.ClickGround(collider);
                break;

            case "Unit":
                this.ClickUnit(collider);
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {collider.tag}");
        }

        return hit;
    }
}

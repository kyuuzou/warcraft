using System;
using UnityEngine;

public class InteractionModeDefault : InteractionMode {

    private SpawnableSprite selectedSprite = null;

    private void ClickBuilding(Collider2D collider) {
        SpawnableSprite selected = this.selectedSprite;

        if (selected) {
            selected.SetSelected(false);
        }

        this.selectedSprite = collider.GetComponent<Building>();
        this.selectedSprite.SetSelected(true);
    }

    private void ClickGround(Collider2D collider) {
        TileSlot slot = collider.GetComponent<TileSlot>();
        this.ClickGround(slot.Tile);
    }

    private void ClickGround(MapTile tile) {
        SpawnableSprite selected = this.selectedSprite;

        if (selected != null && selected.tag == "Unit") {
            Unit unit = (Unit)selected;
            unit.Move(tile);
        }
    }

    private void ClickUnit(Collider2D collider) {
        Unit unit = collider.GetComponent<Unit>();

        if (unit.IsDead()) {
            this.ClickGround(unit.Tile);
        } else {
            SpawnableSprite selected = this.selectedSprite;

            if (selected) {
                selected.SetSelected(false);
            }

            this.selectedSprite = unit;
            //this.selectedSprite.SetSelected(true);
            ServiceLocator.Instance.GameController.CurrentGroup.Set(true, this.selectedSprite);
        }
    }

    public override void DisableMode() {

    }

    public override void EnableMode(InteractionModeArgs args = null) {

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

            case "Untagged":
                // Ignore click
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {collider.tag}");
        }

        return hit;
    }

    public override void Update() {
        base.Update();
    }
}

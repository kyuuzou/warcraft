using System;
using UnityEngine;

public class InteractionModeHarvest : InteractionMode {

    private Unit harvester;

    public InteractionModeHarvest() : base() {

    }

    private void ClickBuilding(Collider2D collider) {
        Building building = collider.GetComponent<Building>();

        if (building.Type == BuildingType.GoldMine) {
            this.harvester.Mine(building);
            //this.harvester.PressCancel();

            InputHandler.Instance.SetMode(InteractionModeType.Regular);
        }
    }

    private void ClickGround(Collider2D collider) {
        TileSlot slot = collider.GetComponent<TileSlot>();

        if (slot.Tile.Type == TileType.Tree) {
            /*
            this.harvester.GetTrait<UnitTraitHarvester>(). (slot.Tile);
            this.harvester.PressCancel ();
            
            InputHandler.Instance.SetMode (InteractionModeType.Regular);*/
        }
    }

    public override void DisableMode() {

    }

    public override void EnableMode(InteractionModeArgs args = null) {
        if (args == null) {
            return;
        }

        InteractionModeHarvestArgs modeArgs = (InteractionModeHarvestArgs)args;
        this.harvester = modeArgs.GetHarvester();
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
        }

        return hit;
    }
}

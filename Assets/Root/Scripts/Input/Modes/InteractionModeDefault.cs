// #define DEBUG

using UnityEngine;
using System.Collections;

public class InteractionModeDefault : InteractionMode {

    private SpawnableSprite selectedSprite = null;

    public InteractionModeDefault () : base () {
        
    }
    
    private void ClickBuilding (Collider2D collider) {
        SpawnableSprite selected = this.selectedSprite;

        if (selected)
            selected.SetSelected (false);
        
        this.selectedSprite = collider.GetComponent<Building> ();
        this.selectedSprite.SetSelected (true);
    }
    
    private void ClickGround (Collider2D collider) {
        TileSlot slot = collider.GetComponent<TileSlot> ();
        this.ClickGround (slot.Tile);

#if DEBUG
        TileSlot tileSlot = collider.GetComponent<TileSlot>();
        Debug.Log (tileSlot.Tile.MapPosition, tileSlot.Tile.AtlasIndex);
#endif
    }

    private void ClickGround (MapTile tile) {
        SpawnableSprite selected = this.selectedSprite;

        if (selected != null && selected.tag == "Unit") {
            Unit unit = (Unit) selected;
            unit.Move(tile);
        }
    }
    
    private void ClickUnit (Collider2D collider) {
        Unit unit = collider.GetComponent<Unit> ();

        if (unit.IsDead ()) {
            this.ClickGround (unit.Tile);
        } else {
            SpawnableSprite selected = this.selectedSprite;

            if (selected)
                selected.SetSelected (false);
            
            this.selectedSprite = unit;
            this.selectedSprite.SetSelected (true);
        }
    }
    
    public override void DisableMode () {
        
    }
    
    public override void EnableMode (InteractionModeArgs args = null) {
        
    }
    
    protected override RaycastHit2D HandleClick () {
        RaycastHit2D hit = base.HandleClick ();
        Collider2D collider = hit.collider;
        
        if (collider == null)
            return hit;
        
        switch (collider.tag) {
            case "Building":
                this.ClickBuilding (collider);
                break;
            
            case "Ground":
                this.ClickGround (collider);
                break;
            
            case "Unit":
                this.ClickUnit (collider);
                break;
        }
        
        return hit;
    }
    
    public override void Update () {
        base.Update ();
    }
}

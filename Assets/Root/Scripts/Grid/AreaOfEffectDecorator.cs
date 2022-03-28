using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectDecorator : SceneObject {

    [SerializeField]
    private Color enemyColor;

    [SerializeField]
    private Color friendlyColor;

    private Grid grid;
    private Map map;

    private void ColorTile (MapTile tile, Color color) {
        /*
        for (int i = 0; i < 2; i ++) {
            SpriteRenderer renderer = tile.Slot.GetLayer (i).SpriteRenderer;

            //Mix with the color that was already there
            renderer.color = Color.Lerp (renderer.color, color, 0.5f);
        }

        Debug.Log("a", "b", tile);
        */
    }

    private void ColorExtendedTiles () {
        /*
        IList<Unit> units = this.map.Units;
        
        foreach (Unit unit in units) {
            IUnitTraitAttacker attackerTrait = unit.GetTrait<IUnitTraitAttacker> ();

            List<MapTile> extendedTilesInRange = attackerTrait.GetExtendedTilesInRange ();
            this.RemoveSlotlessTiles (extendedTilesInRange);
            
            foreach (MapTile tile in extendedTilesInRange) {
                tile.Slot.GetLayer (0).SpriteRenderer.color = Color.black;
            }
        }*/
    }

    private void ColorTiles () {
        /*
        IList<Unit> units = this.map.Units;

        foreach (Unit unit in units) {
            IUnitTraitAttacker attackerTrait = unit.GetTrait<IUnitTraitAttacker> ();
            List<MapTile> tilesInRange = attackerTrait.GetTilesInRange ();

            Color baseColor = unit.Faction.ControllingPlayer.Data.humanPlayer ? this.friendlyColor : this.enemyColor;

            if (unit.TargetTile == null) {
                this.RemoveSlotlessTiles (tilesInRange);

                foreach (MapTile tile in tilesInRange) {
                    this.ColorTile (tile, baseColor);
                }
            } else {
                float distance = Vector3.Distance (unit.Transform.position, unit.TargetTile.RealPosition);
                float delta = Mathf.InverseLerp (75.0f, 5.0f, distance);

                List<MapTile> nextTilesInRange = this.GetNextTilesInRange (tilesInRange, unit.Direction);

                this.RemoveSlotlessTiles (tilesInRange);
                this.RemoveSlotlessTiles (nextTilesInRange);

                foreach (MapTile tile in tilesInRange) {
                    if (nextTilesInRange.Contains (tile)) {
                        this.ColorTile (tile, baseColor);
                        nextTilesInRange.Remove (tile);
                    } else {
                        this.ColorTile (tile, Color.Lerp (baseColor, Color.white, delta));
                    }
                }

                foreach (MapTile tile in nextTilesInRange) {
                    this.ColorTile (tile, Color.Lerp (Color.white, baseColor, delta));
                }
            }

#if COLOR_EXTENDED_TILES
            this.ColorExtendedTiles ();
#endif
        }
        */
    }

    private List<MapTile> GetNextTilesInRange (List<MapTile> tilesInRange, Direction direction) {
        List<MapTile> nextTilesInRange = new List<MapTile> ();
        
        foreach (MapTile tile in tilesInRange) {
            MapTile neighbour = tile.GetNeighbour (direction);
            
            if (neighbour != null) {
                nextTilesInRange.Add (neighbour);
            }
        }

        return nextTilesInRange;
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        ServiceLocator locator = ServiceLocator.Instance;
        this.grid = locator.Grid;
        this.map = locator.Map;
    }

    private void LateUpdate () {
        //this.ColorTiles ();
	}

    private void RemoveSlotlessTiles (List<MapTile> tiles) {
        for (int i = tiles.Count - 1; i >= 0; i --) {
            if (tiles[i].Slot == null) {
                tiles.RemoveAt (i);
            }
        }
    }
}

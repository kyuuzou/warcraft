using System.Collections;
using UnityEngine;

public class HealingCrystal : Item {

    public override ItemIdentifier Identifier {
        get { return ItemIdentifier.HealingCrystal; }
    }

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private int hitPoints = 25;
    
    public override void InitializeExternals () {
        base.InitializeExternals ();

        this.SpriteRenderer = this.spriteRenderer;
    }

    private void OnTriggerEnter2D (Collider2D other) {
        Unit unit = other.GetComponent<Unit> ();

        if (unit != null && unit.Faction.ControllingPlayer.Data.HumanPlayer) {
            foreach (Unit groupUnit in this.GameController.CurrentGroup.Units) {
                groupUnit.Restore (this.hitPoints);
            }

            GameObject.Destroy (this.GameObject);
        }
    }

    public override void SetTile (MapTile tile) {
        base.SetTile (tile);

        this.Transform.position = tile.RealPosition;
    }

}

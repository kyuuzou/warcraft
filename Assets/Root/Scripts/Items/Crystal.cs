using System.Collections;
using UnityEngine;

public class Crystal : Item {

    public override ItemIdentifier Identifier {
        get { return ItemIdentifier.Crystal; }
    }

    [SerializeField]
    private Color lowValueColor;

    [SerializeField]
    private Color highValueColor;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Range (0.0f, 100.0f)]
    private float probabilityOfHighValue = 20.0f;

    private int value;
    
    public override void InitializeExternals () {
        base.InitializeExternals ();

        this.SpriteRenderer = this.spriteRenderer;

        int percentage = Random.Range (0, 100);

        if (percentage > probabilityOfHighValue) {
            this.value = 1;
            this.SpriteRenderer.color = lowValueColor;
        } else {
            this.value = 10;
            this.SpriteRenderer.color = highValueColor;
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        Unit unit = other.GetComponent<Unit> ();

        if (unit != null && unit.Faction.ControllingPlayer.Data.HumanPlayer) {
            this.GameController.IncreaseGold (this.value);
            GameObject.Destroy (this.GameObject);
        }
    }

    public override void SetTile (MapTile tile) {
        base.SetTile (tile);

        this.Transform.position = tile.RealPosition;
    }

}

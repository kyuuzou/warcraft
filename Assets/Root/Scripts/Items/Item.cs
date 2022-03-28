using System.Collections;
using UnityEngine;

public abstract class Item : CustomSprite {

    public abstract ItemIdentifier Identifier { get; }

    public Vector3 RelativePosition { get; set; }

    [SerializeField]
    [Range (0.0f, 100.0f)]
    private float probability = 20.0f;
    public float Probability {
        get { return this.probability; }
    }

    protected virtual void LateUpdate () {
        /*      if (this.Tile != null) {
                  Vector3 position = this.Tile.RealPosition;
                  */
        /*
        position.y = position.y - this.Grid.DefaultSlotSize.y * 0.5f + this.SpriteRenderer.bounds.size.y * 0.5f;
        position.y -= 47.0f;
        */

        /*if (this.Tile.Visible) {
            Vector3 tilePosition = this.Tile.Slot.GetLayer (1).Transform.position;
            position.y = tilePosition.y + 25.0f;
            position.z = tilePosition.z - 0.1f;
        }

        this.Transform.position = position + this.RelativePosition;
    }*/
    }
}

using System.Collections;
using UnityEngine;

public class HealingTile : Item {

    public override ItemIdentifier Identifier {
        get { return ItemIdentifier.HealingTile; }
    }

    public UnitType UnitType { get; set; }

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private int price = 3;

    private ScrollPricePool scrollPricePool;

    private void Flicker () {
        this.StartCoroutine (this.FlickerCoroutine ());
    }

    private IEnumerator FlickerCoroutine () {
        float delta;

        Color first = Color.white;
        Color second = Color.grey;

        for (int i = 0; i < 4; i ++) {
            delta = 0.0f;

            do {
                this.sprite.color = Color.Lerp (first, second, delta);

                delta += 4.0f * Time.deltaTime;

                yield return null;
            } while (delta < 1.0f);

            Color aux = first;
            first = second;
            second = aux;
        }
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.scrollPricePool = serviceLocator.GetPool<ScrollPricePool> ();
    }

    private void OnTriggerEnter2D (Collider2D other) {
        Unit unit = other.GetComponent<Unit> ();

        if (unit == null || ! unit.Selected) {
            return;
        }
    
        if (! unit.Faction.ControllingPlayer.Data.HumanPlayer) {
            return;
        }

        if (this.GameController.GetGold () < this.price) {
            this.scrollPricePool.Scroll (this.Transform, this.price, false);
        } else {
            this.GameController.DecreaseGold (this.price);
                
            this.Flicker ();
            this.scrollPricePool.Scroll (this.Transform, this.price, true);

            foreach (Unit groupUnit in unit.Group.Units) {
                groupUnit.Restore (groupUnit.Data.HitPoints - groupUnit.CurrentHitPoints);
            }
        }
    }
}

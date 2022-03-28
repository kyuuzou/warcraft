using System.Collections;
using UnityEngine;

public class SpawnTile : Item {

    public override ItemIdentifier Identifier {
        get { return ItemIdentifier.SpawnTile; }
    }

    public UnitType UnitType { get; set; }

    [SerializeField]
    private SpriteRenderer sprite;

    private ScrollPricePool scrollPricePool;
    private SpawnFactory spawnFactory;

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
        this.spawnFactory = serviceLocator.SpawnFactory;
    }

    private void OnTriggerEnter2D (Collider2D other) {
        Unit unit = other.GetComponent<Unit> ();

        if (unit == null || ! unit.Selected) {
            return;
        }
    
        if (! unit.Faction.ControllingPlayer.Data.HumanPlayer) {
            return;
        }

        UnitData spawnedData = this.spawnFactory.GetData (this.UnitType);

        if (this.GameController.GetGold () < spawnedData.Cost) {
            this.scrollPricePool.Scroll (this.Transform, spawnedData.Cost, false);
        } else {
            this.GameController.DecreaseGold (spawnedData.Cost);
                
            this.Flicker ();
            this.scrollPricePool.Scroll (this.Transform, spawnedData.Cost, true);
            this.SpawnUnit (unit);
        }
    }

    private void SpawnUnit (Unit spawner) {
        Unit lastUnit = spawner.Group.GetLastUnit ();
        MapTile tile = lastUnit.Tile.GetNeighbour (lastUnit.Direction.Invert ());

        Unit spawnedUnit = this.spawnFactory.SpawnUnit (this.UnitType, spawner.Faction, tile.MapPosition);
        spawnedUnit.Group = spawner.Group;
        spawner.Group.Add (spawnedUnit);
        spawnedUnit.SetSelected (true);

        spawnedUnit.Move (lastUnit.Tile);

        Vector3 lastUnitRelativePosition = lastUnit.GetTrait<IUnitTraitMoving> ().RelativePosition;
        spawnedUnit.GetTrait<IUnitTraitMoving> ().RelativePosition = lastUnitRelativePosition;
    }
}

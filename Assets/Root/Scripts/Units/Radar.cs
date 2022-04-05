using UnityEngine;

public class Radar : SceneObject {

    public Unit Unit { get; set; }

    [SerializeField]
    private int maximumDistance;

    private bool searching = true;
    public bool Searching {
        get { return this.searching; }
        set {
            if (value && !this.GetComponent<Collider2D>().enabled) {
                this.GetComponent<Collider2D>().enabled = true;
            }

            this.searching = value;
        }
    }

    private void OnDetect(Collider collider) {
        //if (this.unit.HasTrait (UnitTraitType.Attacker)) {
        Transform parent = collider.transform.parent;

        Unit targetUnit = parent.GetComponent<Unit>();
        Vector2Int position = this.Unit.Tile.MapPosition;
        Vector2Int targetPosition;

        if (targetUnit == null) {
            Building targetBuilding = parent.GetComponent<Building>();
            targetPosition = targetBuilding.Tile.MapPosition;

            if (position.EstimateDistance(targetPosition) < this.maximumDistance) {
                this.Unit.Detect(targetBuilding);
            }
        } else {
            if (targetUnit.IsDead() || this.Unit.IsDead()) {
                return;
            }

            if (targetUnit.Tile == null) {
                return;
            }

            targetPosition = targetUnit.Tile.MapPosition;

            if (position.EstimateDistance(targetPosition) < this.maximumDistance) {
                this.Unit.Detect(targetUnit);
            }
        }
        //}
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null && unit != this.Unit && !unit.Dead && !this.Unit.Dead) {
            this.Unit.Attack(unit);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (this.Searching) {
            this.OnDetect(collider);
        }
    }

    private void OnTriggerStay(Collider collider) {
        if (this.Searching) {
            this.OnDetect(collider);
        }
    }

    public void SetEnabled(bool enabled) {
        this.Collider.enabled = enabled;
    }
}

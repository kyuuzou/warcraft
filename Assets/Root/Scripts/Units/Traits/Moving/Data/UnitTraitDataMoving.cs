using UnityEngine;

public class UnitTraitDataMoving : UnitTraitData {

    [SerializeField]
    private MovementType movementType;
    public MovementType MovementType {
        get { return this.movementType; }
    }

    [SerializeField]
    private float speed = 50.0f;
    public float Speed {
        get { return this.speed * 2.0f; }
    }

    [SerializeField]
    private bool mayMoveDiagonally = true;
    public bool MayMoveDiagonally {
        get { return this.mayMoveDiagonally; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Moving; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitMoving trait = unit.gameObject.AddComponent<UnitTraitMoving>();
        trait.Initialize(unit, UnitTraitDataMoving.Instantiate(this));

        unit.SetTrait<IUnitTraitMoving>(trait);

        return trait;
    }
}

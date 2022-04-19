using UnityEngine;

public class UnitTraitDataMender : UnitTraitData {

    [SerializeField]
    private float mendCooldown = 1.0f;
    public float MendCooldown {
        get { return this.mendCooldown; }
    }

    [SerializeField]
    private int mendRange;
    public int MendRange {
        get { return this.mendRange; }
    }

    [SerializeField]
    private int minimumMendAmount;
    public int MinimumMendAmount {
        get { return this.minimumMendAmount; }
    }

    [SerializeField]
    private int randomMendAmount;
    public int RandomMendAmount {
        get { return this.randomMendAmount; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Mender; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitMender trait = unit.gameObject.AddComponent<UnitTraitMender>();
        trait.Initialize(unit, UnitTraitDataMender.Instantiate(this));

        unit.SetTrait<IUnitTraitMender>(trait);

        return trait;
    }
}

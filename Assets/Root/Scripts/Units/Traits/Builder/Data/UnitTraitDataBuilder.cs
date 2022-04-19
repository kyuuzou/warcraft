using UnityEngine;

public class UnitTraitDataBuilder : UnitTraitData {

    [SerializeField]
    private AudioIdentifier workCompleteSound;
    public AudioIdentifier WorkCompleteSound {
        get { return this.workCompleteSound; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Builder; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitBuilder trait = unit.gameObject.AddComponent<UnitTraitBuilder>();
        trait.Initialize(unit, UnitTraitDataBuilder.Instantiate(this));

        unit.SetTrait<IUnitTraitBuilder>(trait);

        return trait;
    }
}

using System.Collections;
using UnityEngine;

public class UnitTraitDecaying : UnitTrait, IUnitTraitDecaying {

    public UnitTraitDataDecaying Data { get; private set; }

    public override UnitTraitType Type {
        get { return UnitTraitType.Decaying; }
    }

    public override void Activate() {
        base.Activate();

        this.StartCoroutine(this.Decay());
    }

    private IEnumerator Decay() {
        do {
            this.Unit.SpendMana(1);

            yield return new WaitForSeconds(this.Data.DecayRate / 45.0f);
        } while (this.Unit.CurrentManaPoints > 0);

        this.Unit.Die();
    }

    public void Initialize(Unit unit, UnitTraitDataDecaying data) {
        base.Initialize(unit);

        this.Data = data;
    }

    public void OnOrderAccepted() {

    }
}

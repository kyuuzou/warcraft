using System;
using UnityEngine;

public class UnitTraitNonInteractive : UnitTrait, IUnitTraitInteractive {

    public UnitTraitDataNonInteractive Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Interactive; }
    }
    
    public void Initialize (Unit unit, UnitTraitDataNonInteractive data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public void Interact (IUnitTraitInteractive trait) {
        Debug.Log ("Trying to attack without an interactive trait: " + this.Unit.Type);
    }

    public void Interact (MapTile tile) {
        Debug.Log ("Trying to attack without an interactive trait: " + this.Unit.Type);
    }

    public void OnOrderAccepted () {

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class UnitTraitInteractive : UnitTrait, IUnitTraitInteractive {

    public UnitTraitDataInteractive Data { get; private set; }

    public override UnitTraitType Type {
        get { return UnitTraitType.Interactive; }
    }

    public override void Deactivate () {
        base.Deactivate ();

    }
    
    public void Initialize (Unit unit, UnitTraitDataInteractive data) {
        base.Initialize (unit);

        this.Data = data;
    }

    public void Interact (IUnitTraitInteractive trait) {
    }

    public void Interact (MapTile tile) {
    }

    public void OnOrderAccepted () {
        
    }
}

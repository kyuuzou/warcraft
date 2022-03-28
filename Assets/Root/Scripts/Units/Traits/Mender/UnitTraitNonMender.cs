using System;
using UnityEngine;

public class UnitTraitNonMender : UnitTrait, IUnitTraitMender {

    public UnitTraitDataNonMender Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Mender; }
    }
    
    public void Initialize (Unit unit, UnitTraitDataNonMender data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public void Mend (Building building) {
        this.Warning ();
    }
    
    public void Mend (Decoration decoration) {
        this.Warning ();
    }
    
    public void MendAfterCooldown () {
        
    }
    
    public void OnAnimationTrigger (AnimationType animationType, AnimationTriggerType triggerType) {

    }

    public void OnMend () {

    }

    public void OnOrderAccepted () {
        
    }
    
    private void Warning () {
        Debug.LogWarning (
            string.Format (
                "Trying to mend on '{0}', but it does not have the Mender trait.",
                this.Unit.Type
            )
        );
    }
}

using System;
using UnityEngine;

public class UnitTraitNonShooter : UnitTrait, IUnitTraitShooter {

    public UnitTraitDataNonShooter Data { get; private set; }

    public override bool IsNullObject {
        get { return true; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Shooter; }
    }
    
    public void Initialize (Unit unit, UnitTraitDataNonShooter data) {
        base.Initialize (unit);
        
        this.Data = data;
    }

    public void OnOrderAccepted () {
        
    }
    
    public void Shoot (IShootingListener listener, Projectile projectilePrefab, ITarget target) {
        Debug.LogWarning (
            string.Format (
                "Trying to shoot '{0}' on '{1}', but it does not have the Shooter trait.",
                projectilePrefab.name,
                this.Unit.Type
            )
        );
    }
}

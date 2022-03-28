﻿using System;
using System.Collections;
using UnityEngine;

public class UnitTraitDataRangedAttacker : UnitTraitDataAttacker {

    [SerializeField]
    private Projectile projectilePrefab;
    public Projectile ProjectilePrefab {
        get { return this.projectilePrefab; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }
    
    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitRangedAttacker trait = unit.gameObject.AddComponent<UnitTraitRangedAttacker> ();
        trait.Initialize (unit, (UnitTraitDataRangedAttacker) UnitTraitDataRangedAttacker.Instantiate (this));

        unit.SetTrait<IUnitTraitAttacker> (trait);
        
        return trait;
    }
}

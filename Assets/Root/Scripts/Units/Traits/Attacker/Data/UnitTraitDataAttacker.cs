using System.Collections;
using UnityEngine;

public class UnitTraitDataAttacker : UnitTraitData {

    [SerializeField]
    private float attackCooldown = 1.0f;
    public float AttackCooldown {
        get { return this.attackCooldown; }
    }
    
    [SerializeField]
    private int attackRange;
    public int AttackRange {
        get { return this.attackRange; }
    }
    
    [SerializeField]
    private AudioIdentifier attackSound;
    public AudioIdentifier AttackSound {
        get { return this.attackSound; }
    }
    
    [SerializeField]
    private int minimumDamage;
    public int MinimumDamage {
        get { return this.minimumDamage; }
    }

    [SerializeField]
    private int randomDamage;
    public int RandomDamage {
        get { return this.randomDamage; }
    }

    [SerializeField]
    [TextArea ()]
    private string rangePattern;
    public string RangePattern {
        get { return this.rangePattern; }
    }

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public override UnitTrait AddTrait (Unit unit) {
        UnitTraitAttacker trait = unit.gameObject.AddComponent<UnitTraitAttacker> ();
        trait.Initialize (unit, UnitTraitDataAttacker.Instantiate (this));
        
        unit.SetTrait<IUnitTraitAttacker> (trait);

        return trait;
    }
}

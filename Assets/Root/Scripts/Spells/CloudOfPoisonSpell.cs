using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudOfPoisonSpell : Spell {

    [SerializeField]
    private CloudOfPoisonProjectile projectilePrefab;

    public override SpellType Type {
        get { return SpellType.CloudOfPoison; }
    }

    public override void Cast (Unit caster, Building target, MapTile tile) {
        this.Cast (caster, tile);
    }

    public override void Cast (Unit caster, MapTile target) {
        this.Initialize ();

        caster.FindDirection (caster.Tile, target);
        caster.Play (AnimationType.Attacking);

        caster.SpendMana (this.ManaCost);

        CloudOfPoisonProjectile projectile = CloudOfPoisonProjectile.Instantiate<CloudOfPoisonProjectile> (
            this.projectilePrefab
        );
        
        projectile.Initialize (target);
        projectile.Activate ();
    }

    public override void Cast (Unit caster, Unit target, MapTile tile) {
        this.Cast (caster, tile);
    }
}

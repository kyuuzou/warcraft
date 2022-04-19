using UnityEngine;

public class FarSeeingSpell : Spell {

    [SerializeField]
    private Projectile projectilePrefab;

    public override SpellType Type {
        get { return SpellType.FarSeeing; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {
        this.Cast(caster, tile);
    }

    public override void Cast(Unit caster, MapTile target) {
        caster.FindDirection(caster.Tile, target);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        Projectile projectile = Projectile.Instantiate<Projectile>(this.projectilePrefab);
        projectile.Tile = target;

        projectile.Activate();

        target.Discover();
    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        this.Cast(caster, tile);
    }
}

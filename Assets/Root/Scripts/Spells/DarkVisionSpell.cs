using UnityEngine;

public class DarkVisionSpell : Spell {

    [SerializeField]
    private Projectile projectilePrefab;

    public override SpellType Type {
        get { return SpellType.DarkVision; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {
        this.Cast(caster, tile);
    }

    public override void Cast(Unit caster, MapTile target) {
        caster.FindDirection(caster.Tile, target);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        Projectile projectile = Projectile.Instantiate<Projectile>(this.projectilePrefab);
        projectile.transform.position = target.RealPosition;
        projectile.transform.SetZ((int)DepthLayer.Projectiles);
        projectile.Activate();

        target.Discover();
    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        this.Cast(caster, tile);
    }
}

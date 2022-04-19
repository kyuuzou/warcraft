using UnityEngine;

public class HealingSpell : Spell {

    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private int healingAmount = 50;

    public override SpellType Type {
        get { return SpellType.Healing; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {

    }

    public override void Cast(Unit caster, MapTile target) {

    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        caster.FindDirection(caster.Tile, tile);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        Projectile projectile = Projectile.Instantiate<Projectile>(this.projectilePrefab);
        projectile.transform.parent = target.Transform;
        projectile.transform.position = tile.RealPosition;
        projectile.transform.SetZ((int)DepthLayer.Projectiles);
        projectile.Activate();

        target.Restore(this.healingAmount);
    }

    public override void Deactivate() {

    }
}

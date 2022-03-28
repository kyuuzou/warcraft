using System.Collections;
using UnityEngine;

public abstract class SummonSpell : Spell {

    [SerializeField]
    private UnitType unitType;

    [SerializeField]
    private bool multiple = false;

    [SerializeField]
    private Projectile projectilePrefab;

    private Map map;
    private SpawnFactory spawnFactory;

    public override void Cast (Unit caster, Building target, MapTile tile) {

    }

    public override void Cast (Unit caster, MapTile target) {
        this.Initialize ();

        caster.FindDirection (caster.Tile, target);
        caster.Play (AnimationType.Attacking);

        do {
            caster.SpendMana (this.ManaCost);

            MapTile closest = this.map.FindClosestTraversableTile (target, MovementType.Land);

            Unit unit = this.spawnFactory.SpawnUnit (this.unitType, caster.Faction, closest.MapPosition);
            unit.Decay ();

            Projectile projectile = Projectile.Instantiate<Projectile> (this.projectilePrefab);
            projectile.transform.parent = unit.Transform;
            projectile.transform.position = closest.RealPosition;
            projectile.transform.SetZ ((int) DepthLayer.Projectiles);
            projectile.Tile = closest;
            projectile.Activate ();
        } while (this.multiple && caster.CurrentManaPoints >= this.ManaCost);
    }

    public override void Cast (Unit caster, Unit target, MapTile tile) {

    }

    public override void Initialize () {
        base.Initialize ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.map = serviceLocator.Map;
        this.spawnFactory = serviceLocator.SpawnFactory;
    }
}

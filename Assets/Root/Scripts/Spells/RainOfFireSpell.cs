using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfFireSpell : Spell {

    [SerializeField]
    private RainOfFireProjectile projectilePrefab;

    private Map map;

    public override SpellType Type {
        get { return SpellType.RainOfFire; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {
        this.Cast(caster, tile);
    }

    public override void Cast(Unit caster, MapTile target) {
        this.Initialize();

        caster.FindDirection(caster.Tile, target);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        CoroutineSurrogate.Instance.StartCoroutine(this.CastWaves(caster, target));
    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        this.Cast(caster, tile);
    }

    private IEnumerator CastWaves(Unit caster, MapTile target) {
        List<MapTile> area = this.map.GetCircularArea(target, 3);
        List<MapTile> possibleTiles = new List<MapTile>(area);

        for (int wave = 0; wave < 3; wave++) {
            possibleTiles.AddRange(area);

            for (int i = 0; i < 5; i++) {
                int index = Random.Range(0, possibleTiles.Count);

                MapTile tile = possibleTiles[index];
                possibleTiles.RemoveAt(index);

                RainOfFireProjectile projectile = RainOfFireProjectile.Instantiate<RainOfFireProjectile>(
                    this.projectilePrefab
                );

                projectile.Initialize(tile);
                projectile.Activate();
            }

            possibleTiles.Clear();

            yield return new WaitForSeconds(1.0f);
        }

        if (caster.MayCast(this.Type)) {
            caster.Cast(this.Type, target);
        }
    }

    public override void Initialize() {
        if (this.Initialized) {
            return;
        }

        base.Initialize();

        this.map = ServiceLocator.Instance.Map;
    }
}

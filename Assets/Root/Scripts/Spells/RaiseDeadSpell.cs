public class RaiseDeadSpell : Spell {

    private SpawnFactory spawnFactory;

    public override SpellType Type {
        get { return SpellType.RaiseDead; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {

    }

    public override void Cast(Unit caster, MapTile target) {

    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        caster.FindDirection(caster.Tile, tile);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        target.ManualDestroy();

        Unit unit = this.spawnFactory.SpawnUnit(UnitType.OrcSkeleton, caster.Faction, tile.MapPosition);
        unit.Play(AnimationType.Resurrect);
    }

    public override void Deactivate() {

    }

    public override void Initialize() {
        base.Initialize();

        this.spawnFactory = ServiceLocator.Instance.SpawnFactory;
    }
}

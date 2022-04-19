public class UnitTraitDataMiner : UnitTraitData {

    public override UnitTraitType Type {
        get { return UnitTraitType.Miner; }
    }

    public override UnitTrait AddTrait(Unit unit) {
        UnitTraitMiner trait = unit.gameObject.AddComponent<UnitTraitMiner>();
        trait.Initialize(unit, UnitTraitDataMiner.Instantiate(this));

        unit.SetTrait<IUnitTraitMiner>(trait);

        return trait;
    }
}

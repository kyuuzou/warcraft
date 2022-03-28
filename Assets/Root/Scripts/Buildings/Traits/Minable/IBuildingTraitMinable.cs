
public interface IBuildingTraitMinable : IBuildingTrait {

    bool IsMinable ();

    int Mine (IUnitTraitMiner miner);
}

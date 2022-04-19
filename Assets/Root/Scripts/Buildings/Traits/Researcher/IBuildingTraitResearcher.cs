
public interface IBuildingTraitResearcher : IBuildingTrait {

    void Research(Upgrade upgrade, int rank = 0);

    void Research(UpgradeIdentifier identifier, int rank = 0);
}

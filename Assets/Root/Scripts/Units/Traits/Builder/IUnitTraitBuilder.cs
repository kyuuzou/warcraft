
public interface IUnitTraitBuilder : IUnitTrait {

    void Build(BuildingType type);

    void MoveToConstructionSite(Building building);

    void OnWorkComplete();

    void ShowAdvancedStructures();

    void ShowBasicStructures();
}


public interface IUnitTrait : ISpawnableTrait {

    bool IsNullObject { get; }
    UnitTraitType Type { get; }
    Unit Unit { get; }

    void OnOrderAccepted ();
}


public interface IUnitTraitInteractive : IUnitTrait {

    void Interact(IUnitTraitInteractive trait);

    void Interact(MapTile tile);
}

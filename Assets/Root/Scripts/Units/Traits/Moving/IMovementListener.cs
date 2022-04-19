public interface IMovementListener {

    void ApproachingTarget();
    bool IsTileTraversable(MapTile tile);
    void ReachedTarget();
    void TileChanged();
}

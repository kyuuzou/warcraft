using System.Collections;
using UnityEngine;

public interface IMovementDestination {

    /// <summary>
    /// The main tile (usually the top left corner)
    /// </summary>
    MapTile Pivot { get; }

    Vector2Int TileSize { get; }

    Vector2Int[] GetBoundaries ();

    bool IsAdjacent (IMovementDestination destination);
    
    bool Overlaps (IMovementDestination destination);
}

using System.Collections;
using UnityEngine;

public interface IMovementDestination {

    /// <summary>
    /// The main tile (usually the top left corner)
    /// </summary>
    MapTile Pivot { get; }

    IntVector2 TileSize { get; }

    IntVector2[] GetBoundaries ();

    bool IsAdjacent (IMovementDestination destination);
    
    bool Overlaps (IMovementDestination destination);
}

using System.Collections.Generic;
using UnityEngine;

public interface IUnitTraitMoving : IUnitTrait, IMovementListener {

    bool MayMoveDiagonally { get; }

    MovementType MovementType { get; }

    Vector3 RelativePosition { get; set; }

    void ChangePath(List<MapTile> waypoints);

    void LateManualUpdate();

    void ManualUpdate();

    void Move(
        IMovementDestination destination, IMovementListener movementListener, bool overlapTarget, bool recalculation
    );

    void OnGroupChanged();
    void RefreshPosition();
    void SetDestination(MapTile tile);
}

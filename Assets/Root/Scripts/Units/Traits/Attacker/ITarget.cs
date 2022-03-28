using System.Collections;
using UnityEngine;

public interface ITarget : IMovementDestination {

    bool PhasedOut { get; }

    MapTile TargetTile { get; }

    MapTile Tile { get; }

    void AddDeathListener (IDeathListener listener);

    void AddPhasedOutListener (IPhasedOutListener listener);

    void Damage (int damage);

    MapTile GetClosestTile ();

    MapTile GetRealTile ();

    bool IsDead ();

    void RemoveDeathListener (IDeathListener listener);

    void RemovePhasedOutListener (IPhasedOutListener listener);
}

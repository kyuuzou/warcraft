using UnityEngine;

public class DirectionDictionary : InspectorDictionary<DirectionDictionary, Direction, DirectionData> {

    public Direction GetKey(Vector2Int offset) {
        foreach (DirectionData entry in this.Entries) {
            if (entry.Offset == offset) {
                return entry.Key;
            }
        }

        return Direction.None;
    }
}

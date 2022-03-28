using UnityEngine;

public class DirectionDictionary : InspectorDictionary<DirectionDictionary, Direction, DirectionData> {

    public Direction GetKey (IntVector2 offset) {
        foreach (DirectionData entry in this.Entries) {
            if (entry.Offset == offset) {
                return entry.Key;
            }
        }
        
        return Direction.None;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PathfindingAlgorithm : MonoBehaviour {
    
    protected Map map;
    
    public abstract IEnumerator FindPath (
        Task parent,
        Unit unit,
        Vector2Int origin,
        Vector2Int destination,
        IMovementListener movementListener,
        bool overlapTarget = true
    );

    protected virtual void Awake () {
        this.map = ServiceLocator.Instance.Map;
    }
}

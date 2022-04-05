using System.Collections;
using UnityEngine;

[System.Serializable]
public class Muzzle {

    [SerializeField]
    private string name;

    [SerializeField]
    private Direction direction;
    public Direction Direction {
        get { return this.direction; }
    }

    [SerializeField]
    private Vector2Int position;
    public Vector2Int Position {
        get { return this.position; }
    }
}

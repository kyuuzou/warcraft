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
    private IntVector2 position;
    public IntVector2 Position {
        get { return this.position; }
    }
}

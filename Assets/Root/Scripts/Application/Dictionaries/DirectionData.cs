using System.Collections;
using UnityEngine;

public class DirectionData : ScriptableObject, IInspectorDictionaryEntry<Direction> {

    [SerializeField]
    private Direction identifier;
    public Direction Identifier {
        get { return this.identifier; }
    }

    public Direction Key {
        get { return this.Identifier; }
    }

    [SerializeField]
    private Vector2Int offset;
    public Vector2Int Offset {
        get { return this.offset; }
    }

    /// <summary>
    /// The multiplier the sprite's local scale's x axis should be multiplied by.
    /// </summary>
    [SerializeField]
    private int spriteMultiplier;
    public int SpriteMultiplier {
        get { return this.spriteMultiplier; }
    }

    [SerializeField]
    private int spriteOffset;
    public int SpriteOffset {
        get { return this.spriteOffset; }
    }

    public Vector2Int OppositeOffset {
        get { return this.Offset * -1; }
    }
}

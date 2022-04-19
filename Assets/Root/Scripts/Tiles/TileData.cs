using UnityEngine;

public class TileData : CustomScriptableObject {

    [SerializeField]
    private TileType type;
    public TileType Type {
        get { return this.type; }
        set { this.type = value; }
    }

    [SerializeField]
    private int variation;
    public int Variation {
        get { return this.variation; }
        set { this.variation = value; }
    }

    [SerializeField]
    private Color color = Color.white;
    public Color Color {
        get { return this.color; }
        set { this.color = value; }
    }

    [SerializeField]
    private bool diffuse;
    public bool Diffuse {
        get { return this.diffuse; }
        set { this.diffuse = value; }
    }

    /// <summary>
    /// Height of the tile, in tiles.
    /// This is used to calculate the height of a map tile, to check how high a unit is, compared to its surroundings.
    /// A wall has a height of 1, while a rug has a height of 0, for example.
    /// </summary>
    [SerializeField]
    private int height;
    public int Height {
        get { return this.height; }
        set { this.height = value; }
    }

    [SerializeField]
    private bool traversable = true;
    public bool Traversable => this.traversable;

    /// <summary>
    /// The neighbours pattern that leads to this type of tile. To be filled only if applicable.
    /// This is used on roads and trees, for example.
    /// Multiple patterns should be separated by commas
    /// </summary>
    [SerializeField]
    private string pattern;
    public string Pattern {
        get { return this.pattern; }
    }

    public string Atlas { get; set; }

    /// <summary>
    /// The corresponding index on an atlas. To be filled only if there's a need to know the atlas index
    /// of this type, because then you're forced to use a separate TileData variation for other atlas indexes
    /// that otherwise share all other field values of this class.
    /// This is used on chopped trees, for example.
    /// </summary>
    [SerializeField]
    private int atlasIndex;
    public int AtlasIndex {
        get { return this.atlasIndex; }
    }

    /// <summary>
    /// The next stage for this kind of tile, if applicable.
    /// Used for the various stages of destruction of walls, for example.
    /// </summary>
    [SerializeField]
    private TileData nextStage;
    public TileData NextStage {
        get { return this.nextStage; }
    }

    /// <summary>
    /// Types of tiles that are affected by this tile.
    /// Example: Both parts of horizontal doors get damaged when one of them is hit.
    /// </summary>
    [SerializeField]
    private TileData[] dependencies;
    public TileData[] Dependencies {
        get { return this.dependencies; }
    }

    /// <summary>
    /// Only necessary when this sprite is not part of an atlas.
    /// </summary>
    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite {
        get { return this.sprite; }
        set { this.sprite = value; }
    }

    /// <summary>
    /// Sprite offset, set on Tiled's tileset Offset property.
    /// </summary>
    public Vector2 Offset { get; set; }

    public string ImageSource { get; set; }
}

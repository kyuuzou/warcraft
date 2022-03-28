using UnityEngine;

[System.Serializable]
public class DecorationData : SpawnableSpriteData {

    [Header ("Decoration data")]

    [SerializeField]
    private DecorationType type = DecorationType.None;
    public DecorationType Type {
        get { return this.type; }
    }

    [SerializeField]
    private AudioIdentifier placementSound;
    public AudioIdentifier PlacementSound {
        get { return this.placementSound; }
    }

    /// <summary>
    /// Whether this decoration should spawn even if it is explicitly told to do so.
    /// </summary>
    [SerializeField]
    private bool spawnable = true;
    public bool Spawnable {
        get { return this.spawnable; }
    }
    
    /// <summary>
    /// The tile type that should spawn this decoration, if applicable.
    /// </summary>
    [SerializeField]
    private TileType tileType;
    public TileType TileType {
        get { return this.tileType; }
    }
    
    [Header ("Textures")]

    [SerializeField]
    private Texture dungeonTexture;

    [SerializeField]
    private Texture forestTexture;
    
    [SerializeField]
    private Texture snowTexture;
    
    [SerializeField]
    private Texture swampTexture;
    
    public Texture GetTexture (MapType type) {
        Texture texture = null;

        switch (type) {
            case MapType.Dungeon:
                texture = this.dungeonTexture;
                break;

            case MapType.Forest:
                texture = this.forestTexture;
                break;
        
            case MapType.Snow:
                texture = this.snowTexture;
                break;

            case MapType.Swamp:
                texture = this.swampTexture;
                break;
        }

        return texture;
    }
}

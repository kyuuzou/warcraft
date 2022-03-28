using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTypeData : CustomScriptableObject, IInspectorDictionaryEntry<MapType> {

    [SerializeField]
    private MapType type;
    public MapType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Atlas atlas;
    public Atlas Atlas {
        get { return this.atlas; }
    }

    [SerializeField]
    private TextAsset layout;
    public TextAsset Layout {
        get { return this.layout; }
    }

    [SerializeField]
    private TileData[] tileData;

    private Dictionary<TileType, Dictionary<int, TileData>> dataByTileType;
    private Dictionary<int, TileData> tileDataByIndex;

    public MapType Key {
        get { return this.Type; }
    }

    public List<int> GetIndexesOfType (TileType type) {
        List<int> indexes = new List<int> ();
        
        foreach (int key in this.tileDataByIndex.Keys) {
            TileData data = this.tileDataByIndex[key];

            TileType tileType = data.Type;
            
            if (tileType == type) {
                indexes.Add (key);
            }
        }

        return indexes;
    }

    public TileData GetTileData (TileType type, int variation) {
        if (! this.dataByTileType.ContainsKey (type)) {
            return null;
        }

        Dictionary<int, TileData> variations = this.dataByTileType[type];

        if (variations.ContainsKey (variation)) {
            return variations[variation];
        }

        return null;
    }

    public TileData GetTileDataForIndex (int index) {
        if (this.tileDataByIndex == null || ! this.tileDataByIndex.ContainsKey (index)) {
#if WARNING
            Debug.LogWarning (string.Format ("No tile data set for {0}", index));
#endif
            return null;
        }
        
        return this.tileDataByIndex[index];
    }
    
    public TileData GetTileDataForPattern (TileType type, string pattern) {
        if (! this.dataByTileType.ContainsKey (type)) {
            return null;
        }

        Dictionary<int, TileData> variations = this.dataByTileType[type];

        foreach (TileData data in variations.Values) {
            if (data.Pattern.Contains (pattern)) {
                return data;
            }
        }

        return null;
    }
                                           
    public override void Initialize () {
        base.Initialize ();

        this.tileDataByIndex = new Dictionary<int, TileData> ();

        this.InitializeTileData ();
    }

    private void InitializeTileData () {
        this.dataByTileType = new Dictionary<TileType, Dictionary<int, TileData>> ();
        
        foreach (TileData data in this.tileData) {
            if (data == null) {
                continue;
            }

            if (! this.dataByTileType.ContainsKey (data.Type)) {
                this.dataByTileType[data.Type] = new Dictionary<int, TileData> ();
            }

            this.dataByTileType[data.Type][data.Variation] = data;
            data.Initialize ();
        }
    }
    
    public void SetTileDataForIndex (int index, TileData data) {
        this.tileDataByIndex[index] = data;

        //TODO: check if the code below has unexpected adverse effects
        if (! this.dataByTileType.ContainsKey (data.Type)) {
            this.dataByTileType[data.Type] = new Dictionary<int, TileData> ();
        }

        this.dataByTileType[data.Type][index] = data;
    }
}

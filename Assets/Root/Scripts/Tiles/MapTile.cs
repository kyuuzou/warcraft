using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapTile : ITarget {

    public static readonly Color UndiscoveredColor = new Color (0.2f, 0.2f, 0.2f, 1.0f);

    public TileType Type { get; private set; }
    public TileData Data { get; private set; }

    public IntVector2        AtlasCoordinates { get; private set; }
    public string            Caption          { get; set; }
    public float             CurrentDepth     { get; set; }
    public bool              Dirty            { get; set; }
    public int               Height           { get; private set; }
    public Color             LightColor       { get; set; }
    public IntVector2        MapPosition      { get; private set; }
    public MapTile[]         Neighbours       { get; set; }
    public float             Offset           { get; set; }
    public Vector3           RealPosition     { get; set; }
    public TileSlot          Slot             { get; set; }
    public float             TargetDepth      { get; set; }
    public bool              Visible          { get; set; }

    private int atlasIndex;
    public int AtlasIndex {
        get { return this.atlasIndex; }
        set {
            this.atlasIndex = value;
            this.AtlasCoordinates = new IntVector2 (value % 16, value / 16);
        }
    }

    private Color color;
    public Color Color {
        get { return this.color; }
        private set {
            this.color = value;

            //TODO: don't set tile, just update the colors, also: don't set the colors every single frame
            /*
            if (this.Slot != null) {
                this.Slot.SetTile (this);
            }
            */
        }
    }

    private bool discovered;
    public bool Discovered {
        get { return this.discovered; }
        set {
            this.discovered = value;
            this.RefreshTargetColor ();

            this.Dirty = true;
        }
    }

    public bool PhasedOut {
        get { return false; }
    }

    /// <summary>
    /// For IMovementDestination.
    /// </summary>
    public MapTile Pivot {
        get { return this; }
    }

    public MapTile TargetTile {
        get { return this; }
    }

    /// <summary>
    /// For ITarget.
    /// </summary>
    public MapTile Tile {
        get { return this; }
    }

    public IntVector2 TileSize {
        get { return new IntVector2(1, 1); }
    }

    /// <summary>
    /// Tiles that are in range of the inhabitant get coloured differently.
    /// </summary>
    private Dictionary<IInhabitant, Color> colorByInhabitant;
    
    private Dictionary<Type, List<IInhabitant>> inhabitants;
    private Dictionary<int, MapTileLayer> layers;

    private GameController gameController;
    private Grid grid;
    private Map map;
    private MapTypeData mapData;
    private Color targetColor;
    private IEnumerator refreshColorEnumerator;

    public MapTile (TileType type, int variation, int column, int row) {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.grid = serviceLocator.Grid;
        this.map = serviceLocator.Map;

        this.InitializeInhabitants ();
        this.InitializeLayers ();

        this.MapPosition = new IntVector2 (column, row);
        this.mapData = this.gameController.CurrentLevel.MapType.GetData ();

        this.SetType (type, variation);

        this.Color = MapTile.UndiscoveredColor;
        this.Discovered = ! Settings.Instance.FogOfWar;
        this.CurrentDepth = - this.grid.DefaultSlotSize.y;

        this.RefreshHeight ();
        this.RefreshTargetColor ();

        this.Dirty = true;
    }

    /// <param name="typeFilter">If set, the tile will only adapt if it is of the specified type.</param>
    public void AdaptToNeighbours (TileType typeFilter = TileType.None) {
        if (typeFilter != TileType.None && typeFilter != this.Type) {
            return;
        }

        string pattern = this.GetNeighbourPattern (true, false, typeFilter);
        TileData data = this.mapData.GetTileDataForPattern (this.Type, pattern);

        if (data == null) {
            pattern = this.GetNeighbourPattern (false, false, typeFilter);
            data = this.mapData.GetTileDataForPattern (this.Type, pattern);
        }

        if (data == null) {
            Debug.LogError (string.Format ("No {0} data found for pattern: {1}", this.Type, pattern));
            return;
        }
        
        this.SetType (this.Type, data.Variation);
    }

    public void AddColor (IInhabitant inhabitant, Color color) {
        this.colorByInhabitant[inhabitant] = color;
    }

    public void AddInhabitant (IInhabitant inhabitant) {
        Type type = inhabitant.GetType ();

        if (! this.inhabitants.ContainsKey (type)) {
            this.inhabitants[type] = new List<IInhabitant> ();
        }

        this.inhabitants[type].Add (inhabitant);
    }

    public void AddDeathListener (IDeathListener listener) {

    }
    
    public void AddPhasedOutListener (IPhasedOutListener listener) {
        
    }

    public void Damage (int damage) {

    }
    
    public void Discover () {
		IntVector2 position = this.MapPosition;
        int column = position.X;
        int row = position.Y;

        MapTile tile;

		this.Discovered = true;

        int range = 6;

        for (int x = -1 - range; x < 2 + range; x ++) {
            for (int y = -2 - range; y < 3 + range; y ++) {
                tile = this.map.GetTile (column + x, row + y);

                if (tile != null) {
                    tile.Discovered = true;
                }
            }
        }

		for (int y = -1 - range; y < 2 + range; y ++) {
			tile = this.map.GetTile (column - 2, row + y);

            if (tile != null) {
                tile.Discovered = true;
            }

			tile = this.map.GetTile (column + 2, row + y);

            if (tile != null) {
                tile.Discovered = true;
            }
		}
	}

    public IntVector2[] GetBoundaries () {
        return new IntVector2[] { this.MapPosition };
    }

    public MapTile GetClosestTile () {
        return this;
    }

    public List<Color> GetColors () {
        return new List<Color> (this.colorByInhabitant.Values);
    }

    public T GetInhabitant<T> () where T : IInhabitant {
        Type type = typeof (T);

        if (this.inhabitants.ContainsKey (type)) {
            if (this.inhabitants[type].Count > 0) {
                return (T) this.inhabitants[type][0];
            }
        }

        return default (T);
    }

    public MapTileLayer GetLayer (int layerIndex) {
        return this.layers[layerIndex];
    }

    public MapTile GetNeighbour (Direction direction) {
        return this.Neighbours[(int) direction - 1];
    }

    public string GetNeighbourPattern (bool includeDiagonals, bool valueIfNull, params TileType[] types) {
        bool[] matches = this.GetNeighbourTypes (includeDiagonals, valueIfNull, this.Type);
        
        string pattern = string.Empty;
        
        foreach (bool match in matches) {
            pattern += match ? 1 : 0;
        }

        return pattern;
    }

    public MapTile[] GetNeighbours (bool includeDiagonals = true) {
        if (includeDiagonals) {
            return this.Neighbours;
        }

        return new MapTile[4] {
            this.Neighbours[0], this.Neighbours[2], this.Neighbours[4], this.Neighbours[6]
        };
    }

    public MapTile[] GetNeighboursOfData (params TileData[] data) {
        List<MapTile> neighbours = new List<MapTile> (this.Neighbours);
        
        for (int i = neighbours.Count - 1; i >= 0; i --) {
            TileData neighbourData = neighbours[i].Data;

            bool remove = true;

            foreach (TileData tileData in data) {
                if (tileData.Type == neighbourData.Type && tileData.Variation == neighbourData.Variation) {
                    remove = false;
                    break;
                }
            }

            if (remove) {
                neighbours.RemoveAt (i);
            }
        }
        
        return neighbours.ToArray ();
    }

    public MapTile[] GetNeighboursOfType (params TileType[] types) {
        List<MapTile> neighbours = new List<MapTile> (this.Neighbours);

        for (int i = neighbours.Count - 1; i >= 0; i --) {
            TileType neighbourType = neighbours[i].Type;

            foreach (TileType type in types) {
                if (type == neighbourType) {
                    neighbours.RemoveAt (i);
                    break;
                }
            }
        }

        return neighbours.ToArray ();
    }

    public bool[] GetNeighbourTypes (bool includeDiagonals, bool valueIfNull, params TileType[] types) {
        bool[] neighbourTypes = new bool[8];
        int i = 0;

        foreach (MapTile tile in this.Neighbours) {
            if (tile == null) {
                neighbourTypes[i] = valueIfNull;
            } else {
                neighbourTypes[i] = false;

                foreach (TileType type in types) {
                    if (tile.Type == type) {
                        neighbourTypes[i] = true;
                        break;
                    }
                }
            }

            i ++;
        }

        if (! includeDiagonals) {
            bool[] filteredTypes = new bool[4];

            for (i = 0; i < 4; i ++) {
                filteredTypes[i] = neighbourTypes[i * 2];
            }

            return filteredTypes;
        }

        return neighbourTypes;
    }

    public MapTile GetRealTile () {
        return this;
    }

    private void InitializeInhabitants () {
        this.inhabitants = new Dictionary<Type, List<IInhabitant>> ();
        this.colorByInhabitant = new Dictionary<IInhabitant, Color> ();
    }

    private void InitializeLayers () {
        this.layers = new Dictionary<int, MapTileLayer> ();

        for (int i = 0; i < 4; i ++) {
            this.layers[i] = new MapTileLayer ();
        }
    }
    
    public bool IsAdjacent (IMovementDestination destination) {
        return this.Overlaps (destination, 1);
    }

    public bool IsDead () {
        return false;
    }

    /// <param name="exception">Ignores this inhabitant when determining whether the tile is traversable.</param>
    public bool IsTraversable (MovementType movementType, params IInhabitant[] exceptions) {
        if (this.Type == TileType.None) {
            return false;
        }

        foreach (List<IInhabitant> inhabitants in this.inhabitants.Values) {
            foreach (IInhabitant inhabitant in inhabitants) {
                if (inhabitant.Traversable) {
                    continue;
                }

                if (! Array.Exists<IInhabitant> (exceptions, element => element == inhabitant)) {
                    return false;
                }
            }
        }

        for (int i = 0; i < 2; i ++) {
            MapTileLayer layer = this.layers[i];

            if (layer != null && layer.TileData != null) {
                if (! layer.TileData.IsTraversable (movementType)) {
                    return false;
                }
            }
        }

        /*
        // TODO: This should take into account the height of the tile, in comparison with the height of the tile the
        // unit is on
        if (layers[1] != null && layers[1].TileData != null) {
            if (! layers[1].TileData.TraversableSameLevel) {
                return false;
            }
        }
        */

        return this.Data.IsTraversable (movementType);
    }

    public bool Overlaps (IMovementDestination destination) {
        return this.Overlaps (destination, 0);
    }
    
    /// <param name="padding">
    ///     Width, in tiles, around the destination, that are still considered as tolerance for overlapping.
    /// </param>
    public bool Overlaps (IMovementDestination destination, int padding) {
        IntVector2 radiusA = new IntVector2(1, 1) * 0.5f;
        IntVector2 radiusB = destination.TileSize * 0.5f;
        
        radiusA.X += padding;
        radiusA.Y += padding;
        
        IntVector2 centerA = this.MapPosition + radiusA;
        IntVector2 centerB = destination.Pivot.MapPosition + radiusB;
        
        centerA.X -= padding;
        centerA.Y -= padding;
        
        bool intersects = false;
        
        if (Mathf.Abs (centerA.X - centerB.X) < (radiusA.X + radiusB.X)) {
            if (Mathf.Abs (centerA.Y - centerB.Y) < (radiusA.Y + radiusB.Y)) {
                intersects = true;
            }
        }
        
        return intersects;
    }

    public void PrintNeighbours () {
        string neighbours = "";
        Direction[] directions = (Direction[]) Enum.GetValues (typeof (Direction));
        
        for (int i = 0; i < directions.Length - 1; i++) {
            Direction direction = directions[i];
            
            neighbours += " " + direction.ToString () + ": " + this.Neighbours[(int) direction];
        }
        
        Debug.Log (neighbours);
    }

    public void RefreshHeight () {
        int height = 0;

        foreach (MapTileLayer layer in this.layers.Values) {
            if (layer.TileData != null) {
                height += layer.TileData.Height;
            }
        }

        this.Height = height;
    }

    public void RefreshNeighbours (TileType typeFilter = TileType.None) {
        if (typeFilter == TileType.None) {
            typeFilter = this.Type;
        }

        this.AdaptToNeighbours (typeFilter);

        foreach (MapTile neighbour in this.Neighbours) {
            if (neighbour != null) {
                neighbour.AdaptToNeighbours (typeFilter);
            }
        }
    }

    private IEnumerator RefreshColor () {
        do {
            if (this.targetColor == this.color) {
                yield break;
            }

            this.color = this.color.MoveTowards (this.targetColor, 0.25f * Time.deltaTime);

            yield return null;
        } while (true);
    }

    private void RefreshTargetColor () {
        this.targetColor = this.Discovered ? Color.white : MapTile.UndiscoveredColor;

        if (this.targetColor == this.Color) {
            return;
        }
    }

    public void RemoveColor (IInhabitant inhabitant) {
        this.colorByInhabitant.Remove (inhabitant);
    }

    public void RemoveDeathListener (IDeathListener listener) {

    }

    public void RemoveInhabitant (IInhabitant inhabitant) {
        Type type = inhabitant.GetType ();
        
        if (this.inhabitants.ContainsKey (type)) {
            this.inhabitants[type].Remove (inhabitant);
        }
    }
    
    public void RemovePhasedOutListener (IPhasedOutListener listener) {
        
    }

    public void SetData (TileData data) {
        this.Data = data;
        this.Type = data.Type;

        /*
        //TODO:
        //Should TreeTile be Decoration, like Wall? If only for consistency.
        //Check for draw call issues beforehand though.
        this.Tree = (this.Type == TileType.Tree) ? new TreeTile (this) : null;
        */

        if (this.Data == null) {
            Debug.LogError (string.Format ("No data found for {0}, variation {1}.", data.Type, data.Variation));
            return;
        }
        
        if (this.Data.AtlasIndex > 0) {
            this.AtlasIndex = this.Data.AtlasIndex - 1;
        }

        Decoration decoration = this.GetInhabitant<Decoration> ();

        if (decoration != null) {
            decoration.TileData = this.Data;
        }
    }

    public void SetType (TileType type, int variation = 0) {
        this.SetData (type.GetData (variation));
    }

    public override string ToString () {
        return string.Concat (this.Type, this.MapPosition);
    }
}

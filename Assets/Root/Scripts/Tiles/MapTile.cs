using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapTile : ITarget {

    public static readonly Color UndiscoveredColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    public TileType Type { get; private set; }
    public TileData Data { get; private set; }

    public Vector2Int AtlasCoordinates { get; private set; }
    public string Caption { get; set; }
    public bool Dirty { get; set; }
    public Vector2Int MapPosition { get; private set; }
    public MapTile[] Neighbours { get; set; }
    public float Offset { get; set; }
    public Vector3 RealPosition { get; set; }
    public TileSlot Slot { get; set; }
    public float TargetDepth { get; set; }
    public bool Visible { get; set; }

    private int atlasIndex;
    public int AtlasIndex {
        get { return this.atlasIndex; }
        set {
            this.atlasIndex = value;
            this.AtlasCoordinates = new Vector2Int(value % 16, value / 16);
        }
    }

    private Color color;
    public Color Color {
        get { return this.color; }
        private set {
            this.color = value;
        }
    }

    private bool discovered;
    public bool Discovered {
        get { return this.discovered; }
        set {
            this.discovered = value;
            this.RefreshTargetColor();

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

    public Vector2Int TileSize {
        get { return new Vector2Int(1, 1); }
    }

    /// <summary>
    /// Tiles that are in range of the inhabitant get coloured differently.
    /// </summary>
    private Dictionary<IInhabitant, Color> colorByInhabitant;

    private Dictionary<Type, List<IInhabitant>> inhabitants;

    private GameController gameController;
    private MapGrid grid;
    private Map map;
    private MapTypeData mapData;
    private Color targetColor;
    private IEnumerator refreshColorEnumerator;

    public MapTile(TileType type, int variation, int column, int row) {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.grid = serviceLocator.Grid;
        this.map = serviceLocator.Map;

        this.InitializeInhabitants();

        this.MapPosition = new Vector2Int(column, row);
        this.mapData = this.gameController.CurrentLevel.MapType.GetData();

        this.SetType(type, variation);

        this.Color = MapTile.UndiscoveredColor;
        this.Discovered = !Settings.Instance.FogOfWar;

        this.RefreshTargetColor();

        this.Dirty = true;
    }

    /// <param name="typeFilter">If set, the tile will only adapt if it is of the specified type.</param>
    public void AdaptToNeighbours(TileType typeFilter = TileType.None) {
        if (typeFilter != TileType.None && typeFilter != this.Type) {
            return;
        }

        string pattern = this.GetNeighbourPattern(true, false, typeFilter);
        TileData data = this.mapData.GetTileDataForPattern(this.Type, pattern);

        if (data == null) {
            pattern = this.GetNeighbourPattern(false, false, typeFilter);
            data = this.mapData.GetTileDataForPattern(this.Type, pattern);
        }

        if (data == null) {
            Debug.LogError(string.Format("No {0} data found for pattern: {1}", this.Type, pattern));
            return;
        }

        this.SetType(this.Type, data.Variation);
    }

    public void AddColor(IInhabitant inhabitant, Color color) {
        this.colorByInhabitant[inhabitant] = color;
    }

    public void AddInhabitant(IInhabitant inhabitant) {
        Type type = inhabitant.GetType();

        if (!this.inhabitants.ContainsKey(type)) {
            this.inhabitants[type] = new List<IInhabitant>();
        }

        this.inhabitants[type].Add(inhabitant);
    }

    public void AddDeathListener(IDeathListener listener) {

    }

    public void AddPhasedOutListener(IPhasedOutListener listener) {

    }

    public void Damage(int damage) {

    }

    public void Discover() {
        Vector2Int position = this.MapPosition;
        int column = position.x;
        int row = position.y;

        MapTile tile;

        this.Discovered = true;

        int range = 6;

        for (int x = -1 - range; x < 2 + range; x++) {
            for (int y = -2 - range; y < 3 + range; y++) {
                tile = this.map.GetTile(column + x, row + y);

                if (tile != null) {
                    tile.Discovered = true;
                }
            }
        }

        for (int y = -1 - range; y < 2 + range; y++) {
            tile = this.map.GetTile(column - 2, row + y);

            if (tile != null) {
                tile.Discovered = true;
            }

            tile = this.map.GetTile(column + 2, row + y);

            if (tile != null) {
                tile.Discovered = true;
            }
        }
    }

    public Vector2Int[] GetBoundaries() {
        return new Vector2Int[] { this.MapPosition };
    }

    public MapTile GetClosestTile() {
        return this;
    }

    public List<Color> GetColors() {
        return new List<Color>(this.colorByInhabitant.Values);
    }

    public T GetInhabitant<T>() where T : IInhabitant {
        Type type = typeof(T);

        if (this.inhabitants.ContainsKey(type)) {
            if (this.inhabitants[type].Count > 0) {
                return (T)this.inhabitants[type][0];
            }
        }

        return default(T);
    }

    public MapTile GetNeighbour(Direction direction) {
        return this.Neighbours[(int)direction - 1];
    }

    public string GetNeighbourPattern(bool includeDiagonals, bool valueIfNull, params TileType[] types) {
        bool[] matches = this.GetNeighbourTypes(includeDiagonals, valueIfNull, this.Type);

        StringBuilder pattern = new StringBuilder();

        foreach (bool match in matches) {
            pattern.Append(match ? 1 : 0);
        }

        return pattern.ToString();
    }

    public MapTile[] GetNeighbours(bool includeDiagonals = true) {
        if (includeDiagonals) {
            return this.Neighbours;
        }

        return new MapTile[4] {
            this.Neighbours[0], this.Neighbours[2], this.Neighbours[4], this.Neighbours[6]
        };
    }

    public MapTile[] GetNeighboursOfData(params TileData[] data) {
        List<MapTile> neighbours = new List<MapTile>(this.Neighbours);

        for (int i = neighbours.Count - 1; i >= 0; i--) {
            TileData neighbourData = neighbours[i].Data;

            bool remove = true;

            foreach (TileData tileData in data) {
                if (tileData.Type == neighbourData.Type && tileData.Variation == neighbourData.Variation) {
                    remove = false;
                    break;
                }
            }

            if (remove) {
                neighbours.RemoveAt(i);
            }
        }

        return neighbours.ToArray();
    }

    public MapTile[] GetNeighboursOfType(params TileType[] types) {
        List<MapTile> neighbours = new List<MapTile>(this.Neighbours);

        for (int i = neighbours.Count - 1; i >= 0; i--) {
            TileType neighbourType = neighbours[i].Type;

            foreach (TileType type in types) {
                if (type == neighbourType) {
                    neighbours.RemoveAt(i);
                    break;
                }
            }
        }

        return neighbours.ToArray();
    }

    public bool[] GetNeighbourTypes(bool includeDiagonals, bool valueIfNull, params TileType[] types) {
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

            i++;
        }

        if (!includeDiagonals) {
            bool[] filteredTypes = new bool[4];

            for (i = 0; i < 4; i++) {
                filteredTypes[i] = neighbourTypes[i * 2];
            }

            return filteredTypes;
        }

        return neighbourTypes;
    }

    public MapTile GetRealTile() {
        return this;
    }

    private void InitializeInhabitants() {
        this.inhabitants = new Dictionary<Type, List<IInhabitant>>();
        this.colorByInhabitant = new Dictionary<IInhabitant, Color>();
    }

    public bool IsAdjacent(IMovementDestination destination) {
        return this.Overlaps(destination, 1);
    }

    public bool IsDead() {
        return false;
    }

    /// <param name="exception">Ignores this inhabitant when determining whether the tile is traversable.</param>
    public bool IsTraversable(MovementType movementType, params IInhabitant[] exceptions) {
        if (this.Type == TileType.None) {
            return false;
        }

        foreach (List<IInhabitant> inhabitants in this.inhabitants.Values) {
            foreach (IInhabitant inhabitant in inhabitants) {
                if (inhabitant.Traversable) {
                    continue;
                }

                if (!Array.Exists<IInhabitant>(exceptions, element => element == inhabitant)) {
                    return false;
                }
            }
        }

        return this.Data.Traversable;
    }

    public bool Overlaps(IMovementDestination destination) {
        return this.Overlaps(destination, 0);
    }

    /// <param name="padding">
    ///     Width, in tiles, around the destination, that are still considered as tolerance for overlapping.
    /// </param>
    public bool Overlaps(IMovementDestination destination, int padding) {
        Vector2Int radiusA = Vector2Int.one;
        Vector2Int radiusB = destination.TileSize.Multiply(0.5f);

        radiusA.x += padding;
        radiusA.y += padding;

        Vector2Int centerA = this.MapPosition + radiusA;
        Vector2Int centerB = destination.Pivot.MapPosition + radiusB;

        centerA.x -= padding;
        centerA.y -= padding;

        bool intersects = false;

        if (Mathf.Abs(centerA.x - centerB.x) < (radiusA.x + radiusB.x)) {
            if (Mathf.Abs(centerA.y - centerB.y) < (radiusA.y + radiusB.y)) {
                intersects = true;
            }
        }

        return intersects;
    }

    public void PrintNeighbours() {
        Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
        StringBuilder neighbours = new StringBuilder();
        
        for (int i = 0; i < directions.Length - 1; i++) {
            Direction direction = directions[i];
            neighbours.Append($" {direction}: {this.Neighbours[(int)direction]}");
        }

        Debug.Log(neighbours);
    }

    public void RefreshNeighbours(TileType typeFilter = TileType.None) {
        if (typeFilter == TileType.None) {
            typeFilter = this.Type;
        }

        this.AdaptToNeighbours(typeFilter);

        foreach (MapTile neighbour in this.Neighbours) {
            if (neighbour != null) {
                neighbour.AdaptToNeighbours(typeFilter);
            }
        }
    }

    private IEnumerator RefreshColor() {
        do {
            if (this.targetColor == this.color) {
                yield break;
            }

            this.color = this.color.MoveTowards(this.targetColor, 0.25f * Time.deltaTime);

            yield return null;
        } while (true);
    }

    private void RefreshTargetColor() {
        this.targetColor = this.Discovered ? Color.white : MapTile.UndiscoveredColor;

        if (this.targetColor == this.Color) {
            // do nothing
        }
    }

    public void RemoveColor(IInhabitant inhabitant) {
        this.colorByInhabitant.Remove(inhabitant);
    }

    public void RemoveDeathListener(IDeathListener listener) {

    }

    public void RemoveInhabitant(IInhabitant inhabitant) {
        Type type = inhabitant.GetType();

        if (this.inhabitants.ContainsKey(type)) {
            this.inhabitants[type].Remove(inhabitant);
        }
    }

    public void RemovePhasedOutListener(IPhasedOutListener listener) {

    }

    public void SetData(TileData data) {
        this.Data = data;
        this.Type = data.Type;

        if (this.Data == null) {
            Debug.LogError(string.Format("No data found for {0}, variation {1}.", data.Type, data.Variation));
            return;
        }

        if (this.Data.AtlasIndex > 0) {
            this.AtlasIndex = this.Data.AtlasIndex - 1;
        }

        Decoration decoration = this.GetInhabitant<Decoration>();

        if (decoration != null) {
            decoration.TileData = this.Data;
        }
    }

    public void SetType(TileType type, int variation = 0) {
        this.SetData(type.GetData(variation));
    }

    public override string ToString() {
        return string.Concat(this.Type, this.MapPosition);
    }
}

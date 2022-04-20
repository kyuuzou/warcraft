using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : SceneObject {

    public int Columns { get; private set; }
    public int Rows { get; private set; }

    [SerializeField]
    private PathfindingAlgorithmType pathfindingAlgorithm;

    public List<Building> Buildings { get; private set; }
    public List<Unit> Units { get; private set; }

    public delegate void BuildingRemovedHandler(object sender, BuildingRemovedArgs e);
    private event BuildingRemovedHandler BuildingRemoved;

    public delegate void UnitRemovedHandler(object sender, UnitRemovedArgs e);
    private event UnitRemovedHandler UnitRemoved;

    private Dictionary<BuildingType, List<Building>> buildingByType;
    private GameController gameController;
    private Dictionary<UnitType, List<Unit>> unitByType;
    private PathfindingAlgorithm pathfinding;
    private Vector2 size;
    private SpawnFactory spawnFactory;
    private MapTile[,] tiles;

    public override void Activate() {
        base.Activate();

        foreach (Unit unit in this.Units) {
            unit.Activate();
        }

        this.StartCoroutine(this.SpawnLater());
    }

    public void AddBuilding(Building building) {
        this.Buildings.Add(building);

        BuildingType type = building.Type;

        if (!this.buildingByType.ContainsKey(type)) {
            this.buildingByType[type] = new List<Building>();
        }

        this.buildingByType[type].Add(building);
    }

    public void AddTile(MapTile tile) {
        Vector2Int position = tile.MapPosition;

        this.tiles[position.x, position.y] = tile;
    }

    public void AddUnit(Unit unit) {
        this.Units.Add(unit);

        UnitType type = unit.Type;

        if (!this.unitByType.ContainsKey(type)) {
            this.unitByType[type] = new List<Unit>();
        }

        this.unitByType[type].Add(unit);
    }

    public void AssignNeighbours() {
        List<DirectionData> directionData = DirectionDictionary.Instance.GetValues();
        Rect bounds = new Rect(0, 0, this.Columns, this.Rows);

        if (directionData.Count == 0) {
            Debug.LogError("Direction dictionary is empty, revert it to prefab.");
        }

        Vector2Int neighbour = Vector2Int.zero;

        for (int column = 0; column < this.Columns; column++) {
            for (int row = 0; row < this.Rows; row++) {
                MapTile[] neighbours = new MapTile[8];

                MapTile tile = this.tiles[column, row];

                if (tile == null) {
                    continue;
                }

                for (int i = 0; i < directionData.Count; i++) {
                    Vector2Int offset = directionData[i].Offset;

                    neighbour.x = column + offset.x;
                    neighbour.y = row + offset.y;

                    if (neighbour.IsWithinBounds(bounds)) {
                        neighbours[i] = this.tiles[neighbour.x, neighbour.y];
                    } else {
                        neighbours[i] = null;
                    }
                }

                tile.Neighbours = neighbours;
            }
        }
    }

    public void AssignNoise() {
        foreach (MapTile tile in this.tiles) {
            if (tile == null || tile.Type == TileType.None || tile.Type == TileType.Water) {
                continue;
            }

            //tile.Offset = Random.Range (0.0f, 20.0f);
        }
    }

    protected override void Awake() {
        base.Awake();

        this.pathfinding = this.pathfindingAlgorithm.AddToGameObject(this.gameObject);

        this.Buildings = new List<Building>();
        this.Units = new List<Unit>();

        this.buildingByType = new Dictionary<BuildingType, List<Building>>();
        this.unitByType = new Dictionary<UnitType, List<Unit>>();
    }

    public void ClearCaptions() {
        foreach (MapTile tile in this.tiles) {
            tile.Caption = "";
        }
    }

    public void DamageArea(MapTile center, int radius = 1, int damage = 1) {
        List<SpawnableSprite> inhabitants = new List<SpawnableSprite>();

        for (int x = -(radius - 1); x < radius; x++) {
            for (int y = -(radius - 1); y < radius; y++) {
                MapTile tile = this.GetTile(x + center.MapPosition.x, y + center.MapPosition.y);

                if (tile != null) {
                    Unit unit = tile.GetInhabitant<Unit>();

                    if (unit != null) {
                        inhabitants.AddExclusive(unit);
                    }

                    Building building = tile.GetInhabitant<Building>();

                    if (building != null) {
                        inhabitants.AddExclusive(building);
                    }

                    Decoration decoration = tile.GetInhabitant<Decoration>();

                    if (decoration != null) {
                        inhabitants.AddExclusive(decoration);
                    }
                }
            }
        }

        foreach (SpawnableSprite inhabitant in inhabitants) {
            inhabitant.Damage(damage);
        }
    }

    public override void Deactivate() {
        base.Deactivate();

        foreach (Unit unit in this.Units) {
            unit.Deactivate();
        }
    }

    public void Discover(int column, int row) {
        MapTile tile = this.GetTile(column, row);

        if (tile != null) {
            tile.Discovered = true;
        }
    }

    public bool DoesAreaContainType(Vector2 topLeftCorner, Vector2 tileSize, TileType type) {
        int column = (int)topLeftCorner.x;
        int row = (int)topLeftCorner.y;

        for (int x = column; x < tileSize.x + column; x++) {
            for (int y = row; y < tileSize.y + row; y++) {
                MapTile tile = this.GetTile(x, y);

                if (tile != null && tile.Type == type) {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector2Int FindClosestBoundary(MapTile origin, IMovementDestination destination) {
        Vector2Int originPosition = origin.MapPosition;
        Vector2Int destinationPosition;

        if (destination.TileSize == new Vector2Int(1, 1)) {
            destinationPosition = destination.Pivot.MapPosition;
        } else {
            // Calculate distances to each border tile of the target sprite
            Vector2Int[] borderPositions = destination.GetBoundaries();
            RepeatableSortedList<Vector2Int> closest = new RepeatableSortedList<Vector2Int>();

            foreach (Vector2Int borderPosition in borderPositions) {
                closest.Add(originPosition.EstimateDistance(borderPosition), borderPosition);
            }

            // Call find path on the tile with the least distance
            destinationPosition = closest[0].Value;
        }

        return destinationPosition;
    }

    public MapTile FindClosestTraversableTile(MapTile origin, MovementType movementType) {
        List<MapTile> open = new List<MapTile>();
        List<MapTile> closed = new List<MapTile>();

        open.Add(origin);
        closed.Add(origin);

        int maximumIterations = 100;
        int i = 0;

        while (open.Count > 0) {
            MapTile tile = open[0];
            open.RemoveAt(0);

            if (tile.IsTraversable(movementType)) {
                return tile;
            }

            foreach (MapTile neighbour in tile.Neighbours) {
                if (neighbour != null && !closed.Contains(neighbour)) {
                    open.Add(neighbour);
                    closed.Add(neighbour);
                }
            }

            if (i++ > maximumIterations) {
                Debug.LogError("Maximum iterations.");
                break;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the closest traversable tile as well as the closest untraversable tile.
    /// For example, if you click a peasant and tell him to go harvest an unreachable tree, this method will return
    /// the closest traversable tile to the chosen tree, and will set the untraversable parameter to the last detected
    /// untraversable tile, so the peasant can cut down that tree instead.
    /// </summary>
    /// <returns>The closest traversable tile.</returns>
    /// <param name="untraversable">The closest untraversable tile.</param>
    private MapTile FindClosestTraversableTile(
        Unit unit, Vector2Int origin, Vector2Int destination, IMovementListener movementListener, out MapTile untraversable
    ) {
        untraversable = this.GetTile(destination);

        int maximumTries = 100;
        int minimumDistance = int.MaxValue;

        List<MapTile> open = new List<MapTile>() { untraversable };
        List<MapTile> closed = new List<MapTile>() { untraversable };

        int depth = 0;
        MapTile traversable = null;

        do {
            MapTile tile = open[0];
            open.RemoveAt(0);
            closed.Add(tile);

            int distance = tile.MapPosition.CalculateManhattanDistance(destination);

            if (distance > depth) {
                depth = distance;

                if (traversable != null) {
                    break;
                }
            }

            foreach (MapTile neighbour in tile.Neighbours) {
                if (neighbour == null) {
                    continue;
                }

                if (this.IsAreaTraversable(neighbour.MapPosition, unit.TileSize, movementListener)) {
                    //if (movementListener.IsTileTraversable (neighbour)) {
                    distance = neighbour.MapPosition.CalculateManhattanDistance(origin);

                    if (distance < minimumDistance) {
                        minimumDistance = distance;
                        traversable = neighbour;
                    }
                } else if (!closed.Contains(neighbour)) {
                    open.Add(neighbour);
                    closed.Add(neighbour);
                }
            }
        } while ((open.Count > 0) && (--maximumTries > 0));

        if (traversable != null) {
            return traversable;
        }

        if (maximumTries <= 0) {
            Debug.LogError("Infinite loop");
        }

        return null;
    }

    public IEnumerator FindPath(
        Task parent,
        Unit unit,
        MapTile origin,
        IMovementDestination destination,
        bool overlapTarget,
        IMovementListener movementListener
    ) {
        Vector2Int destinationPosition = this.FindClosestBoundary(origin, destination);
        MapTile destinationTile = this.tiles[destinationPosition.x, destinationPosition.y];

        if (!this.IsAreaTraversable(destination.Pivot.MapPosition, unit.TileSize, movementListener)) {
            //if (! destinationTile.Data.IsTraversable (movementTrait.MovementType)) {
            MapTile untraversable;

            destinationTile = this.FindClosestTraversableTile(
                unit, origin.MapPosition, destinationTile.MapPosition, movementListener, out untraversable
            );

            if (destinationTile == null) {
#if CUSTOM_DEBUG
                Debug.LogWarning ("Closest traversable tile is origin. Unit will not move.");
#endif
                yield break;
            }

            overlapTarget = true;

            //unit.SetDestination (untraversable);
            destinationPosition = destinationTile.MapPosition;
        }

        IEnumerator coroutine = this.pathfinding.FindPath(
            parent,
            unit,
            origin.MapPosition,
            destinationPosition,
            movementListener,
            overlapTarget
        );

        Task task = new Task(coroutine, false, parent);
        yield return this.StartCoroutine(task.YieldStart());
    }

    public List<Building> GetBuildings(BuildingType type) {
        return this.buildingByType[type];
    }

    public List<MapTile> GetCircularArea(MapTile center, int radius) {
        List<MapTile> open = new List<MapTile>();
        List<MapTile> closed = new List<MapTile>();
        List<MapTile> area = new List<MapTile>();

        open.Add(center);
        closed.Add(center);

        int maximumIterations = 100;
        int i = 0;

        while (open.Count > 0) {
            MapTile tile = open[0];
            open.RemoveAt(0);

            if (tile.MapPosition.CalculateManhattanDistance(center.MapPosition) > radius) {
                continue;
            }

            area.Add(tile);

            foreach (MapTile neighbour in tile.Neighbours) {
                if (neighbour != null && !closed.Contains(neighbour)) {
                    open.Add(neighbour);
                    closed.Add(neighbour);
                }
            }

            if (i++ > maximumIterations) {
                Debug.LogError("Maximum iterations.");
                break;
            }
        }

        return area;
    }

    public Building GetNearestBuilding(BuildingType type, MapTile tile) {
        if (!this.buildingByType.ContainsKey(type)) {
            return null;
        }

        int shortest = int.MaxValue;
        Building chosen = null;
        Vector2Int origin = tile.MapPosition;

        foreach (Building building in this.buildingByType[type]) {
            Vector2Int destination = building.Tile.MapPosition;
            int distance = origin.CalculateManhattanDistance(destination);

            if (distance < shortest) {
                shortest = distance;
                chosen = building;
            }
        }

        return chosen;
    }

    public MapTile GetNearestTile(Vector3 realPosition) {
        MapTile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (MapTile tile in this.tiles) {
            float distance = Vector2.Distance(realPosition, tile.RealPosition);

            if (distance < closestDistance) {
                closestTile = tile;
                closestDistance = distance;
            }
        }

        return closestTile;
    }

    public Vector2 GetSize() {
        return this.size;
    }

    public IEnumerator GetTilesEnumerator() {
        return this.tiles.GetEnumerator();
    }

    public MapTile GetTile(float column, float row) {
        return this.GetTile((int)column, (int)row);
    }

    public MapTile GetTile(int column, int row) {
        if (column < 0 || (column > this.Columns - 1) || row < 0 || (row > this.Rows - 1)) {
            return null;
        }

        return this.tiles[column, row];
    }

    public MapTile GetTile(Vector2Int position) {
        return this.GetTile(position.x, position.y);
    }

    public List<Unit> GetUnits(UnitType type) {
        if (!this.unitByType.ContainsKey(type)) {
            this.unitByType[type] = new List<Unit>();
        }

        return this.unitByType[type];
    }

    public override void InitializeExternals() {
        base.InitializeExternals();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.spawnFactory = serviceLocator.SpawnFactory;
    }

    public bool IsAreaTraversable(Vector2Int pivot, Vector2Int tileSize, IMovementListener movementListener) {
        int column = pivot.x;
        int row = pivot.y;

        for (int x = column; x < tileSize.x + column; x++) {
            for (int y = row; y < tileSize.y + row; y++) {
                MapTile tile = this.GetTile(x, y);

                if (tile == null || !movementListener.IsTileTraversable(tile)) {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsAreaTraversable(Vector2 topLeftCorner, Vector2 tileSize, Unit unit) {
        int column = (int)topLeftCorner.x;
        int row = (int)topLeftCorner.y;

        for (int x = column; x < tileSize.x + column; x++) {
            for (int y = row; y < tileSize.y + row; y++) {
                MapTile tile = this.GetTile(x, y);

                if (tile == null || !tile.IsTraversable(MovementType.Land, unit)) {
                    return false;
                }
            }
        }

        return true;
    }

    public void ManualUpdate() {
        foreach (Unit unit in this.Units) {
            unit.ManualUpdate();
        }

        foreach (Building building in this.Buildings) {
            building.ManualUpdate();
        }
    }

    private void OnBuildingRemoved(BuildingRemovedArgs e) {
        if (this.BuildingRemoved != null) {
            this.BuildingRemoved(this, e);
        }
    }

    public void OnFinishedParsingLevel() {
        for (int row = 0; row < this.Rows; row++) {
            for (int column = 0; column < this.Columns; column++) {
                MapTile tile = this.GetTile(column, row);

                TreeTile tree = tile.GetInhabitant<TreeTile>();

                if (tree != null && tree.Cluster == null) {
                    tree.InitializeCluster();
                }
            }
        }
    }

    private void OnUnitRemoved(UnitRemovedArgs e) {
        if (this.UnitRemoved != null) {
            this.UnitRemoved(this, e);
        }
    }

    public void RefreshPositions() {
        foreach (Unit unit in this.Units) {
            unit.RefreshPosition();
        }

        foreach (Building building in this.Buildings) {
            building.RefreshPosition();
        }
    }

    public void RegisterBuildingRemoved(BuildingRemovedHandler handler) {
        this.BuildingRemoved += new BuildingRemovedHandler(handler);
    }

    public void RegisterUnitRemoved(UnitRemovedHandler handler) {
        this.UnitRemoved += new UnitRemovedHandler(handler);
    }

    public void RemoveBuilding(Building building) {
        this.Buildings.Remove(building);
        this.buildingByType[building.Type].Remove(building);

        this.OnBuildingRemoved(new BuildingRemovedArgs(building));
    }

    public void RemoveUnit(Unit unit) {
        this.Units.Remove(unit);
        this.unitByType[unit.Type].Remove(unit);

        this.OnUnitRemoved(new UnitRemovedArgs(unit));
    }

    public void SetSize(int columns, int rows) {
        this.Columns = columns;
        this.Rows = rows;

        this.size = new Vector2(columns, rows);

        this.tiles = new MapTile[columns, rows];
    }

    private IEnumerator SpawnLater() {
        foreach (Unit unit in this.Units) {
            unit.GameObject.SetActive(false);
            //unit.Sprite.SpriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(1.0f);

        foreach (Unit unit in this.Units) {
            unit.GameObject.SetActive(true);
            //unit.Sprite.SpriteRenderer.enabled = true;
        }
    }

    private void Update() {
#if PRINT_UNIT_TYPE
        foreach (Tile tile in this.tiles) {
            tile.Caption = tile.Unit == null ? string.Empty : tile.Unit.Type.ToString ().Substring (5);
        }
#endif
    }
}

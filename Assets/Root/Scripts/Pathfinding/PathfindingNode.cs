using UnityEngine;

public class PathfindingNode {

    public int DistanceToTarget { get; set; }
    public float Cost { get; private set; }
    public float Score { get; set; }
    public PathfindingNode Parent { get; private set; }

    public Vector2Int Position { get; private set; }
    public MapTile[] Neighbours { get; private set; }

    private MapTile tile;
    public MapTile Tile {
        get { return this.tile; }

        private set {
            this.tile = value;
            this.Position = value.MapPosition;
            this.Neighbours = value.Neighbours;
        }
    }

    public PathfindingNode(MapTile tile, float cost, PathfindingNode parent) {
        this.Tile = tile;
        this.Cost = cost;
        this.Parent = parent;
    }

    public override string ToString() {
        return this.Position.ToString();
    }
}

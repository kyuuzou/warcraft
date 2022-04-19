//#define CUSTOM_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : PathfindingAlgorithm {

    // Sorted by score
    private List<MapTile> open;

    // No specific order
    private List<MapTile> closed;

    private Dictionary<MapTile, PathfindingNode> nodes;

    //All units with an outstanding pathfinding request
    private List<Unit> units;

    private int cutoff = 30;

    private void AddToOpenList(MapTile tile, PathfindingNode node) {
        if (this.open.Contains(tile)) {
            return;
        }

        int i;

        for (i = 0; i < this.open.Count; i++) {
            MapTile openTile = this.open[i];
            PathfindingNode openNode = this.nodes[openTile];

            if (node.Score < openNode.Score) {
                break;
            }
        }

        this.open.Insert(i, tile);
        this.nodes[tile] = node;
    }

    protected override void Awake() {
        base.Awake();

        this.open = new List<MapTile>();
        this.closed = new List<MapTile>();
        this.nodes = new Dictionary<MapTile, PathfindingNode>();
        this.units = new List<Unit>();
    }

    private void BuildPath(Task parent, Unit unit, PathfindingNode node, bool overlapTarget) {
        List<MapTile> waypoints = new List<MapTile>();
        waypoints.Add(node.Tile);

        while (node.Parent != null) {
            node = node.Parent;
            waypoints.Insert(0, node.Tile);
        }

        if (!parent.Aborted) {
            int index = waypoints.Count - 1;
            unit.SetDestination(waypoints[index]);

            if (!overlapTarget) {
                waypoints.RemoveAt(index);
            }

            unit.ChangePath(waypoints);
        } else {
#if CUSTOM_DEBUG
            Debug.Log ("Dismissed");
#endif
        }
    }

    public override IEnumerator FindPath(
        Task parent,
        Unit unit,
        Vector2Int origin,
        Vector2Int destination,
        IMovementListener movementListener,
        bool overlapTarget = true
    ) {
        int steps = 0;

        IUnitTraitMoving movementTrait = unit.GetTrait<IUnitTraitMoving>();

#if CUSTOM_DEBUG
        Debug.Log ("AStar.FindPath");
#endif

        bool foundDestination = false;

        this.open.Clear();
        this.closed.Clear();
        this.nodes.Clear();

        MapTile tile = this.map.GetTile(origin);
        this.GetNode(tile, destination);

        MapTile destinationTile = this.map.GetTile(destination);
        SpawnableSprite targetSprite = destinationTile.GetInhabitant<Building>();

        if (targetSprite == null) {
            targetSprite = destinationTile.GetInhabitant<Unit>();
        }

#if CUSTOM_DEBUG
        Debug.Log (
            "origin", origin, "destination", destination, "targetSprite", targetSprite, "overlap", overlapTarget
        );
#endif

        this.open.Add(tile);

        while (this.open.Count > 0) {
            steps++;

            tile = this.open[0]; //node has the lowest score
            PathfindingNode node = this.nodes[tile];
            this.open.RemoveAt(0);

            if (node.Position == destination) {
                foundDestination = true;
#if CUSTOM_DEBUG
                Debug.Log ("Reached destination");
#endif

                this.BuildPath(parent, unit, node, overlapTarget);
                break;
            }

            bool diagonal = true;
            bool mayMoveDiagonally = movementTrait.MayMoveDiagonally;

            foreach (MapTile neighbour in node.Neighbours) {
                diagonal = !diagonal;

                if (neighbour == null) {
                    continue;
                } else {
                    if (diagonal && !mayMoveDiagonally) {
                        continue;
                    }

                    SpawnableSprite sprite = neighbour.GetInhabitant<Building>();

                    if (sprite != null && targetSprite != sprite) {
                        continue;
                    }

                    sprite = neighbour.GetInhabitant<Unit>();

                    if (sprite != null && sprite != unit && targetSprite != sprite && !sprite.PhasedOut) {
                        continue;
                    }

                    if (!this.map.IsAreaTraversable(neighbour.MapPosition, unit.TileSize, movementListener)) {
                        //if (! movementListener.IsTileTraversable (neighbour)) {
                        continue;
                    }

                    /*
                    if (! neighbour.Data.TraversableByLand) {
                        continue;
                    }
                    */
                }

                float cost = node.Cost + (diagonal ? 0.5f : 0.45f);

                PathfindingNode neighbourNode = this.GetNode(neighbour, destination, node, cost);

                if (neighbourNode == null) {
                    continue;
                }

                if (this.closed.Contains(neighbour)) {
                    this.closed.Remove(neighbour);
                }

                this.AddToOpenList(neighbour, neighbourNode);

#if CUSTOM_DEBUG
                neighbour.Caption = neighbourNode.Score.ToString ();
#endif
            }

            this.closed.Add(tile);

            if (steps > this.cutoff) {
                yield return null;
                steps = 0;
            }
        }

        if (!foundDestination) {
            unit.OnPathFindingFailed();
            unit.Stop();
        }

#if CUSTOM_DEBUG
        Debug.Log ("FindDestination finished: " + foundDestination + ": " + destination);
#endif

        this.units.Remove(unit);
    }

    private PathfindingNode GetNode(MapTile tile, Vector2Int destination, PathfindingNode parent = null, float cost = 0) {
        PathfindingNode node;

        if (this.nodes.ContainsKey(tile)) {
            node = this.nodes[tile];

            if (node.Cost <= cost) {
                return null;
            }
        } else {
            // g(n) is the actual cheapest cost of arriving at n from the start
            node = new PathfindingNode(tile, cost, parent);
        }

        // h(n) is the heuristic estimate of the cost to the goal from n
        node.DistanceToTarget = node.Position.EstimateDistance(destination);

        // f(n) is the score assigned to node n (lower is better)
        node.Score = node.Cost + node.DistanceToTarget;

        this.nodes[tile] = node;

        return node;
    }
}

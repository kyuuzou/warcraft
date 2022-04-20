using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeTile : IInhabitant {

    public delegate void TreeDepletedHandler(object sender, EventArgs e);
    public event TreeDepletedHandler TreeDepleted;

    private static readonly int maximumPortions = 50;

    public int Portions { get; private set; }

    public List<MapTile> Cluster { get; set; }

    private GameController gameController;
    private MapGrid grid;
    private MapTypeData mapData;
    private MapTile tile;

    // Appropriate atlas index depending on neighbours
    private static FlagDictionary treePatterns;

    public bool Traversable {
        get { return false; }
    }

    static TreeTile() {
        FlagDictionary treePatterns = new FlagDictionary();

        treePatterns.AddPattern("111X0X0X", 71);
        treePatterns.AddPattern("11110110", 71);
        treePatterns.AddPattern("11100110", 71);

        treePatterns.AddPattern("111X1X0X", 72);
        treePatterns.AddPattern("10111000", 72);
        treePatterns.AddPattern("10111100", 72);
        treePatterns.AddPattern("10111101", 72);

        treePatterns.AddPattern("0X111X0X", 73);
        treePatterns.AddPattern("01111011", 73);
        treePatterns.AddPattern("00111011", 73);

        treePatterns.AddPattern("11111110", 74);
        treePatterns.AddPattern("0X11111X", 75);

        treePatterns.AddPattern("0X0X111X", 76);
        treePatterns.AddPattern("01101110", 76);
        treePatterns.AddPattern("01101111", 76);

        treePatterns.AddPattern("10111111", 77);

        treePatterns.AddPattern("1X0X1111", 78);
        treePatterns.AddPattern("10001011", 78);
        treePatterns.AddPattern("10011110", 78);
        treePatterns.AddPattern("110X1011", 78);
        treePatterns.AddPattern("11011110", 78);

        treePatterns.AddPattern("1X0X0X11", 79);
        treePatterns.AddPattern("10110011", 79);
        treePatterns.AddPattern("10110111", 79);

        treePatterns.AddPattern("11101111", 80);
        treePatterns.AddPattern("11111011", 81);
        treePatterns.AddPattern("10111011", 82);
        treePatterns.AddPattern("11101110", 83);
        treePatterns.AddPattern("10111001", 84);

        treePatterns.AddPattern("10001110", 85);
        treePatterns.AddPattern("11001110", 85);

        treePatterns.AddPattern("10011011", 87);
        treePatterns.AddPattern("10111110", 88);
        treePatterns.AddPattern("11101011", 89);

        treePatterns.AddPattern("0X0X1X0X", 90);
        treePatterns.AddPattern("00011011", 90);
        treePatterns.AddPattern("01001011", 90);
        treePatterns.AddPattern("01011011", 90);
        treePatterns.AddPattern("01101001", 90);
        treePatterns.AddPattern("01101011", 90);
        treePatterns.AddPattern("00101100", 90);
        treePatterns.AddPattern("01101100", 90);
        treePatterns.AddPattern("01101101", 90);
        treePatterns.AddPattern("00001011", 90);
        treePatterns.AddPattern("01101000", 90);

        treePatterns.AddPattern("1X0X1X0X", 91);

        treePatterns.AddPattern("1X00000X", 92);
        treePatterns.AddPattern("1011000X", 92);
        treePatterns.AddPattern("100X0110", 92);
        treePatterns.AddPattern("100X0100", 92);
        treePatterns.AddPattern("1001000X", 92);
        treePatterns.AddPattern("101101X0", 92);
        treePatterns.AddPattern("110X0110", 92);
        treePatterns.AddPattern("10010101", 92);
        treePatterns.AddPattern("10110101", 92);
        treePatterns.AddPattern("11000100", 92);
        treePatterns.AddPattern("11000101", 92);
        treePatterns.AddPattern("11010001", 92);
        treePatterns.AddPattern("11010100", 92);
        treePatterns.AddPattern("11010101", 92);
        treePatterns.AddPattern("10000101", 92);
        treePatterns.AddPattern("11010000", 92);

        treePatterns.AddPattern("111X0X11", 93);

        treePatterns.AddPattern("11111111", 94);

        TreeTile.treePatterns = treePatterns;
    }

    public TreeTile(MapTile tile) {
        this.tile = tile;

        this.Portions = Settings.Instance.FastHarvest ? 3 : TreeTile.maximumPortions;

        this.Cluster = null;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.grid = serviceLocator.Grid;

        this.mapData = this.gameController.CurrentLevel.MapType.GetData();
    }

    private void AdjustNeighbour(MapTile tile) {
        if (tile == null || (tile.Type != TileType.Tree)) {
            return;
        }

        bool[] types = tile.GetNeighbourTypes(true, true, TileType.Tree);

        string key = string.Empty;

        foreach (bool type in types) {
            key += type ? 1 : 0;
        }

        if (treePatterns.ContainsKey(key)) {
            tile.AtlasIndex = treePatterns[key];

            TileData data = this.mapData.GetTileDataForIndex(tile.AtlasIndex);
            tile.SetType(data.Type, data.Variation);
        } else {
            Debug.LogWarning("Position: " + tile.MapPosition + "; Invalid key: " + key);
        }
    }

    public void Deplete(bool adjustNeighbours = false) {
        this.tile.SetType(TileType.ChoppedTree);

        this.Cluster.Remove(this.tile);

        this.HarvestIfIsolated(this.tile.GetNeighbour(Direction.North));
        this.HarvestIfIsolated(this.tile.GetNeighbour(Direction.South));

        if (adjustNeighbours) {
            foreach (MapTile neighbour in this.tile.Neighbours) {
                this.AdjustNeighbour(neighbour);
            }

            if (this.tile.GetNeighbour(Direction.North) != null) {
                foreach (MapTile neighbour in this.tile.GetNeighbour(Direction.North).Neighbours) {
                    this.AdjustNeighbour(neighbour);
                }
            }

            if (this.tile.GetNeighbour(Direction.South) != null) {
                foreach (MapTile neighbour in this.tile.GetNeighbour(Direction.South).Neighbours) {
                    this.AdjustNeighbour(neighbour);
                }
            }

            this.grid.Refresh();
        }

        this.OnTreeDepleted(EventArgs.Empty);
    }

    public int Harvest() {
        if (this.Portions < 1) {
            Debug.LogWarning("Tried to harvest depleted tree.");
            return 0;
        }

        this.Portions--;

        if (this.Portions == 0) {
            this.Deplete(true);
        }

        return 1;
    }

    private void HarvestIfIsolated(MapTile tile) {
        if (tile == null) {
            return;
        }

        if (tile.Type == TileType.Tree) {
            MapTile neighbour = tile.GetNeighbour(Direction.North);
            bool isolated = neighbour != null && (neighbour.Type != TileType.Tree);

            neighbour = tile.GetNeighbour(Direction.South);
            isolated = isolated && neighbour != null && (neighbour.Type != TileType.Tree);

            if (isolated) {
                tile.GetInhabitant<TreeTile>().Deplete();
            }
        }
    }

    public void InitializeCluster() {
        List<MapTile> cluster = new List<MapTile>();
        List<MapTile> neighbours = new List<MapTile>();

        this.tile.GetInhabitant<TreeTile>().Cluster = cluster;
        neighbours.Add(this.tile);

        do {
            MapTile current = neighbours[0];

            cluster.Add(current);
            neighbours.RemoveAt(0);

            foreach (MapTile neighbour in current.Neighbours) {
                if (neighbour == null) {
                    continue;
                }

                if (neighbour.Type == TileType.Tree && neighbour.GetInhabitant<TreeTile>().Cluster == null) {
                    neighbour.GetInhabitant<TreeTile>().Cluster = cluster;
                    neighbours.Add(neighbour);
                }
            }
        } while (neighbours.Count > 0);
    }

    private void OnTreeDepleted(EventArgs e) {
        if (this.TreeDepleted != null) {
            this.TreeDepleted(this, e);
        }
    }

    public void Register(TreeDepletedHandler handler) {
        this.TreeDepleted += handler;
    }

    public void Unregister(TreeDepletedHandler handler) {
        this.TreeDepleted -= handler;
    }
}

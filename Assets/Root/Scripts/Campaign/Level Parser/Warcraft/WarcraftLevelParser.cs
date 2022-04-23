using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WarcraftLevelParser : ILevelParser {

    private Level level;
    //private string[] text;

    private GameController gameController;
    private MapGrid grid;
    private Map map;
    private SpawnFactory spawnFactory;

    public WarcraftLevelParser(Level level) {
        this.level = level;
        //this.text = level.MapLayout.text.Split (new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.grid = serviceLocator.Grid;
        this.map = serviceLocator.Map;
        this.spawnFactory = serviceLocator.SpawnFactory;
    }

    public void Parse() {
        this.ParseMapType();
        this.ParseMapSize();
        this.ParseMapLayout();
        this.ParseMapTiles();
        this.map.AssignNeighbours();

        this.ParseUnits();
        this.ParseTileType(TileType.Door);
        this.ParseTileType(TileType.Road);
        this.ParseTileType(TileType.Wall);

        this.map.OnFinishedParsingLevel();
        this.grid.OnFinishedParsingLevel();
    }

    private void ParseMapLayout() {
        MapTypeData mapData = this.level.MapType.GetData();
        mapData.Initialize();

        string[] lines = mapData.Layout.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++) {
            string[] line = lines[i].Split(':');

            TileType type = (TileType)Enum.Parse(typeof(TileType), line[0], true);
            TileData tileData = type.GetData(line.Length > 1 ? int.Parse(line[1]) : 0);

#if UNITY_EDITOR
            if (tileData == null) {
                //Debug.LogWarning ("No data found for type: " + type);

                tileData = ScriptableObject.CreateInstance<TileData>();
                tileData.Type = type;
            }
#endif

            mapData.SetTileDataForIndex(i, tileData);
        }
    }

    private void ParseMapSize() {
        string text = this.level.MapPresentation.text;
        Match match = Regex.Match(text, @"PresentMap\((.*)(?=\))");

        if (match.Success) {
            string[] parameters = match.Groups[1].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            this.map.SetSize(
                int.Parse(parameters[2]),
                int.Parse(parameters[3])
            );
        } else {
            Debug.LogWarning("Could not parse map size.");
        }
    }

    private void ParseMapTiles() {
        string levelLayout = this.level.MapLayout.text;
        MatchCollection matches = Regex.Matches(levelLayout, @"SetTile\((.*)(?=\))");

        MapTypeData mapData = this.level.MapType.GetData();

        foreach (Match match in matches) {
            string[] parameters = match.Groups[1].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int tileIndex = int.Parse(parameters[0]);
            TileData data = mapData.GetTileDataForIndex(tileIndex);

            MapTile tile = new MapTile(
                data.Type,
                data.Variation,
                int.Parse(parameters[1]),
                int.Parse(parameters[2])
            );

            tile.AtlasIndex = tileIndex;

            this.map.AddTile(tile);

            if (data.Type == TileType.Door) {
                // do nothing
            } else if (data.Type == TileType.Tree) {
                tile.AddInhabitant(new TreeTile(tile));
            }
        }
    }

    private void ParseMapType() {
    }

    private void ParseTileType(TileType tileType) {
        string text = this.level.MapLayout.text;
        string pattern = string.Concat(@"Create", tileType.ToString(), @"s\({(.+?)(?=}\))");
        Match match = Regex.Match(text, pattern, RegexOptions.Singleline);

        text = match.Groups[1].Value;

        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        List<MapTile> tiles = new List<MapTile>();

        foreach (string line in lines) {
            string filteredLine = Regex.Replace(line, "[^0-9,]", string.Empty);

            string[] parameters = filteredLine.Split(
                new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries
            );

            int x = int.Parse(parameters[1]);
            int y = int.Parse(parameters[2]);

            MapTile tile = this.map.GetTile(x, y);
            tiles.Add(tile);

            tile.SetType(tileType, 1);

            if (tile.GetInhabitant<Decoration>() != null) {
                continue;
            }

            this.spawnFactory.SpawnDecoration(
                tileType,
                this.gameController.GetFaction(FactionIdentifier.Neutral),
                new Vector2Int(x, y)
            );
        }

        foreach (MapTile tile in tiles) {
            tile.AdaptToNeighbours(tileType);
        }
    }

    private void ParseUnits() {
        string text = this.level.MapLayout.text;
        Match match = Regex.Match(text, @"local PlaceUnits(.*)(?=end)", RegexOptions.Singleline);

        text = match.Value;

        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        SpawnableSprite lastSpawnedSprite = null;

        foreach (string line in lines) {
            match = Regex.Match(line, @"CreateUnit\((.*)(?=\))");

            if (match.Success) {
                string[] parameters = match.Groups[1].Value.Split(
                    new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries
                );

                string type = parameters[0].Replace("\"", string.Empty);
                int factionIndex = int.Parse(parameters[1]);
                Vector2Int position = new Vector2Int(
                    int.Parse(parameters[2].Replace("{", string.Empty)),
                    int.Parse(parameters[3].Replace("}", string.Empty))
                );

                lastSpawnedSprite = this.spawnFactory.Spawn(
                    type,
                    this.gameController.GetFaction(factionIndex),
                    position
                );

                if (lastSpawnedSprite == null) {
#if PRINT_ERRORS
                    Debug.LogWarning ("Could not spawn: " + type);
#endif
                }
            } else {
                match = Regex.Match(line, @"SetResourcesHeld\((.*)(?=\))");

                if (match.Success) {
                    string[] parameters = match.Groups[1].Value.Split(
                        new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries
                    );

                    int resources = int.Parse(parameters[1]);

                    if (lastSpawnedSprite != null) {
                        lastSpawnedSprite.SetResourcesHeld(resources);
                    }
                }
            }
        }
    }
}

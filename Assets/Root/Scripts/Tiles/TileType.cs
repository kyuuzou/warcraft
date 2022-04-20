public enum TileType {
    None,
    Bridge = 1,
    Bush = 4,
    ChoppedTree = 5,
    Crater = 6,
    DamagedDoor = 27,
    DamagedWall = 25,
    DestroyedDoor = 28,
    DestroyedWall = 26,
    Door = 21,
    Exit = 23,
    Floor = 18,
    Fog = 7,
    Grass = 8,
    Ground = 22,
    Path = 9,
    Road = 10,
    Rock = 12,
    ScorchedGround = 24,
    Sign = 13,
    Tree = 14,
    UnpassableDecoration = 20,
    Wall = 15,
    Water = 16,

    /// <summary>
    /// Unknown means the type wasn't set on the map editor. Here to differentiate from None, in the sense that None
    /// means there's no tile there at all, while Unknown means there's a tile there with an indeterminate type.
    /// </summary>
    Unknown = 1000,
}

public static class TileTypeExtension {

    private static GameController gameController = null;

    public static TileData GetData(this TileType type, int variation = 0) {
        TileTypeExtension.Initialize();

        MapType mapType = TileTypeExtension.gameController.CurrentLevel.MapType;
        return mapType.GetData().GetTileData(type, variation);
    }

    private static void Initialize() {
        if (TileTypeExtension.gameController != null) {
            return;
        }

        ServiceLocator serviceLocator = ServiceLocator.Instance;

        if (serviceLocator != null) {
            TileTypeExtension.gameController = serviceLocator.GameController;
        }
    }
}

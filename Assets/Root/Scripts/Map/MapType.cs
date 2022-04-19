public enum MapType {
    None,
    Dungeon = 1,
    Forest = 2,
    Snow = 4,
    Swamp = 3
}

public static class MapTypeExtension {

    public static MapTypeData GetData(this MapType type) {
        return MapTypeDataDictionary.Instance.GetValue(type);
    }
}

public enum Direction {
    None,
    North = 1,
    Northeast = 2,
    East = 3,
    Southeast = 4,
    South = 5,
    Southwest = 6,
    West = 7,
    Northwest = 8
}

public static class DirectionExtension {

    public static DirectionData GetData(this Direction direction) {
        return DirectionDictionary.Instance.GetValue(direction);
    }

    public static Direction Invert(this Direction direction) {
        return (direction + 4).RoundClamp();
    }

    public static Direction RoundClamp(this Direction direction) {
        return (Direction)((int)direction).RoundClamp((int)Direction.North, (int)Direction.Northwest);
    }
}

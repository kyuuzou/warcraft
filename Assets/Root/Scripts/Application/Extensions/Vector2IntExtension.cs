using UnityEngine;

public static class Vector2IntExtension {

    public static float CalculateEuclideanDistance(this Vector2Int a, Vector2Int b) {
        int dx = a.x - b.x;
        int dy = a.y - b.y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }

    public static int CalculateManhattanDistance(this Vector2Int a, Vector2Int b) {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return dx + dy;
    }

    /// <summary>
    /// Estimates the distance between two tile positions.
    /// Because diagonals have the same cost as verticals and horizontals, the global distance is equal to the largest
    /// distance in any axis.
    /// </summary>
    public static int EstimateDistance(this Vector2Int a, Vector2Int b) {
        if (a == b) {
            return 0;
        }

        a -= b;

        return Mathf.Max(Mathf.Abs(a.x), Mathf.Abs(a.y));
    }

    public static bool IsWithinBounds(this Vector2Int a, Rect bounds) {
        if (!a.x.IsWithinRange(bounds.xMin, bounds.xMax)) {
            return false;
        }

        if (!a.y.IsWithinRange(bounds.yMin, bounds.yMax)) {
            return false;
        }

        return true;
    }

    public static Vector2Int Multiply(this Vector2Int a, float multiplier) {
        a.x = (int)(a.x * multiplier);
        a.y = (int)(a.y * multiplier);

        return a;
    }
}

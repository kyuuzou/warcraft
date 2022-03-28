using UnityEngine;
using System.Collections;

public static class Vector2Extension {

    public static float CalculateEuclideanDistance (this Vector2 a, Vector2 b) {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        
        return Mathf.Sqrt ((dx * dx) + (dy * dy));
    }
    
    public static int CalculateManhattanDistance (this Vector2 a, Vector2 b) {
        float dx = Mathf.Abs (a.x - b.x);
        float dy = Mathf.Abs (a.y - b.y);
        
        return (int) (dx + dy);
    }
    
    /// <summary>
    /// Estimates the distance between two tile positions.
    /// Because diagonals have the same cost as verticals and horizontals, the global distance is equal to the largest
    /// distance in any axis.
    /// </summary>
    public static int EstimateDistance (this Vector2 a, Vector2 b) {
        if (a == b)
            return 0;
        
        a -= b;
        
        return (int) Mathf.Max (Mathf.Abs (a.x), Mathf.Abs (a.y));
    }

    public static Vector2 InvertAxis (this Vector2 vector) {
        float aux = vector.x;

        vector.x = vector.y;
        vector.y = aux;

        return vector;
    }
    
    public static bool IsAdjacentTo (this Vector2 a, Vector2 b) {
        return Mathf.Abs (a.x -  b.x) <= 1 && Mathf.Abs (a.y - b.y) <= 1;
    }
    
    public static bool IsWithinBounds (this Vector2 a, Rect bounds) {
        if (! a.x.IsWithinRange (bounds.xMin, bounds.xMax))
            return false;
        
        if (! a.y.IsWithinRange (bounds.yMin, bounds.yMax))
            return false;
        
        return true;
    }

    public static Vector2 Multiply (this Vector2 a, Vector2 b) {
        return new Vector2 (a.x * b.x, a.y * b.y);
    }
}

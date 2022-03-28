using UnityEngine;

public static class IntVector2Extension {

    /// <summary>
    /// Estimates the distance between two tile positions.
    /// Because diagonals have the same cost as verticals and horizontals, the global distance is equal to the largest
    /// distance in any axis.
    /// </summary>
    public static int EstimateDistance(this IntVector2 a, IntVector2 b) {
        if (a == b) {
            return 0;
        }

        a -= b;

        return Mathf.Max(Mathf.Abs(a.X), Mathf.Abs(a.Y));
    }
}

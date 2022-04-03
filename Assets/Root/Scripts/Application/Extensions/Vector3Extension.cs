using UnityEngine;

public static class Vector3Extension {

    public static Vector3 Abs(this Vector3 vector) {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    public static Vector3 Add(this Vector3 a, Vector2 b) {
        return new Vector3(a.x + b.x, a.y + b.y, a.z);
    }

    /// <summary>
    /// Determines whether this instance is within circle, by ignoring the Z coordinate.
    /// </summary>
    public static bool IsWithinCircle(this Vector3 vector, Vector3 center, float radius) {
        float dx = Mathf.Abs(vector.x - center.x);
        float dy = Mathf.Abs(vector.y - center.y);

        // Imagine a square drawn around it such that it's sides are tangents to this circle
        if (dx > radius || dy > radius) {
            return false;
        }

        // Now imagine a square diamond drawn inside this circle such that it's vertices touch this circle
        if (dx + dy <= radius) {
            return true;
        }

        // Now we have covered most of our space and only a small area of this circle remains in between
        // our square and diamond to be tested. Here we revert to Pythagoras
        return ((dx * dx) + (dy * dy)) <= (radius * radius);
    }

    public static Vector3 Multiply(this Vector3 a, Vector2 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z);
    }

    public static Vector3 Subtract(this Vector3 a, Vector2 b) {
        return new Vector3(a.x - b.x, a.y - b.y, a.z);
    }

    public static string ToFullString(this Vector3 vector) {
        return vector.ToString("F4");
    }
}

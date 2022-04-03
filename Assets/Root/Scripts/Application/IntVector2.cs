using UnityEngine;

[System.Serializable]
public class IntVector2 {

    [SerializeField]
    private int x;
    public int X {
        get { return this.x; }
        set { this.x = value; }
    }

    [SerializeField]
    private int y;
    public int Y {
        get { return this.y; }
        set { this.y = value; }
    }

    public IntVector2(int x, int y) {
        this.X = x;
        this.Y = y;
    }

    public IntVector2(float x, float y) : this((int)x, (int)y) {

    }

    public IntVector2(Vector2 a) : this((int)a.x, (int)a.y) {

    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.X + b.X, a.Y + b.Y);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.X - b.X, a.Y - b.Y);
    }

    public static IntVector2 operator *(IntVector2 a, float b) {
        return new IntVector2(a.X * b, a.Y * b);
    }

    public static Vector3 operator *(IntVector2 a, Vector3 b) {
        return new Vector3(a.X * b.x, a.Y * b.y, b.z);
    }

    public static Vector3 operator /(IntVector2 a, float b) {
        return new Vector3(a.X / b, a.Y / b, 0.0f);
    }

    public static bool operator ==(IntVector2 a, IntVector2 b) {
        return (a.X == b.X) && (a.Y == b.Y);
    }

    public static bool operator !=(IntVector2 a, IntVector2 b) {
        return (a.X != b.X) || (a.Y != b.Y);
    }

    public static string ArrayToString(IntVector2[] vectors) {
        string output = "";

        foreach (IntVector2 vector in vectors) {
            output += vector + " ";
        }

        return output;
    }

    public float CalculateEuclideanDistance(IntVector2 b) {
        int dx = this.X - b.X;
        int dy = this.Y - b.Y;

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }

    public int CalculateManhattanDistance(IntVector2 b) {
        int dx = Mathf.Abs(this.X - b.X);
        int dy = Mathf.Abs(this.Y - b.Y);

        return dx + dy;
    }

    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }

        if (!(obj is IntVector2)) {
            return false;
        }

        IntVector2 vector = (IntVector2)obj;

        return (this.X == vector.X) && (this.Y == vector.Y);
    }

    public bool IsAdjacentTo(IntVector2 other) {
        return Mathf.Abs(this.X - other.X) <= 1 && Mathf.Abs(this.Y - other.Y) <= 1;
    }

    public bool IsWithinBounds(Rect bounds) {
        if (!this.X.IsWithinRange(bounds.xMin, bounds.xMax)) {
            return false;
        }

        if (!this.Y.IsWithinRange(bounds.yMin, bounds.yMax)) {
            return false;
        }

        return true;
    }

    public override int GetHashCode() {
        int hash = 17;

        hash = hash * 23 + this.X.GetHashCode();
        hash = hash * 23 + this.Y.GetHashCode();

        return hash;
    }

    public IntVector2 Normalize() {
        this.X = Mathf.Clamp(this.X, -1, 1);
        this.Y = Mathf.Clamp(this.Y, -1, 1);

        return this;
    }

    public override string ToString() {
        return "(" + this.X + ", " + this.Y + ")";
    }
}

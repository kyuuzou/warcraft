using UnityEngine;

public static class ObjectExtension {

    public static bool In<T>(this T item, params T[] items) {
        if (items == null) {
            Debug.LogError("Null reference error at ObjectExtension.In<T>(this T, params T[])");
            return false;
        }

        foreach (T objectReference in items) {
            if (item.Equals(objectReference)) {
                return true;
            }
        }

        return false;
    }
}

using UnityEngine;

public static class TransformExtension {

    public static void AddToPosition(this Transform transform, Vector3 offset) {
        TransformExtension.AddToPosition(transform, offset.x, offset.y, offset.z);
    }

    public static void AddToPosition(this Transform transform, float x, float y, float z) {
        Vector3 position = transform.position;

        position.x += x;
        position.y += y;
        position.z += z;

        transform.position = position;
    }

    public static Transform FindChildByName(this Transform parent, string name) {
        Component[] transforms = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in transforms) {
            if (transform.name == name) {
                return transform;
            }
        }

        return null;
    }

    public static Transform FindChildRecursively(this Transform parent, string name) {
        if (parent.name.Equals(name)) {
            return parent;
        }

        foreach (Transform child in parent) {
            Transform result = FindChildRecursively(child, name);
            if (result != null) {
                return result;
            }
        }

        return null;
    }

    public static void SetActiveChildren(this Transform parent, bool value) {
        foreach (Transform child in parent) {
            child.gameObject.SetActive(value);
        }
    }

    private static Vector3 SetCoordinates(Vector3 position, float x, float y, float z) {
        position.x = x;
        position.y = y;
        position.z = z;

        return position;
    }

    public static void SetEulerAngles(this Transform transform, float x, float y, float z) {
        transform.eulerAngles = TransformExtension.SetCoordinates(transform.eulerAngles, x, y, z);
    }

    public static void SetEulerX(this Transform transform, float x) {
        transform.eulerAngles = TransformExtension.SetX(transform.eulerAngles, x);
    }

    public static void SetEulerY(this Transform transform, float y) {
        transform.eulerAngles = TransformExtension.SetY(transform.eulerAngles, y);
    }

    public static void SetEulerZ(this Transform transform, float z) {
        transform.eulerAngles = TransformExtension.SetZ(transform.eulerAngles, z);
    }

    public static void SetLocalEulerAngles(this Transform transform, float x, float y, float z) {
        transform.localEulerAngles = TransformExtension.SetCoordinates(transform.localEulerAngles, x, y, z);
    }

    public static void SetLocalEulerX(this Transform transform, float x) {
        transform.localEulerAngles = TransformExtension.SetX(transform.localEulerAngles, x);
    }

    public static void SetLocalEulerY(this Transform transform, float y) {
        transform.localEulerAngles = TransformExtension.SetY(transform.localEulerAngles, y);
    }

    public static void SetLocalEulerZ(this Transform transform, float z) {
        transform.localEulerAngles = TransformExtension.SetZ(transform.localEulerAngles, z);
    }

    public static void SetLocalPosition(this Transform transform, float x, float y, float z) {
        transform.localPosition = TransformExtension.SetCoordinates(transform.localPosition, x, y, z);
    }

    public static void SetLocalScale(this Transform transform, float x, float y, float z) {
        transform.localScale = TransformExtension.SetCoordinates(transform.localScale, x, y, z);
    }

    public static void SetLocalScaleX(this Transform transform, float x) {
        transform.localScale = TransformExtension.SetX(transform.localScale, x);
    }

    public static void SetLocalScaleY(this Transform transform, float y) {
        transform.localScale = TransformExtension.SetY(transform.localScale, y);
    }

    public static void SetLocalScaleZ(this Transform transform, float z) {
        transform.localScale = TransformExtension.SetZ(transform.localScale, z);
    }

    public static void SetLocalX(this Transform transform, float x) {
        transform.localPosition = TransformExtension.SetX(transform.localPosition, x);
    }

    public static void SetLocalY(this Transform transform, float y) {
        transform.localPosition = TransformExtension.SetY(transform.localPosition, y);
    }

    public static void SetLocalZ(this Transform transform, float z) {
        transform.localPosition = TransformExtension.SetZ(transform.localPosition, z);
    }

    public static void SetPosition(this Transform transform, float x, float y, float z) {
        transform.position = TransformExtension.SetCoordinates(transform.position, x, y, z);
    }

    private static Vector3 SetX(Vector3 vector, float x) {
        vector.x = x;
        return vector;
    }

    public static void SetX(this Transform transform, float x) {
        transform.position = TransformExtension.SetX(transform.position, x);
    }

    private static Vector3 SetY(Vector3 vector, float y) {
        vector.y = y;
        return vector;
    }

    public static void SetY(this Transform transform, float y) {
        transform.position = TransformExtension.SetY(transform.position, y);
    }

    private static Vector3 SetZ(Vector3 vector, float z) {
        vector.z = z;
        return vector;
    }

    public static void SetZ(this Transform transform, float z) {
        transform.position = TransformExtension.SetZ(transform.position, z);
    }
}
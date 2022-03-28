using UnityEngine;
using System.Collections;

public static class CameraExtension {

    /// <summary>
    /// Gets the world distance correspondent to the distance between two viewport points.
    /// </summary>
    public static float GetWorldDistanceFromViewport (this Camera camera, Vector2 a, Vector2 b) {
        Vector3 left = camera.ViewportToWorldPoint (a);
        Vector3 right = camera.ViewportToWorldPoint (b);
        
        return Vector3.Distance (left, right);
    }
}

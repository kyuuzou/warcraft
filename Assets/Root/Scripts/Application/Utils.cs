using System.Collections;
using System.Reflection;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : Singleton<Utils> {

    [SerializeField]
    private GUIText debugText;

    [SerializeField]
    private Transform positionMarker;

    public static float[] ColorToRGB(Color color) {
        return new float[] {
            color.r * 255.0f,
            color.g * 255.0f,
            color.b * 255.0f
        };
    }

    public static void Die() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void Dump(System.Object o, bool die = false) {
        if (o == null) {
            UnityEngine.Debug.Log(o);

        } else {
            StringBuilder sb = new StringBuilder();

            // Include the type of the object
            System.Type type = o.GetType();
            sb.Append("Type: " + type.Name);

            // Include information for each Field
            sb.Append("\r\n\r\nFields:");
            System.Reflection.FieldInfo[] fi = type.GetFields();
            if (fi.Length > 0) {
                foreach (FieldInfo f in fi) {
                    sb.Append("\r\n " + f.ToString() + " = " +
                           f.GetValue(o));
                }
            } else {
                sb.Append("\r\n None");
            }

            // Include information for each Property
            sb.Append("\r\n\r\nProperties:");
            System.Reflection.PropertyInfo[] pi = type.GetProperties();
            if (pi.Length > 0) {
                foreach (PropertyInfo p in pi) {
                    sb.Append("\r\n " + p.ToString() + " = " +
                           p.GetValue(o, null));
                }
            } else {
                sb.Append("\r\n None");
            }

            UnityEngine.Debug.Log(sb.ToString());
        }

        if (die) {
            Utils.Die();
        }
    }

    /// <summary>
    /// Gets the rendering bounds of the transform.
    /// </summary>
    /// <param name="transform">The game object to get the bounding box for.</param>
    /// <param name="pBound">The bounding box reference that will </param>
    /// <param name="encapsulate">Used to determine if the first bounding box to be calculated should be encapsulated
    /// into the <see cref="pBound"/> argument.
    /// </param>
    /// <returns>Returns true if at least one bounding box was calculated.</returns>
    public static bool GetBoundWithChildren(Transform transform, ref Bounds pBound, ref bool encapsulate) {
        var bound = new Bounds();
        var didOne = false;

        // get 'this' bound
        if (transform.gameObject.GetComponent<Renderer>() != null) {
            bound = transform.gameObject.GetComponent<Renderer>().bounds;
            if (encapsulate) {
                pBound.Encapsulate(bound.min);
                pBound.Encapsulate(bound.max);
            } else {
                pBound.min = bound.min;
                pBound.max = bound.max;
                encapsulate = true;
            }

            didOne = true;
        }

        // union with bound(s) of any/all children
        foreach (Transform child in transform) {
            if (GetBoundWithChildren(child, ref pBound, ref encapsulate)) {
                didOne = true;
            }
        }

        return didOne;
    }

    public static Texture2D GetTextureFromAtlas(Texture2D atlas, int rows, int columns, int index) {
        int width = atlas.width / columns;
        int height = atlas.height / rows;

        int x = (index % columns) * width;
        int y = atlas.height - (Mathf.CeilToInt((index + 1) / (float)columns) * height);

        Color[] tile = atlas.GetPixels(x, y, width, height);
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(tile);
        texture.Apply();

        return texture;
    }

    public static Color RGBToColor(int r, int g, int b) {
        return new UnityEngine.Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public static void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void ScrollResourceNumber(ref float visibleNumber, int actualNumber, TextMesh textMesh) {
        if (visibleNumber != actualNumber) {
            if (Mathf.Abs(actualNumber - visibleNumber) < 1) {
                visibleNumber = actualNumber;
            } else {
                visibleNumber = Mathf.Lerp(visibleNumber, actualNumber, 3.0f * Time.deltaTime);
            }

            textMesh.text = ((int)visibleNumber).ToString("#,##0");
        }
    }

    public static void ScrollResourceNumber(ref float visibleNumber, int actualNumber, GUIText text) {

    }

    public static void SetLayerRecursively(GameObject gameObject, int layerIndex) {
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true)) {
            transform.gameObject.layer = layerIndex;
        }
    }

    public static void SetLayerRecursively(GameObject gameObject, string layerName) {
        int layerIndex = LayerMask.NameToLayer(layerName);

        Utils.SetLayerRecursively(gameObject, layerIndex);
    }

    public void SetMarkerPosition(Vector3 position) {
        this.positionMarker.position = position;
    }

    public static IEnumerator Slide(
        Transform transform,
        Vector3 destination,
        float multiplier = 5.0f,
        float tolerance = 3.0f,
        Space space = Space.World
        ) {
        Vector3 position;
        Vector3 origin = transform.position;
        float deltaTime = 0.0f;

        do {
            deltaTime += Time.deltaTime * multiplier;

            position = Vector3.Lerp(origin, destination, deltaTime);
            if (space == Space.World) {
                transform.position = position;
            } else {
                transform.localPosition = position;
            }

            yield return null;
        } while (Vector3.Distance(position, destination) > tolerance);
    }

    public static string SplitPascalCamelCase(object input) {

        return System.Text.RegularExpressions.Regex.Replace(
            input.ToString(),
            "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
            " $1"
        ).Trim();
    }
}

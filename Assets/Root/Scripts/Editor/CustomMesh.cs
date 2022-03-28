using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateMesh : ScriptableWizard {
    
    public float width = 1.0f;
    public float height = 1.0f;
    
    static Camera cam;
    static Camera lastUsedCam;
    
    [MenuItem("Assets/Create/Mesh")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard ("Create Mesh", typeof(CreateMesh));
    }
    
    void OnWizardCreate ()
    {
        Vector2 anchorOffset = Vector2.zero;
        
        string planeAssetName = width + "x" + height + ".asset";
        Mesh m = new Mesh ();
        
        m.name = planeAssetName;
        
        int hCount2 = 2;
        int vCount2 = 2;
        int numTriangles = 6;
        int numVertices = hCount2 * vCount2;
        
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];
        
        int index = 0;
        float uvFactorX = 1.0f;
        float uvFactorY = 1.0f;
        float scaleX = width;
        float scaleY = height;
        for (float y = 0.0f; y < vCount2; y++) {
            for (float x = 0.0f; x < hCount2; x++) {
                vertices [index] = new Vector3 (
                    x * scaleX - width / 2f - anchorOffset.x, y * scaleY - height / 2f - anchorOffset.y, 0.0f
                    );
                uvs [index++] = new Vector2 (x * uvFactorX, y * uvFactorY);
            }
        }
        
        index = 0;
        for (int y = 0; y < 1; y++) {
            for (int x = 0; x < 1; x++) {
                triangles [index] = (y * hCount2) + x;
                triangles [index + 1] = ((y + 1) * hCount2) + x;
                triangles [index + 2] = (y * hCount2) + x + 1;
                
                triangles [index + 3] = ((y + 1) * hCount2) + x;
                triangles [index + 4] = ((y + 1) * hCount2) + x + 1;
                triangles [index + 5] = (y * hCount2) + x + 1;
                index += 6;
            }
        }
        
        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.RecalculateNormals ();
        
        string path = AssetDatabase.GetAssetPath (Selection.activeObject);
        
        if (path == "") {
            path = "Assets";
        } else if (Path.GetExtension (path) != "") {
            path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
        }
        
        AssetDatabase.CreateAsset (m, AssetDatabase.GenerateUniqueAssetPath (path + "/" + planeAssetName));
        AssetDatabase.SaveAssets ();
        
        m.RecalculateBounds ();
    }
}
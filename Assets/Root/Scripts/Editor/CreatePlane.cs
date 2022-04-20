using UnityEditor;
using UnityEngine;

public class CreatePlane : ScriptableWizard {

    public enum Orientation {
        Horizontal,
        Vertical
    }

    public enum AnchorPoint {
        TopLeft,
        TopHalf,
        TopRight,
        RightHalf,
        BottomRight,
        BottomHalf,
        BottomLeft,
        LeftHalf,
        Center
    }

    [SerializeField]
    private string targetAssetPath = "Assets/Root/Meshes/";
    
    [SerializeField]
    private int widthSegments = 1;
    
    [SerializeField]
    private int lengthSegments = 1;
    
    [SerializeField]
    private float width = 1.0f;
    
    [SerializeField]
    private float length = 1.0f;
    
    [SerializeField]
    private Orientation orientation = Orientation.Horizontal;
    
    [SerializeField]
    private AnchorPoint anchor = AnchorPoint.Center;
    
    [SerializeField]
    private bool addCollider = false;
    
    [SerializeField]
    private bool createAtOrigin = true;
    
    [SerializeField]
    private string optionalName;
    
    private static Camera cam;
    private static Camera lastUsedCam;


    [MenuItem("GameObject/Create Other/Custom Plane...")]
    private static void CreateWizard() {
        cam = Camera.current;

        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam) {
            cam = lastUsedCam;
        } else {
            lastUsedCam = cam;
        }

        ScriptableWizard.DisplayWizard("Create Plane", typeof(CreatePlane));
    }

    private void OnWizardUpdate() {
        this.widthSegments = Mathf.Clamp(this.widthSegments, 1, 254);
        this.lengthSegments = Mathf.Clamp(this.lengthSegments, 1, 254);
        if (!this.targetAssetPath.Contains("Assets/Root/Meshes/")) {
            this.targetAssetPath = "Assets/Root/Meshes/";
            this.errorString = "Invalid asset path!";
            this.isValid = false;
        } else {
            this.errorString = "";
            this.isValid = true;
        }
    }

    private void OnWizardCreate() {
        GameObject plane = new GameObject();

        if (!string.IsNullOrEmpty(this.optionalName)) {
            plane.name = this.optionalName;
        } else {
            plane.name = "Plane";
        }

        if (!this.createAtOrigin && cam) {
            plane.transform.position = cam.transform.position + cam.transform.forward * 5.0f;
        } else {
            plane.transform.position = Vector3.zero;
        }

        Vector2 anchorOffset;
        string anchorId;
        switch (this.anchor) {
            case AnchorPoint.TopLeft:
                anchorOffset = new Vector2(-this.width / 2.0f, this.length / 2.0f);
                anchorId = "TL";
                break;
            case AnchorPoint.TopHalf:
                anchorOffset = new Vector2(0.0f, this.length / 2.0f);
                anchorId = "TH";
                break;
            case AnchorPoint.TopRight:
                anchorOffset = new Vector2(this.width / 2.0f, this.length / 2.0f);
                anchorId = "TR";
                break;
            case AnchorPoint.RightHalf:
                anchorOffset = new Vector2(this.width / 2.0f, 0.0f);
                anchorId = "RH";
                break;
            case AnchorPoint.BottomRight:
                anchorOffset = new Vector2(this.width / 2.0f, -this.length / 2.0f);
                anchorId = "BR";
                break;
            case AnchorPoint.BottomHalf:
                anchorOffset = new Vector2(0.0f, -this.length / 2.0f);
                anchorId = "BH";
                break;
            case AnchorPoint.BottomLeft:
                anchorOffset = new Vector2(-this.width / 2.0f, -this.length / 2.0f);
                anchorId = "BL";
                break;
            case AnchorPoint.LeftHalf:
                anchorOffset = new Vector2(-this.width / 2.0f, 0.0f);
                anchorId = "LH";
                break;
            case AnchorPoint.Center:
            default:
                anchorOffset = Vector2.zero;
                anchorId = "C";
                break;
        }

        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));

        string planeAssetName = plane.name + this.widthSegments + "x" + this.lengthSegments + "W" + this.width + "L" + this.length + (this.orientation == Orientation.Horizontal ? "H" : "V") + anchorId + ".asset";
        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath(this.targetAssetPath + planeAssetName, typeof(Mesh));

        if (m == null) {
            m = new Mesh();
            m.name = plane.name;

            int hCount2 = this.widthSegments + 1;
            int vCount2 = this.lengthSegments + 1;
            int numTriangles = this.widthSegments * this.lengthSegments * 6;
            int numVertices = hCount2 * vCount2;

            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];
            int[] triangles = new int[numTriangles];

            int index = 0;
            float uvFactorX = 1.0f / this.widthSegments;
            float uvFactorY = 1.0f / this.lengthSegments;
            float scaleX = this.width / this.widthSegments;
            float scaleY = this.length / this.lengthSegments;
            for (float y = 0.0f; y < vCount2; y++) {
                for (float x = 0.0f; x < hCount2; x++) {
                    if (this.orientation == Orientation.Horizontal) {
                        vertices[index] = new Vector3(x * scaleX - this.width / 2f - anchorOffset.x, 0.0f, y * scaleY - this.length / 2f - anchorOffset.y);
                    } else {
                        vertices[index] = new Vector3(x * scaleX - this.width / 2f - anchorOffset.x, y * scaleY - this.length / 2f - anchorOffset.y, 0.0f);
                    }
                    uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                }
            }

            index = 0;
            for (int y = 0; y < this.lengthSegments; y++) {
                for (int x = 0; x < this.widthSegments; x++) {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = ((y + 1) * hCount2) + x;
                    triangles[index + 2] = (y * hCount2) + x + 1;

                    triangles[index + 3] = ((y + 1) * hCount2) + x;
                    triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                    triangles[index + 5] = (y * hCount2) + x + 1;
                    index += 6;
                }
            }

            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();

            if (!this.targetAssetPath.EndsWith("/")) {
                this.targetAssetPath += "/";
            }

            string tempString = this.targetAssetPath.TrimEnd("/".ToCharArray());
            int stringIndex = tempString.LastIndexOf("/");
            string folderName = tempString.Substring(stringIndex + 1);
            string basePath = tempString.Substring(0, stringIndex);

            if (!System.IO.Directory.Exists(Application.dataPath + System.IO.Path.GetDirectoryName(this.targetAssetPath).TrimStart("Assets".ToCharArray()))) {
                AssetDatabase.CreateFolder(basePath, folderName);
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(this.targetAssetPath + planeAssetName);
            Debug.Log("Creating " + assetPathAndName);
            AssetDatabase.CreateAsset(m, assetPathAndName);
            AssetDatabase.SaveAssets();
        }

        meshFilter.sharedMesh = m;
        m.RecalculateBounds();

        if (this.addCollider) {
            plane.AddComponent(typeof(BoxCollider));
        }

        Selection.activeObject = plane;
    }
}
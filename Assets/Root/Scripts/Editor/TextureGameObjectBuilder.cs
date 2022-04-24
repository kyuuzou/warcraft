using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TextureGameObjectBuilder : Editor {

    [MenuItem("Assets/Create/GameObject from Texture")]
    public static void CreateGameObjectFromTexture() {

        object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        foreach (Texture2D texture in selectedTextures) {

            Mesh mesh = CreatePlaneMeshFromTexture(texture);
            Material material = CreateMaterialFromTexture(texture);

            if (mesh != null && material != null) {

                GameObject gameObject = new GameObject(texture.name, typeof(MeshFilter), typeof(MeshRenderer));

                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                meshFilter.mesh = mesh;

                MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                meshRenderer.material = material;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;

                Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObject from Texture");
            }
        }

        AssetDatabase.Refresh();
    }

    private static Material CreateMaterialFromTexture(Texture2D texture) {
        Material material = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
        material.name = texture.name;
        material.mainTexture = texture;

        string textureFullPath = AssetDatabase.GetAssetPath(texture);
        int lastSlashIndex = textureFullPath.LastIndexOf("/", StringComparison.Ordinal);
        string textureFolder = textureFullPath.Substring(0, lastSlashIndex + 1);

        StringBuilder stringBuilder = new StringBuilder(textureFolder);
        stringBuilder.Replace("Textures", "Materials")
            .Replace("Unspecified Platform/", string.Empty)
            .Replace("Splash Screen/", string.Empty)
            .Replace("iPhone5 (1136x640)/", string.Empty)
            .Replace("iPad3 (2048x1536)/", string.Empty)
            .Replace("iPad2 (1024x768)/", string.Empty);

        stringBuilder.Append(material.name);
        stringBuilder.Append(".mat");

        string materialFullPath = stringBuilder.ToString();

        if (AssetDatabase.LoadAssetAtPath(materialFullPath, typeof(Material))) {
            Debug.Log("A material with that name already exists, skipping asset creation.");
        } else {
            Debug.Log("Creating " + materialFullPath);
            lastSlashIndex = materialFullPath.LastIndexOf("/", StringComparison.Ordinal);
            string[] folders = materialFullPath.Substring(0, lastSlashIndex).Split("/".ToCharArray());
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < folders.Length; i++) {
                sb.Append(folders[i] + "/");
                string dir = Application.dataPath + Path.GetDirectoryName(sb.ToString()).TrimStart("Assets".ToCharArray());
                if (!Directory.Exists(dir)) {
                    Debug.Log($"Creating folder {sb}");
                    AssetDatabase.CreateFolder(sb.ToString().Replace("/" + folders[i] + "/", string.Empty), folders[i]);
                }
            }

            AssetDatabase.CreateAsset(material, AssetDatabase.GenerateUniqueAssetPath(materialFullPath));
        }

        return AssetDatabase.LoadAssetAtPath(materialFullPath, typeof(Material)) as Material;
    }

    private static Mesh CreatePlaneMeshFromTexture(Texture2D texture) {
        int textureWidth = 0;
        int textureHeight = 0;

        var mesh = new Mesh();
        var meshVertices = new Vector3[4];
        var meshTriangles = new int[6];
        var meshNormals = new Vector3[4];
        var meshUVs = new Vector2[4];

        TextureGameObjectBuilder.GetImageSize(texture, out textureWidth, out textureHeight);

        //Set up vertices, with the mesh center at (0, 0)
        meshVertices[0] = new Vector3(-textureWidth / 2, -textureHeight / 2, 0);    //lower left vertex
        meshVertices[1] = new Vector3(textureWidth / 2, -textureHeight / 2, 0);    //lower right vertex
        meshVertices[2] = new Vector3(-textureWidth / 2, textureHeight / 2, 0);    //upper left vertex
        meshVertices[3] = new Vector3(textureWidth / 2, textureHeight / 2, 0);    //upper right vertex
        mesh.vertices = meshVertices;

        //Set up texture coordinates
        meshUVs[0] = new Vector2(0, 0);
        meshUVs[1] = new Vector2(1, 0);
        meshUVs[2] = new Vector2(0, 1);
        meshUVs[3] = new Vector2(1, 1);
        mesh.uv = meshUVs;

        //Set up normals - for a billboarded plane they all point towards negative Z
        meshNormals[0] = meshNormals[1] = meshNormals[2] = meshNormals[3] = -Vector3.forward;
        mesh.normals = meshNormals;

        //Set up triangles
        //Lower left triangle
        meshTriangles[0] = 0; //lower left vertex
        meshTriangles[1] = 2; //upper left vertex
        meshTriangles[2] = 1; //lower right vertex
        //Upper right triangle
        meshTriangles[3] = 2; //upper left vertex
        meshTriangles[4] = 3; //upper right vertex
        meshTriangles[5] = 1; //lower right vertex
        mesh.triangles = meshTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mesh.name = "Plane1x1W" + textureWidth + "L" + textureHeight + "VC";

        string textureFullPath = AssetDatabase.GetAssetPath(texture);
        string textureFolder = textureFullPath.Substring(0, textureFullPath.LastIndexOf("/") + 1);

        StringBuilder stringBuilder = new StringBuilder(textureFolder);
        stringBuilder.Replace("Textures", "Meshes")
            .Replace("Unspecified Platform/", string.Empty)
            .Replace("Splash Screen/", string.Empty)
            .Replace("iPhone5 (1136x640)/", string.Empty)
            .Replace("iPad3 (2048x1536)/", string.Empty)
            .Replace("iPad2 (1024x768)/", string.Empty);

        stringBuilder.Append(mesh.name);
        stringBuilder.Append(".asset");

        string meshFullPath = stringBuilder.ToString();

        if (AssetDatabase.LoadAssetAtPath(meshFullPath, typeof(Mesh))) {
            Debug.Log("A mesh with those dimensions already exists, skipping asset creation.");
        } else {
            Debug.Log("Creating " + meshFullPath);
            int lastSlashIndex = meshFullPath.LastIndexOf("/", StringComparison.Ordinal);
            string[] folders = meshFullPath.Substring(0, lastSlashIndex).Split("/".ToCharArray());
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < folders.Length; i++) {
                sb.Append(folders[i] + "/");
                string dir = Application.dataPath + Path.GetDirectoryName(sb.ToString()).TrimStart("Assets".ToCharArray());
                if (!Directory.Exists(dir)) {
                    AssetDatabase.CreateFolder(sb.ToString().Replace("/" + folders[i] + "/", string.Empty), folders[i]);
                }
            }

            AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(meshFullPath));
        }

        return AssetDatabase.LoadAssetAtPath(meshFullPath, typeof(Mesh)) as Mesh;
    }

    public static bool GetImageSize(Texture2D asset, out int width, out int height) {
        if (asset != null) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null) {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod(
                    "GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];

                return true;
            }
        }

        height = width = 0;
        return false;
    }
}
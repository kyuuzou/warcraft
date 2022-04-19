using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility {

    public static void CreateAsset<T>() where T : ScriptableObject {
        ScriptableObjectUtility.CreateAsset(typeof(T).ToString());
    }

    public static void CreateAsset(string type) {
        ScriptableObject asset = ScriptableObject.CreateInstance(type);

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "") {
            path = "Assets";
        } else if (Path.GetExtension(path) != "") {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(
            path + "/New " + type + ".asset"
        );

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

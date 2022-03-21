using System;
using UnityEditor;
using UnityEngine;

public class WarcraftAssetImporter : EditorWindow {

    [MenuItem("Window/Warcraft Asset Importer _w", false, (int)'W')]
    static void Init() {
		// because the Inspector class is internal, we need to get it through reflection
		Type inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
		Type[] desiredDockNextTo = new Type[]{inspectorType};

		WarcraftAssetImporter importer = EditorWindow.GetWindow<WarcraftAssetImporter>(
			"Asset Importer", true, desiredDockNextTo
		);
		
        importer.Show();
    }
	
	private void Import() {
		Debug.LogError("Not implemented yet!");
	}
	
    private void OnGUI() {
		if (GUILayout.Button("Import")) {
			this.Import();
		}
    }
}

using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class WarcraftAssetImporter : EditorWindow {
	
	private const string FFMpegURL = "https://github.com/Wargus/stratagus/releases/download/2015-30-11/ffmpeg.exe";
	private const string WindowTitle = "Asset Importer";
	
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
	
    private void OnGUI() {
        if (GUILayout.Button("Import")) {
            EditorUtility.DisplayProgressBar(WindowTitle, "Preparing...", 0.0f);
			
			string guid = AssetDatabase.CreateFolder("Assets", "Tools");
			string toolsPath = AssetDatabase.GUIDToAssetPath(guid);
			
			WWW www = new WWW(FFMpegURL);
			
			do {
	            EditorUtility.DisplayProgressBar(WindowTitle, "Downloading FFMpeg...", www.progress * 0.5f);
                Thread.Sleep(200);
			} while (! www.isDone);
			
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError(www.error);
			} else {
				Debug.Log(www.size);
			}
			
            EditorUtility.ClearProgressBar();
        }
    }
}

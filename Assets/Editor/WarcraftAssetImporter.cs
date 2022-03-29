using System;
using System.Collections;
using System.IO;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;

public class WarcraftAssetImporter : EditorWindow {
	
	private const string FFMpegURL = "https://github.com/Wargus/stratagus/releases/download/2015-30-11/ffmpeg.exe";
	private const string WindowTitle = "Warcraft Asset Importer";

	private UnityWebRequest currentRequest = null;
	private EditorCoroutine importingCoroutine = null;

    [MenuItem("Window/Warcraft Asset Importer _w", false, (int)'W')]
    private static void Init() {
		// because the Inspector class is internal, we need to get it through reflection
		Type inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
		Type[] desiredDockNextTo = {inspectorType};

		WarcraftAssetImporter importer = EditorWindow.GetWindow<WarcraftAssetImporter>(
			WarcraftAssetImporter.WindowTitle, true, desiredDockNextTo
		);
		
        importer.Show();
    }

    private void Abort() {
	    if (this.currentRequest != null) {
		    this.currentRequest.Abort();
		    this.currentRequest = null;
	    }

	    if (this.importingCoroutine != null) {
		    EditorUtility.ClearProgressBar();
		    EditorCoroutineUtility.StopCoroutine(this.importingCoroutine);
		    this.importingCoroutine = null;
	    }
    }
    
    private IEnumerator Import() {
	    EditorUtility.DisplayProgressBar(WindowTitle, "Preparing...", 0.0f);

	    string toolsPath = "Tools";
            
	    if (! AssetDatabase.IsValidFolder("Assets/" + toolsPath)) {
		    AssetDatabase.CreateFolder("Assets", toolsPath);
	    }
	    
	    this.currentRequest = new UnityWebRequest(WarcraftAssetImporter.FFMpegURL);
	    this.currentRequest.downloadHandler = new DownloadHandlerBuffer();
	    this.currentRequest.SendWebRequest();
	    
	    do {
		    EditorUtility.DisplayProgressBar(
			    WindowTitle,
			    "Downloading FFMpeg...",
			    this.currentRequest.downloadProgress
			);
		    
		    yield return null;
	    } while (! this.currentRequest.downloadHandler.isDone);

	    if (this.currentRequest.isNetworkError || this.currentRequest.isHttpError) {
		    Debug.LogError(this.currentRequest.error);
		    this.Abort();
		    yield break;
	    } 

	    string path = Path.Combine(
		    Application.dataPath,
		    toolsPath, 
		    Path.GetFileName(WarcraftAssetImporter.FFMpegURL)
		);
	    
	    File.WriteAllBytes(path, this.currentRequest.downloadHandler.data);
            
	    EditorUtility.ClearProgressBar();
	    AssetDatabase.Refresh();
	    this.currentRequest = null;
	    this.importingCoroutine = null;
	    
	    this.Repaint();
    }
    
    private void OnGUI() {
	    if (this.importingCoroutine == null) {
		    if (GUILayout.Button("Import")) {
			    this.importingCoroutine = EditorCoroutineUtility.StartCoroutine(this.Import(), this);
		    }
	    } else {
		    GUILayout.Label("Importing...");
		    
		    if (GUILayout.Button("Abort")) {
			    this.Abort();
		    }
	    }
    }
}

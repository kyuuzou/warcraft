using System;
using System.Collections;
using System.IO;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;

public class WarcraftAssetImporter : EditorWindow {
	
	private const string ToolsPath = "Tools";
	private const string WindowTitle = "Warcraft Asset Importer";

	private const string ToolsURL = "https://github.com/kyuuzou/warcraft/releases/download/war1tool_v3.2.1/";
	private const string FFMpegFilename = "ffmpeg.exe";
	private const string War1toolFilename = "war1tool.exe";

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

    private IEnumerator Download(string url, int stepNumber, int totalSteps) {
	    this.currentRequest = new UnityWebRequest(url);
	    this.currentRequest.downloadHandler = new DownloadHandlerBuffer();
	    this.currentRequest.SendWebRequest();
	    
	    string progressInfo = $"Step {stepNumber}/{totalSteps}: Downloading {Path.GetFileNameWithoutExtension(url)}";
	    
	    do {
		    EditorUtility.DisplayProgressBar(
			    WarcraftAssetImporter.WindowTitle, 
			    progressInfo,
			    this.currentRequest.downloadProgress
		    );
		    
		    yield return null;
	    } while (! this.currentRequest.downloadHandler.isDone);

	    if (this.currentRequest.isNetworkError || this.currentRequest.isHttpError) {
		    Debug.LogError(this.currentRequest.error);
		    this.Abort();
		    yield break;
	    } 

	    string path = Path.Combine(Application.dataPath, WarcraftAssetImporter.ToolsPath, Path.GetFileName(url));
	    File.WriteAllBytes(path, this.currentRequest.downloadHandler.data);
    }
    
    private IEnumerator Import() {
	    EditorUtility.DisplayProgressBar(WindowTitle, "Preparing...", 0.0f);
            
	    if (! AssetDatabase.IsValidFolder($"Assets/{WarcraftAssetImporter.ToolsPath}")) {
		    AssetDatabase.CreateFolder("Assets", WarcraftAssetImporter.ToolsPath);
	    }

	    string[] toolFilenames = {WarcraftAssetImporter.FFMpegFilename, WarcraftAssetImporter.War1toolFilename};
        int step = 1;
        int totalSteps = toolFilenames.Length;

	    foreach (string toolFilename in toolFilenames) {
		    string toolURL = WarcraftAssetImporter.ToolsURL + toolFilename;
		    yield return EditorCoroutineUtility.StartCoroutine(this.Download(toolURL, step++, totalSteps), this);
	    }

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

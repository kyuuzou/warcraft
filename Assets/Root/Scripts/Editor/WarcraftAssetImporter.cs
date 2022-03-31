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

    [SerializeField]
    private string dataWarPath = string.Empty;

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
        } while (!this.currentRequest.downloadHandler.isDone);

        if (this.currentRequest.isNetworkError || this.currentRequest.isHttpError) {
            Debug.LogError(this.currentRequest.error);
            this.Abort();
            yield break;
        }

        string path = Path.Combine(Application.dataPath, WarcraftAssetImporter.ToolsPath, Path.GetFileName(url));
        File.WriteAllBytes(path, this.currentRequest.downloadHandler.data);
    }

    private IEnumerator DownloadTools() {
        if (!AssetDatabase.IsValidFolder($"Assets/{WarcraftAssetImporter.ToolsPath}")) {
            AssetDatabase.CreateFolder("Assets", WarcraftAssetImporter.ToolsPath);
        }

        string[] toolFilenames = { WarcraftAssetImporter.FFMpegFilename, WarcraftAssetImporter.War1toolFilename };
        int step = 1;
        int totalSteps = toolFilenames.Length;

        foreach (string toolFilename in toolFilenames) {
            string toolURL = WarcraftAssetImporter.ToolsURL + toolFilename;
            yield return EditorCoroutineUtility.StartCoroutine(this.Download(toolURL, step++, totalSteps), this);
        }
    }

    private IEnumerator Import() {
	    EditorUtility.DisplayProgressBar(WindowTitle, "Preparing...", 0.0f);

        yield return this.DownloadTools();

	    EditorUtility.ClearProgressBar();
	    AssetDatabase.Refresh();
	    this.currentRequest = null;
	    this.importingCoroutine = null;
	    
	    this.Repaint();
    }

    private void OnGUI() {
        this.RenderStatus();
        this.RenderImporter();
    }

    private void RenderImporter() {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Importer:", EditorStyles.boldLabel);

        if (this.importingCoroutine == null) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Path to DATA.WAR:", this.dataWarPath);

            if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
                this.dataWarPath = EditorUtility.OpenFilePanelWithFilters(
                    "Select Warcraft: Orcs and Humans' DATA.WAR file",
                    ".",
                    new string[] { "WAR files", "WAR" }
                );
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Import assets")) {
                this.importingCoroutine = EditorCoroutineUtility.StartCoroutine(this.Import(), this);
            }
        } else {
            GUILayout.Label("Importing...");

            if (GUILayout.Button("Abort")) {
                this.Abort();
            }
        }
    }

    private void RenderStatus() {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);

        string[] toolFilenames = { WarcraftAssetImporter.FFMpegFilename, WarcraftAssetImporter.War1toolFilename };

        foreach (string toolFilename in toolFilenames) {
            string toolPath = $"Assets/Tools/{toolFilename}";
            bool ready = string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(toolPath));
            string icon = ready ? "redLight" : "greenLight";

            GUILayout.BeginHorizontal();

            GUILayout.Label(
                EditorGUIUtility.IconContent(icon),
                GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(false)
            );

            GUILayout.Label(toolFilename);
            GUILayout.EndHorizontal();
        }
    }
}

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using Debug = UnityEngine.Debug;

public class WarcraftAssetImporter : EditorWindow {
	
	private const string ToolPath = "Tools";
	private const string WindowTitle = "Warcraft Asset Importer";

    private const string DataFile = "DATA.WAR";
	private const string ToolsURL = "https://github.com/kyuuzou/warcraft/releases/download/war1tool_v3.2.1/";
	private const string FFMpegFilename = "ffmpeg.exe";
	private const string War1toolFilename = "war1tool.exe";

	private UnityWebRequest currentRequest = null;
	private EditorCoroutine importingCoroutine = null;

    private string dataWarPath = string.Empty;
    private string lastOutputMessage = string.Empty;

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

        AssetDatabase.Refresh();
    }

    private IEnumerator Download(string url, int stepNumber, int totalSteps) {
        string path = Path.Combine(Application.dataPath, WarcraftAssetImporter.ToolPath, Path.GetFileName(url)); this.currentRequest = new UnityWebRequest(url);

        if (File.Exists(path)) {
            Debug.Log($"File exists! Skipping {Path.GetFileName(url)}");
            yield break;
        }

        this.currentRequest = new UnityWebRequest(url);
        this.currentRequest.downloadHandler = new DownloadHandlerBuffer();
        this.currentRequest.SendWebRequest();

        string progressInfo = $"Step {stepNumber}/{totalSteps}: Downloading {Path.GetFileNameWithoutExtension(url)}";

        do {
            EditorUtility.DisplayCancelableProgressBar(
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

        File.WriteAllBytes(path, this.currentRequest.downloadHandler.data);
    }

    private IEnumerator DownloadTools() {
        if (!AssetDatabase.IsValidFolder($"Assets/{WarcraftAssetImporter.ToolPath}")) {
            AssetDatabase.CreateFolder("Assets", WarcraftAssetImporter.ToolPath);
        }

        string[] toolFilenames = { WarcraftAssetImporter.FFMpegFilename, WarcraftAssetImporter.War1toolFilename };
        int step = 1;
        int totalSteps = toolFilenames.Length;

        foreach (string toolFilename in toolFilenames) {
            string toolURL = WarcraftAssetImporter.ToolsURL + toolFilename;
            yield return EditorCoroutineUtility.StartCoroutine(this.Download(toolURL, step++, totalSteps), this);
        }
    }

    private void ExtractAssets() {
#if UNITY_EDITOR_WIN        
        string toolPath = $"{Application.dataPath}\\{WarcraftAssetImporter.ToolPath}\\";

        // Inject the script into a command in case running scripts is disabled on the system.
        string script = File.ReadAllText($"{toolPath}ExtractFilesOnWindows.ps1");
        script = script.Replace("{DATA_WAR_PATH}", this.dataWarPath);
        script = script.Replace("{TOOL_PATH}", toolPath);

        Process process = Process.Start("powershell.exe", script);
        process.WaitForExit();
        process.Close();
# elif UNITY_EDITOR_OSX
        string toolPath = $"{Application.dataPath}/{WarcraftAssetImporter.ToolPath}/";
        string script = File.ReadAllText($"{toolPath}ExtractFilesOnMac.applescript");

        Process process = Process.Start("osascript", $"-e '{script}'");
        process.WaitForExit();
        process.Close();
#endif
    }

    private bool FindDataWarFile() {
        try {
            if (string.IsNullOrEmpty(this.dataWarPath)) {
                throw new FileNotFoundException();
            }
            
            if (!(File.Exists(this.dataWarPath) || Directory.Exists(this.dataWarPath))) {
                throw new FileNotFoundException();
            }

            string fileName = Path.GetFileName(this.dataWarPath);

            if (string.Compare(fileName, WarcraftAssetImporter.DataFile, StringComparison.OrdinalIgnoreCase) == 0) {
                return true;
            }

            if (Directory.Exists(this.dataWarPath)) {
                string[] files = Directory.GetFiles(
                    this.dataWarPath,
                    WarcraftAssetImporter.DataFile,
                    SearchOption.AllDirectories
                );

                if (files.Length > 0) {
                    this.dataWarPath = files[0];
                    return true;
                }
            }

            throw new FileNotFoundException();
        } catch (FileNotFoundException exception) {
            Debug.LogError($"Could not find {WarcraftAssetImporter.DataFile}. Please select it before importing.");
            this.Abort();
        }
        
        return false;
    }

    private IEnumerator Import() {
        EditorUtility.DisplayCancelableProgressBar(WindowTitle, "Preparing...", 0.0f);

        yield return this.DownloadTools();
        
        if (!this.FindDataWarFile()) {
            yield break;
        }
        
        this.ExtractAssets();

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
            EditorGUILayout.TextField($"Path to {WarcraftAssetImporter.DataFile}:", this.dataWarPath);

            if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
                this.dataWarPath = EditorUtility.OpenFilePanelWithFilters(
                    $"Select Warcraft: Orcs and Humans' {WarcraftAssetImporter.DataFile} file",
                    ".",
                    new string[] { "WAR files", "WAR", "Mac OS Application", "app" }
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

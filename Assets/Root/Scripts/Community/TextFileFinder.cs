using UnityEngine;

public class TextFileFinder : MonoBehaviour {

    protected string m_textPath;

    protected FileBrowser m_fileBrowser;

    [SerializeField]
    private GUISkin skin;

    [SerializeField]
    protected Texture2D m_directoryImage;

    [SerializeField]
    protected Texture2D m_fileImage;

    protected void OnGUI() {
        GUI.skin = this.skin;

        if (this.m_fileBrowser != null) {
            this.m_fileBrowser.OnGUI();
        } else {
            this.OnGUIMain();
        }
    }

    protected void OnGUIMain() {

        GUILayout.BeginHorizontal();
        GUILayout.Label("Text File", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Label(this.m_textPath ?? "none selected");
        if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
            this.m_fileBrowser = new FileBrowser(
                    new Rect(100, 100, 600, 500),
                    "Choose Text File",
                    this.FileSelectedCallback
                );
            this.m_fileBrowser.SelectionPattern = "*.txt";
            this.m_fileBrowser.DirectoryImage = this.m_directoryImage;
            this.m_fileBrowser.FileImage = this.m_fileImage;
        }
        GUILayout.EndHorizontal();
    }

    protected void FileSelectedCallback(string path) {
        this.m_fileBrowser = null;
        this.m_textPath = path;
    }
}
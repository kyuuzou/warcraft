using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    File browser for selecting files or folders at runtime.
 */

public enum FileBrowserType {
    File,
    Directory
}

public class FileBrowser {

    // Called when the user clicks cancel or select
    public delegate void FinishedCallback(string path);
    // Defaults to working directory
    public string CurrentDirectory {
        get {
            return this.m_currentDirectory;
        }
        set {
            this.SetNewDirectory(value);
            this.SwitchDirectoryNow();
        }
    }
    protected string m_currentDirectory;
    // Optional pattern for filtering selectable files/folders. See:
    // http://msdn.microsoft.com/en-us/library/wz42302f (v=VS.90).aspx
    // and
    // http://msdn.microsoft.com/en-us/library/6ff71z1w (v=VS.90).aspx
    public string SelectionPattern {
        get {
            return this.m_filePattern;
        }
        set {
            this.m_filePattern = value;
            this.ReadDirectoryContents();
        }
    }
    protected string m_filePattern;

    // Optional image for directories
    public Texture2D DirectoryImage {
        get {
            return this.m_directoryImage;
        }
        set {
            this.m_directoryImage = value;
            this.BuildContent();
        }
    }
    protected Texture2D m_directoryImage;

    // Optional image for files
    public Texture2D FileImage {
        get {
            return this.m_fileImage;
        }
        set {
            this.m_fileImage = value;
            this.BuildContent();
        }
    }
    protected Texture2D m_fileImage;

    // Browser type. Defaults to File, but can be set to Folder
    public FileBrowserType BrowserType {
        get {
            return this.m_browserType;
        }
        set {
            this.m_browserType = value;
            this.ReadDirectoryContents();
        }
    }
    protected FileBrowserType m_browserType;
    protected string m_newDirectory;
    protected string[] m_currentDirectoryParts;

    protected string[] m_files;
    protected GUIContent[] m_filesWithImages;
    protected int m_selectedFile;

    protected string[] m_nonMatchingFiles;
    protected GUIContent[] m_nonMatchingFilesWithImages;
    protected int m_selectedNonMatchingDirectory;

    protected string[] m_directories;
    protected GUIContent[] m_directoriesWithImages;
    protected int m_selectedDirectory;

    protected string[] m_nonMatchingDirectories;
    protected GUIContent[] m_nonMatchingDirectoriesWithImages;

    protected bool m_currentDirectoryMatches;

    protected GUIStyle CentredText {
        get {
            if (this.m_centredText == null) {
                this.m_centredText = new GUIStyle(GUI.skin.label);
                this.m_centredText.alignment = TextAnchor.MiddleLeft;
                this.m_centredText.fixedHeight = GUI.skin.button.fixedHeight;
            }
            return this.m_centredText;
        }
    }
    protected GUIStyle m_centredText;

    protected string m_name;
    protected Rect m_screenRect;

    protected Vector2 m_scrollPosition;

    protected FinishedCallback m_callback;

    // Browsers need at least a rect, name and callback
    public FileBrowser(Rect screenRect, string name, FinishedCallback callback) {
        this.m_name = name;
        this.m_screenRect = screenRect;
        this.m_browserType = FileBrowserType.File;
        this.m_callback = callback;
        this.SetNewDirectory(Directory.GetCurrentDirectory());
        this.SwitchDirectoryNow();
    }

    protected void SetNewDirectory(string directory) {
        this.m_newDirectory = directory;
    }

    protected void SwitchDirectoryNow() {
        if (this.m_newDirectory == null || this.m_currentDirectory == this.m_newDirectory) {
            return;
        }
        this.m_currentDirectory = this.m_newDirectory;
        this.m_scrollPosition = Vector2.zero;
        this.m_selectedDirectory = this.m_selectedNonMatchingDirectory = this.m_selectedFile = -1;
        this.ReadDirectoryContents();
    }

    protected void ReadDirectoryContents() {
        if (this.m_currentDirectory == "/") {
            this.m_currentDirectoryParts = new string[] { "" };
            this.m_currentDirectoryMatches = false;
        } else {
            this.m_currentDirectoryParts = this.m_currentDirectory.Split(Path.DirectorySeparatorChar);
            if (this.SelectionPattern != null) {
                string[] generation = Directory.GetDirectories(
                    Path.GetDirectoryName(this.m_currentDirectory),
                    this.SelectionPattern
                );
                this.m_currentDirectoryMatches = Array.IndexOf(generation, this.m_currentDirectory) >= 0;
            } else {
                this.m_currentDirectoryMatches = false;
            }
        }

        if (this.BrowserType == FileBrowserType.File || this.SelectionPattern == null) {
            this.m_directories = Directory.GetDirectories(this.m_currentDirectory);
            this.m_nonMatchingDirectories = new string[0];
        } else {
            this.m_directories = Directory.GetDirectories(this.m_currentDirectory, this.SelectionPattern);
            var nonMatchingDirectories = new List<string>();
            foreach (string directoryPath in Directory.GetDirectories(this.m_currentDirectory)) {
                if (Array.IndexOf(this.m_directories, directoryPath) < 0) {
                    nonMatchingDirectories.Add(directoryPath);
                }
            }
            this.m_nonMatchingDirectories = nonMatchingDirectories.ToArray();
            for (int i = 0; i < this.m_nonMatchingDirectories.Length; ++i) {
                int lastSeparator = this.m_nonMatchingDirectories[i].LastIndexOf(Path.DirectorySeparatorChar);
                this.m_nonMatchingDirectories[i] = this.m_nonMatchingDirectories[i].Substring(lastSeparator + 1);
            }
            Array.Sort(this.m_nonMatchingDirectories);
        }

        for (int i = 0; i < this.m_directories.Length; ++i) {
            this.m_directories[i] = this.m_directories[i].Substring(this.m_directories[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }

        if (this.BrowserType == FileBrowserType.Directory || this.SelectionPattern == null) {
            this.m_files = Directory.GetFiles(this.m_currentDirectory);
            this.m_nonMatchingFiles = new string[0];
        } else {
            this.m_files = Directory.GetFiles(this.m_currentDirectory, this.SelectionPattern);
            var nonMatchingFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(this.m_currentDirectory)) {
                if (Array.IndexOf(this.m_files, filePath) < 0) {
                    nonMatchingFiles.Add(filePath);
                }
            }
            this.m_nonMatchingFiles = nonMatchingFiles.ToArray();
            for (int i = 0; i < this.m_nonMatchingFiles.Length; ++i) {
                this.m_nonMatchingFiles[i] = Path.GetFileName(this.m_nonMatchingFiles[i]);
            }
            Array.Sort(this.m_nonMatchingFiles);
        }
        for (int i = 0; i < this.m_files.Length; ++i) {
            this.m_files[i] = Path.GetFileName(this.m_files[i]);
        }
        Array.Sort(this.m_files);
        this.BuildContent();
        this.m_newDirectory = null;
    }

    protected void BuildContent() {
        this.m_directoriesWithImages = new GUIContent[this.m_directories.Length];
        for (int i = 0; i < this.m_directoriesWithImages.Length; ++i) {
            this.m_directoriesWithImages[i] = new GUIContent(this.m_directories[i], this.DirectoryImage);
        }
        this.m_nonMatchingDirectoriesWithImages = new GUIContent[this.m_nonMatchingDirectories.Length];
        for (int i = 0; i < this.m_nonMatchingDirectoriesWithImages.Length; ++i) {
            this.m_nonMatchingDirectoriesWithImages[i] = new GUIContent(this.m_nonMatchingDirectories[i], this.DirectoryImage);
        }
        this.m_filesWithImages = new GUIContent[this.m_files.Length];
        for (int i = 0; i < this.m_filesWithImages.Length; ++i) {
            this.m_filesWithImages[i] = new GUIContent(this.m_files[i], this.FileImage);
        }
        this.m_nonMatchingFilesWithImages = new GUIContent[this.m_nonMatchingFiles.Length];
        for (int i = 0; i < this.m_nonMatchingFilesWithImages.Length; ++i) {
            this.m_nonMatchingFilesWithImages[i] = new GUIContent(this.m_nonMatchingFiles[i], this.FileImage);
        }
    }

    public void OnGUI() {
        GUILayout.BeginArea(
            this.m_screenRect,
            this.m_name,
            GUI.skin.window
        );
        GUILayout.BeginHorizontal();
        for (int parentIndex = 0; parentIndex < this.m_currentDirectoryParts.Length; ++parentIndex) {
            if (parentIndex == this.m_currentDirectoryParts.Length - 1) {
                GUILayout.Label(this.m_currentDirectoryParts[parentIndex], this.CentredText);
            } else if (GUILayout.Button(this.m_currentDirectoryParts[parentIndex])) {
                string parentDirectoryName = this.m_currentDirectory;
                for (int i = this.m_currentDirectoryParts.Length - 1; i > parentIndex; --i) {
                    parentDirectoryName = Path.GetDirectoryName(parentDirectoryName);
                }
                this.SetNewDirectory(parentDirectoryName);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        this.m_scrollPosition = GUILayout.BeginScrollView(
                this.m_scrollPosition,
                false,
                true,
                GUI.skin.horizontalScrollbar,
                GUI.skin.verticalScrollbar,
                GUI.skin.box
            );
        this.m_selectedDirectory = GUILayoutx.SelectionList(
                    this.m_selectedDirectory,
                    this.m_directoriesWithImages,
                    this.DirectoryDoubleClickCallback
                );
        if (this.m_selectedDirectory > -1) {
            this.m_selectedFile = this.m_selectedNonMatchingDirectory = -1;
        }
        this.m_selectedNonMatchingDirectory = GUILayoutx.SelectionList(
                    this.m_selectedNonMatchingDirectory,
                    this.m_nonMatchingDirectoriesWithImages,
                    this.NonMatchingDirectoryDoubleClickCallback
                );
        if (this.m_selectedNonMatchingDirectory > -1) {
            this.m_selectedDirectory = this.m_selectedFile = -1;
        }
        GUI.enabled = this.BrowserType == FileBrowserType.File;
        this.m_selectedFile = GUILayoutx.SelectionList(
                    this.m_selectedFile,
                    this.m_filesWithImages,
                    this.FileDoubleClickCallback
                );
        GUI.enabled = true;
        if (this.m_selectedFile > -1) {
            this.m_selectedDirectory = this.m_selectedNonMatchingDirectory = -1;
        }
        GUI.enabled = false;
        GUILayoutx.SelectionList(
            -1,
            this.m_nonMatchingFilesWithImages
        );
        GUI.enabled = true;
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Cancel", GUILayout.Width(50))) {
            this.m_callback(null);
        }
        if (this.BrowserType == FileBrowserType.File) {
            GUI.enabled = this.m_selectedFile > -1;
        } else {
            if (this.SelectionPattern == null) {
                GUI.enabled = this.m_selectedDirectory > -1;
            } else {
                GUI.enabled = this.m_selectedDirectory > -1 ||
                                (
                                    this.m_currentDirectoryMatches &&
                                    this.m_selectedNonMatchingDirectory == -1 &&
                                    this.m_selectedFile == -1
                                );
            }
        }
        if (GUILayout.Button("Select", GUILayout.Width(50))) {
            if (this.BrowserType == FileBrowserType.File) {
                this.m_callback(Path.Combine(this.m_currentDirectory, this.m_files[this.m_selectedFile]));
            } else {
                if (this.m_selectedDirectory > -1) {
                    this.m_callback(Path.Combine(this.m_currentDirectory, this.m_directories[this.m_selectedDirectory]));
                } else {
                    this.m_callback(this.m_currentDirectory);
                }
            }
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (Event.current.type == EventType.Repaint) {
            this.SwitchDirectoryNow();
        }
    }

    protected void FileDoubleClickCallback(int i) {
        if (this.BrowserType == FileBrowserType.File) {
            this.m_callback(Path.Combine(this.m_currentDirectory, this.m_files[i]));
        }
    }

    protected void DirectoryDoubleClickCallback(int i) {
        this.SetNewDirectory(Path.Combine(this.m_currentDirectory, this.m_directories[i]));
    }

    protected void NonMatchingDirectoryDoubleClickCallback(int i) {
        this.SetNewDirectory(Path.Combine(this.m_currentDirectory, this.m_nonMatchingDirectories[i]));
    }

}
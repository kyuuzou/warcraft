using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SceneObject {

    [SerializeField]
    private Atlas[] atlases;

    private Dictionary<string, Atlas> atlasByImagePath;

    public Atlas GetAtlas(string key) {
        if (this.atlasByImagePath.ContainsKey(key)) {
            return this.atlasByImagePath[key];
        }

        if (key.Length > 0) {
            Debug.LogError("Atlas not found: " + key);
        }

        return null;
    }

    protected override void InitializeInternals() {
        if (this.InitializedInternals) {
            return;
        }

        base.InitializeInternals();

        this.InitializeAtlases();
    }

    private void InitializeAtlases() {
        this.atlasByImagePath = new Dictionary<string, Atlas>();

        foreach (Atlas atlas in this.atlases) {
            atlas.Initialize();
            this.atlasByImagePath[atlas.TextureAtlas.imagePath.RemoveFileExtension()] = atlas;
        }
    }
}

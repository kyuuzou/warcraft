using System.Collections;
using UnityEngine;

public class IsometricTileSlotLayer : SceneObject {

    private Atlas atlas;
    private MapTile currentTile;
    private ResourceManager resourceManager;
    private TextureAtlasSprite sprite;

    protected override void InitializeInternals () {
        if (this.InitializedInternals) {
            return;
        }

        base.InitializeInternals ();

        if (this.Mesh != null) {
            this.Mesh = Mesh.Instantiate (this.Mesh);
        }
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.resourceManager = ServiceLocator.Instance.ResourceManager;
    }

    public void SetColor (Color color) {
        Color[] colors = new Color [4];

        for (int i = 0; i < colors.Length; i ++) {
            colors[i] = color;
        }

        this.Mesh.colors = colors;
    }

    public void SetTile (MapTile tile) {
        this.currentTile = tile;

        string atlasString = tile.Data.Atlas;
        this.atlas = this.resourceManager.GetAtlas (atlasString);

        if (atlas == null) {
            Debug.LogError ("Could not find atlas: " + atlasString);
        } else {
            this.MeshRenderer.sharedMaterial = atlas.Material;
            this.sprite = this.atlas.GetSprite (tile.Data.ImageSource);
            this.UpdateUV ();
        }
    }

    private void UpdateUV () {
        if (this.DefaultMeshData == null || this.sprite == null || this.atlas == null) {
            this.GameObject.SetActive (false);
            return;
        }

        Vector2 size = new Vector2 (
            this.sprite.w / this.atlas.TextureSize.x,
            this.sprite.h / this.atlas.TextureSize.y
        ); 

        IntVector2 position = new IntVector2 (
            this.sprite.x / this.atlas.TextureSize.x,
            1.0f - (this.sprite.y / this.atlas.TextureSize.y)
        );

        Vector2[] uvs = new Vector2 [4];

        uvs[0] = new Vector2 (position.X, position.Y - size.y);
        uvs[1] = new Vector2 (position.X + size.x, position.Y - size.y);
        uvs[2] = new Vector2 (position.X, position.Y);
        uvs[3] = new Vector2 (position.X + size.x, position.Y);

        //position.y = 0.038f;

        this.Mesh.uv = uvs;
    }
}

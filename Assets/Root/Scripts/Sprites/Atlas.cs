using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Atlas : CustomScriptableObject {

    [SerializeField]
    private Material material;
    public Material Material {
        get { return this.material; }
    }

    [SerializeField]
    private Texture2D texture;

    [SerializeField]
    private TextAsset xmlFile;

    public TextureAtlas TextureAtlas { get; private set; }
    public Vector2 TextureSize { get; private set; }

    private Dictionary<string, TextureAtlasSprite> spriteByName;

    public TextureAtlasSprite GetSprite (string name) {
        if (! this.spriteByName.ContainsKey (name)) {
            //Debug.LogError ("Atlas does not contain sprite: " + name);
            return null;
        }

        return this.spriteByName[name];
    }

    public override void Initialize () {
        base.Initialize ();

        this.InitializeTextureAtlas ();
    }

    private void InitializeTextureAtlas () {
        this.TextureAtlas = TextureAtlas.ParseFromFile (this.xmlFile.text);

        this.spriteByName = new Dictionary<string, TextureAtlasSprite> ();

        foreach (TextureAtlasSprite sprite in this.TextureAtlas.Sprites) {
            this.spriteByName[sprite.n] = sprite;
        }

        this.TextureSize = new Vector2 (this.texture.width, this.texture.height);
    }
}

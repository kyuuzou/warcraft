using UnityEngine;
using System.Collections;

public class TiledElement : MonoBehaviour {
    
    [SerializeField]
    private int columns = 1;
    
    [SerializeField]
    private int rows = 1;

    [SerializeField]
    public IntVector2 currentTile = new IntVector2(0, 0);

    [SerializeField]
    private bool inverted = false;

    private int yOffset = 0;

    private Vector2[] originalUV;
    private Mesh mesh;

    private void Awake () {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.originalUV = this.mesh.uv;

        this.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (1.0f / this.columns, 1.0f / this.rows);

        if (this.inverted) {
            this.yOffset = this.rows - 1;
        }
    }

    private void OnApplicationQuit () {
        this.mesh.uv = this.originalUV;
    }
    
    public void SetCurrentTile (IntVector2 currentTile) {
        this.currentTile = currentTile;

        this.UpdateUV (currentTile);
    }

    public void SetCurrentTileX (int index) {
        this.SetCurrentTile (new IntVector2 (index, 0.0f));
    }

    public void SetCurrentTileY (int index) {
        if (index > 1000) {
            index -= 1000;
        }

        this.SetCurrentTile (new IntVector2 (0.0f, index));
    }

    public void SetTexture (Texture texture) {
        this.GetComponent<Renderer> ().sharedMaterial.mainTexture = texture;
    }

    private void Start () {
        this.SetCurrentTile (this.currentTile);
    }

    private void Update () {
        this.UpdateUV (this.currentTile);
    }

    private void UpdateUV (IntVector2 position) {
        Vector2[] uvs = new Vector2 [this.originalUV.Length];
        int i = 0;

        while (i < uvs.Length) {
            uvs[i] = new Vector2 (
                this.originalUV[i].x + position.X,
                this.originalUV[i].y + position.Y + this.yOffset
            );

            i++;
        }

        this.mesh.uv = uvs;
    }
}

using UnityEngine;
using System.Collections;

public class TiledElement : MonoBehaviour {
    
    [SerializeField]
    private int columns = 1;
    
    [SerializeField]
    private int rows = 1;

    [SerializeField]
    public Vector2Int currentTile = new Vector2Int(0, 0);

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
    
    public void SetCurrentTile (Vector2Int currentTile) {
        this.currentTile = currentTile;

        this.UpdateUV (currentTile);
    }

    public void SetCurrentTileX (int index) {
        this.SetCurrentTile (new Vector2Int (index, 0));
    }

    public void SetCurrentTileY (int index) {
        if (index > 1000) {
            index -= 1000;
        }

        this.SetCurrentTile (new Vector2Int (0, index));
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

    private void UpdateUV (Vector2Int position) {
        Vector2[] uvs = new Vector2 [this.originalUV.Length];
        int i = 0;

        while (i < uvs.Length) {
            uvs[i] = new Vector2 (
                this.originalUV[i].x + position.x,
                this.originalUV[i].y + position.y + this.yOffset
            );

            i++;
        }

        this.mesh.uv = uvs;
    }
}

using UnityEngine;

public class TileSlot : CustomSprite {
    private int column;
    private int row;

    private Transform parent;
    private Transform slot;

    private Vector2[] originalUV;

    [SerializeField]
    private GUIText valueText;

    private Vector2Int atlasCoordinates = new Vector2Int(-1, -1);

    public override void Initialize(MapTile tile) {
        base.Initialize(tile);

        this.Mesh = this.GetComponent<MeshFilter>().mesh;
        this.originalUV = this.Mesh.uv;

        this.transform.parent = this.parent;
        this.transform.name = "Slot (" + this.column + ", " + this.row + ")";

        this.transform.localPosition = new Vector3(
            this.column * this.Renderer.bounds.size.x,
            (this.Grid.Rows - this.row - 1) * this.Renderer.bounds.size.y,
            0.0f
        );

        // Pathfinding debug text
        this.valueText.transform.Translate(
            160.248f - this.column * 31.9498571f,
            -159.1f + this.row * 31.92f,
            0.0f,
            Space.Self
        );
    }

    public void SetParent(Transform parent) {
        this.parent = parent;
    }

    public void SetPosition(int column, int row) {
        this.column = column;
        this.row = row;
    }

    public override void SetTile(MapTile tile) {
        base.SetTile(tile);

        this.atlasCoordinates = tile.AtlasCoordinates;
        this.UpdateUV(this.atlasCoordinates.x, this.atlasCoordinates.y);

        Unit unit = tile.GetInhabitant<Unit>();

        if (unit != null) {
            unit.transform.position = this.transform.position;
        }
    }

    private void LateUpdate() {
        this.valueText.text = this.Tile.Caption;

        Vector2Int atlasCoordinates = this.Tile.AtlasCoordinates;

        if (atlasCoordinates != this.atlasCoordinates) {
            this.UpdateUV(atlasCoordinates.x, atlasCoordinates.y);
        }
    }

    private void UpdateUV(int column, int row) {
        Vector2[] uvs = new Vector2[this.originalUV.Length];
        int i = 0;

        while (i < uvs.Length) {
            uvs[i] = new Vector2(
                this.originalUV[i].x + column,
                this.originalUV[i].y + (this.AtlasSize.y - row - 1)
            );
            i++;
        }

        this.Mesh.uv = uvs;
    }
}

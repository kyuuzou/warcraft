using System.Collections.Generic;
using UnityEngine;

public class Minimap : SceneObject {

    private MapGrid grid;
    private Map map;

    [SerializeField]
    private Material backLayerMaterial;
    private Texture2D backLayerTexture;

    [SerializeField]
    private Material frontLayerMaterial;
    private Texture2D frontLayerTexture;

    private readonly int x = 6;
    private readonly int y = 12;
    private readonly int width = 128;
    private readonly int height = 128;

    private Rect viewport;
    private Rect invertedViewport;

    private readonly int horizontalRadius = 15;
    private readonly int verticalRadius = 11;

    private int lastColumn = -1;
    private int lastRow = -1;
    private bool isPressing;
    private Vector2Int position = Vector2Int.zero;

    private List<Unit> units;
    private List<Building> buildings;

    public void AddBuilding(Building building) {
        this.buildings.Add(building);
    }

    public void AddUnit(Unit unit) {
        this.units.Add(unit);
    }

    protected override void Awake() {
        base.Awake();

        this.map = ServiceLocator.Instance.Map;
        this.grid = ServiceLocator.Instance.Grid;

        this.isPressing = false;

        this.units = new List<Unit>();
        this.buildings = new List<Building>();
    }

    private void ClearFrontLayer() {
        for (int row = 0; row < 128; row++) {
            for (int column = 0; column < 128; column++) {
                this.frontLayerTexture.SetPixel(column, row, Color.clear);
            }
        }
    }

    private void ColorBuildings() {
        foreach (Building building in this.map.Buildings) {
            Color color = building.Faction.ControllingPlayer.Data.Color;

            foreach (MapTile tile in building.ClaimedTiles) {
                Vector2Int position = tile.MapPosition;
                this.ColorPixel(this.frontLayerTexture, position.x * 2, this.height - (position.y * 2), color);
            }
        }
    }

    private void ColorPixel(Texture2D layer, int x, int y, Color color) {
        layer.SetPixel(x, y, color);
        layer.SetPixel(x, y + 1, color);
        layer.SetPixel(x + 1, y, color);
        layer.SetPixel(x + 1, y + 1, color);
    }

    private void ColorUnits() {
        foreach (Unit unit in this.map.Units) {
            Vector2Int unitPosition = unit.Tile.MapPosition;
            Color color = unit.Faction.ControllingPlayer.Data.Color;
            this.ColorPixel(this.frontLayerTexture, unitPosition.x * 2, this.height - (unitPosition.y * 2), color);
        }
    }

    private void DrawRectangle(int column, int row, Color color) {
        for (int x = column - this.horizontalRadius + 1; x < column + this.horizontalRadius; x++) {
            for (int y = row - this.verticalRadius; y <= row + this.verticalRadius; y += this.verticalRadius * 2 - 2) {
                this.frontLayerTexture.SetPixel(x, y, color);
                this.frontLayerTexture.SetPixel(x, y + 1, color);
            }
        }

        for (int y = row - this.verticalRadius; y < row + this.verticalRadius; y++) {
            for (int x = column - this.horizontalRadius; x <= column + this.horizontalRadius; x += this.horizontalRadius * 2 - 2) {
                this.frontLayerTexture.SetPixel(x, y, color);
                this.frontLayerTexture.SetPixel(x + 1, y, color);
            }
        }
    }

    private void HandleClick(Vector3 clickPosition) {
        Vector2Int position = new Vector2Int(
            (int)clickPosition.x - this.x,
            (int)clickPosition.y - (Screen.height - this.height - this.y)
        );

        this.UpdatePosition(position);
    }

    private void HandleInput() {
        //        if (InteractionHandler.Enabled) {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 position = Input.mousePosition;

            if (this.invertedViewport.Contains(position)) {
                this.isPressing = true;
                this.HandleClick(position);
            }

        } else if (Input.GetMouseButtonUp(0)) {
            this.isPressing = false;

        } else if (Input.GetMouseButton(0)) {
            if (this.isPressing) {
                //to-do: lock the mouse in the viewport
                this.HandleClick(Input.mousePosition);
            }
        }
        // }
    }

    public void Initialize(Vector2Int position) {
        this.SetPosition(position);
    }

    private void NormalizeCoordinate(ref int coordinate, int minimum, int maximum) {
        coordinate = Mathf.Max(maximum, coordinate);
        coordinate = Mathf.Min(minimum, coordinate);

        if (coordinate % 2 == 0) {
            coordinate--;
        }
    }

    public void RemoveBuilding(Building building) {
        this.buildings.Remove(building);
    }

    public void RemoveUnit(Unit unit) {
        this.units.Remove(unit);
    }

    public void SetPosition(Vector2Int position) {
        position.x += this.horizontalRadius;
        position.y = this.height - position.y - this.verticalRadius;

        this.position = position;
    }

    protected override void Start() {
        base.Start();
        
        this.viewport = new Rect(this.x, this.y, this.width, this.height);
        this.invertedViewport = new Rect(this.x, Screen.height - this.y - this.height, this.width, this.height);

        this.backLayerTexture = new Texture2D(this.width, this.width);
        this.frontLayerTexture = new Texture2D(this.width, this.width);

        for (int row = 0; row < 64; row++) {
            for (int column = 0; column < 64; column++) {
                MapTile tile = this.map.GetTile(column, row);

                this.ColorPixel(this.backLayerTexture, column * 2, this.height - (row * 2), tile.Data.Color);
            }
        }

        this.ClearFrontLayer();
        this.backLayerTexture.Apply();
        this.backLayerMaterial.mainTexture = this.backLayerTexture;

        if (this.position != Vector2Int.zero) {
            this.UpdatePosition(this.position);
        } else {
            this.frontLayerTexture.Apply();
            this.frontLayerMaterial.mainTexture = this.frontLayerTexture;
        }
    }

    private void Update() {
        this.HandleInput();

        this.ClearFrontLayer();
        this.ColorBuildings();
        this.ColorUnits();

        this.DrawRectangle(this.lastColumn, this.lastRow, Utils.RGBToColor(192, 192, 192));
        this.frontLayerTexture.Apply();
        this.frontLayerMaterial.mainTexture = this.frontLayerTexture;
    }

    private void UpdatePosition(Vector2Int position) {
        int column = position.x;
        int row = position.y;

        this.NormalizeCoordinate(ref column, this.width - this.horizontalRadius, this.horizontalRadius);
        this.NormalizeCoordinate(ref row, this.height - this.verticalRadius, this.verticalRadius);

        //if (this.lastColumn != -1)
        //    this.DrawRectangle (this.lastColumn, this.lastRow, Color.clear);

        this.lastColumn = column;
        this.lastRow = row;

        //this.DrawRectangle (column, row, Utils.ColorFromRGb (192, 192, 192));
        //this.frontLayer.Apply ();

        column = (column - this.horizontalRadius) / 2;
        row = (this.height - (row + this.verticalRadius)) / 2;

        this.grid.SetPosition(new Vector2Int(column, row));
    }
}

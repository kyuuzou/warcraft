using UnityEngine;

public class Grid : SceneObject {

    [SerializeField]
    private int columns = 15;
    public int Columns {
        get { return this.columns; }
        private set { this.columns = value; }
    }

    [SerializeField]
    private int rows = 11;
    public int Rows {
        get { return this.rows; }
        private set { this.rows = value; }
    }

    [SerializeField]
    private Vector2 defaultSlotSize;
    public Vector2 DefaultSlotSize {
        get { return this.defaultSlotSize; }
    }

    [SerializeField]
    private Vector2 defaultSlotPadding;
    public Vector2 DefaultSlotPadding {
        get { return this.defaultSlotPadding; }
    }

    private Rect viewport;
    private Vector2Int position = new Vector2Int(-1, -1);

    //Distance in pixels from the upper left edge of the grid to the origin of the world coordinates
    private Vector2Int center;

    [SerializeField]
    private Transform slot;

    private Map map;
    private TileSlot[,] slots;

    protected override void Awake() {
        base.Awake();

        Vector3 position = this.transform.position;

        this.center = new Vector2Int(
            (int)Mathf.Abs(position.x),
            (int)Mathf.Abs(position.y)
        );

        this.viewport = new Rect(position.x - 16.0f, position.y - 16.0f, this.columns * 32.0f, this.rows * 32.0f);

        Debug.DrawLine(
            new Vector3(this.viewport.xMin, this.viewport.yMin),
            new Vector3(this.viewport.xMax, this.viewport.yMax),
            Color.cyan
        );
    }

    public Vector2Int GetPosition() {
        return this.position;
    }

    public TileSlot GetTileSlot(MapTile tile) {
        Vector2Int tilePosition = tile.MapPosition;

        if ((tilePosition.x < this.position.x) || (tilePosition.x > this.position.x + this.columns)) {
            return null;
        }

        if ((tilePosition.y < this.position.y) || (tilePosition.y > this.position.y + this.rows)) {
            return null;
        }

        return this.slots[tilePosition.x - this.position.x, tilePosition.y - this.position.y];
    }

    public Rect GetViewport() {
        return this.viewport;
    }

    public void OnFinishedParsingLevel() {
        this.InitializeExternals();

        /*
        UnitGroup currentGroup = this.gameController.CurrentGroup;

        if (currentGroup.Units.Count > 0) {
            Vector2Int position = currentGroup.Units[0].Tile.MapPosition * 2;
            this.SetPosition(position * 0.5f);
        } else {
            this.SetPosition(Vector2Int.zero);
        }*/
    }

    public void Refresh() {
        this.SetPosition(this.position);
    }

    public void SetPosition(Vector2Int position) {
        if (this.position == position) {
            return;
        }

        this.position = position;

        MapTile tile;

        for (int row = 0; row < this.map.Rows; row++) {
            for (int column = 0; column < this.map.Columns; column++) {

                tile = this.map.GetTile(column, row);

                bool visible = false;

                if ((column >= position.x) && column < (this.columns + position.x)) {
                    if ((row >= position.y) && row < (this.rows + position.y)) {
                        visible = true;
                    }
                }

                //tile.SetVisible (visible);

                tile.RealPosition = new Vector3(
                    (column - position.x) * 32.0f - this.center.x,
                    this.center.y - ((row - position.y) * 32.0f),
                    -1.0f
                );

                if (visible) {
                    this.slots[column - position.x, row - position.y].SetTile(tile);
                }
            }
        }
    }

    private void Start() {
        this.map = ServiceLocator.Instance.Map;

        this.slots = new TileSlot[this.columns, this.rows];

        for (int column = 0; column < this.columns; column++) {
            for (int row = 0; row < this.rows; row++) {
                Transform transform = Transform.Instantiate(this.slot, Vector3.zero, this.slot.rotation);
                transform.name = this.slot.name;

                TileSlot slot = transform.GetComponent<TileSlot>();
                slot.SetPosition(column, row);
                slot.SetParent(this.transform);
                slot.Initialize(null);

                this.slots[column, row] = slot;
            }
        }
    }

    private void Update() {
        this.Refresh();
    }
}

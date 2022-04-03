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
    private IntVector2 position = new IntVector2(-1, -1);

    //Distance in pixels from the upper left edge of the grid to the origin of the world coordinates
    private IntVector2 center;

    [SerializeField]
    private Transform slot;

    private Map map;
    private TileSlot[,] slots;

    private void Awake() {
        Vector3 position = this.transform.position;

        this.center = new IntVector2(
            Mathf.Abs(position.x),
            Mathf.Abs(position.y)
        );

        this.viewport = new Rect(position.x - 16.0f, position.y - 16.0f, this.columns * 32.0f, this.rows * 32.0f);

        Debug.DrawLine(
            new Vector3(this.viewport.xMin, this.viewport.yMin),
            new Vector3(this.viewport.xMax, this.viewport.yMax),
            Color.cyan
        );
    }

    public IntVector2 GetPosition() {
        return this.position;
    }

    public TileSlot GetTileSlot(MapTile tile) {
        IntVector2 tilePosition = tile.MapPosition;

        if ((tilePosition.X < this.position.X) || (tilePosition.X > this.position.X + this.columns)) {
            return null;
        }

        if ((tilePosition.Y < this.position.Y) || (tilePosition.Y > this.position.Y + this.rows)) {
            return null;
        }

        return this.slots[tilePosition.X - this.position.X, tilePosition.Y - this.position.Y];
    }

    public Rect GetViewport() {
        return this.viewport;
    }

    public void OnFinishedParsingLevel() {
        this.InitializeExternals();

        /*
        UnitGroup currentGroup = this.gameController.CurrentGroup;

        if (currentGroup.Units.Count > 0) {
            IntVector2 position = currentGroup.Units[0].Tile.MapPosition * 2;
            this.SetPosition(position * 0.5f);
        } else {
            this.SetPosition(IntVector2.zero);
        }*/
    }

    public void Refresh() {
        this.SetPosition(this.position);
    }

    public void SetPosition(IntVector2 position) {
        if (this.position == position) {
            return;
        }

        this.position = position;

        MapTile tile;

        for (int row = 0; row < this.map.Rows; row++) {
            for (int column = 0; column < this.map.Columns; column++) {

                tile = this.map.GetTile(column, row);

                bool visible = false;

                if ((column >= (int)position.X) && column < (this.columns + (int)position.X)) {
                    if ((row >= (int)position.Y) && row < (this.rows + (int)position.Y)) {
                        visible = true;
                    }
                }

                //tile.SetVisible (visible);

                tile.RealPosition = new Vector3(
                    (column - position.X) * 32.0f - this.center.X,
                    this.center.Y - ((row - position.Y) * 32.0f),
                    -1.0f
                );

                if (visible) {
                    this.slots[column - (int)position.X, row - (int)position.Y].SetTile(tile);
                }
            }
        }
    }

    private void Start() {
        this.map = ServiceLocator.Instance.Map;

        this.slots = new TileSlot[this.columns, this.rows];

        for (int column = 0; column < this.columns; column++) {
            for (int row = 0; row < this.rows; row++) {
                Transform transform = Transform.Instantiate(this.slot, Vector3.zero, this.slot.rotation) as Transform;
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

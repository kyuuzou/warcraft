using UnityEngine;
using System.Collections;

public class IsometricGrid : SceneObject {
    
    [SerializeField]
    private int columns = 15;
    public int Columns {
        get { return this.columns; }
    }
    
    [SerializeField]
    private int rows = 11;
    public int Rows {
        get { return this.rows; }
    }
    
    [SerializeField]
    private Transform slotPrefab;

    [SerializeField]
    private Vector2 defaultSlotPadding;
    public Vector2 DefaultSlotPadding {
        get { return this.defaultSlotPadding; }
    }

    [SerializeField]
    private Vector2 defaultSlotSize;
    public Vector2 DefaultSlotSize {
        get { return this.defaultSlotSize; }
    }

    private GameController gameController;
    private MainCamera mainCamera;
    private Map map;
    private Minimap minimap;
    private IntVector2 position = new IntVector2 (0, 0);
    private IsometricTileSlot[,] slots;
    private Rect viewport;
    
    public SceneObject SceneObject {
        get { return this; }
    }
    
    protected override void Awake () {
        base.Awake ();
        
        this.viewport = new Rect (0.0f, 0.0f, 640.0f, 480.0f);
    }

    public void ClearColors () {
        /*
        foreach (IsometricTileSlot slot in this.slots) {
            for (int i = 0; i < 2; i ++) {
                slot.GetLayer (i).SpriteRenderer.color = Color.white;
            }
        }
        */
    }

    public MapTile FindClosestTile (Vector2 mousePosition) {
        float minimumDistance = float.MaxValue;
        MapTile closestTile = null;

        mousePosition = Camera.main.ScreenToWorldPoint (mousePosition);

        foreach (IsometricTileSlot slot in this.slots) {
            if (slot.Tile == null) {
                continue;
            }

            Vector2 position = slot.Tile.RealPosition;
            position.y += slot.Tile.CurrentDepth + slot.Tile.Offset;

            float distance = Vector2.Distance (position, mousePosition);

            if (distance < minimumDistance) {
                minimumDistance = distance;
                closestTile = slot.Tile;
            }
        }

        //Debug.Log (closestTile);

        return closestTile;
    }

    public IntVector2 GetPosition () {
        return this.position;
    }
    
    public IsometricTileSlot GetTileSlot (MapTile tile) {
        IntVector2 tilePosition = tile.MapPosition;
        
        if ((tilePosition.X < this.position.X) || (tilePosition.X > this.position.X + this.columns)) {
            return null;
        }
        
        if ((tilePosition.Y < this.position.Y) || (tilePosition.Y > this.position.Y + this.rows)) {
            return null;
        }
        
        return this.slots[(int) (tilePosition.X - this.position.X), (int) (tilePosition.Y - this.position.Y)];
    }
    
    public Rect GetViewport () {
        return this.viewport;
    }
    
    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }
        
        base.InitializeExternals ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.mainCamera = serviceLocator.MainCamera;
        this.map = serviceLocator.Map;
        this.minimap = serviceLocator.Minimap;

        this.slots = new IsometricTileSlot[this.columns, this.rows];

        for (int column = 0; column < this.columns; column ++) {
            for (int row = 0; row < this.rows; row ++) {
                Transform transform = Transform.Instantiate (
                    this.slotPrefab, Vector3.zero, this.slotPrefab.rotation
                ) as Transform;

                transform.name = this.slotPrefab.name;
                
                IsometricTileSlot slot = transform.GetComponent <IsometricTileSlot>();
                slot.SetPosition (column, row);
                slot.SetParent (this.transform);
                
                this.slots[column, row] = slot;
            }
        }
    }
    
    public void OnFinishedParsingLevel () {
        this.InitializeExternals ();

        UnitGroup currentGroup = this.gameController.CurrentGroup;

        if (currentGroup.Units.Count > 0) {
            IntVector2 position = currentGroup.Units[0].Tile.MapPosition * 2;
            this.SetPosition (position * 0.5f);
        } else {
            this.SetPosition(new IntVector2(0, 0));
        }
    }

    public override bool OnManualMouseDown () {
        /*
        Unit unit = this.gameController.CurrentGroup.GetSquadLeader ();
        MapTile tile = this.FindClosestTile (Input.mousePosition);

        //Debug.Log (tile.MapPosition);

        if (this.inputController.MayNotify (unit)) {
            unit.Interact (tile);
        }*/

        return true;

        /*
        }

        Vector2 mousePosition = this.mainCamera.Camera.ScreenToWorldPoint (Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer ("Slots");
        RaycastHit2D[] hits = Physics2D.RaycastAll (mousePosition, Vector2.zero, 1000.0f, layerMask);

        float closestDistance = float.MaxValue;
        IsometricTileSlot closestSlot = null;

        foreach (RaycastHit2D hit in hits) {
            IsometricTileSlot slot = hit.transform.GetComponent<IsometricTileSlot> ();
            Vector2 tilePosition = slot.GetLayer (0).Transform.position;
            tilePosition.y += 48.0f;

            float distance = Vector2.Distance (tilePosition, mousePosition);

            if (distance < closestDistance) {
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        if (closestSlot == null) {
            return false;
        }

        if (closestSlot == null) {
            Debug.LogError ("Slot is null.");
            return false;
        }

        return closestSlot.OnManualMouseDown ();
        */
    }

    public void Refresh () {
        this.SetPosition (this.position, true);
    }
    
    public void Refresh (MapTile tile) {
        /*
        if (tile.Visible) {
            IntVector2 tilePosition = tile.MapPosition;
            
            int x = tilePosition.X - this.position.X;
            int y = tilePosition.Y - this.position.Y;

            TileSlot slot = this.slots [x, y];
            slot.SetTile (tile);
            tile.Slot = slot;

            slot.Updated = true;
        } else {
            tile.Slot = null;
        }
        */
    }
    
    public void Refresh (MapTile tile, bool apply) {
        this.Refresh ();
    }
    
    public void SetPosition (IntVector2 position, bool forceRefresh = false) {
        position.X -= Mathf.FloorToInt (this.Columns * 0.5f);
        position.Y -= Mathf.FloorToInt (this.Rows * 0.5f);

        if (this.position == position && ! forceRefresh) {
            return;
        }

        this.position = position;
        
        MapTile tile;

        Vector2 half = this.DefaultSlotSize * 0.5f;

        for (int row = 0; row < this.map.Rows; row ++) {
            for (int column = 0; column < this.map.Columns; column ++) {
                tile = this.map.GetTile (column, row);
                
                bool visible = false;
                
                if ((column >= (int) position.X) && column < (this.columns + (int) position.X)) {
                    if ((row >= (int) position.Y) && row < (this.rows + (int) position.Y)) {
                        visible = true;
                    }
                }

                tile.Visible = visible;

                Vector3 realPosition = new Vector3 (
                    (column - position.X) * half.x - (row - position.Y) * half.x,
                    (row - position.Y) * half.y + (column - position.X) * half.y + this.transform.localPosition.y,
                    -1.0f
                );

                tile.RealPosition = realPosition;
                this.Refresh (tile);
            }
        }

        foreach (IsometricTileSlot slot in this.slots) {
            if (slot.Updated) {
                slot.Updated = false;
            } else {
                slot.SetTile (null);
            }
        }

        //this.texture.Apply ();
    }
}

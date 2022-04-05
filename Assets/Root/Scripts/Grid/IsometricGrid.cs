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
    private Vector2Int position = new Vector2Int (0, 0);
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

    public Vector2Int GetPosition () {
        return this.position;
    }
    
    public IsometricTileSlot GetTileSlot (MapTile tile) {
        Vector2Int tilePosition = tile.MapPosition;
        
        if ((tilePosition.x < this.position.x) || (tilePosition.x > this.position.x + this.columns)) {
            return null;
        }
        
        if ((tilePosition.y < this.position.y) || (tilePosition.y > this.position.y + this.rows)) {
            return null;
        }
        
        return this.slots[tilePosition.x - this.position.x, tilePosition.y - this.position.y];
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
                );

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
            Vector2Int position = currentGroup.Units[0].Tile.MapPosition * 2;
            this.SetPosition (position.Multiply(0.5f));
        } else {
            this.SetPosition(new Vector2Int(0, 0));
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
            Vector2Int tilePosition = tile.MapPosition;
            
            int x = tilePosition.x - this.position.x;
            int y = tilePosition.y - this.position.y;

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
    
    public void SetPosition (Vector2Int position, bool forceRefresh = false) {
        position.x -= Mathf.FloorToInt (this.Columns * 0.5f);
        position.y -= Mathf.FloorToInt (this.Rows * 0.5f);

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
                
                if ((column >= position.x) && column < (this.columns + position.x)) {
                    if ((row >= position.y) && row < (this.rows + position.y)) {
                        visible = true;
                    }
                }

                tile.Visible = visible;

                Vector3 realPosition = new Vector3 (
                    (column - position.x) * half.x - (row - position.y) * half.x,
                    (row - position.y) * half.y + (column - position.x) * half.y + this.transform.localPosition.y,
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

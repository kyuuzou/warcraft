using System.Collections;
using UnityEngine;

public class IsometricTileSlot : SceneObject {

    [SerializeField]
    private Transform layerRoot;
    public Transform LayerRoot {
        get { return this.layerRoot; }
    }

    [SerializeField]
    private TextMesh valueText;
    
    private IsometricTileSlotLayer[] layers;
    private Vector2[] originalUV;
    private Transform parent;
    private Transform slot;

    //private CursorStylist cursorStylist;
    private GameController gameController;
    private Grid grid;
    private ResourceManager resourceManager;

    public int Column { get; private set; }
    public bool IsBehindEverything { get; set; }
    public int Row { get; private set; }
    public MapTile Tile { get; private set; }
    public bool Updated { get; set; }

    public SceneObject SceneObject {
        get { return this; }
    }

    public IsometricTileSlotLayer GetLayer (int index) {
        return this.layers[index];
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        //this.cursorStylist = serviceLocator.CursorStylist;
        this.gameController = serviceLocator.GameController;
        this.grid = serviceLocator.Grid;
        this.resourceManager = serviceLocator.ResourceManager;

        this.InitializeLayers ();

        //this.valueText.gameObject.SetActive (false);
        this.valueText.transform.SetZ (- 100.0f);
    }

    private void InitializeLayers () {
        this.layers = this.GetComponentsInChildren<IsometricTileSlotLayer> (true, true);

        foreach (IsometricTileSlotLayer layer in this.layers) {
            layer.InitializeExternals ();
        }

        //to prevent z fighting
        float zOffset = - 0.01f * this.Column;

        this.layers[0].Transform.SetLocalZ (zOffset);
        //this.layers[0].Transform.SetZ (300.0f + zOffset);

        for (int i = 1; i < this.layers.Length; i ++) {
            this.layers[i].MeshRenderer.enabled = false;

            this.layers[i].Transform.SetLocalPosition (
                0.0f,
                i * this.grid.DefaultSlotSize.y,
                - i * 0.5f + zOffset
            );
        }
    }

#if DRAW_GIZMOS
    private void OnDrawGizmos () {
        Gizmos.color = Color.red;

        if (this.Tile == null) {
            return;
        }

        Vector3 position = this.Tile.RealPosition;
        position.y += this.Tile.CurrentDepth + this.Tile.Offset;
        Gizmos.DrawSphere (position, 15.0f);

        Gizmos.DrawLine (position - new Vector3 (64.0f, 0.0f, 0.0f), position + new Vector3 (64.0f, 0.0f, 0.0f));
        Gizmos.DrawLine (position - new Vector3 (0.0f, 32.0f, 0.0f), position + new Vector3 (0.0f, 32.0f, 0.0f));
    }
#endif

    /*
    public override bool OnManualMouseDown () {
        base.OnManualMouseDown ();

        Unit unit = this.gameController.CurrentGroup.GetSquadLeader ();
        MapTile tile = this.grid.FindClosestTile (Input.mousePosition);

        if (this.inputController.MayNotify (unit)) {
            unit.Interact (tile);
        }

        return true;
    }
    */

    public void Refresh () {
        if (this.Tile == null) {
            return;
        }

        this.LayerRoot.SetLocalY (this.Tile.CurrentDepth);

        foreach (IsometricTileSlotLayer layer in this.layers) {
            layer.SetColor (this.Tile.Color);
        }

        this.layers[0].MeshRenderer.enabled = true;
        this.layers[0].SetTile (this.Tile);

        float index = this.Tile.MapPosition.X + this.Tile.MapPosition.Y;

        float y = this.layers[0].MeshRenderer.bounds.size.y * 0.5f;
        y = y - this.grid.DefaultSlotPadding.y - this.grid.DefaultSlotSize.y * 0.5f;
        y = y - this.Tile.Data.Offset.y + this.Tile.Offset;

        this.layers[0].Transform.SetLocalY (y);

        for (int i = 1; i < this.layers.Length; i ++) {
            IsometricTileSlotLayer layer = this.layers[i];
            TileData data = this.Tile.GetLayer (i).TileData;

            if (data == null) {
                layer.MeshRenderer.enabled = false;
            } else {
                layer.MeshRenderer.enabled = true;

                if (data.Sprite != null) {
                    layer.Transform.SetLocalY (i * this.grid.DefaultSlotSize.y + data.Sprite.bounds.size.y * 0.5f);
                }
            }
        }
    }

    public void SetParent (Transform parent) {
        this.transform.parent = this.parent = parent;
    }
    
    public void SetPosition (int column, int row) {
        this.InitializeExternals ();

        this.Column = column;
        this.Row = row;

        this.transform.name = "Slot (" + this.Column + ", " + this.Row + ")";

        Vector2 half = this.grid.DefaultSlotSize * 0.5f;
        
        this.Transform.localPosition = new Vector3 (
            this.Column * half.x - this.Row * half.x,
            this.Row * half.y + this.Column * half.y,
            (this.Row + this.Column) * 10.0f
        );
    }
    
    public void SetTile (MapTile tile) {
        this.Tile = tile;

        if (tile == null || tile.Type == TileType.None/* || ! tile.Lit*/) {
            foreach (IsometricTileSlotLayer layer in this.layers) {
                layer.MeshRenderer.enabled = false;
            }

#if UNITY_EDITOR
            this.valueText.text = string.Empty;
#endif
            return;
        }

        this.Refresh ();
    }

    private void Update () {
        // TODO: Uncomment and optimize
        /*
        if (this.MouseIsOver) {
            if (this.interactionHandler.CurrentMode == InteractionModeType.Building) {

            } else if (this.interactionHandler.CurrentMode == InteractionModeType.Default) {
                if (Input.GetKey (KeyCode.LeftControl)) {
                    this.cursorStylist.SetCursor (CursorType.SmallGreenCrosshair);
                }
            } else {
                this.cursorStylist.SetCursor (CursorType.YellowCrosshair);
            }
        }

        this.UpdateCaption ();
        */
    }

    private void UpdateCaption () {
#if UNITY_EDITOR && SKIP
        if (this.Tile != null) {
            //this.valueText.text = this.Tile.Type.ToString ();
            //this.valueText.text = this.Tile.Caption;
            this.valueText.text = this.Tile.Visible.ToString ();
            //this.valueText.text = string.Format ("({0}, {1})", Column, this.Row);
            //this.valueText.text = this.Tile.MapPosition.ToString ();
            //this.valueText.text = this.Tile.LayerRootDepth.ToString ();
            //this.valueText.text = this.Tile.Zone == null ? string.Empty : this.Tile.Zone.Index.ToString ();
            /*
            this.valueText.text = string.Format (
                "({0}, {1}, {2}, {3})",
                this.Transform.position.z,
                this.layers[0].Transform.position.z,
                this.layers[1].Transform.position.z,
                this.layers[2].Transform.position.z
            );
            */

            //Unit unit = this.Tile.GetInhabitant<Unit> ();
            //this.valueText.text = unit == null ? string.Empty : unit.Type.ToString ();
        } else {
            this.valueText.text = string.Empty;
        }
#endif
    }
}

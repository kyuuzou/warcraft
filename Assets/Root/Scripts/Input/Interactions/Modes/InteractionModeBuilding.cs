using UnityEngine;

public class InteractionModeBuilding : InteractionMode {

    private MapGrid grid;
    private Map map;
    private Rect viewport;

    private Building building;
    private Transform buildingTransform;
    private Vector2Int buildingSize;
    private Vector3 offset;

    private Unit builder;

    private SpriteSelection selection;

    public InteractionModeBuilding() {
        this.grid = ServiceLocator.Instance.Grid;
        this.map = ServiceLocator.Instance.Map;
    }

    public override void DisableMode() {
        GameFlags.Building = false;
    }

    public override void EnableMode(InteractionModeArgs args = null) {
        GameFlags.Building = true;

        if (args == null) {
            return;
        }

        InteractionModeBuildingArgs modeArgs = (InteractionModeBuildingArgs)args;

        this.building = modeArgs.GetBuilding();
        this.buildingTransform = this.building.transform;
        this.buildingSize = this.building.TileSize;
        this.offset = this.building.Offset;
        this.offset.z = -3.0f;

        this.building.Collider.enabled = false;

        this.builder = modeArgs.GetBuilder();

        this.selection = this.building.GetSelection();
        this.selection.SetLocked(true);
    }

    public override void Start() {
        this.viewport = this.grid.GetViewport();
    }

    public override void Update() {
        base.Update();

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = this.camera.ScreenToWorldPoint(mousePosition);

        if (this.viewport.Contains(worldMousePosition)) {
            Vector3 mouseWorldPoint = this.camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPoint, Vector2.zero);
            RaycastHit2D hit = new RaycastHit2D();

            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].collider.tag == "Ground") {
                    hit = hits[i];
                    break;
                }
            }

            if (hit.collider != null) {
                if (!this.selection.IsVisible()) {
                    this.selection.SetVisible(true, true);
                }

                this.buildingTransform.position = hit.transform.position + this.offset;

                TileSlot slot = hit.collider.GetComponent<TileSlot>();
                MapTile tile = slot.Tile;
                bool invalid = false;

                Vector2Int position = tile.MapPosition;
                int column = position.x;
                int row = position.y;

                for (int y = column; y < this.buildingSize.y + column; y++) {
                    for (int x = row; x < this.buildingSize.x + row; x++) {
                        MapTile neighbour = this.map.GetTile(y, x);

                        if (neighbour.GetInhabitant<Building>() || neighbour.GetInhabitant<Unit>() || !neighbour.IsTraversable(MovementType.Land)) {
                            invalid = true;
                        }
                    }
                }

                this.selection.SetInvalid(invalid);

                if (!invalid) {
                    if (Input.GetMouseButtonDown(0)) {
                        this.building.Initialize(tile);
                        this.building.SetUpConstructionSite(this.builder, tile);

                        //this.builder.Build (this.building);

                        InteractionHandler.Instance.SetMode(InteractionModeType.Regular);
                        //this.builder.PressCancel ();
                    }
                }
            }
        } else if (this.selection.IsVisible()) {
            this.selection.SetVisible(false, true);
        }
    }
}

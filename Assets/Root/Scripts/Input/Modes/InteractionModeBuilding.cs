using UnityEngine;

public class InteractionModeBuilding : InteractionMode {

    private Grid grid;
    private Map map;
    private Rect viewport;

    private Building building;
    private Transform buildingTransform;
    private IntVector2 buildingSize;
    private Vector3 offset;

    private Unit builder;

    private SpriteSelection selection;

    public InteractionModeBuilding() : base() {
        this.grid = ServiceLocator.Instance.Grid;
        this.map = ServiceLocator.Instance.Map;
    }

    public override void DisableMode() {
        GameFlags.building = false;
    }

    public override void EnableMode(InteractionModeArgs args = null) {
        GameFlags.building = true;

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
            Ray ray = this.camera.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 30.0f);

            RaycastHit hit = new RaycastHit();

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

                IntVector2 position = tile.MapPosition;
                int column = position.X;
                int row = position.Y;

                for (int y = column; y < this.buildingSize.Y + column; y++) {
                    for (int x = row; x < this.buildingSize.X + row; x++) {
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
                        /*
                        this.building.SetUpConstructionSite();
                        
                        this.peasant.Build (this.building);
                        
                        InputHandler.Instance.SetMode (InteractionModeType.Regular);
                        this.peasant.PressCancel ();*/
                    }
                }
            }
        } else if (this.selection.IsVisible()) {
            this.selection.SetVisible(false, true);
        }
    }
}

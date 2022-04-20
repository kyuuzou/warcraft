using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomSprite : SceneObject, IAnimationTriggerListener {

    [SerializeField]
    private Vector2 atlasSize;
    protected Vector2 AtlasSize => this.atlasSize;

    public Vector2 Offset { get; set; }
    public MapTile Tile { get; set; }

    public List<MapTile> ClaimedTiles { get; protected set; }
    protected Vector3 Position { get; set; }

    protected AudioManager AudioManager { get; private set; }
    protected Map Map { get; private set; }
    protected GameController GameController { get; private set; }
    protected MapGrid Grid { get; private set; }

    private Direction direction;
    public Direction Direction {
        get { return this.direction; }
        set {
            this.direction = value;
            this.MeshAnimator.Direction = value;
        }
    }

    public delegate void GameObjectDestroyedHandler(object sender, EventArgs e);
    private event GameObjectDestroyedHandler GameObjectDestroyed;

    protected override void Awake() {
        base.Awake();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.AudioManager = serviceLocator.AudioManager;
        this.Map = serviceLocator.Map;
        this.Grid = serviceLocator.Grid;
        this.GameController = serviceLocator.GameController;
    }

    public virtual void ClaimTile(MapTile tile) {

    }

    public Direction FindDirection(MapTile origin, MapTile destination, bool allowDiagonals = true) {
        return this.FindDirection(origin.MapPosition, destination.MapPosition, allowDiagonals);
    }

    public Direction FindDirection(Vector2Int origin, Vector2Int destination, bool allowDiagonals = true) {
        Vector2Int offset = destination - origin;
        //offset.y *= -1.0f;

        Vector2Int absOffset = new Vector2Int(
            Mathf.Abs(offset.x),
            Mathf.Abs(offset.y)
        );

        if (absOffset.x > 1 || absOffset.y > 1) {
            if (absOffset.x > absOffset.y) {
                offset.x /= absOffset.x;
                offset.y = 0;
            } else if (absOffset.x < absOffset.y) {
                offset.x = 0;
                offset.y /= absOffset.y;
            } else {
                offset.x /= absOffset.x;
                offset.y /= absOffset.y;
            }
        }

        Direction direction = DirectionDictionary.Instance.GetKey(offset);

        if (!allowDiagonals && ((int)direction) % 2 == 0) {
            direction = (direction - 1).RoundClamp();
        }

        if (direction != Direction.None) {
            this.Direction = direction;
        }

        return direction;
    }

    public virtual void Initialize(MapTile tile) {
        this.Tile = tile;

        this.ClaimedTiles = new List<MapTile>();
    }

    public void ManualDestroy() {
        MonoBehaviour.DestroyImmediate(this.gameObject);

        if (this.GameObjectDestroyed != null) {
            this.GameObjectDestroyed(this, EventArgs.Empty);
        }
    }

    public virtual void ManualUpdate() {

    }

    public virtual void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {

    }

    protected void OnManualDestroy(object sender, EventArgs args) {
        this.ManualDestroy();
    }

    public void Register(GameObjectDestroyedHandler handler) {
        this.GameObjectDestroyed += handler;
    }

    public void SetAnimatorLayer(int layer) {
    }

    public virtual void SetTile(MapTile tile) {
        this.Tile = tile;
    }
}

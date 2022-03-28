using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomSprite: SceneObject, IAnimationTriggerListener {

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
    protected Grid Grid { get; private set; }

    private Direction direction;
    public Direction Direction {
        get { return this.direction; }
        set {
            this.direction = value;
            this.MeshAnimator.Direction = value;
        }
    }

    public delegate void GameObjectDestroyedHandler (object sender, EventArgs e);
    private event GameObjectDestroyedHandler GameObjectDestroyed;

    protected override void Awake () {
        base.Awake ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.AudioManager = serviceLocator.AudioManager;
        this.Map = serviceLocator.Map;
        this.Grid = serviceLocator.Grid;
        this.GameController = serviceLocator.GameController;
    }

    public virtual void ClaimTile (MapTile tile) {

    }

    public Direction FindDirection (MapTile origin, MapTile destination, bool allowDiagonals = true) {
        return this.FindDirection (origin.MapPosition, destination.MapPosition, allowDiagonals);
    }

    public Direction FindDirection (IntVector2 origin, IntVector2 destination, bool allowDiagonals = true) {
        IntVector2 offset = destination - origin;
        //offset.y *= -1.0f;

        IntVector2 absOffset = new IntVector2 (
            Mathf.Abs (offset.X),
            Mathf.Abs (offset.Y)
        );

        if (absOffset.X > 1 || absOffset.Y > 1) {
            if (absOffset.X > absOffset.Y) {
                offset.X /= absOffset.X;
                offset.Y = 0;
            } else if (absOffset.X < absOffset.Y) {
                offset.X = 0;
                offset.Y /= absOffset.Y;
            } else {
                offset.X /= absOffset.X;
                offset.Y /= absOffset.Y;
            }
        }

        Direction direction = DirectionDictionary.Instance.GetKey (offset);

        if (! allowDiagonals && ((int) direction) % 2 == 0) {
            direction = (direction - 1).RoundClamp ();
        }

        if (direction != Direction.None) {
            this.Direction = direction;
        }

        return direction;
    }
    
    public virtual void Initialize (MapTile tile) {
        this.Tile = tile;

        this.ClaimedTiles = new List<MapTile> ();

        /*Vector2 tilePosition = this.Tile.RealPosition;
        this.Position = new Vector3 (tilePosition.x, tilePosition.y, this.Transform.position.z);
        this.transform.position = this.Position;*/
    }

    public void ManualDestroy () {
        this.GetComponent<Renderer>().enabled = false;
        MonoBehaviour.DestroyImmediate (this.gameObject);

        if (this.GameObjectDestroyed != null) {
            this.GameObjectDestroyed (this, EventArgs.Empty);
        }
    }

    public virtual void ManualUpdate () {

    }

    public virtual void OnAnimationTrigger (AnimationType animationType, AnimationTriggerType triggerType) {
        
    }
    
    protected void OnManualDestroy (object sender, EventArgs args) {
        this.ManualDestroy ();
    }

    public void Register (GameObjectDestroyedHandler handler) {
        this.GameObjectDestroyed += handler;
    }

    public void SetAnimatorLayer (int layer) {
        /*
        for (int i = 0; i < this.Animator.layerCount; i ++) {
            this.Animator.SetLayerWeight (i, 0.0f);
		}

        this.Animator.SetLayerWeight (layer, 1.0f);
        */
    }

    public virtual void SetTile (MapTile tile) {
        this.Tile = tile;
    }
}

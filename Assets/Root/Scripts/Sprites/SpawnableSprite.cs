using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class SpawnableSprite : CustomSprite, IInhabitant, ITarget {

    protected abstract SpawnableSpriteData BasicData { get; }

    public SpawnableSpriteData Data {
        get { return this.BasicData; }
    }

    public virtual bool Selectable {
        get { return true; }
    }

    [SerializeField]
    private SpriteSelection selectionPrefab;

    [SerializeField]
    private SceneObject sprite;
    public SceneObject Sprite {
        get { return this.sprite; }
    }

    protected SpriteSelection selection;
    public SpriteSelection Selection {
        get { return this.selection; }
        set { this.selection = value; }
    }

    protected int statusBackgroundIndex = 8;
    public int StatusBackgroundIndex {
        get { return this.statusBackgroundIndex; }
        set { this.statusBackgroundIndex = value; }
    }

    public GameButtonType Portrait {
        get { return this.Data.Portrait; }
    }

    public string Title {
        get { return this.Data.Title; }
    }

    public float HitPointPercentage {
        get { return this.CurrentHitPoints / (float)this.Data.HitPoints; }
    }

    public float ManaPointPercentage {
        get { return this.CurrentManaPoints / (float)this.Data.ManaPoints; }
    }

    [SerializeField]
    private Vector2Int tileSize = new Vector2Int(1, 1);
    public Vector2Int TileSize {
        get { return this.tileSize; }
        private set { this.tileSize = value; }
    }

    public MapTile Pivot {
        get { return this.GetRealTile(); }
    }

    public virtual MapTile TargetTile {
        get { return this.Tile; }
        set { this.Tile = value; }
    }

    public bool Traversable {
        get { return false; }
    }

    protected ContextMenu ContextMenu { get; private set; }
    protected CursorStylist CursorStylist { get; private set; }
    protected MissionStatistics MissionStatistics { get; private set; }
    protected SpawnFactory SpawnFactory { get; private set; }
    protected StatusPane StatusPane { get; private set; }

    public int CurrentHitPoints { get; private set; }
    public int CurrentManaPoints { get; private set; }
    public bool Dead { get; private set; }
    public Faction Faction { get; set; }
    public bool Invincible { get; set; }
    public bool Selected { get; protected set; }
    public abstract SpawnableSpriteType SpriteType { get; }

    private List<IDeathListener> deathListeners;
    private List<IPhasedOutListener> phasedOutListeners;
    private Dictionary<Type, ISpawnableTrait> traitByType;

    private bool phasedOut = false;
    public bool PhasedOut {
        get { return this.phasedOut; }
        set {
            this.phasedOut = value;

            if (value) {
                this.OnPhasedOut();
            }
        }
    }

    //private int consecutiveSelections = 0;
    //private float lastSelection = float.MinValue;
    //private int selectionsBeforeAnnoyed = 3;

    protected virtual void AdaptTexturesToMapType() {

    }

    public void AddDeathListener(IDeathListener listener) {
        if (!this.deathListeners.Contains(listener)) {
            this.deathListeners.Add(listener);
        }
    }

    public void AddPhasedOutListener(IPhasedOutListener listener) {
        if (!this.phasedOutListeners.Contains(listener)) {
            this.phasedOutListeners.Add(listener);
        }
    }

    private void AdjustCollider() {
        BoxCollider2D collider = this.GetCollider<BoxCollider2D>();
        collider.size = this.TileSize * this.Grid.DefaultSlotSize;
    }

    protected override void Awake() {
        base.Awake();

        this.InitializeExternals();
    }

    public override void ClaimTile(MapTile tile) {
        this.ReleaseClaimedTiles();

        Vector2Int position = tile.MapPosition;
        int column = position.x;
        int row = position.y;

        for (int x = 0; x < this.TileSize.x; x++) {
            for (int y = 0; y < this.TileSize.y; y++) {
                MapTile neighbour = this.Map.GetTile(x + column, row + y);
                neighbour.AddInhabitant(this);
                //neighbour.Caption = this.Type.ToString ();

                this.ClaimedTiles.Add(neighbour);
            }
        }
    }

    public virtual void Damage(int damage) {
        if (this.Invincible) {
            return;
        }

        if (!this.Dead) {
            this.SetHitPoints(this.CurrentHitPoints - damage);

            if (this.CurrentHitPoints <= 0) {
                this.Die();
            }
        }
    }

    public void DamagePercentage(float percentage) {
        this.Damage((int)(this.CurrentHitPoints * percentage));
    }

    public virtual void Die() {
        if (!this.Dead) {
            this.Dead = true;

            foreach (IDeathListener listener in this.deathListeners) {
                listener.OnDeathNotification(this);
            }

            this.deathListeners.Clear();
        }
    }

    public void FilterButtons(ref List<GameButtonType> buttons) {
        foreach (ISpawnableTrait trait in this.traitByType.Values) {
            trait.FilterButtons(ref buttons);
        }
    }

    public Vector2Int[] GetBoundaries() {
        Vector2Int position = this.Tile.MapPosition;
        Vector2Int size = this.TileSize;

        if (this.tileSize == new Vector2Int(1, 1)) {
            return new Vector2Int[] { position };
        }

        Vector2Int[] boundaries = new Vector2Int[size.x * size.y];
        int index = 0;

        int columns = size.x;
        int rows = size.y;

        for (int column = 0; column < columns; column++) {
            for (int row = 0; row < rows; row += rows - 1) {
                boundaries[index++] = new Vector2Int(position.x + column, position.y + row);
            }
        }

        for (int row = 1; row < size.y - 1; row++) {
            for (int column = 0; column < columns; column += columns - 1) {
                boundaries[index++] = new Vector2Int(position.x + column, position.y + row);
            }
        }

        return boundaries;
    }

    public virtual MapTile GetClosestTile() {
        return this.Tile;
    }

    public List<MapTile> GetNeighbours(bool includeDiagonals) {
        List<MapTile> neighbours = new List<MapTile>();
        List<MapTile> claimedTiles = this.ClaimedTiles;

        foreach (MapTile claimedTile in claimedTiles) {
            foreach (MapTile neighbour in claimedTile.GetNeighbours(false)) {
                if (!claimedTiles.Contains(neighbour)) {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    public virtual MapTile GetRealTile() {
        return this.Tile;
    }

    public SpriteSelection GetSelection() {
        return this.selection;
    }

    public T GetTrait<T>() where T : ISpawnableTrait {
        if (this.traitByType.ContainsKey(typeof(T))) {
            return (T)this.traitByType[typeof(T)];
        }

        return default(T);
    }

    public override void InitializeExternals() {
        base.InitializeExternals();

        this.Selected = false;
        this.sprite.InitializeExternals();

        this.MeshAnimator = this.sprite.MeshAnimator;
        this.MeshFilter = this.sprite.MeshFilter;
        this.Renderer = this.sprite.Renderer;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.ContextMenu = serviceLocator.ContextMenu;
        this.CursorStylist = serviceLocator.CursorStylist;
        this.MissionStatistics = serviceLocator.MissionStatistics;
        this.SpawnFactory = serviceLocator.SpawnFactory;
        this.StatusPane = serviceLocator.StatusPane;

        this.deathListeners = new List<IDeathListener>();
        this.phasedOutListeners = new List<IPhasedOutListener>();
    }

    protected virtual void InitializeSelection() {
        this.selection.InitializeSelection(this.transform, this.TileSize);
    }

    protected void InitializeTraits() {
        if (this.traitByType == null) {
            this.traitByType = new Dictionary<Type, ISpawnableTrait>();
        } else {
            this.traitByType.Clear();
        }
    }

    public bool IsAdjacent(IMovementDestination destination) {
        return this.Overlaps(destination, 1);
    }

    /// <summary>
    /// Here for ITarget interface
    /// </summary>
    public bool IsDead() {
        return this.Dead;
    }

    public override void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        base.OnAnimationTrigger(animationType, triggerType);

        if (triggerType == AnimationTriggerType.OnDecomposed) {
            this.OnDecomposed();
        }
    }

    public virtual void OnButtonPress(GameButtonType action) {
        this.Invoke("Press" + action, 0.0f);
    }

    private void OnDecomposed() {
        this.Sprite.Renderer.enabled = false;
        MonoBehaviour.Destroy(this.gameObject);
    }

    public override void OnManualMouseEnter() {
        base.OnManualMouseEnter();

        //this.CursorStylist.SetCursor (CursorType.MagnifyingGlass);
    }

    public override void OnManualMouseExit() {
        base.OnManualMouseExit();

        //this.CursorStylist.SetCursor (CursorType.Default);
    }

    private void OnPhasedOut() {
        foreach (IPhasedOutListener listener in this.phasedOutListeners) {
            listener.OnPhasedOut(this);
        }
    }

    public bool Overlaps(IMovementDestination destination) {
        return this.Overlaps(destination, 0);
    }

    /// <param name="padding">
    ///     Width, in tiles, around the destination, that are still considered as tolerance for overlapping.
    /// </param>
    private bool Overlaps(IMovementDestination destination, int padding) {
        Vector2 radiusA = new Vector2(this.TileSize.x * 0.5f, this.TileSize.y * 0.5f);
        Vector2 radiusB = new Vector2(destination.TileSize.x * 0.5f, destination.TileSize.y * 0.5f);

        radiusA.x += padding;
        radiusA.y += padding;

        Vector2Int tileMapPosition = this.Tile.MapPosition;
        Vector2Int destinationMapPosition = destination.Pivot.MapPosition;
        Vector2 centerA = new Vector2(tileMapPosition.x + radiusA.x, tileMapPosition.y + radiusA.y);
        Vector2 centerB = new Vector2(destinationMapPosition.x + radiusB.x, destinationMapPosition.y + radiusB.y);

        centerA.x -= padding;
        centerA.y -= padding;

        bool intersects = false;

        if (Mathf.Abs(centerA.x - centerB.x) < (radiusA.x + radiusB.x)) {
            if (Mathf.Abs(centerA.y - centerB.y) < (radiusA.y + radiusB.y)) {
                intersects = true;
            }
        }

        return intersects;
    }

    public override void Play(AnimationType type, bool inverted = false) {
        base.Play(type, inverted);

        this.AdjustCollider();
    }

    protected virtual void PlaySelectionSound() {
#if SKIP        
        if (this.Data.SelectionSound == AudioIdentifier.None) {
            return;
        }

        if (this.Data.ConsecutiveSelectionSound == AudioIdentifier.None) {
            this.AudioManager.PlayUnique (this.Data.SelectionSound);
        } else if (! this.AudioManager.IsPlaying (this.Data.SelectionSound, this.Data.ConsecutiveSelectionSound)) {
            if (Time.time - this.lastSelection > 1.5f) {
                this.consecutiveSelections = 0;
            }

            if (this.consecutiveSelections > 5) {
                this.AudioManager.Play (this.Data.SelectionSound);
                this.consecutiveSelections = 1;
            } else {
                if (this.consecutiveSelections < this.selectionsBeforeAnnoyed) {
                    this.AudioManager.Play (this.Data.SelectionSound);
                } else {
                    this.AudioManager.Play (
                        this.Data.ConsecutiveSelectionSound,
                        forceIndex: this.consecutiveSelections - this.selectionsBeforeAnnoyed
                    );
                }

                this.consecutiveSelections ++;
            }

            this.lastSelection = Time.time;
        }
#endif
    }

    public virtual void RefreshPosition() {

    }

    public void ReleaseClaimedTiles() {
        foreach (MapTile tile in this.ClaimedTiles) {
            if (tile.Slot != null) {
                //tile.Slot.GetLayer (1).SpriteRenderer.color = Color.white;
            }

            tile.RemoveInhabitant(this);
            tile.Caption = string.Empty;
        }

        this.ClaimedTiles.Clear();
    }

    public void RemoveDeathListener(IDeathListener listener) {
        if (this.deathListeners.Contains(listener)) {
            this.deathListeners.Remove(listener);
        }
    }

    public void RemovePhasedOutListener(IPhasedOutListener listener) {
        if (this.phasedOutListeners.Contains(listener)) {
            this.phasedOutListeners.Remove(listener);
        }
    }

    public void Restore(int hitPoints) {
        if (!this.Dead) {
            this.SetHitPoints(this.CurrentHitPoints + hitPoints);
        }
    }

    private IEnumerator RestoreMana() {
        do {
            if (this.CurrentManaPoints < this.Data.ManaPoints) {
                this.CurrentManaPoints++;
                this.StatusPane.ManualUpdate();
            }

            yield return new WaitForSeconds(1.0f);
        } while (true);
    }

    protected void SetData(SpawnableSpriteData data) {
        this.Sprite.InitializeExternals();

        //this.CurrentHitPoints = data.HitPoints;
        this.SetHitPoints(data.HitPoints);
        this.CurrentManaPoints = data.ManaPoints;

        if (data.ManaPoints > 0) {
            this.StatusBackgroundIndex = 7;
        }

        this.TileSize = new Vector2Int(data.TileSize.x, data.TileSize.y);

        this.Offset = new Vector2(
            (this.TileSize.x - 1) * this.Grid.DefaultSlotSize.x * 0.5f,
            -(this.TileSize.y - 1) * this.Grid.DefaultSlotSize.y * 0.5f
        );

        BoxCollider2D collider = this.GetComponent<Collider2D>() as BoxCollider2D;

        if (collider != null) {
            Vector2Int size = this.TileSize.Multiply(32.0f);
            collider.size = new Vector2(size.x, size.y);
        }

        if (this.Selectable) {
            this.selection = Transform.Instantiate(this.selectionPrefab);
            this.selection.name = this.selectionPrefab.name;
            this.InitializeSelection();
        }

        data.Title = data.Title.ToUpper();

        if (this.MeshAnimator != null) {
            this.MeshAnimator.SetAnimations(data.Animations);
        }

        if (data.ManaPoints > 0 && data.RegainsMana) {
            this.StartCoroutine(this.RestoreMana());
        }

        if (this.Data.RootMenuNode != null) {
            this.Data.RootMenuNode.Initialize();
        }
    }

    protected virtual void SetHitPoints(int hitPoints) {
        this.CurrentHitPoints = Mathf.Clamp(hitPoints, 0, this.Data.HitPoints);

        this.StatusPane.ManualUpdate();
    }

    protected virtual void SetManaPoints(int manaPoints) {
        this.CurrentManaPoints = Mathf.Clamp(manaPoints, 0, this.Data.ManaPoints);

        this.StatusPane.ManualUpdate();
    }

    public virtual void SetResourcesHeld(int amount) {

    }

    public virtual void SetSelected(bool selected) {
        if (this.Selectable) {
            this.Selected = selected;

            this.selection.SetSelected(selected);
            this.selection.SetVisible(selected);

            this.StatusPane.ManualUpdate();
            this.ContextMenu.SetNode(this.Data.RootMenuNode);
            //this.ContextMenu.ManualUpdate ();

            if (selected) {
                this.PlaySelectionSound();
            }
        }
    }

    public void SetTrait<T>(T trait) where T : ISpawnableTrait {
        T oldTrait = this.GetTrait<T>();

        if (oldTrait != null) {
            oldTrait.Deactivate();
        }

        this.traitByType[typeof(T)] = trait;
    }

    public void SpendMana(int mana) {
        this.SetManaPoints(this.CurrentManaPoints - mana);
    }

    protected override void Start() {
        //this.SetHitPoints ((int) this.Data.HitPoints);
    }
}

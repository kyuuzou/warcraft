using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator> {

    [Header ("Services")]
    [SerializeField]
    private AudioManager audioManager;
    public AudioManager AudioManager {
        get { return this.audioManager; }
    }

    [SerializeField]
    private ContextMenu contextMenu;
    public ContextMenu ContextMenu {
        get { return this.contextMenu; }
    }

    [SerializeField]
    private CursorStylist cursorStylist;
    public CursorStylist CursorStylist {
        get { return this.cursorStylist; }
    }

    [SerializeField]
    private GameController gameController;
    public GameController GameController {
        get { return this.gameController; }
    }

    [SerializeField]
    private Grid grid;
    public Grid Grid {
        get { return this.grid; }
    }

    [SerializeField]
    private GroupSelection groupSelection;
    public GroupSelection GroupSelection {
        get { return this.groupSelection; }
    }

    [SerializeField]
	private Camera guiCamera;
	public Camera GUICamera {
        get { return this.guiCamera; }
    }

    [SerializeField]
    private InputHandler inputHandler;
    public InputHandler InputHandler {
        get { return this.inputHandler; }
    }

    [SerializeField]
    private Map map;
    public Map Map {
        get { return this.map; }
    }

    [SerializeField]
    private MainCamera mainCamera;
    public MainCamera MainCamera {
        get { return this.mainCamera; }
    }

    [SerializeField]
    private Minimap minimap;
    public Minimap Minimap {
        get { return this.minimap; }
    }

    [SerializeField]
    private Mission mission;
    public Mission Mission {
        get { return this.mission; }
    }

    [SerializeField]
    private MissionStatistics missionStatistics;
    public MissionStatistics MissionStatistics {
        get { return this.missionStatistics; }
    }

    [SerializeField]
    private ResourceManager resourceManager;
    public ResourceManager ResourceManager {
        get { return this.resourceManager; }
    }

    [SerializeField]
    private TextMesh scoreText;
    public TextMesh ScoreText {
        get { return this.scoreText; }
    }

    [SerializeField]
    private SpawnFactory spawnFactory;
    public SpawnFactory SpawnFactory {
        get { return this.spawnFactory; }
    }

    [SerializeField]
    private StatusBar statusBar;
    public StatusBar StatusBar {
        get { return this.statusBar; }
    }

    [SerializeField]
    private StatusPane statusPane;
    public StatusPane StatusPane {
        get { return this.statusPane; }
    }

    [Header ("Pools")]
    [SerializeField]
    private PoolBase[] pools;

    private Dictionary<Type, PoolBase> poolByType;

    public T GetPool<T> () {
        Type poolType = typeof (T);
            
        if (this.poolByType.ContainsKey (poolType)) {
            return (T) (object) this.poolByType[poolType];
        }

        return default (T);
    }

    protected override void InitializeInternals () {
        if (this.InitializedInternals) {
            return;
        }

        base.InitializeInternals ();

        this.InitializePools ();
    }

    private void InitializePools () {
        this.poolByType = new Dictionary<Type, PoolBase> ();

        foreach (PoolBase pool in this.pools) {
            this.poolByType[pool.PoolType] = pool;
        }
    }
}

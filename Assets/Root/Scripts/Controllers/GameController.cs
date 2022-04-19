using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SceneObject {

    [Header("UI Elements")]
    [SerializeField]
    private TextMesh goldText;

    [SerializeField]
    private TextMesh lumberText;

    public SpawnableSpriteGroup CurrentGroup { get; private set; }
    public bool Paused { get; private set; }

    public delegate void BuildingCompleteHandler(object sender, BuildingCompleteArgs e);
    private event BuildingCompleteHandler BuildingComplete;

    [Header("Campaigns")]
    [SerializeField]
    private Campaign[] campaigns;

    [SerializeField]
    private CampaignType currentCampaignType;

    [Range(1, 12)]
    [SerializeField]
    private int currentLevel;
    public Level CurrentLevel {
        get { return this.campaignByType[this.currentCampaignType].GetLevel(this.currentLevel - 1); }
    }

    [Header("Upgrades")]
    [SerializeField]
    private Upgrade[] upgrades;

    private AudioManager audioManager;
    private ContextMenu contextMenu;
    private Grid grid;
    private Map map;
    private Minimap minimap;

    private Dictionary<CampaignType, Campaign> campaignByType;
    private Dictionary<FactionIdentifier, Faction> factionByIdentifier;
    private List<Faction> factions;
    private Player mainPlayer;
    private Mission mission;
    private List<Player> players;
    private Dictionary<int, SpawnableSpriteGroup> savedGroups;
    private Dictionary<UpgradeIdentifier, Upgrade> upgradeByIdentifier;
    private Rect windowRect;

    private float visibleGold;
    private float visibleLumber;
    private int gold = 10000;
    private int lumber = 4000;

    public void AddPlayer(Player player) {
        this.players.Add(player);

        if (player.Data.HumanPlayer) {
            this.mainPlayer = player;
        }
    }

    protected override void Awake() {
        base.Awake();

        //Application.runInBackground = true;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.contextMenu = serviceLocator.ContextMenu;
        this.minimap = serviceLocator.Minimap;

        this.players = new List<Player>();

        this.visibleGold = this.gold;
        this.visibleLumber = this.lumber;
        this.goldText.text = this.gold.ToString();
        this.lumberText.text = this.lumber.ToString();

        this.savedGroups = new Dictionary<int, SpawnableSpriteGroup>();
        this.CurrentGroup = new SpawnableSpriteGroup();

        this.campaignByType = new Dictionary<CampaignType, Campaign>();

        foreach (Campaign campaign in this.campaigns) {
            this.campaignByType[campaign.Type] = campaign;
        }
    }

    public void ClearSelection() {
        this.CurrentGroup.Clear();

        this.contextMenu.SetVisible(false);
    }

    public void DecreaseGold(int gold) {
        this.gold -= gold;
    }

    public void DecreaseLumber(int lumber) {
        this.lumber -= lumber;
    }

    public List<Player> GetEnemyPlayers(Player player) {
        List<Player> enemies = new List<Player>(this.players);

        enemies.Remove(player);

        return enemies;
    }

    public Faction GetFaction(FactionIdentifier identifier) {
        this.InitializeExternals();

        if (!this.factionByIdentifier.ContainsKey(identifier)) {
            Debug.LogError("Faction not found: " + identifier);

            return null;
        }

        return this.factionByIdentifier[identifier];
    }

    public Faction GetFaction(int levelParserIndex) {
        foreach (Faction faction in this.factions) {
            if (faction.Data.LevelParserIndex == levelParserIndex) {
                return faction;
            }
        }

        Debug.LogError("Faction index not found: " + levelParserIndex);

        return null;
    }

    public int GetGold() {
        return this.gold;
    }

    public int GetLumber() {
        return this.lumber;
    }

    public SpawnableSpriteGroup GetSavedGroup(int key) {
        if (this.savedGroups.ContainsKey(key)) {
            return new SpawnableSpriteGroup(this.savedGroups[key]);
        }

        return null;
    }

    public Upgrade GetUpgrade(UpgradeIdentifier identifier) {
        if (!this.upgradeByIdentifier.ContainsKey(identifier)) {
            Debug.LogError("No upgrade found with identifier: " + identifier);
            return null;
        }

        Upgrade upgrade = Upgrade.Instantiate(this.upgradeByIdentifier[identifier]);
        upgrade.Initialize();

        return upgrade;
    }

    public void IncreaseGold(int gold) {
        this.gold += gold;
    }

    public void IncreaseLumber(int lumber) {
        this.lumber += lumber;
    }

    public Player GetMainPlayer() {
        return this.mainPlayer;
    }

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.audioManager = serviceLocator.AudioManager;
        this.grid = serviceLocator.Grid;
        this.map = serviceLocator.Map;
        this.mission = serviceLocator.Mission;

        this.InitializeUpgrades();
    }

    private void InitializeFactions() {
        this.factions = new List<Faction>();
        this.factionByIdentifier = new Dictionary<FactionIdentifier, Faction>();

        Level level = this.CurrentLevel;

        foreach (FactionData data in level.Factions) {
            Faction faction = new Faction(data);

            this.factions.Add(faction);
            this.factionByIdentifier[data.Identifier] = faction;
        }

        foreach (PlayerData data in level.Players) {
            Player player;

            if (data.HumanPlayer) {
                player = this.gameObject.AddComponent<Player>();
            } else {
                player = this.gameObject.AddComponent<AIPlayer>();
            }

            player.Initialize(data);
            this.players.Add(player);
        }
    }

    private void InitializeUpgrades() {
        this.upgradeByIdentifier = new Dictionary<UpgradeIdentifier, Upgrade>();

        foreach (Upgrade upgrade in this.upgrades) {
            this.upgradeByIdentifier[upgrade.Identifier] = upgrade;
        }
    }

    public void OnBuildingComplete(Building building) {
        BuildingCompleteArgs e = new BuildingCompleteArgs(building);

        this.OnBuildingComplete(e);
    }

    private void OnBuildingComplete(BuildingCompleteArgs e) {
        if (this.BuildingComplete != null) {
            this.BuildingComplete(this, e);
        }
    }

    public void OnGameOver() {
        this.StartCoroutine(this.OnGameOverCoroutine());
    }

    private IEnumerator OnGameOverCoroutine() {
        yield return new WaitForSeconds(3.0f);

        Utils.RestartLevel();
    }

    public override void Pause() {
        this.Paused = true;
        Time.timeScale = 0.0f;
    }

    private void PlayLevel(int index) {
        if (this.currentCampaignType == CampaignType.None) {
            Debug.LogError("No campaign selected.");
            return;
        }

        this.InitializeFactions();

        Level level = this.CurrentLevel;
        level.Initialize();

        this.minimap.Initialize(level.StartPosition);

        ILevelParser parser = new WarcraftLevelParser(level);
        parser.Parse();

        this.mission.Activate();

        //this.IncreaseGold (level.StartCurrency);

        this.audioManager.Play(AudioIdentifier.CampaignMusic);

        this.PlayLevelIntro(level);

        this.minimap.Activate();
    }

    private void PlayLevelIntro(Level level) {
        //this.grid.Deactivate();
        //this.map.Deactivate();
    }

    public void RegisterBuildingComplete(BuildingCompleteHandler handler) {
        this.BuildingComplete += new BuildingCompleteHandler(handler);
    }

    public override void Resume() {
        this.Paused = false;
        Time.timeScale = 1.0f;
    }

    public void SaveGroup(int key) {
        this.savedGroups[key] = new SpawnableSpriteGroup(this.CurrentGroup);
    }

    public void SetGold(int gold) {
        this.gold = gold;
    }

    public void SetLumber(int lumber) {
        this.lumber = lumber;
    }

    protected override void Start() {
        base.Start();

        this.PlayLevel(this.currentLevel - 1);

        this.map.ManualUpdate();
        //ServiceLocator.Instance.MainCamera.CenterOnUnit(this.CurrentGroup.GetSquadLeader());
    }

    private void Update() {
        this.map.ManualUpdate();

        Utils.ScrollResourceNumber(ref this.visibleGold, this.gold, this.goldText);
        Utils.ScrollResourceNumber(ref this.visibleLumber, this.lumber, this.lumberText);
    }
}

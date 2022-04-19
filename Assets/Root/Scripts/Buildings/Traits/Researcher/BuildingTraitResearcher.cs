using System.Collections;

public class BuildingTraitResearcher : BuildingTrait, IBuildingTraitResearcher {

    public BuildingTraitDataResearcher Data { get; set; }

    private ContextMenu contextMenu;
    private GameController gameController;
    private StatusPane statusPane;

    private IEnumerator researchEnumerator = null;
    private bool researching = false;

    private int rank;
    private Upgrade upgrade;

    public override void Deactivate() {
        base.Deactivate();

        if (this.researching) {
            this.Building.StopCoroutine(this.researchEnumerator);

            UpgradeRank upgradeRank = this.upgrade.GetRank(this.rank);

            this.gameController.IncreaseGold(upgradeRank.GoldCost);
            this.gameController.IncreaseLumber(upgradeRank.LumberCost);

            this.Building.StatusBackgroundIndex = 8;
            this.statusPane.SetBackgroundIndex(this.Building.StatusBackgroundIndex);
        }
    }

    public void Initialize(Building building, BuildingTraitDataResearcher data) {
        base.Initialize(building);

        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.contextMenu = serviceLocator.ContextMenu;
        this.gameController = serviceLocator.GameController;
        this.statusPane = serviceLocator.StatusPane;
    }

    public void Research(Upgrade upgrade, int rank) {
        if (this.researchEnumerator != null) {
            this.Building.StopCoroutine(this.researchEnumerator);
        }

        this.researchEnumerator = this.ResearchCoroutine(upgrade, rank);
        this.Building.StartCoroutine(this.researchEnumerator);
    }

    public void Research(UpgradeIdentifier identifier, int rank = 0) {
        this.Research(this.gameController.GetUpgrade(identifier), rank);
    }

    private IEnumerator ResearchCoroutine(Upgrade upgrade, int rank = 0) {
        this.researching = true;

        this.upgrade = upgrade;
        this.rank = rank;

        UpgradeRank upgradeRank = upgrade.GetRank(rank);

        this.gameController.DecreaseGold(upgradeRank.GoldCost);
        this.gameController.DecreaseLumber(upgradeRank.LumberCost);

        if (this.Building.Selected) {
            this.contextMenu.SetNode(this.contextMenu.CancelNode);
        }

        this.researchEnumerator = this.Building.Progress(upgradeRank.ResearchTime);
        yield return this.Building.StartCoroutine(this.researchEnumerator);

        this.Building.Faction.AddUpgrade(upgrade, rank);
        this.Building.StatusBackgroundIndex = 8;

        if (this.Building.Selected) {
            this.statusPane.SetBackgroundIndex(this.Building.StatusBackgroundIndex);
            this.contextMenu.SetNode(this.Building.Data.RootMenuNode);
        } else {
            this.contextMenu.RefreshButtons();
        }

        this.researching = false;
    }
}

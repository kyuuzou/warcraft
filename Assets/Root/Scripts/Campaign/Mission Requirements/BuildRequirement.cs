using UnityEngine;

public class BuildRequirement : MissionRequirement {

    [SerializeField]
    private SpawnableSpriteType type;

    [SerializeField]
    private int quantity;

    private GameController gameController;
    private Map map;

    public override void Initialize(Mission mission) {
        base.Initialize(mission);

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.map = serviceLocator.Map;

        this.gameController.RegisterBuildingComplete(this.Validate);
    }

    private void Validate(object sender, BuildingCompleteArgs args) {
        if ((int)this.type != (int)args.Building.Type) {
            return;
        }

        int count = this.map.GetBuildings(args.Building.Type).Count;

        if (count >= this.quantity && !this.Satisfied) {
            this.SetSatisfied(true);
        } else if (count < this.quantity) {
            this.SetSatisfied(false);
        }
    }

}

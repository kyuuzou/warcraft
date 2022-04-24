public class InteractionModeBuildingArgs : InteractionModeArgs {

    private readonly Building building;
    private readonly Unit builder;

    public InteractionModeBuildingArgs(Building building, Unit builder) {
        this.building = building;
        this.builder = builder;
    }

    public Building GetBuilding() {
        return this.building;
    }

    public Unit GetBuilder() {
        return this.builder;
    }
}

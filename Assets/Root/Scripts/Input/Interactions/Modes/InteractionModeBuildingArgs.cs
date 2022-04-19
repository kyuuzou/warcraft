public class InteractionModeBuildingArgs : InteractionModeArgs {

    private Building building;
    private Unit builder;

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

public class InteractionModeHarvestArgs : InteractionModeArgs {

    private readonly Unit harvester;

    public InteractionModeHarvestArgs(Unit harvester) {
        this.harvester = harvester;
    }

    public Unit GetHarvester() {
        return this.harvester;
    }
}

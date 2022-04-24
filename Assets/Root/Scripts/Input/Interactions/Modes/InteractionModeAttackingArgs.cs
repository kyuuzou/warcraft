public class InteractionModeAttackingArgs : InteractionModeArgs {

    private readonly Unit unit;

    public InteractionModeAttackingArgs(Unit unit) {
        this.unit = unit;
    }

    public Unit GetUnit() {
        return this.unit;
    }
}

using System;

public class UnitRemovedArgs : EventArgs {

    public Unit Unit { get; private set; }

    public UnitRemovedArgs(Unit unit) {
        this.Unit = unit;
    }
}

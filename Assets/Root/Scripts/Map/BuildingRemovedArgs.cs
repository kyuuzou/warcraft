using System;

public class BuildingRemovedArgs : EventArgs {

    public Building Building { get; private set; }

    public BuildingRemovedArgs(Building building) {
        this.Building = building;
    }
}

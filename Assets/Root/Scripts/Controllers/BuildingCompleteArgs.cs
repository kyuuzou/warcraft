using System;

public class BuildingCompleteArgs : EventArgs {

    public Building Building { get; private set; }

    public BuildingCompleteArgs (Building building) {
        this.Building = building;
    }
}

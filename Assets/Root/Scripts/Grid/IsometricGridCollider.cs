using System.Collections;
using UnityEngine;

public class IsometricGridCollider : SceneObject {

    private Grid grid;

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.grid = ServiceLocator.Instance.Grid;
    }

    public override bool OnManualMouseDown () {
        return grid.OnManualMouseDown ();
    }
}

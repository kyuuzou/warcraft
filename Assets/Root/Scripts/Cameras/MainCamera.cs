using UnityEngine;

public class MainCamera : SceneObject {

    [SerializeField]
    private float speed;

    private Unit lastTarget;
    private Map map;

    public void CenterOnUnit(Unit target) {
        // TODO: when moving instantaneously, take the last offset into consideration
        if (!target.Selected) {
            return;
        }

        Vector3 unitPosition = target.Transform.position;
        Vector3 cameraPosition = this.Transform.position;

        Vector3 destination = new Vector3(unitPosition.x, unitPosition.y, cameraPosition.z);

        if (this.lastTarget == null) {
            this.Transform.position = destination;
        } else {
            this.Transform.position = Vector3.MoveTowards(
                this.Transform.position,
                destination,
                this.speed * Time.deltaTime
            );
        }

        this.map.RefreshPositions();
        this.lastTarget = target;
    }

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

        this.map = ServiceLocator.Instance.Map;
    }
}

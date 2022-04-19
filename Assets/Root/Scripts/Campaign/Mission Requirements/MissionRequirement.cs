public abstract class MissionRequirement : CustomScriptableObject {

    private Mission mission;

    public bool Satisfied { get; private set; }

    public virtual void Initialize(Mission mission) {
        this.Initialize();

        this.mission = mission;
    }

    protected void SetSatisfied(bool satisfied) {
        this.Satisfied = satisfied;

        if (satisfied) {
            this.mission.OnRequirementSatisfied();
        }
    }
}

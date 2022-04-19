public class UnitTraitRangedAttacker : UnitTraitAttacker, IShootingListener {

    public new UnitTraitDataRangedAttacker Data { get; private set; }

    public override void Attack(MapTile target) {
        this.Activate();

        this.SetTarget(target);
        this.OnOrderAccepted();
    }

    public void Initialize(Unit unit, UnitTraitDataRangedAttacker data) {
        base.Initialize(unit, data);

        this.Data = data;
    }

    public override void OnAttack() {
        if (!this.Active) {
            return;
        }

        if (this.IsTargetInRange()) {
            this.Unit.Shoot(this, this.Data.ProjectilePrefab, this.Target);
        } else {
            this.Deactivate();
        }
    }

    public void OnProjectileConnected() {
        this.DamageTarget(this.CalculateAttackDamage());
    }
}

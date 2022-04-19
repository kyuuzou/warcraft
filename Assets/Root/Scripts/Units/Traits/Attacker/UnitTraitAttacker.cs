using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class UnitTraitAttacker : UnitTrait, IDeathListener, IMovementListener, IPhasedOutListener, IUnitTraitAttacker {

    public UnitTraitDataAttacker Data { get; private set; }

    protected AudioManager AudioManager { get; private set; }
    protected ITarget Target { get; private set; }

    private IEnumerator attackAfterCooldownCoroutine = null;
    private bool engaging = false;
    private float lastAttack = float.MinValue;
    private Map map;

    public override UnitTraitType Type {
        get { return UnitTraitType.Attacker; }
    }

    public void ApproachingTarget() {

    }

    public void Attack(SpawnableSprite target) {
        this.Activate();

        this.SetTarget(target);
        this.Unit.OnOrderAccepted();

        if (this.IsTargetInRange()) {
            this.Engage();
        } else {
            this.Disengage();
            this.Unit.Move(this.Target, this, false, false);
        }
    }

    public virtual void Attack(MapTile target) {

    }

    public void AttackAfterCooldown() {
        if (this.attackAfterCooldownCoroutine != null) {
            this.StopCoroutine(this.attackAfterCooldownCoroutine);
        }

        this.attackAfterCooldownCoroutine = this.AttackAfterCooldown(this.Data.AttackCooldown);
        this.StartCoroutine(this.attackAfterCooldownCoroutine);
    }

    protected virtual IEnumerator AttackAfterCooldown(float delay) {
        if (!this.Active || this.Unit.IsDead()) {
            yield break;
        }

        // So triggers on the last frame of an attack animation get a chance to be processed
        yield return null;

        if (!this.IsTargetInRange()) {
            this.Disengage();
            this.Unit.Move(this.Target, this, false, false);
            yield break;
        }

        yield return new WaitForSeconds(delay);

        if (!this.Active || this.Unit.IsDead()) {
            yield break;
        }

        if (this.IsTargetInRange()) {
            this.lastAttack = Time.time;
            this.Unit.Play(AnimationType.Attacking);
        } else {
            this.Disengage();
            this.Unit.Move(this.Target, this, false, false);
        }
    }

    protected int CalculateAttackDamage() {
        UpgradeRank upgrade = this.Unit.Faction.GetUpgrade(UpgradeType.Attack, this.Unit.Type);

        int strength = upgrade == null ? 0 : upgrade.Strength;

#if STRONG_ATTACKS
        strength = 500;
#endif

        return this.Data.MinimumDamage + Random.Range(0, this.Data.RandomDamage) + strength;
    }

    public virtual void DamageTarget(int damage) {
        if (!this.Active) {
            return;
        }

        this.Unit.FindDirection(this.Unit.Tile, this.Target.GetRealTile());
        this.Target.Damage(damage);
    }

    public override void Deactivate() {
        base.Deactivate();

        if (!this.Unit.Dead) {
            this.Unit.Stop();
        }

        this.Unit.MeshAnimator.UnregisterTriggerListener(this);

        if (this.attackAfterCooldownCoroutine != null) {
            this.StopCoroutine(this.attackAfterCooldownCoroutine);
        }

        this.Disengage();
    }

    private void Disengage() {
        this.engaging = false;

        if (this.Unit.MeshAnimator.CurrentAnimation.Type == AnimationType.Attacking) {
            this.Unit.Play(AnimationType.Idle);
        }
    }

    private void Engage() {
        if (this.engaging) {
            return;
        }

        if (this.Target.IsDead() || this.Target.PhasedOut) {
            this.Deactivate();
            return;
        }

        this.engaging = true;
        this.lastAttack = Time.time;

        this.Unit.FindDirection(this.Unit.Tile, this.Target.Tile);
        this.Unit.Play(AnimationType.Attacking);
    }

    public void Initialize(Unit unit, UnitTraitDataAttacker data) {
        base.Initialize(unit);

        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.AudioManager = serviceLocator.AudioManager;
        this.map = serviceLocator.Map;
    }

    protected bool IsTargetInRange() {
        MapTile attackerTile = this.Unit.Tile;
        Vector2Int attackerMapPosition = attackerTile.MapPosition;
        Vector2Int targetMapPosition = this.map.FindClosestBoundary(attackerTile, this.Target);

        // Add 1 to cover the minimum distance between tiles, as Warcraft considers range 0 to be the melee distance 
        int range = this.Data.AttackRange + 1;
        float distance = attackerMapPosition.EstimateDistance(targetMapPosition);

        return distance <= range;
    }

    public bool IsTileTraversable(MapTile tile) {
        return tile.IsTraversable(this.Unit.GetTrait<IUnitTraitMoving>().MovementType, this.Unit);
    }

    public void ManualUpdate() {

    }

    public void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        if (animationType == AnimationType.Attacking && triggerType == AnimationTriggerType.OnFinished) {
            this.AttackAfterCooldown();
        }
    }

    public virtual void OnAttack() {
        if (!this.Active) {
            return;
        }

        if (this.IsTargetInRange()) {
            this.DamageTarget(this.CalculateAttackDamage());
            this.AudioManager.Play(this.Data.AttackSound);
        }
    }

    public void OnDeathNotification(SpawnableSprite sprite) {
        if (sprite == this.Target) {
            this.Deactivate();
        }
    }

    public void OnOrderAccepted() {

    }

    public void OnPhasedOut(SpawnableSprite sprite) {

    }

    public void ReachedTarget() {
        if (this.IsTargetInRange()) {
            this.Engage();
        } else {
            this.Disengage();
            this.Unit.Move(this.Target, this, false, false);
        }
    }

    protected void SetTarget(ITarget target) {
        this.Target = target;
        this.Target.AddDeathListener(this);
        this.Target.AddPhasedOutListener(this);
    }

    public void TileChanged() {

    }
}

public interface IUnitTraitAttacker : IAnimationTriggerListener, IUnitTrait {

    void Attack(SpawnableSprite sprite);

    void Attack(MapTile tile);

    void AttackAfterCooldown();

    void ManualUpdate();

    void OnAttack();
}

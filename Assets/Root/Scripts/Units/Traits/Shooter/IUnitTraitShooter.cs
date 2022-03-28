using System.Collections;
using UnityEngine;

public interface IUnitTraitShooter : IUnitTrait {

    void Shoot (IShootingListener listener, Projectile projectilePrefab, ITarget target);
}

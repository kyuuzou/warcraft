using UnityEngine;

public class CatapultProjectile : Projectile {

    [SerializeField]
    private Explosion explosionPrefab;

    [SerializeField]
    private int damage;

    public override void OnProjectileConnected(MapTile tile) {
        base.OnProjectileConnected(tile);

        Explosion explosion = Explosion.Instantiate<Explosion>(this.explosionPrefab);
        explosion.Tile = tile;

        explosion.Activate(this.damage);
    }
}

public class Explosion : Projectile {

    public void Activate(int damage) {
        this.InitializeExternals();
        this.Activate();

        this.Map.DamageArea(this.Tile, 2, damage);
        this.OnProjectileConnected(this.Tile);
    }
}

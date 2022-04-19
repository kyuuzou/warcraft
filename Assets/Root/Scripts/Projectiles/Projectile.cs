using System.Collections;
using UnityEngine;

public class Projectile : CustomAnimatedSprite {

    [SerializeField]
    private float lifeSpan = 0;

    [SerializeField]
    private bool stayOnTile = false;

    [SerializeField]
    private AudioIdentifier launchSound;

    [SerializeField]
    private AudioIdentifier hitSound;

    public override void Activate() {
        this.InitializeExternals();

        base.Activate();

        this.Transform.SetZ((int)DepthLayer.Projectiles);

        this.Play(AnimationType.Idle);

        if (this.stayOnTile) {
            this.StartCoroutine(this.StayOnTile());
        }

        if (this.lifeSpan > 0) {
            this.StartCoroutine(this.Expire());
        }

        this.AudioManager.Play(this.launchSound);
    }

    private IEnumerator Expire() {
        yield return new WaitForSeconds(this.lifeSpan);

        this.ManualDestroy();
    }

    public virtual void OnProjectileConnected(MapTile tile) {
        this.AudioManager.Play(this.hitSound);
    }

    private IEnumerator StayOnTile() {
        do {
            this.Transform.position = this.Tile.RealPosition;
            this.Transform.SetZ((int)DepthLayer.Projectiles);

            yield return null;
        } while (true);
    }
}

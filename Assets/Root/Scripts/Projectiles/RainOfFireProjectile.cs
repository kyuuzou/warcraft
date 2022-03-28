using System;
using System.Collections;
using UnityEngine;

public class RainOfFireProjectile : Projectile {

    [SerializeField]
    private Explosion explosionPrefab;

    [SerializeField]
    private int damage;

    public override void Activate () {
        this.InitializeExternals ();

        this.StartCoroutine (this.Fall ());
    }

    private IEnumerator Fall () {
        this.Play (AnimationType.Idle);

        float delta = 0.0f;

        do {
            delta += Time.deltaTime * 2.0f;

            this.Transform.position = this.Tile.RealPosition;
            this.Transform.SetLocalY (Mathf.Lerp (160.0f, 0.0f, delta) + this.Transform.localPosition.y);
            this.Transform.SetZ ((int) DepthLayer.Projectiles);

            yield return null;
        } while (delta < 1.0f);

        this.Transform.localScale *= 2.0f;
        this.Play (AnimationType.Dying);

        Explosion explosion = Explosion.Instantiate<Explosion> (this.explosionPrefab);
        explosion.Tile = this.Tile;

        //explosion.transform.SetLocalZ (this.transform.localPosition.z - 1.0f);
        explosion.Activate (this.damage);

        this.ManualDestroy ();
    }

    public override void Initialize (MapTile tile) {
        this.InitializeExternals ();

        this.Tile = tile;
        this.Transform.SetZ ((int) DepthLayer.Projectiles);
    }
}

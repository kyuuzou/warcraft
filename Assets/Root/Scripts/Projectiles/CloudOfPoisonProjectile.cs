using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class CloudOfPoisonProjectile : Projectile {

    [SerializeField]
    private int damage;

    private Vector2 offset;

    public override void Activate () {
        base.Activate ();

        this.StartCoroutine (this.Wander ());
        this.StartCoroutine (this.Poison ());
    }

    public override void Initialize (MapTile tile) {
        this.InitializeExternals ();

        this.Tile = tile;
        this.Transform.SetZ ((int) DepthLayer.Projectiles);
    }

    private IEnumerator Poison () {
        do {
            MapTile center = this.Map.GetNearestTile (this.Tile.RealPosition.Add (offset));

            this.Map.DamageArea (center, 2, this.damage);

            yield return new WaitForSeconds (1.0f);
        } while (true);
    }

    private IEnumerator Wander () {
        this.Play (AnimationType.Idle);
        
        Vector2 target = Vector2.zero;
        this.offset = Vector2.zero;
        
        do {
            if (Vector2.Distance (target, offset) == 0.0f) {
                target.x = Random.Range (50.0f, 100.0f) * (Random.Range (0, 2) == 1 ? 1 : -1);
                target.y = Random.Range (50.0f, 100.0f) * (Random.Range (0, 2) == 1 ? 1 : -1);
            }
            
            offset = Vector2.MoveTowards (offset, target, Time.deltaTime * 10.0f);
            
            this.Transform.position = this.Tile.RealPosition.Add (offset);
            this.Transform.SetZ ((int) DepthLayer.Projectiles);
            
            yield return null;
        } while (true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTraitShooter : UnitTrait, IUnitTraitShooter {

    public UnitTraitDataShooter Data { get; private set; }

    private Dictionary<Direction, Vector2Int> muzzles;
    private ITarget target;
    private IShootingListener shootingListener;

    private Map map;
    private SpawnFactory spawnFactory;
    
    public override UnitTraitType Type {
        get { return UnitTraitType.Shooter; }
    }
    
    public void Initialize (Unit unit, UnitTraitDataShooter data) {
        base.Initialize (unit);
        
        this.Data = data;

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.map = serviceLocator.Map;
        this.spawnFactory = serviceLocator.SpawnFactory;

        this.InitializeMuzzles ();
    }

    private void InitializeMuzzles () {
        this.muzzles = new Dictionary<Direction, Vector2Int> ();
        
        foreach (Muzzle muzzle in this.Data.Muzzles) {
            this.muzzles[muzzle.Direction] = muzzle.Position;
        }
        
        if (this.muzzles.ContainsKey (Direction.Southeast)) {
            Vector2Int position = this.muzzles[Direction.Southeast];
            position.x = (int)this.Unit.Renderer.bounds.size.x - position.x;
            this.muzzles [Direction.Southwest] = position;
        }
        
        if (this.muzzles.ContainsKey (Direction.East)) {
            Vector2Int position = this.muzzles[Direction.East];
            position.x = (int)this.Unit.Renderer.bounds.size.x - position.x;
            this.muzzles [Direction.West] = position;
        }
        
        if (this.muzzles.ContainsKey (Direction.Northeast)) {
            Vector2Int position = this.muzzles[Direction.Northeast];
            position.x = (int)this.Unit.Renderer.bounds.size.x - position.x;
            this.muzzles [Direction.Northwest] = position;
        }
    }
    
    private IEnumerator MoveProjectile (Projectile projectile, MapTile originTile, Vector2 offsetFromOrigin) {
        Transform projectileTransform = projectile.transform;
        float projectileDepth = (int) DepthLayer.Projectiles;
        
        MapTile closestTile = this.map.GetTile (this.map.FindClosestBoundary (this.Unit.Tile, this.target));
        Vector3 destination = closestTile.RealPosition;
        destination.z = projectileDepth;

        float distance = Vector2.Distance (originTile.RealPosition.Add (offsetFromOrigin), destination);
        float t = 0.0f;

        do {
            t += (200 * Time.deltaTime) / distance;

            destination = closestTile.RealPosition;
            destination.z = projectileDepth;

            Vector3 position = Vector3.Lerp (
                originTile.RealPosition.Add (offsetFromOrigin),
                destination,
                t
            );

            position.z = projectileDepth;
            projectileTransform.position = position;
            
            yield return null;
        } while (t < 1.0f);

        if (! this.target.IsDead ()) {
            this.shootingListener.OnProjectileConnected ();
        }

        projectile.OnProjectileConnected (closestTile);
        
        GameObject.Destroy (projectile.gameObject);
    }

    public void OnOrderAccepted () {
        
    }
    
    public void Shoot (IShootingListener listener, Projectile projectilePrefab, ITarget target) {
        this.Activate ();

        this.shootingListener = listener;
        this.target = target;

        Projectile projectile = Transform.Instantiate (projectilePrefab);
        projectile.transform.parent = this.spawnFactory.ProjectileParent;
        projectile.transform.position = this.Unit.Transform.position;
        
        projectile.Activate ();
        
        Direction direction = projectile.FindDirection (this.Unit.Tile, target.Tile);
        projectile.Play (AnimationType.Idle);

        if (this.muzzles.ContainsKey (direction)) {
            Vector2 localPosition = projectile.Transform.localPosition;
            Vector2 radius = this.Unit.Renderer.bounds.size * 0.5f;
            
            localPosition.x += - radius.x + this.muzzles[direction].x;
            localPosition.y += radius.y - this.muzzles [direction].y;
            projectile.Transform.localPosition = localPosition;
        } else {
            Debug.LogWarning (this.Unit + " does not have a muzzle when it's facing " + direction);
        }

        projectile.Transform.SetZ ((int) DepthLayer.Projectiles);

        Vector2 offset = projectile.transform.position - this.Unit.Tile.RealPosition;

        this.Unit.StartCoroutine (this.MoveProjectile (projectile, this.Unit.Tile, offset));
    }
}

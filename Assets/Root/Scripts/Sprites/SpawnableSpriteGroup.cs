using System.Collections.Generic;

public class SpawnableSpriteGroup {

    public List<SpawnableSprite> Sprites { get; private set; }

    public Faction Faction {
        get { return this.Sprites.Count > 0 ? this.Sprites[0].Faction : null; }
    }
    
    private static readonly int maximumElements = 99;

    public SpawnableSpriteGroup () {
        this.Sprites = new List<SpawnableSprite> ();
    }

    public SpawnableSpriteGroup (SpawnableSpriteGroup group) : this () {
        foreach (SpawnableSprite sprite in group.Sprites) {
            this.Add (sprite);
        }
    }

    public bool Add (SpawnableSprite sprite) {
        if (this.Sprites.Count >= SpawnableSpriteGroup.maximumElements) {
            return false;
        }
        
        this.Sprites.AddExclusive (sprite);
        //sprite.Group = this;

        this.OnGroupChanged ();

        return true;
    }

    public bool Add (SpawnableSprite sprite, int index) {
        if (! this.Add (sprite)) {
            return false;
        }

        this.OnGroupChanged ();

        return true;
    }

    public void Clear () {
        this.SetSelected (false);
        this.Sprites.Clear ();
    }

    /// <param name="index">The index of the sprite that is causing this chain death.</param>
    public void Die (int index) {
        for (int i = this.Sprites.Count - 1; i > index; i --) {
            this.Sprites[i].Die ();
        }
    }

    public int GetIndex (SpawnableSprite sprite) {
        return this.Sprites.IndexOf (sprite);
    }

    private void OnGroupChanged () {
        foreach (SpawnableSprite sprite in this.Sprites) {
            //sprite.GetTrait<ISpawnableSpriteTraitMoving> ().OnGroupChanged ();
        }
    }

    public void Remove (SpawnableSprite sprite) {
        this.Sprites.Remove (sprite);
        sprite.SetSelected (false);

        this.OnGroupChanged ();
    }

    public void Set (bool selected, params SpawnableSprite[] sprites) {
        this.Clear ();

        foreach (SpawnableSprite sprite in sprites) {
            this.Add (sprite);
            sprite.SetSelected (selected);
        }
    }

    public void SetSelected (bool selected) {
        foreach (SpawnableSprite sprite in this.Sprites) {
            sprite.SetSelected (selected);
        }
    }

    public void Toggle (SpawnableSprite sprite) {
        if (this.Sprites.Contains (sprite)) {
            this.Remove (sprite);
        } else {
            this.Add (sprite);
        }
    }

    public override string ToString () {
        return string.Join (":", this.Sprites.ConvertAll (sprite => sprite.ToString ()).ToArray ());
    }
}

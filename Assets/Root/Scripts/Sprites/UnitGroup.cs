using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableSpriteGroup {

    public List<SpawnableSprite> Sprites { get; private set; }

    public Faction Faction {
        get { return this.Sprites[0].Faction; }
    }
    
    private static readonly int maximumElements = 99;

    public SpawnableSpriteGroup () {
        this.Sprites = new List<SpawnableSprite> ();
    }

    public SpawnableSpriteGroup (SpawnableSpriteGroup group) : this () {
        foreach (Unit unit in group.Sprites) {
            this.Add (unit);
        }
    }

    public bool Add (Unit unit) {
        if (this.Sprites.Count >= SpawnableSpriteGroup.maximumElements) {
            return false;
        }
        
        this.Sprites.AddExclusive (unit);
        unit.Group = this;

        this.OnGroupChanged ();

        return true;
    }

    public bool Add (Unit unit, int index) {
        Debug.Log (unit);

        if (! this.Add (unit)) {
            return false;
        }

        this.OnGroupChanged ();

        return true;
    }

    public void Clear () {
        this.SetSelected (false);
        this.Sprites.Clear ();
    }

    /// <param name="index">The index of the unit that is causing this chain death.</param>
    public void Die (int index) {
        for (int i = this.Sprites.Count - 1; i > index; i --) {
            this.Sprites[i].Die ();
        }
    }

    public int GetIndex (Unit unit) {
        return this.Sprites.IndexOf (unit);
    }

    private void OnGroupChanged () {
        foreach (Unit unit in this.Sprites) {
            unit.GetTrait<IUnitTraitMoving> ().OnGroupChanged ();
        }
    }

    public void Remove (Unit unit) {
        this.Sprites.Remove (unit);
        unit.SetSelected (false);

        this.OnGroupChanged ();
    }

    public void Set (bool selected, params Unit[] units) {
        this.Clear ();

        foreach (Unit unit in units) {
            this.Add (unit);
            unit.SetSelected (selected);
        }
    }

    public void SetSelected (bool selected) {
        foreach (Unit unit in this.Sprites) {
            unit.SetSelected (selected);
        }
    }

    public void Toggle (Unit unit) {
        if (this.Sprites.Contains (unit)) {
            this.Remove (unit);
        } else {
            this.Add (unit);
        }
    }

    public override string ToString () {
        return string.Join (":", this.Sprites.ConvertAll (unit => unit.ToString ()).ToArray ());
    }
}

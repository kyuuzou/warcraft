using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup {

    public List<Unit> Units { get; private set; }

    public Faction Faction {
        get { return this.Units[0].Faction; }
    }
    
    private static readonly int maximumElements = 99;

    private Dictionary<Unit, int> preferredPositionByUnit;

    public UnitGroup () {
        this.Units = new List<Unit> ();
        this.preferredPositionByUnit = new Dictionary<Unit, int> ();
    }

    public UnitGroup (UnitGroup group) : this () {
        foreach (Unit unit in group.Units) {
            this.Add (unit);
        }
    }

    public bool Add (Unit unit) {
        if (this.Units.Count >= UnitGroup.maximumElements) {
            return false;
        }
        
        this.Units.AddExclusive (unit);
        unit.Group = this;

        this.OnGroupChanged ();

        return true;
    }

    public bool Add (Unit unit, int index) {
        Debug.Log (unit);

        if (! this.Add (unit)) {
            return false;
        }

        this.preferredPositionByUnit[unit] = index;

        this.Units.Sort (
            delegate (Unit x, Unit y) {
                int xPosition = int.MaxValue;
                int yPosition = int.MaxValue;

                if (this.preferredPositionByUnit.ContainsKey (x)) {
                    xPosition = this.preferredPositionByUnit[x];
                }

                if (this.preferredPositionByUnit.ContainsKey (y)) {
                    yPosition = this.preferredPositionByUnit[y];
                }

                return xPosition.CompareTo (yPosition);
            }
        );

        string output = string.Join (":", this.Units.ConvertAll<string> (delegate(Unit input) {
            return string.Concat ("(", input.name, ", ", this.preferredPositionByUnit[input], ")"); 
        }).ToArray ());
        Debug.Log (output);

        this.OnGroupChanged ();

        return true;
    }

    public void Clear () {
        this.SetSelected (false);
        this.Units.Clear ();
    }

    /// <param name="index">The index of the unit that is causing this chain death.</param>
    public void Die (int index) {
        for (int i = this.Units.Count - 1; i > index; i --) {
            this.Units[i].Die ();
        }
    }

    public int GetIndex (Unit unit) {
        return this.Units.IndexOf (unit);
    }

    public Unit GetLastUnit () {
        return this.Units[this.Units.Count - 1];
    }

    public Unit GetNextUnit (Unit unit) {
        if (! this.Units.Contains (unit)) {
            return null;
        }

        int index = this.Units.IndexOf (unit);
        index ++;

        if (index >= this.Units.Count) {
            return null;
        }

        return this.Units[index];
    }

    private void OnGroupChanged () {
        this.UpdateHealthbars ();

        foreach (Unit unit in this.Units) {
            unit.GetTrait<IUnitTraitMoving> ().OnGroupChanged ();
        }
    }

    public void Remove (Unit unit) {
        this.Units.Remove (unit);
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
        foreach (Unit unit in this.Units) {
            unit.SetSelected (selected);
        }
    }

    public void Toggle (Unit unit) {
        if (this.Units.Contains (unit)) {
            this.Remove (unit);
        } else {
            this.Add (unit);
        }
    }

    public override string ToString () {
        return string.Join (":", this.Units.ConvertAll (unit => unit.ToString ()).ToArray ());
    }

    private void UpdateHealthbars () {
        if (this.Units.Count > 0) {
            this.Units[0].HealthBar.SetSize (0.85f);
        }

        for (int i = 1; i < this.Units.Count; i ++) {
            this.Units[i].HealthBar.SetSize (0.6f);
        }
    }
}

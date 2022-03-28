using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnableTrait : MonoBehaviour {

    public bool Active { get; private set; }

    public virtual void Activate () {
        this.Active = true;
    }
    
    public virtual void Deactivate () {
        this.Active = false;
    }

    public virtual void FilterButtons (ref List<GameButtonType> buttons) {

    }
}

using UnityEngine;
using System.Collections;

public class InteractionModeAttackingArgs : InteractionModeArgs {
    
    private Unit unit;
    
    public InteractionModeAttackingArgs (Unit unit) {
        this.unit = unit;
    }
    
    public Unit GetUnit () {
        return this.unit;
    }
}

using UnityEngine;

public abstract class Spell : CustomScriptableObject {

    [SerializeField]
    private int manaCost;
    public int ManaCost {
        get { return this.manaCost; }
    }

    /// <summary>
    /// The range of the spell.
    /// 0 means the spell has unlimited range.
    /// </summary>
    [SerializeField]
    private int range;
    public int Range {
        get { return this.range; }
    }

    /// <summary>
    /// Whether the spell requires a target.
    /// If it doesn't, it will bypass user input and go straight into casting.
    /// </summary>
    [SerializeField]
    private bool requireTarget = true;
    public bool RequireTarget {
        get { return this.requireTarget; }
    }

    public abstract SpellType Type { get; }

    public virtual void Cast(Unit caster, Building target, MapTile tile) {

    }

    public virtual void Cast(Unit caster, Unit target, MapTile tile) {

    }

    public virtual void Cast(Unit caster, MapTile target) {

    }
}

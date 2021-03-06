using UnityEngine;

public class InvisibilitySpell : Spell {

    [SerializeField]
    private AnimatedSpriteSelection selectionPrefab;

    private Unit target;

    public override SpellType Type {
        get { return SpellType.Invisibility; }
    }

    public override void Cast(Unit caster, Building target, MapTile tile) {

    }

    public override void Cast(Unit caster, MapTile target) {

    }

    public override void Cast(Unit caster, Unit target, MapTile tile) {
        this.target = target;

        caster.FindDirection(caster.Tile, target.Tile);
        caster.Play(AnimationType.Attacking);

        caster.SpendMana(this.ManaCost);

        AnimatedSpriteSelection selection = AnimatedSpriteSelection.Instantiate<AnimatedSpriteSelection>(
            this.selectionPrefab
        );

        selection.Initialize(target);
        selection.Activate(this);

        target.Invisible = true;
    }

    public override void Deactivate() {
        this.target.Invisible = false;
    }
}

public interface IUnitTraitSpellcaster : IUnitTrait {

    void Cast(SpellType spellType, Building target, MapTile tile);

    void Cast(SpellType spellType, MapTile tile);

    void Cast(SpellType spellType, Unit target, MapTile tile);

    bool MayCast(SpellType type);

    bool RequiresTarget(SpellType type);
}

using UnityEngine;

[System.Serializable]
public class BuildingData : SpawnableSpriteData {

    [Header("Building data")]

    [SerializeField]
    private BuildingType type = BuildingType.None;
    public BuildingType Type {
        get { return this.type; }
    }

    //Rooted buildings must be rebuilt in place.
    //Example: Town Hall
    [SerializeField]
    private bool rooted = false;
    public bool Rooted {
        get { return this.rooted; }
    }

    //Unique buildings are limited at one of a kind per faction.
    //A player may have more than one unique building if it controls more than one faction.
    [SerializeField]
    private bool unique = false;
    public bool Unique {
        get { return this.unique; }
    }

    [Header("Traits")]

    [SerializeField]
    private BuildingTraitData minableTrait;
    public BuildingTraitData MinableTrait {
        get { return this.minableTrait; }
    }

    [SerializeField]
    private BuildingTraitData researcherTrait;
    public BuildingTraitData ResearcherTrait {
        get { return this.researcherTrait; }
    }

    [SerializeField]
    private BuildingTraitData trainerTrait;
    public BuildingTraitData TrainerTrait {
        get { return this.trainerTrait; }
    }

    [Header("Textures")]

    [SerializeField]
    private Texture forestTexture;

    [SerializeField]
    private Texture forestInConstruction;

    [SerializeField]
    private Texture swampTexture;

    [SerializeField]
    private Texture swampInConstruction;

    public Texture GetTexture(MapType type, bool construction) {
        if (type == MapType.Forest) {
            return construction ? this.forestInConstruction : this.forestTexture;
        } else {
            return construction ? this.swampInConstruction : this.swampTexture;
        }
    }
}

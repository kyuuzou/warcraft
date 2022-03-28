using System.Collections;
using UnityEngine;

public class BuildingTraitDataTrainer : BuildingTraitData {

    [SerializeField]
    private AudioIdentifier trainedSound;
    public AudioIdentifier TrainedSound {
        get { return this.trainedSound; }
    }

    public override BuildingTraitType Type {
        get { return BuildingTraitType.Trainer; }
    }

    public override BuildingTrait AddTrait (Building building) {
        BuildingTraitTrainer trait = building.gameObject.AddComponent<BuildingTraitTrainer> ();
        trait.Initialize (building, (BuildingTraitDataTrainer) BuildingTraitDataTrainer.Instantiate (this));
        
        building.SetTrait<IBuildingTraitTrainer> (trait);
        
        return trait;
    }
}

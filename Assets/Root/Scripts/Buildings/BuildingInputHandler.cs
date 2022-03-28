
public partial class Building {

    private void PressArcher () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanArcher);
    }

    private void PressBreedFasterHorses1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.FasterHorses, 0);
    }

    private void PressBreedFasterHorses2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.FasterHorses, 1);
    }
    
    private void PressBreedFasterWolves1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.FasterWolves, 0);
    }
    
    private void PressBreedFasterWolves2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.FasterWolves, 1);
    }

    private void PressBuildRoad () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }

    private void PressBuildWall () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }
    
    public override void PressCancel () {
        base.PressCancel ();

        this.GetTrait<IBuildingTraitResearcher> ().Deactivate ();
        this.GetTrait<IBuildingTraitTrainer> ().Deactivate ();

        if (this.underConstruction) {
            this.CancelConstruction ();
        }
    }

    private void PressCleric () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanCleric);
    }

    private void PressCloudOfPoisonResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.CloudOfPoison);
    }
    
    private void PressConjurer () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanConjurer);
    }

    private void PressDaemonResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.OrcMajorSummoning);
    }
    
    private void PressDarkVisionResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.DarkVision);
    }

    private void PressFarSeeingResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.FarSeeing);
    }

    private void PressHealingResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.Healing);
    }

    private void PressHumanCatapult () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanCatapult);
    }

    private void PressHumanUpgradeShield1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.HumanShieldStrength, 0);
    }

    private void PressHumanUpgradeShield2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.HumanShieldStrength, 1);
    }
    
    private void PressFootman () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanFootman);
    }

    private void PressInvisibilityResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.Invisibility);
    }
    
    private void PressKnight () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanKnight);
    }

    private void PressOrcUpgradeShield1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.OrcShieldStrength, 0);
    }
    
    private void PressOrcUpgradeShield2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.OrcShieldStrength, 1);
    }
    
    private void PressPeasant () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.HumanPeasant);
    }

    private void PressRainOfFireResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.RainOfFire);
    }

    private void PressScorpionResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.HumanMinorSummoning);
    }

    private void PressSpiderResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.OrcMinorSummoning);
    }
    
    private void PressUnholyArmorResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.UnholyArmor);
    }

    private void PressUpgradeArrowStrength1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.ArrowStrength, 0);
    }

    private void PressUpgradeArrowStrength2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.ArrowStrength, 1);
    }

    private void PressUpgradeAxeStrength1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.AxeStrength, 0);
    }

    private void PressUpgradeAxeStrength2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.AxeStrength, 1);
    }

    private void PressUpgradeSpearStrength1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.SpearStrength, 0);
    }

    private void PressUpgradeSpearStrength2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.SpearStrength, 1);
    }

    private void PressUpgradeSwordStrength1 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.SwordStrength, 0);
    }

    private void PressUpgradeSwordStrength2 () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.SwordStrength, 1);
    }
    
    private void PressWarlock () {
        this.GetTrait<IBuildingTraitTrainer> ().Train (UnitType.OrcWarlock);
    }
    
    private void PressWaterElementalResearch () {
        this.GetTrait<IBuildingTraitResearcher> ().Research (UpgradeIdentifier.HumanMajorSummoning);
    }
}

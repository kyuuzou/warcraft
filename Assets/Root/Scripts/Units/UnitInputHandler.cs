
public partial class Unit {

    private void PressArrow () {
        this.EnterAttackingMode ();
    }

    private void PressAxe () {
        this.EnterAttackingMode ();
    }
    
    private void PressBuildAdvancedStructure () {
        this.GetTrait<IUnitTraitBuilder> ().ShowAdvancedStructures ();
    }
    
    private void PressBuildBasicStructure () {
        this.GetTrait<IUnitTraitBuilder> ().ShowBasicStructures ();
    }

    public override void PressCancel () {
        this.GetTrait<IUnitTraitBuilder> ().Deactivate ();

        base.PressCancel ();
    }

    private void PressCloudOfPoison () {
        this.EnterSpellcastingMode (SpellType.CloudOfPoison);
    }

    private void PressDaemon () {
        this.EnterSpellcastingMode (SpellType.MajorSummon);
    }

    private void PressDarkVision () {
        this.EnterSpellcastingMode (SpellType.DarkVision);
    }

    private void PressElementalBlast () {
        this.EnterAttackingMode ();
    }

    private void PressFarSeeing () {
        this.EnterSpellcastingMode (SpellType.FarSeeing);
    }

    private void PressFireball () {
        this.EnterAttackingMode ();
    }

    private void PressHealing () {
        this.EnterSpellcastingMode (SpellType.Healing);
    }

    private void PressHolyLance () {
        this.EnterAttackingMode ();
    }

    private void PressHumanBarracks () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanBarracks);
    }
    
    private void PressHumanBlacksmith () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanBlacksmith);
    }
    
    private void PressHumanChurch () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanChurch);
    }
    
    private void PressHumanFarm () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanFarm);
    }
    
    private void PressHumanLumberMill () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanLumberMill);
    }
    
    private void PressHumanMove () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }
    
    private void PressHumanShield () {
        this.Stop ();
    }
    
    private void PressHumanStables () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanStables);
    }
    
    private void PressHumanTower () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanTower);
    }
    
    private void PressHumanTownHall () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.HumanTownHall);
    }

    private void PressInvisibility () {
        this.EnterSpellcastingMode (SpellType.Invisibility);
    }

    private void PressOrcBarracks () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcBarracks);
    }

    private void PressOrcBlacksmith () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcBlacksmith);
    }

    private void PressOrcFarm () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcFarm);
    }

    private void PressOrcKennels () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcKennels);
    }
    
    private void PressOrcLumberMill () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcLumberMill);
    }

    private void PressOrcMove () {
        this.ContextMenu.SetNode (this.ContextMenu.CancelNode);
    }

    private void PressOrcShield () {
        this.Stop ();
    }

    private void PressOrcTemple () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcTemple);
    }

    private void PressOrcTownHall () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcTownHall);
    }

    private void PressOrcTower () {
        this.GetTrait<IUnitTraitBuilder> ().Build (BuildingType.OrcTower);
    }

    private void PressRainOfFire () {
        this.EnterSpellcastingMode (SpellType.RainOfFire);
    }

    private void PressRaiseDead () {
        this.EnterSpellcastingMode (SpellType.RaiseDead, false);
    }

    private void PressRepair () {
        this.EnterMendingMode ();
    }

    private void PressScorpion () {
        this.EnterSpellcastingMode (SpellType.MinorSummon);
    }

    private void PressShadowSpear () {
        this.EnterAttackingMode ();
    }

    private void PressSpider () {
        this.EnterSpellcastingMode (SpellType.MinorSummon);
    }

    private void PressSword () {
        this.EnterAttackingMode ();
    }

    private void PressUnholyArmor () {
        this.EnterSpellcastingMode (SpellType.UnholyArmor);
    }

    private void PressWaterElemental () {
        this.EnterSpellcastingMode (SpellType.MajorSummon);
    }
}

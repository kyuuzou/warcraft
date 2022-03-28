using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData : SpawnableSpriteData {

    [Header ("Unit data")]

    [SerializeField]
    private UnitType type = UnitType.None;
    public UnitType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Projectile projectilePrefab;
    public Projectile ProjectilePrefab {
        get { return this.projectilePrefab; }
    }

    [SerializeField]
    private IntVector2 selectionSize = new IntVector2 (1, 1);
    public IntVector2 SelectionSize {
        get { return this.selectionSize; }
    }

    [SerializeField]
    private bool wanderWhileIdle = true;
    public bool WanderWhileIdle {
        get { return this.wanderWhileIdle; }
    }

    [Header ("Unit Sound")]
    
    [SerializeField]
    private AudioIdentifier acknowledgeSound;
    public AudioIdentifier AcknowledgeSound {
        get { return this.acknowledgeSound; }
    }

    [SerializeField]
    private AudioIdentifier annoyedSound;
    public AudioIdentifier AnnoyedSound {
        get { return this.annoyedSound; }
    }

    [SerializeField]
    private AudioIdentifier deadSound;
    public AudioIdentifier DeadSound {
        get { return this.deadSound; }
    }

    [SerializeField]
    private AudioIdentifier readySound;
    public AudioIdentifier ReadySound {
        get { return this.readySound; }
    }

    [Header ("Traits")]

    [SerializeField]
    private UnitTraitData attackerTrait;
    public UnitTraitData AttackerTrait {
        get { return this.attackerTrait; }
    }

    [SerializeField]
    private UnitTraitData builderTrait;
    public UnitTraitData BuilderTrait {
        get { return this.builderTrait; }
    }

    [SerializeField]
    private UnitTraitData carrierTrait;
    public UnitTraitData CarrierTrait {
        get { return this.carrierTrait; }
    }

    [SerializeField]
    private UnitTraitData decayingTrait;
    public UnitTraitData DecayingTrait {
        get { return this.decayingTrait; }
    }

    [SerializeField]
    private UnitTraitData harvesterTrait;
    public UnitTraitData HarvesterTrait {
        get { return this.harvesterTrait; }
    }

    [SerializeField]
    private UnitTraitData interactiveTrait;
    public UnitTraitData InteractiveTrait {
        get { return this.interactiveTrait; }
    }

    [SerializeField]
    private UnitTraitData menderTrait;
    public UnitTraitData MenderTrait {
        get { return this.menderTrait; }
    }
    
    [SerializeField]
    private UnitTraitData minerTrait;
    public UnitTraitData MinerTrait {
        get { return this.minerTrait; }
    }

    [SerializeField]
    private UnitTraitData movingTrait;
    public UnitTraitData MovingTrait {
        get { return this.movingTrait; }
    }

    [SerializeField]
    private UnitTraitData portableTrait;
    public UnitTraitData PortableTrait {
        get { return this.portableTrait; }
    }

    [SerializeField]
    private UnitTraitData shooterTrait;
    public UnitTraitData ShooterTrait {
        get { return this.shooterTrait; }
    }

    [SerializeField]
    private UnitTraitData spellcasterTrait;
    public UnitTraitData SpellcasterTrait {
        get { return this.spellcasterTrait; }
    }
}

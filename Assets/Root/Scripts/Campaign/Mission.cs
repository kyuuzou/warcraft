using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : SceneObject {

    [SerializeField]
    private GameObject missionCompleteMenu;

    private AudioManager audioManager;
    private GameController gameController;
    private List<MissionRequirement> requirements;

    public override void Activate () {
        base.Activate ();

        this.InitializeExternals ();
        this.InitializeRequirements ();
    }

    // The mission is only won if the requirements are still satisfied after 5 seconds
    private IEnumerator CompleteMission () {
        yield return new WaitForSeconds (5.0f);

        if (this.EvaluateRequirements ()) {
            this.gameController.Pause ();
            this.missionCompleteMenu.SetActive (true);

            Faction faction = this.gameController.GetMainPlayer ().Factions[0];
            FactionTypeData data = FactionTypeDictionary.Instance.GetValue (faction.Data.Type);

            this.audioManager.Play (data.VictorySound);
        }
    }

    private bool EvaluateRequirements () {
        foreach (MissionRequirement requirement in this.requirements) {
            if (! requirement.Satisfied) {
                return false;
            }
        }

        return true;
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.audioManager = serviceLocator.AudioManager;
        this.gameController = serviceLocator.GameController;
    }

    private void InitializeRequirements () {
        this.requirements = new List<MissionRequirement> ();

        Level currentLevel = this.gameController.CurrentLevel;

        foreach (MissionRequirement requirement in currentLevel.Requirements) {
            MissionRequirement requirementCopy = requirement.GetInstance<MissionRequirement> ();
            requirementCopy.Initialize (this);

            this.requirements.Add (requirementCopy);
        }
    }

    public void OnRequirementSatisfied () {
        if (this.EvaluateRequirements ()) {
            this.StartCoroutine (this.CompleteMission ());
        }
    }
}

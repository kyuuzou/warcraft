using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MissionCompleteMenu : Menu {

    private void PressContinue () {
        this.GameController.Resume ();

        SceneManager.LoadScene ("Mission Outcome");
    }

    private void PressSave () {

    }
}
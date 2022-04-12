using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : Menu {

    [SerializeField]
    private GameObject buttons;
    
    [SerializeField]
    private Introduction introduction;

    protected override void Awake () {
        base.Awake ();

        //Application.runInBackground = true;
    }

    public void OnIntroductionFinished () {
        this.buttons.SetActive (true);
        this.introduction.gameObject.SetActive (false);
    }

    private void PressReplay () {
        this.buttons.SetActive (false);
        this.introduction.gameObject.SetActive (true);
    }

    private void PressStart () {
        SceneManager.LoadScene (Scene.Mission.ToString());
    }
}

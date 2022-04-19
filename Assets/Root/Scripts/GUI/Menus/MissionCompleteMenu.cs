using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#endif

public class MissionCompleteMenu : Menu {

    private void PressContinue() {
        this.GameController.Resume();

        SceneManager.LoadScene(Scene.MissionOutcome.ToString());
    }

    private void PressSave() {

    }
}

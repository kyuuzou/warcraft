using UnityEditor;

public class ScriptableObjectCreator : ScriptableWizard {

    public string type;

    [MenuItem("Assets/Create/Scriptable Object", false, (int)'S')]
    public static void CreateWizard() {
        ScriptableWizard.DisplayWizard<ScriptableObjectCreator>("Create Scriptable Object", "Create");
    }

    public void OnWizardCreate() {
        ScriptableObjectUtility.CreateAsset(this.type);
    }
}
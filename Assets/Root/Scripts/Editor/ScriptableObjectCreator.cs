using UnityEditor;
using UnityEngine;

public class ScriptableObjectCreator : ScriptableWizard {

    [SerializeField]
    private string type;

    [MenuItem("Assets/Create/Scriptable Object", false, (int)'S')]
    public static void CreateWizard() {
        ScriptableWizard.DisplayWizard<ScriptableObjectCreator>("Create Scriptable Object", "Create");
    }

    public void OnWizardCreate() {
        ScriptableObjectUtility.CreateAsset(this.type);
    }
}
using UnityEngine;

public class FactionTypeData : CustomScriptableObject, IInspectorDictionaryEntry<FactionType> {

    [SerializeField]
    private FactionType type;

    [SerializeField]
    private AudioIdentifier victorySound;
    public AudioIdentifier VictorySound {
        get { return this.victorySound; }
    }

    public FactionType Key {
        get { return this.type; }
    }

}

using UnityEngine;

public class InputIcon : CustomScriptableObject, IInspectorDictionaryEntry<InputType> {

    [SerializeField]
    private InputType type;
    public InputType Type {
        get { return this.type; }
    }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite {
        get { return this.sprite; }
    }

    public InputType Key {
        get { return this.Type; }
    }
}

using UnityEngine;

public class MenuButton : MonoBehaviour {

    [field: SerializeField]
    public MenuButtonType Type { get; private set; } = MenuButtonType.None;

    public void OnPress() {
        this.GetComponentInParent<Menu>().OnButtonPress(this.Type);
    }
}
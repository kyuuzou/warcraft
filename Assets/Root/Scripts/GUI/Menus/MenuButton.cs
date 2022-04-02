using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
 
    [SerializeField]
    private MenuButtonType type;
    public MenuButtonType Type {
        get { return this.type; }
        set { this.type = value; }
    }
    
    [SerializeField]
    private string caption;
    
    [SerializeField]
    private GUIText guiTextPrefab;

	private Camera guiCamera;

    private void Initialize () {
        this.guiCamera = ServiceLocator.Instance.GUICamera;

        this.InitializeCaption ();
    }

    private void InitializeCaption () {
        GUIText guiText = GUIText.Instantiate (this.guiTextPrefab) as GUIText;
        guiText.name = "Caption";
        guiText.text = this.caption;
        guiText.transform.parent = this.transform;
        
		Vector3 position = this.guiCamera.WorldToScreenPoint (this.transform.position);
        position.x /= Screen.width;
        position.y = position.y / Screen.height;
        guiText.transform.position = position;
    }

    private void Start () {
        this.Initialize ();

        this.InitializeCaption ();
    }
}
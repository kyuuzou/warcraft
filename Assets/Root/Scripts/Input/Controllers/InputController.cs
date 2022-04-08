using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : SceneObject {

    // No input at all
    public bool Locked { get; set; }
    
    // Input is only passed on to the elements of unrestrictedListeners
    public bool Restricted { get; set; }
    
    [SerializeField]
    private KeyboardInputController keyboardController;
    public KeyboardInputController KeyboardController {
        get { return this.keyboardController; }
    }

    [SerializeField]
    private MouseInputController mouseController;
    public MouseInputController MouseController {
        get { return this.mouseController; }
    }

    private Dictionary<InputType, bool> inputStatus;
    private Dictionary<InputType, List<SceneObject>> listenersByInput;

    /// <summary>
    /// If restricted, input notifications are sent solely to the elements that are either on this list,
    /// or children of elements on this list.
    /// </summary>
    private List<Transform> unrestrictedListeners;

	private Camera guiCamera;
    private MainCamera mainCamera;

    public void AddInputListener (SceneObject listener, InputType type, bool unrestricted = false) {
        if (! this.listenersByInput.ContainsKey (type)) {
            this.listenersByInput[type] = new List<SceneObject> ();
        }

        if (! this.listenersByInput[type].Contains (listener)) {
            this.listenersByInput[type].Add (listener);
        }

        if (unrestricted) {
            this.AddUnrestrictedListener (listener);
        }
    }

    public void AddUnrestrictedListener (SceneObject listener) {
        if (! this.unrestrictedListeners.Contains (listener.Transform)) {
            this.unrestrictedListeners.Add (listener.Transform);
        }
    }
    
    public List<SceneObject> GetUnrestrictedListeners (List<SceneObject> listeners) {
        if (this.unrestrictedListeners.Count == 0) {
            return listeners;
        }
        
        List<SceneObject> unrestrictedListeners = new List<SceneObject> ();
        
        foreach (SceneObject listener in listeners) {
            if (this.IsUnrestrictedListener (listener)) {
                unrestrictedListeners.Add (listener);
            }
        }
        
        return unrestrictedListeners;
    }
    
    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.keyboardController.Initialize ();
        this.mouseController.Initialize ();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.mainCamera = serviceLocator.MainCamera;
        this.guiCamera = serviceLocator.GUICamera;

        this.inputStatus = new Dictionary<InputType, bool> ();
        this.listenersByInput = new Dictionary<InputType, List<SceneObject>> ();
        this.unrestrictedListeners = new List<Transform> ();
        
        this.Locked = false;
    }

    protected override void InitializeInternals () {
        base.InitializeInternals ();

        this.Restricted = false;
    }
    
    private bool IsUnrestrictedListener (SceneObject sceneObject) {
        if (this.unrestrictedListeners.Count == 0) {
            return false;
        }
        
        if (this.unrestrictedListeners.Contains (sceneObject.Transform)) {
            return true;
        } else {
            Transform transform = sceneObject.Transform;
            
            while (transform.parent != null) {
                transform = transform.parent;
                
                if (this.unrestrictedListeners.Contains (transform)) {
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool MayNotify (SceneObject listener) {
        if (listener == null) {
            return false;
        }

        if (! this.Restricted) {
            return true;
        }

        return this.IsUnrestrictedListener (listener);
    }

    public override void OnInputChanged (InputSource source, InputType type, bool status) {
        if (! this.listenersByInput.ContainsKey (type)) {
            return;
        }

        for (int i = this.listenersByInput[type].Count - 1; i >= 0; i --) {
            SceneObject listener = this.listenersByInput[type][i];

            if (! this.Restricted || (this.Restricted && this.IsUnrestrictedListener (listener))) {
                listener.OnInputChanged (source, type, status);
            }
        }
    }

    private void Raycast () {
		Camera[] cameras = { this.guiCamera, this.mainCamera.Camera };

        foreach (Camera camera in cameras) {
            Vector3 position = camera.ScreenToWorldPoint (Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll (position, Vector2.zero);

            foreach (RaycastHit2D hit in hits) {
                SceneObject sceneObject = hit.collider.GetComponent<SceneObject> ();

                if (this.MayNotify (sceneObject) && sceneObject.OnManualMouseDown ()) {
                    return;
                }
            }
        }
    }

    public void RemoveInputListener (SceneObject listener, InputType type) {
        if (this.listenersByInput.ContainsKey (type)) {
            this.listenersByInput[type].Remove (listener);
        }
    }

    public void RemoveUnrestrictedListener (SceneObject listener) {
        this.unrestrictedListeners.Remove (listener.Transform);
    }

    public bool SetInputStatus (InputSource source, InputType type, bool status) {
        if (! this.inputStatus.ContainsKey (type) || this.inputStatus[type] != status) {
            this.inputStatus[type] = status;

            this.OnInputChanged (source, type, status);
            return true;
        }

        return false;
    }

    public void SetInputStatus (InputSource source, InputType type, bool status, bool raycast) {
        bool inputChanged = this.SetInputStatus (source, type, status);

        if (status && inputChanged) {
            this.Raycast ();
        }
    }

    private void Update () {
        this.keyboardController.Update ();
        this.mouseController.Update ();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MouseInputController {

    private InputController inputController;
    private List<SceneObject> mouseDownListeners;
    private List<SceneObject> mouseOverObjects;
    private List<SceneObject> mouseUpListeners;

    private Camera[] cameras;

    private void HandleMouseDown() {
        this.inputController.SetInputStatus(InputSource.Mouse, InputType.Left, true, true);
    }

    private void HandleMouseUp() {
        this.inputController.SetInputStatus(InputSource.Mouse, InputType.Left, false);
    }

    public void Initialize() {
        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.inputController = serviceLocator.InputController;

        //Debug.Log (serviceLocator.GUICamera.Camera, serviceLocator.MainCamera.Camera);

        this.cameras = new Camera[] { serviceLocator.GUICamera, serviceLocator.MainCamera.Camera };

        this.mouseDownListeners = new List<SceneObject>();
        this.mouseOverObjects = new List<SceneObject>();
        this.mouseUpListeners = new List<SceneObject>();
    }

    public void RemoveMouseDownListener(SceneObject listener) {
        this.mouseDownListeners.Remove(listener);
    }

    public void RemoveMouseUpListener(SceneObject listener) {
        this.mouseUpListeners.Remove(listener);
    }

    public void Update() {
        this.UpdateMouseOver();

        if (this.inputController.Locked) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            this.HandleMouseDown();
        } else if (Input.GetMouseButtonUp(0)) {
            this.HandleMouseUp();
        }
    }

    private void UpdateMouseOver() {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        foreach (Camera camera in this.cameras) {
            Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
            hits.AddRange(Physics2D.RaycastAll(position, Vector2.zero));
        }

        List<SceneObject> mouseExitObjects = new List<SceneObject>(this.mouseOverObjects);
        List<SceneObject> mouseOverObjects = new List<SceneObject>();

        foreach (RaycastHit2D hit in hits) {
            SceneObject sceneObject = hit.transform.GetComponent<SceneObject>();

            if (sceneObject == null) {
                continue;
            }

            if (this.mouseOverObjects.Contains(sceneObject)) {
                mouseExitObjects.Remove(sceneObject);
                continue;
            } else {
                this.mouseOverObjects.Add(sceneObject);
                mouseOverObjects.Add(sceneObject);
            }
        }

        foreach (SceneObject sceneObject in mouseExitObjects) {
            this.mouseOverObjects.Remove(sceneObject);
            sceneObject.OnManualMouseExit();
        }

        foreach (SceneObject sceneObject in mouseOverObjects) {
            sceneObject.OnManualMouseEnter();
        }
    }
}

using UnityEngine;
using System.Collections;

public class TransformData {

    public Vector3 LocalEulerAngles { get; set; }
    public Vector3 LocalPosition { get; set; }
    public Vector3 LocalScale { get; set; }

    public Vector3 EulerAngles { get; set; }
    public Vector3 Position { get; set; }

    private Transform transform;

    public TransformData (Transform transform) {
        this.transform = transform;

        this.Save ();
    }

    public void Save () {
        this.LocalEulerAngles = this.transform.localEulerAngles;
        this.LocalPosition = this.transform.localPosition;
        this.LocalScale = this.transform.localScale;
        
        this.EulerAngles = this.transform.eulerAngles;
        this.Position = this.transform.position;
    }
}

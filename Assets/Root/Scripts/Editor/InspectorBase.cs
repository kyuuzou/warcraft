using UnityEngine;
using UnityEditor;
using System.Collections;

public class InspectorBase<T> : Editor where T : UnityEngine.Object {
    
    protected T Target {
        get { return (T) target; }
    }
}

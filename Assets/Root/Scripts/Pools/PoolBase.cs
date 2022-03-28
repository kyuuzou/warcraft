using System;
using System.Collections;
using UnityEngine;

// Base class just so pools can be shown on the inspector
public abstract class PoolBase : SceneObject {
    
    public abstract Type PoolType { get; }
}

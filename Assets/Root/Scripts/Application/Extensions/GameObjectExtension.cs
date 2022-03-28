using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension {

    public static T AddComponentIfNecessary<T> (this GameObject gameObject) where T : Component {
        T component = gameObject.GetComponent<T> ();

        if (component == null) {
            return gameObject.AddComponent<T> ();
        }

        return component;
    }

    public static T[] GetComponentsInChildren<T> (
        this GameObject gameObject, bool includeInactive = false, bool sortAlphabetically = true
    ) where T : Component {
        return gameObject.transform.GetComponentsInChildren<T> (includeInactive, sortAlphabetically);
    }
}

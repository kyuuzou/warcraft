using System;
using UnityEngine;

public static class ComponentExtension {

    public static T[] GetComponentsInChildren<T>(
        this Component component, bool includeInactive = false, bool sortAlphabetically = true
    ) where T : Component {
        T[] components = component.GetComponentsInChildren<T>(includeInactive);

        if (sortAlphabetically) {
            Array.Sort(
                components,
                delegate (T x, T y) {
                    return x.name.CompareTo(y.name);
                }
            );
        }

        return components;
    }
}

using System.Collections;
using UnityEngine;

public static class FloatExtension {

    public static bool IsWithinRange (this float number, float minimum, float maximum) {
        return number >= minimum && number < maximum;
    }

}

using System.Collections;
using UnityEngine;

public static class SpriteRendererExtension {

    public static void SetOpacity (this SpriteRenderer spriteRenderer, float opacity) {
        Color color = spriteRenderer.color;
        color.a = opacity;
        spriteRenderer.color = color;
    }
}

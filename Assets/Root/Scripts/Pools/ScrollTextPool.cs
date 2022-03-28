using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextPool : Pool<ScrollTextPool, TextMesh> {

    public override TextMesh GetInstance () {
        TextMesh instance = base.GetInstance ();

        instance.SetOpacity (1.0f);

        return instance;
    }

    public void Scroll (Transform parent, Color color, string text) {
        this.StartCoroutine (this.ScrollCoroutine (parent, color, text));
    }

    private IEnumerator ScrollCoroutine (Transform parent, Color color, string text) {
        TextMesh scrollText = this.GetInstance ();
        scrollText.text = text;
        scrollText.color = color;
        scrollText.transform.parent = parent;
        scrollText.transform.SetLocalPosition (0.0f, 25.5f, - 100.0f);

        float delta = - 1.0f;

        do {
            scrollText.transform.SetLocalY (scrollText.transform.localPosition.y + Time.deltaTime * 50.0f);

            scrollText.SetOpacity (1.0f - delta);

            delta += Time.deltaTime;
            yield return null;
        } while (delta < 1.0f);

        this.AddInstance (scrollText);
    }
}

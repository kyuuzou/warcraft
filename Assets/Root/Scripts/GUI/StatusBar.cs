using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class StatusBar : SceneObject {

    private TextMesh textMesh;

    private string nonTimedText = string.Empty;

    private IEnumerator expireTextEnumerator = null;

    private IEnumerator ExpireTextCoroutine (string text, float time) {
        this.textMesh.text = text.ToUpper ();

        yield return new WaitForSeconds (time);

        this.textMesh.text = this.nonTimedText.ToUpper ();
        this.expireTextEnumerator = null;
    }

    private string FormatText (string text) {
        Regex underscore = new Regex ("_");
        int count = underscore.Matches (text).Count;

        if (count % 2 != 0) {
            Debug.LogError ("Status bar text has an odd number of underscores.");
            return text;
        }

        do {
            //Paint the hotkey yellow
            text = underscore.Replace (text, "<color=#FFFF00>", 1);
            text = underscore.Replace (text, "</color>", 1);

            count -= 2;
        } while (count > 0);

        return text;
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.textMesh = this.GetComponent<TextMesh> ();
    }

    /// <param name="time">
    /// How long the text should be shown. If 0, it will remain visible until it is changed once again.
    /// </param>
    public void SetText (string text, float time = 0) {
        this.InitializeExternals ();

        text = this.FormatText (text);

        if (time > 0) {
            if (this.expireTextEnumerator != null) {
                this.StopCoroutine (this.expireTextEnumerator);
            }

            this.expireTextEnumerator = this.ExpireTextCoroutine (text, time);
            this.StartCoroutine (this.expireTextEnumerator);
        } else {
            this.nonTimedText = text;

            if (this.expireTextEnumerator == null) {
                this.textMesh.text = text.ToUpper ();
            }
        }
    }
}

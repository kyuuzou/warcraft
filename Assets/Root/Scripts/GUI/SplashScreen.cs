using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    private GameObject terminal;
    
    [SerializeField]
    private GameObject caret;
    
    [SerializeField]
    private TextMesh[] lines;
    
    [SerializeField]
    private string[] commands;

    [SerializeField]
    private AudioClip[] letterKey;
    
    [SerializeField]
    private AudioClip enterKey;

    private string[] prompt;

    [SerializeField]
    private float characterWidth = 10.0f;

    [SerializeField]
    private GUITexture logo;
    private static readonly float logoAlpha = 1.0f;

    private void AlignCaret (int lineIndex) {
        Vector3 position = this.lines[lineIndex].transform.position;
        position.x += this.lines[lineIndex].text.Length * this.characterWidth;
        this.caret.transform.position =  position;
    }

    private void Awake () {
        //Application.runInBackground = true;

        this.prompt = new string[lines.Length];

        this.prompt[0] = this.lines[0].text;

        for (int i = 1; i < this.lines.Length; i ++) {
            this.prompt[i] = this.lines[i].text;
            this.lines[i].text = string.Empty;
        }

        this.AlignCaret (0);
    }

    private IEnumerator BlinkCaret () {
        while (true) {
            this.caret.SetActive (! this.caret.activeSelf);

            yield return new WaitForSeconds (0.25f);
        }
    }

    private IEnumerator PlayDosSequence () {
        this.StartCoroutine ("BlinkCaret");

        // Play only one sound per two keypresses, so it's not so noisy
        bool playKeySound = true;

        for (int i = 0; i < 2; i ++) {
            yield return new WaitForSeconds (0.2f);
            this.lines[i].text = this.prompt[i];
            this.AlignCaret (i);
            yield return new WaitForSeconds (0.2f);

            int lineLength = this.commands[i].Length;
            int letterIndex = 0;

            while (letterIndex < lineLength) {
                letterIndex ++;

                this.lines[i].text = this.prompt[i] + this.commands[i].Substring (0, letterIndex);
                this.AlignCaret (i);

                if (playKeySound) {
                    this.GetComponent<AudioSource>().clip = this.letterKey[Random.Range (0, this.letterKey.Length)];
                    this.GetComponent<AudioSource>().Play ();
                }

                playKeySound = ! playKeySound;

                yield return new WaitForSeconds (Random.Range (0.01f, 0.1f));
            }

            this.GetComponent<AudioSource>().clip = this.enterKey;
            this.GetComponent<AudioSource>().Play ();
        }

        yield return new WaitForSeconds (0.5f);
        this.lines[2].text = this.prompt[2];
        this.lines[3].text = this.prompt[3];
        this.AlignCaret (4);

        yield return new WaitForSeconds (2.0f);

        this.terminal.SetActive (false);
        this.StopCoroutine ("BlinkCaret");
    }

    private IEnumerator PlayFullSequence () {
        yield return this.StartCoroutine (this.PlayDosSequence ());
        yield return this.StartCoroutine (this.ShowLogo ());

        SceneManager.LoadScene ((int) Scene.MainMenu);
    }

    private IEnumerator ShowLogo () {
        Color color = this.logo.color;

        float elapsed = 0.0f;
        float length = 2.0f;

        while (elapsed < length) {
            color.a = Mathf.Lerp (0.0f, SplashScreen.logoAlpha, elapsed / length);
            this.logo.color = color;

            elapsed += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds (1.0f);

        elapsed = 0.0f;

        while (elapsed < length) {
            color.a = Mathf.Lerp (SplashScreen.logoAlpha, 0.0f, elapsed / length);
            this.logo.color = color;
            
            elapsed += Time.deltaTime;
            
            yield return null;
        }
    }
    
	private void Start () {
	    this.StartCoroutine (this.PlayFullSequence ());
	}
}




















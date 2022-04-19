using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class Introduction : MonoBehaviour {

    [SerializeField]
    private TextAsset textFile;

    [SerializeField]
    private Material[] materials;

    [SerializeField]
    private MainMenu menu;

    [SerializeField]
    private Transform fullCanvas;

    [SerializeField]
    private Transform partialCanvas;

    [SerializeField]
    private GUITexture fadeTexture;

    [SerializeField]
    private GUIText subtitles;

    private string[] subtitlesText;

    private int videoIndex;
    private int subtitleIndex;

    [SerializeField]
    private VideoClip videoClip;

    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] sounds;

    [SerializeField]
    private AudioClip doorSound;

    private int soundIndex;

    private void Awake() {
        this.ParseTextFile();
        this.fullCanvas.GetComponent<Renderer>().enabled = false;
    }

    private IEnumerator ChangeSubtitles(float delay) {
        yield return new WaitForSeconds(delay);
        this.subtitles.text = string.Empty;
        yield return new WaitForSeconds(0.25f);
        this.subtitles.text = this.subtitlesText[this.subtitleIndex++];
    }

    private IEnumerator Fade(float start = 0.5f, float end = 0.0f) {
        Color color = this.fadeTexture.color;

        float elapsed = 0.0f;
        float length = 2.0f;

        while (elapsed < length) {
            color.a = Mathf.Lerp(start, end, elapsed / length);
            this.fadeTexture.color = color;

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FadeIntoMovie() {
        this.subtitles.text = string.Empty;
        yield return this.StartCoroutine(this.Fade(0.0f, 0.5f));
        this.PlayMovie();

        // Wait three frames so the video is visible for the fade in
        for (int i = 0; i < 3; i++) {
            yield return new WaitForEndOfFrame();
        }

        this.videoPlayer.Pause();
        yield return this.StartCoroutine(this.Fade());
        this.videoPlayer.Play();
    }

    private void OnEnable() {
        this.videoIndex = this.subtitleIndex = this.soundIndex = 0;

        // this.StartCoroutine (this.PlaySequence ());
        this.StartCoroutine(this.PlayPremadeVideo());
    }

    private void ParseTextFile() {
        string[] separator = new string[] { "\r\n\r\n", "\n\n" };
        this.subtitlesText = this.textFile.text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    private void PlayMovie(bool loop = false) {
        Material material = this.materials[this.videoIndex++];
        this.partialCanvas.GetComponent<Renderer>().material = material;

        this.videoPlayer.playOnAwake = false;
        this.videoPlayer.clip = this.videoClip;
        this.videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        this.videoPlayer.targetMaterialRenderer = this.GetComponent<Renderer>();
        this.videoPlayer.targetMaterialProperty = "_MainTex";
        this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        this.videoPlayer.SetTargetAudioSource(0, this.audioSource);
        this.videoPlayer.Play();
    }

    private IEnumerator PlayPremadeVideo() {
        this.fadeTexture.gameObject.SetActive(false);
        this.partialCanvas.GetComponent<Renderer>().enabled = false;

        this.fullCanvas.GetComponent<Renderer>().enabled = true;
        this.fullCanvas.GetComponent<Renderer>().material = this.materials[0];

        this.videoPlayer.playOnAwake = false;
        this.videoPlayer.clip = this.videoClip;
        this.videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        this.videoPlayer.targetMaterialRenderer = this.GetComponent<Renderer>();
        this.videoPlayer.targetMaterialProperty = "_MainTex";
        this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        this.videoPlayer.SetTargetAudioSource(0, this.audioSource);
        this.videoPlayer.Play();

        yield return new WaitForSeconds(1.0f);

        /*
        yield return this.StartCoroutine (this.PlaySound (5.0f));
        yield return this.StartCoroutine (this.PlaySound (8.0f));
        yield return this.StartCoroutine (this.PlaySound (19.0f));
        yield return this.StartCoroutine (this.PlaySound (33.0f));
        yield return this.StartCoroutine (this.PlaySound (40.0f));
        yield return this.StartCoroutine (this.PlaySound (47.0f));
        yield return this.StartCoroutine (this.PlaySound (73.0f));
        yield return this.StartCoroutine (this.PlaySound (86.0f));
        yield return this.StartCoroutine (this.PlaySound (99.0f));
        yield return this.StartCoroutine (this.PlaySound (118.0f));
        yield return this.StartCoroutine (this.PlaySound (123.0f));
        */
        yield return this.StartCoroutine(this.WaitForEndOfMovie());

        this.menu.OnIntroductionFinished();
    }

    private IEnumerator PlaySequence() {
        yield return this.StartCoroutine(this.FadeIntoMovie());
        this.subtitles.text = this.subtitlesText[this.subtitleIndex++];

        yield return this.StartCoroutine(this.WaitForEndOfMovie());

        this.PlayMovie(true);

        yield return this.StartCoroutine(this.ChangeSubtitles(1.0f));
        yield return this.StartCoroutine(this.ChangeSubtitles(8.0f));
        yield return this.StartCoroutine(this.ChangeSubtitles(8.0f));
        yield return this.StartCoroutine(this.ChangeSubtitles(8.0f));

        yield return new WaitForSeconds(8.0f);

        /*
        this.videoIndex = 2;
        this.subtitleIndex = 5;
        */

        yield return this.StartCoroutine(this.FadeIntoMovie());
        this.subtitles.text = this.subtitlesText[this.subtitleIndex++];

        yield return this.StartCoroutine(this.ChangeSubtitles(8.0f));

        yield return this.StartCoroutine(this.WaitForEndOfMovie());
        this.PlayMovie(true);

        yield return this.StartCoroutine(this.ChangeSubtitles(7.0f));
        this.PlayMovie();
        yield return this.StartCoroutine(this.WaitForEndOfMovie());

        /*
        this.videoIndex = 5;
        this.subtitleIndex = 8;
        Color color = this.fadeTexture.color;
        color.a = 0.0f;
        this.fadeTexture.color = color;
        */

        this.PlayMovie();
        this.subtitles.text = this.subtitlesText[this.subtitleIndex++];

        yield return this.StartCoroutine(this.WaitForEndOfMovie());
        this.PlayMovie(true);

        yield return this.StartCoroutine(this.ChangeSubtitles(1.5f));
        yield return this.StartCoroutine(this.ChangeSubtitles(5.0f));
        yield return new WaitForSeconds(3.0f);
        this.subtitles.text = string.Empty;

        /*
        this.videoIndex = 7;
        this.subtitleIndex = 11;
        Color color = this.fadeTexture.color;
        color.a = 0.0f;
        this.fadeTexture.color = color;
        */

        this.PlayMovie();
        yield return this.StartCoroutine(this.WaitForEndOfMovie());

        this.fullCanvas.GetComponent<Renderer>().enabled = true;
        this.videoPlayer.Play();
        yield return this.StartCoroutine(this.WaitForEndOfMovie());

        yield return new WaitForSeconds(3.0f);
        //yield return this.StartCoroutine (this.Fade ());

        this.menu.OnIntroductionFinished();
    }

    private IEnumerator PlaySound(float delay) {
        yield return new WaitForSeconds(delay);

        this.GetComponent<AudioSource>().clip = this.sounds[this.soundIndex++];
        this.GetComponent<AudioSource>().Play();
    }

    private IEnumerator WaitForEndOfMovie() {
        // VideoPlayer doesn't set isPlaying until it is finished preparing
        while (this.videoPlayer.frame == 0 || this.videoPlayer.isPlaying) {
            yield return null;
        }
    }
}

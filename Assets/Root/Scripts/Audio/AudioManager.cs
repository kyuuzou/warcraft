using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SceneObject {

    [SerializeField]
    private bool mute = false;

    public float MusicVolume { get; set; }
    public float SoundVolume { get; set; }

    public float MusicSliderPosition { get; set; }
    public float SoundSliderPosition { get; set; }

    [SerializeField]
    private AudioSourcePool musicSources;

    [SerializeField]
    private AudioSourcePool soundEffectSources;

    [SerializeField]
    private AudioIdentifierDictionary samples;

    private Dictionary<AudioSourcePoolType, AudioSourcePool> audioSourcePools;
    private List<AudioSourceWrapper> loopables;

    public void Add (AudioSourcePool pool) {
        this.audioSourcePools.Add (pool.Type, pool); 
    }

    public void Add (AudioSourceWrapper loopable) {
        if (! this.loopables.Contains (loopable)) {
            this.loopables.Add (loopable);
        }
    }

    protected override void Awake () {
        base.Awake ();

        if (! this.enabled) {
            return;
        }

        this.InitializeExternals ();
    }

    public void Crossfade (AudioSourceWrapper source) {
        this.StartCoroutine (this.HandleCrossfading (source));
    }

    private IEnumerator Fade (AudioSourceWrapper source, bool fadeIn = false, bool pause = false) {
        float deltaTime = 0.0f;

        do {
            deltaTime = Mathf.Min (1.0f, deltaTime + Time.deltaTime * 0.25f);

            if (source.IsPlaying) {
                source.VolumeModifier = fadeIn ? deltaTime : 1.0f - deltaTime;
            }

            yield return null;
        } while (deltaTime < 1.0f);

        if (pause) {
            source.Pause ();
        }
    }

    private IEnumerator HandleCrossfading (AudioSourceWrapper currentSource) {
        this.loopables.Remove (currentSource);
        
        AudioSourceWrapper newSource = this.Play (currentSource.AudioType);
        newSource.VolumeModifier = 0.0f;

        this.StartCoroutine (Fade (currentSource, false, true));
        yield return new WaitForSeconds (2.0f);
        this.StartCoroutine (Fade (newSource, true));
    }

    private IEnumerator HandleLoopables () {
        do {
            for (int i = this.loopables.Count - 1; i >= 0; i --) {
                this.loopables[i].PollLoop ();
            }

            yield return new WaitForSeconds (0.25f);
        } while (true);
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.InitializeSources ();

        this.StartCoroutine (this.HandleLoopables ());
    }

    private void InitializeSources () {
        this.audioSourcePools = new Dictionary<AudioSourcePoolType, AudioSourcePool> ();
        this.loopables = new List<AudioSourceWrapper> ();
        
        this.musicSources.Initialize (this);
        this.soundEffectSources.Initialize (this);
    }

    public bool IsPlaying (params AudioIdentifier[] identifiers) {
        foreach (AudioIdentifier identifier in identifiers) {
            AudioSample sample = this.samples.GetValue (identifier);
            AudioSourcePool pool = this.audioSourcePools[sample.PoolType];

            if (pool.IsPlaying (identifier)) {
                return true;
            }
        }
        
        return false;
    }

    public void Pause (AudioIdentifier identifier) {
        AudioSample sample = this.samples.GetValue (identifier);
        AudioSourcePool pool = this.audioSourcePools[sample.PoolType];

        pool.Pause (identifier);
    }

    public void PauseAll (AudioSourcePoolType poolType) {
        this.audioSourcePools[poolType].PauseAll ();
    }

    public AudioSourceWrapper Play (AudioIdentifier identifier, int? forceIndex = null, float volumeModifier = 1.0f) {
        if (this.mute || identifier == AudioIdentifier.None) {
            return null;
        }

        AudioSample sample = this.samples.GetValue (identifier);

        if (sample == null) {
            return null;
        }

        if (sample.LastPlayedTime == Time.time) {
            return null;
        }

        sample.LastPlayedTime = Time.time;

        AudioSourcePool pool = this.audioSourcePools[sample.PoolType];

        return pool.Play (sample, forceIndex, volumeModifier);
    }

    public AudioSourceWrapper PlayUnique (
        AudioIdentifier identifier, int? forceIndex = null, float volumeModifier = 1.0f
    ) {
        if (this.mute || identifier == AudioIdentifier.None) {
            return null;
        }
        
        if (! this.IsPlaying (identifier)) {
            return this.Play (identifier, forceIndex, volumeModifier);
        }

        return null;
    }

    public void Remove (AudioSourceWrapper loopable) {
        this.loopables.Remove (loopable);
    }

    public void Resume (AudioIdentifier identifier) {
        if (this.mute) {
            return;
        }
        
        AudioSample sample = this.samples.GetValue (identifier);
        AudioSourcePool pool = this.audioSourcePools[sample.PoolType];

        pool.Resume (sample);
    }

    public void SetVolume (AudioSourcePoolType poolType, float volume) {
        this.audioSourcePools[poolType].Volume = volume;
    }
}

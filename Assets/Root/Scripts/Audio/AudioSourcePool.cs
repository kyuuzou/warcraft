using UnityEngine;

[System.Serializable]
public class AudioSourcePool {

    [SerializeField]
    private AudioSourcePoolType type;
    public AudioSourcePoolType Type {
        get { return this.type; }
    }

    [SerializeField]
    private int tally = 2;

    [SerializeField]
    private PlayerPreference volumePreference;

    private float volume;
    public float Volume {
        get { return this.volume; }
        set {
            this.volume = value;

            foreach (AudioSourceWrapper source in this.audioSources) {
                source.Volume = value;
            }
        }
    }

    private AudioManager audioManager;
    private AudioSourceWrapper[] audioSources;

    public void Initialize(AudioManager audioManager) {
        this.audioManager = audioManager;

        this.InitializeSources();
        this.InitializeVolume();

        this.audioManager.Add(this);
    }

    private void InitializeSources() {
        this.audioSources = new AudioSourceWrapper[this.tally];

        for (int i = 0; i < this.tally; i++) {
            this.audioSources[i] = new AudioSourceWrapper(this.audioManager);
        }
    }

    private void InitializeVolume() {
        if (!this.volumePreference.Exists()) {
            this.volumePreference.SetFloat(1.0f);
            PlayerPrefs.Save();
        }

        this.Volume = this.volumePreference.GetFloat();
    }

    private AudioSourceWrapper GetFreeSource() {
        foreach (AudioSourceWrapper source in this.audioSources) {
            if (!source.IsPlaying) {
                return source;
            }
        }

        float time = float.MaxValue;
        int index = -1;

        for (int i = 0; i < this.audioSources.Length; i++) {
            AudioSourceWrapper source = this.audioSources[i];

            if (source.IsLoop) {
                continue;
            }

            if (source.PlayTime < time) {
                time = source.PlayTime;
                index = i;
            }
        }

        //Debug.Log ("All channels full, choosing oldest: " + index + ", which is playing since: " + this.audioSources[index].PlayTime);

        return (index == -1) ? this.GetRandomSource() : this.audioSources[index];
    }

    private AudioSourceWrapper GetRandomSource() {
        int index = Random.Range(0, this.tally);

        return this.audioSources[index];
    }

    public bool IsPlaying(AudioIdentifier type) {
        foreach (AudioSourceWrapper source in this.audioSources) {
            if (source.AudioType == type && source.IsPlaying) {
                return true;
            }
        }

        return false;
    }

    public void Pause(AudioIdentifier type) {
        foreach (AudioSourceWrapper source in this.audioSources) {
            if (source.AudioType == type) {
                if (source.IsLoop) {
                    this.audioManager.Remove(source);
                }

                source.Pause();
            }
        }
    }

    public void PauseAll() {
        foreach (AudioSourceWrapper source in this.audioSources) {
            if (source.IsLoop) {
                this.audioManager.Remove(source);
            }

            source.Pause();
        }
    }

    public AudioSourceWrapper Play(AudioSample sample, int? forceIndex = null, float volumeModifier = 1.0f) {
        AudioSourceWrapper source = this.GetFreeSource();

        source.Play(sample, forceIndex, volumeModifier);

        if (sample.Loop) {
            this.audioManager.Add(source);
        }

        return source;
    }

    public void Resume(AudioSample sample) {
        foreach (AudioSourceWrapper source in this.audioSources) {
            if (source.AudioType == sample.Identifier) {
                if (sample.Loop) {
                    this.audioManager.Add(source);
                }

                source.Resume(sample);

                return;
            }
        }

        AudioSourceWrapper freeSource = this.GetFreeSource();

        if (freeSource.IsPlaying && freeSource.IsLoop) {
            this.audioManager.Remove(freeSource);
        }

        freeSource.Resume(sample);
    }
}

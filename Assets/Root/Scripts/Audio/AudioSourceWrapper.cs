using UnityEngine;

public class AudioSourceWrapper {

    private AudioManager audioManager;
    private AudioSource audioSource;

    private AudioSample audioSample;
    private AudioSample AudioSample {
        get { return this.audioSample; }
        set {
            this.audioSample = value;
            this.audioSource.clip = value.CurrentClip;
            this.audioSource.volume = value.Volume * this.Volume;
        }
    }

    private float volume;
    public float Volume {
        get { return this.volume; }
        set {
            this.volume = value;

            if (this.AudioSample == null) {
                this.audioSource.volume = value;
            } else {
                this.audioSource.volume = this.AudioSample.Volume * value;
            }
        }
    }

    //Used during crossfade
    private float volumeModifier;
    public float VolumeModifier {
        get { return this.volumeModifier; }
        set {
            this.volumeModifier = value;

            if (this.AudioSample != null) {
                this.audioSource.volume = this.AudioSample.Volume * this.Volume * value;
            }
        }
    }

    public float PlayTime { get; private set; }

    public AudioIdentifier AudioType {
        get { return this.AudioSample == null ? AudioIdentifier.None : this.AudioSample.Identifier; }
    }

    public bool IsLoop {
        get { return this.AudioSample == null ? false : this.AudioSample.Loop; }
    }

    public bool IsPlaying {
        get { return this.audioSource.isPlaying; }
    }

    public AudioSourceWrapper(AudioManager audioManager) {
        this.audioManager = audioManager;

        this.audioSource = this.audioManager.GameObject.AddComponent<AudioSource>();
        this.audioSource.playOnAwake = false;

        this.Volume = 1.0f;
    }

    public void Pause() {
        if (this.audioSample != null) {
            this.audioSample.PlayedTime = this.audioSource.time;
        }

        this.audioSource.Pause();
    }

    public void PollLoop() {
        if (this.audioSource.time > this.AudioSample.CurrentClip.length - 4.0f) {
            if (!this.IsLoop) {
                this.audioManager.Remove(this);
                this.Pause();
            } else if (this.AudioSample.Crossfade) {
                this.audioManager.Crossfade(this);
            } else {
                this.audioSource.time = 0.0f;
            }
        }
    }

    public void Play(AudioSample sample, int? forceIndex = null, float volumeModifier = 1.0f) {
        if (forceIndex == null) {
            sample.RandomizeClip();
        } else {
            sample.CurrentClipIndex = (int)forceIndex;
        }

        this.AudioSample = sample;

        this.audioSource.time = 0.0f;
        this.audioSource.volume = sample.Volume * volumeModifier;

        this.audioSource.Play();

        this.PlayTime = Time.time;
    }

    public void Resume(AudioSample sample) {
        if (this.AudioSample == sample) {
            if (this.IsPlaying) {
                return;
            }
        } else {
            this.AudioSample = sample;
            this.audioSource.time = sample.PlayedTime;
        }

        this.audioSource.Play();
    }
}

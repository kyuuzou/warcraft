using UnityEngine;

[System.Serializable]
public class AudioSample : ScriptableObject, IInspectorDictionaryEntry<AudioIdentifier> {

    [SerializeField]
    private AudioIdentifier identifier;
    public AudioIdentifier Identifier {
        get { return this.identifier; }
    }

    public AudioIdentifier Key {
        get { return this.Identifier; }
    }

    [SerializeField]
    private AudioSourcePoolType poolType = AudioSourcePoolType.SoundEffect;
    public AudioSourcePoolType PoolType {
        get { return this.poolType; }
    }

    [SerializeField]
    private AudioClip[] clips;
    public AudioClip[] Clips {
        get { return this.clips; }
    }

    [SerializeField]
    private bool loop = false;
    public bool Loop {
        get { return this.loop; }
    }

    [SerializeField]
    private bool crossfade = false;
    public bool Crossfade {
        get { return this.crossfade; }
    }

    [SerializeField]
    private float volume = 0.8f;
    public float Volume {
        get { return this.volume; }
    }

    public int CurrentClipIndex { get; set; }
    public float LastPlayedTime { get; set; }
    public float PlayedTime { get; set; }

    private int clipCount;

    public AudioClip CurrentClip {
        get { return this.Clips[this.CurrentClipIndex]; }
    }

    public void Initialize() {
        this.clipCount = this.clips.Length;
        this.CurrentClipIndex = Random.Range(0, this.clipCount);

        this.PlayedTime = 0.0f;
    }

    public void RandomizeClip() {
        if (this.clipCount == 1) {
            return;
        }

        int index = Random.Range(0, this.clipCount - 1);

        if (index >= this.CurrentClipIndex) {
            index++;
        }

        this.CurrentClipIndex = index;
    }
}

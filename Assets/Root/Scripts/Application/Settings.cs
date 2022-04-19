using UnityEngine;

public class Settings : Singleton<Settings> {

    [SerializeField]
    private bool alternateControls = false;
    public bool AlternateControls {
        get { return this.alternateControls; }
    }

    [SerializeField]
    private bool clearPreferences;

    [SerializeField]
    private bool fogOfWar = true;
    public bool FogOfWar {
        get { return this.fogOfWar; }
    }

    [SerializeField]
    private bool fastHarvest = false;
    public bool FastHarvest {
        get {
#if UNITY_EDITOR
            return this.fastHarvest;
#else
            return false;
#endif
        }
    }

    [SerializeField]
    private bool skipIntro = false;
    public bool SkipIntro {
        get { return this.skipIntro; }
    }

    protected override void Start() {
        base.Start();

        if (this.clearPreferences) {
            PlayerPrefs.DeleteAll();
        }
    }
}
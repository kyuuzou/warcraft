using UnityEngine;

public class Singleton<T> : SceneObject where T : MonoBehaviour {

    public static T Instance { get; private set; }

    [SerializeField]
    private bool destroyOnLoad = true;

    protected override void Awake() {
        if (Instance != null && Instance != this) {
            MonoBehaviour.Destroy(this.gameObject);
            return;
        }

        Instance = (T)(object)this;

        if (!this.destroyOnLoad) {
            MonoBehaviour.DontDestroyOnLoad(this.gameObject);
        }

        base.Awake();
    }
}
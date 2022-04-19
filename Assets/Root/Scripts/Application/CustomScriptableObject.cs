using UnityEngine;

public class CustomScriptableObject : ScriptableObject {

    protected CoroutineSurrogate Surrogate { get; private set; }

    /// <summary>
    /// Because this is a scriptable object and not a monobehaviour, you should not trust the initialized variable,
    /// as it will retain its value between runs. This causes it to be true when it is not really initialized, which
    /// may lead to odd behaviour when accessing other variables that point to instances that may have been destroyed
    /// last run.
    /// </summary>
    private bool initialized = false;
    protected bool Initialized {
        get { return this.initialized; }
        private set { this.initialized = value; }
    }

    public virtual void Activate() {

    }

    public virtual void Deactivate() {

    }

    public T GetInstance<T>() where T : CustomScriptableObject {
        return (T)CustomScriptableObject.Instantiate(this);
    }

    public virtual void Initialize() {
        this.Surrogate = CoroutineSurrogate.Instance;
        this.Initialized = true;
    }

    public virtual void ManualReset() {

    }

    public virtual void Pause() {

    }

    public virtual void Resume() {

    }
}

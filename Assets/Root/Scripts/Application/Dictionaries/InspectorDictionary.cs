using System.Collections.Generic;
using UnityEngine;

public abstract class InspectorDictionary<TInstance, TKey, TValue> : InspectorDictionaryBase
    where TInstance : MonoBehaviour
    where TValue : IInspectorDictionaryEntry<TKey>
{

    [SerializeField]
    private List<TValue> entries;
    protected List<TValue> Entries
    {
        get { return this.entries; }
    }

    [SerializeField]
    private bool dontDestroyOnLoad = false;

    [SerializeField]
    private bool singleton = false;

    private Dictionary<TKey, TValue> entryByKey;
    private bool initialized = false;

    public static TInstance Instance { get; private set; }

    private void Awake()
    {
        this.Initialize();
    }

    public TKey GetKey(TValue value)
    {
        foreach (TValue entry in this.entries)
        {
            if (entry.Equals(value))
            {
                return entry.Key;
            }
        }

        return default(TKey);
    }

    public TValue GetValue(TKey key)
    {
        if (!this.entryByKey.ContainsKey(key))
        {
            Debug.LogError(
                string.Format(
                    "{0} does not contain an entry for {1}",
                    this.name,
                    key
                )
            );

            return default(TValue);
        }

        return this.entryByKey[key];
    }

    public List<TValue> GetValues()
    {
        return this.Entries.GetRange(1, this.Entries.Count - 1);
    }

    private void Initialize()
    {
        if (this.initialized)
        {
            return;
        }

        this.initialized = true;

        this.InitializeEntries();
        this.InitializeSingleton();
    }

    protected virtual void InitializeEntries()
    {
        this.entryByKey = new Dictionary<TKey, TValue>();

        foreach (TValue entry in this.entries) {
            if (entry == null) {
                continue;
            }

            this.entryByKey[entry.Key] = entry;
        }
    }

    private void InitializeSingleton()
    {
        if (!this.singleton)
        {
            return;
        }

        if (Instance != null && Instance != this)
        {
            MonoBehaviour.Destroy(this.gameObject);
            return;
        }

        Instance = (TInstance)(object)this;

        if (this.dontDestroyOnLoad)
        {
            MonoBehaviour.DontDestroyOnLoad(this.gameObject);
        }
    }
}

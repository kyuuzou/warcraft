using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<TPool, TElement> : PoolBase
    where TPool : Pool<TPool, TElement>
    where TElement : Component {

    public override Type PoolType {
        get { return typeof(TPool); }
    }

    [SerializeField]
    private TElement prefab;

    private List<TElement> instances;

    public void AddInstance(TElement instance) {
        this.instances.Add(instance);

        instance.transform.parent = this.transform;
        instance.gameObject.SetActive(false);
    }

    public virtual TElement GetInstance() {
        TElement instance;

        if (this.instances.Count == 0) {
            instance = MonoBehaviour.Instantiate<TElement>(this.prefab);
        } else {
            instance = this.instances[0];
            this.instances.RemoveAt(0);

            instance.gameObject.SetActive(true);
        }

        return instance;
    }

    protected override void InitializeInternals() {
        if (this.InitializedInternals) {
            return;
        }

        base.InitializeInternals();

        this.instances = new List<TElement>();
    }
}

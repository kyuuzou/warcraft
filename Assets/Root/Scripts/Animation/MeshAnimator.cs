using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshAnimator : SceneObject {

    public MeshAnimation CurrentAnimation { get; set; }

    private Direction direction;
    public Direction Direction {
        get { return this.direction; }
        set {
            this.direction = value;

            if (this.CurrentAnimation != null) {
                this.CurrentAnimation.Direction = this.direction;
            }
        }
    }

    private IEnumerator animateEnumerator;
    private Dictionary<AnimationType, MeshAnimation> animationByType;
    private bool lastAnimationInverted;
    private AnimationType lastAnimationType;
    private List<IAnimationTriggerListener> listeners;
    private SceneObject owner;

    private bool ActivateAnimation(AnimationType animationType, bool inverted) {
        this.InitializeExternals();

        if (this.animateEnumerator != null) {
            this.StopCoroutine(this.animateEnumerator);
        }

        if (!this.animationByType.ContainsKey(animationType)) {
            Debug.LogError("Animation not found: " + animationType + ", on " + this);
            return false;
        }

        this.CurrentAnimation = this.animationByType[animationType];
        this.CurrentAnimation.Inverted = inverted;
        this.CurrentAnimation.Direction = this.Direction;
        this.CurrentAnimation.Activate();

        return true;
    }

    private IEnumerator Animate() {
        while (this.CurrentAnimation.Iterate()) {
            yield return new WaitForSeconds(this.CurrentAnimation.Delay);
        }
    }

    protected override void Awake() {
        base.Awake();

        this.InitializeExternals();
    }

    public MeshAnimation GetAnimation(AnimationType type) {
        if (this.animationByType.ContainsKey(type)) {
            return this.animationByType[type];
        }

        return null;
    }

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

        this.listeners = new List<IAnimationTriggerListener>();
        this.animationByType = new Dictionary<AnimationType, MeshAnimation>();

        /*
        CustomSprite sprite = this.GetComponent<CustomSprite> ();

        if (sprite != null) {
            this.RegisterTriggerListener (sprite);
        }
        */
    }

    public void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        foreach (IAnimationTriggerListener listener in this.listeners) {
            listener.OnAnimationTrigger(animationType, triggerType);
        }
    }

    private void OnEnable() {
        if (this.lastAnimationType != AnimationType.None) {
            this.Play(this.lastAnimationType, this.lastAnimationInverted);
        }
    }

    public override void Play(AnimationType animationType, bool inverted) {
        if (this.ActivateAnimation(animationType, inverted)) {
            this.lastAnimationType = animationType;
            this.lastAnimationInverted = inverted;

            if (this.GameObject.activeInHierarchy) {
                if (this.animateEnumerator != null) {
                    this.StopCoroutine(this.animateEnumerator);
                }

                this.animateEnumerator = this.Animate();
                this.StartCoroutine(this.animateEnumerator);
            }
        }
    }

    public void Play(AnimationType animationType, bool inverted, int currentTile) {
        if (this.ActivateAnimation(animationType, inverted)) {
            this.CurrentAnimation.CurrentFrame = currentTile;

            if (this.animateEnumerator != null) {
                this.StopCoroutine(this.animateEnumerator);
            }

            this.animateEnumerator = this.Animate();
            this.StartCoroutine(this.animateEnumerator);
        }
    }

    public void RegisterTriggerListener(IAnimationTriggerListener listener) {
        if (!this.listeners.Contains(listener)) {
            this.listeners.Add(listener);
        }
    }

    public void SetAnimations(MeshAnimation[] animations) {
        foreach (MeshAnimation animationObject in animations) {
            if (animationObject == null) {
                continue;
            }

            MeshAnimation animation = MeshAnimation.Instantiate(animationObject);
            this.animationByType[animation.Type] = animation;
            animation.Initialize(this);
        }
    }

    public void Stop() {
        this.StopAllCoroutines();
    }

    public void UnregisterTriggerListener(IAnimationTriggerListener listener) {
        this.listeners.Remove(listener);
    }

    public override IEnumerator YieldPlay(AnimationType animationType, bool inverted) {
        if (this.ActivateAnimation(animationType, inverted)) {
            if (this.animateEnumerator != null) {
                this.StopCoroutine(this.animateEnumerator);
            }

            this.animateEnumerator = this.Animate();
            yield return this.StartCoroutine(this.animateEnumerator);
        }
    }
}

using UnityEngine;

public class CustomAnimatedSprite : CustomSprite, IAnimationTriggerListener {

    [SerializeField]
    private MeshAnimation[] animations;
    private MeshAnimation[] Animations {
        get { return this.animations; }
    }

    public override void InitializeExternals() {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals();

        this.MeshAnimator.SetAnimations(this.Animations);
        this.MeshAnimator.RegisterTriggerListener(this);
    }

    public override void OnAnimationTrigger(AnimationType animationType, AnimationTriggerType triggerType) {
        if (triggerType == AnimationTriggerType.OnDecomposed) {
            this.ManualDestroy();
        }
    }

    protected override void Start() {
        base.Start();

        this.InitializeExternals();
    }
}

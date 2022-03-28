using UnityEngine;
using System.Collections;

[System.Serializable]
public class MeshAnimationTrigger {

    [SerializeField]
    private int frameIndex;
    public int FrameIndex {
        get { return this.frameIndex; }
        private set { this.frameIndex = value; }
    }

    [SerializeField]
    private AnimationTriggerType type;
    public AnimationTriggerType Type {
        get { return this.type; }
        set { this.type = value; }
    }

    public MeshAnimationTrigger (int frameIndex, AnimationTriggerType type) {
        this.frameIndex = frameIndex;
        this.Type = type;
    }
}

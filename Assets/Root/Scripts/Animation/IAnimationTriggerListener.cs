using UnityEngine;
using System.Collections;

public interface IAnimationTriggerListener {

    void OnAnimationTrigger (AnimationType animationType, AnimationTriggerType triggerType);

}

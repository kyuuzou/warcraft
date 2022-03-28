using System.Collections;
using UnityEngine;

public interface IUnitTraitMender : IAnimationTriggerListener, IUnitTrait {

    void Mend (Building building);

    void Mend (Decoration decoration);

    void MendAfterCooldown ();

    void OnMend ();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSelection : SceneObject {

    [SerializeField]
    private Transform[] bounds;

    [SerializeField]
    private LayerMask selectionMask;

    private GameController gameController;

    protected override void Awake () {
        base.Awake ();

        this.gameController = ServiceLocator.Instance.GameController;

        this.Deactivate ();
    }
    
    public void ManualUpdate (Vector3 origin, Vector3 ending) {
        this.Transform.position = origin;

        Vector3 scale = ending - origin;
        Vector3 horizontalScale = new Vector3 (scale.x * 0.5f, 1.0f, 1.0f);
        Vector3 verticalScale = new Vector3 (1.0f, scale.y * 0.5f, 1.0f);

        this.UpdateBound (0, horizontalScale, new Vector3 (origin.x + scale.x * 0.5f, origin.y, 0.0f));
        this.UpdateBound (1, verticalScale, new Vector3 (ending.x - 1.0f, origin.y + scale.y * 0.5f, 0.0f));
        this.UpdateBound (2, horizontalScale, new Vector3 (origin.x + scale.x * 0.5f, ending.y, 0.0f));
        this.UpdateBound (3, verticalScale, new Vector3 (origin.x + 1.0f, origin.y + scale.y * 0.5f, 0.0f));

        this.UpdateSelectedGroup (origin, ending);
    }

    private void UpdateBound (int index, Vector3 localScale, Vector3 position) {
        Transform bound = this.bounds[index];
        
        bound.localScale = localScale;
        bound.position = position;
    }

    private void UpdateSelectedGroup (Vector3 origin, Vector3 ending) {
        Collider2D[] colliders = Physics2D.OverlapAreaAll (origin, ending, this.selectionMask);

        SpawnableSpriteGroup group = this.gameController.CurrentGroup;
        group.Clear ();

        if (colliders.Length == 0) {
            return;
        }

        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag ("Unit")) {
                Unit unit = collider.GetComponent<Unit> ();
                group.Add (unit);
            }
        }
    }
}

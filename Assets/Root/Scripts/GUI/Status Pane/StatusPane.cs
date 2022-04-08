using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPane : SceneObject {

    [SerializeField]
    private GameObject groupPane;

    private SingleStatusPane[] groupSinglePanes;

    [SerializeField]
    private SingleStatusPane singlePane;

    [SerializeField]
    private Transform progressBar;

    [SerializeField]
    private TextMesh caption;

    [SerializeField]
    private TiledElement healthBarBorders;

    private TiledElement background;

    private GameController gameController;

    protected override void Awake () {
        base.Awake ();

        this.Hide ();
    }

    private void Hide () {
        this.Renderer.enabled = false;
        this.groupPane.SetActive (false);
        this.singlePane.Deactivate ();
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.gameController = ServiceLocator.Instance.GameController;
        
        this.background = this.GetComponent<TiledElement>();
        this.groupSinglePanes = this.groupPane.GetComponentsInChildren<SingleStatusPane> (true, true);
    }

    public void ManualUpdate () {
        this.InitializeExternals ();

        if (! this.GameObject.activeInHierarchy) {
            return;
        }

        SpawnableSpriteGroup group = this.gameController.CurrentGroup;

        switch (group.Sprites.Count) {
            case 0:
                this.Hide ();
                break;

            case 1:
                this.ShowSingle ();
                break;

            default:
                this.ShowGroup ();
                break;
        }
    }

    public void SetBackgroundIndex (int tileIndex) {
        this.progressBar.GetComponent<Renderer>().enabled = tileIndex == 6;

        //this.background.SetCurrentTileY (tileIndex);
    }

    public void SetProgress (float progress) {
        if (! this.progressBar.GetComponent<Renderer>().enabled) {
            this.progressBar.GetComponent<Renderer>().enabled = true;
        }
        
        Vector3 localScale = this.progressBar.localScale;
        localScale.x = progress;
        this.progressBar.localScale = localScale;
        
        Vector3 localPosition = this.progressBar.localPosition;
        localPosition.x = -64 + progress * 64.0f;
        this.progressBar.localPosition = localPosition;
    }
    
    private void ShowGroup () {
        this.Renderer.enabled = true;
        this.groupPane.SetActive (true);
        this.singlePane.Deactivate ();

        IList<SpawnableSprite> sprites = this.gameController.CurrentGroup.Sprites;

        int i;
        int spriteCount = sprites.Count;

        for (i = 0; i < spriteCount; i ++) {
            this.groupSinglePanes[i].Activate ();
            this.groupSinglePanes[i].Owner = sprites[i];
        }

        for (; i < this.groupSinglePanes.Length; i ++) {
            this.groupSinglePanes[i].Deactivate ();
        }

        this.SetBackgroundIndex (Mathf.Abs (spriteCount - 7));
        this.healthBarBorders.SetCurrentTileY (Mathf.Abs (spriteCount - 4));
    }

    private void ShowSingle () {
        this.Renderer.enabled = true;
        this.groupPane.SetActive (false);
        this.singlePane.Activate ();

        SpawnableSprite owner = this.gameController.CurrentGroup.Sprites[0];

        this.singlePane.Owner = owner;
        this.SetBackgroundIndex (owner.StatusBackgroundIndex);
        this.caption.text = owner.Title;
    }
}

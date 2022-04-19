using System.Collections.Generic;
using UnityEngine;

public class GameButton : SceneObject {

    [SerializeField]
    private TiledElement portrait;

    private GameButtonType action;
    private GameController gameController;
    private StatusBar statusBar;
    private string statusText;

    public override void InitializeExternals() {
        base.InitializeExternals();

        ServiceLocator serviceLocator = ServiceLocator.Instance;
        this.gameController = serviceLocator.GameController;
        this.statusBar = serviceLocator.StatusBar;
    }

    public override bool OnManualMouseDown() {
        this.Transform.SetY(this.DefaultTransformData.Position.y - 1);

        return true;
    }

    public override void OnManualMouseUp() {
        base.OnManualMouseUp();

        this.Transform.SetY(this.DefaultTransformData.Position.y);
    }

    private void OnMouseExit() {
        this.statusBar.SetText(string.Empty);
    }

    private void OnMouseOver() {
        this.statusBar.SetText(this.statusText);
    }

    private void OnMouseUpAsButton() {
        if (this.action != GameButtonType.None) {
            IList<SpawnableSprite> sprites = this.gameController.CurrentGroup.Sprites;

            foreach (SpawnableSprite sprite in sprites) {
                sprite.OnButtonPress(this.action);
            }
        }
    }

    public void SetAction(GameButtonType action) {
        bool visible = (action != GameButtonType.None && action != GameButtonType.Invalid);

        if (!visible) {
            this.SetVisible(false);
            return;
        }

        //if the owner (faction, not player) of this button has unlocked an upgrade for this button type,
        //show that one instead
        //it's also possible to have unlocked every upgrade for this action, in which case visible = false
        Faction faction = this.gameController.CurrentGroup.Faction;
        action = faction == null ? GameButtonType.None : faction.GetButton(action);
        visible = (action != GameButtonType.None && action != GameButtonType.Invalid);

        if (!visible) {
            this.SetVisible(false);
            return;
        }

        this.statusText = action.GetDescription().ToUpper();
        this.portrait.SetCurrentTileY((int)action);

        this.action = action;

        this.SetVisible(true);
    }

    public override void SetVisible(bool visible) {
        this.Renderer.enabled = visible;
        this.Collider.enabled = visible;
        this.portrait.GetComponent<Renderer>().enabled = visible;

        if (!visible/* && this.statusBar.text == this.statusText*/) {
            this.OnMouseExit();
        }
    }
}

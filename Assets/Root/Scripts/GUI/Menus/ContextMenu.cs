using UnityEngine;
using System.Collections.Generic;

public class ContextMenu : SceneObject {
    
    [SerializeField]
    private GameObject buttonRoot;
    
    [SerializeField]
    private TextMesh caption;

    [SerializeField]
    private ContextMenuNode cancelNode;
    public ContextMenuNode CancelNode {
        get { return this.cancelNode; }
    }

    private GameButton[] buttons;
    private ContextMenuNode lastNode;
    private GameController gameController;

    public static readonly GameButtonType[] CancelMenu = new GameButtonType[]{
        GameButtonType.None,
        GameButtonType.None,
        GameButtonType.None,
        GameButtonType.None,
        GameButtonType.None,
        GameButtonType.Cancel
    };
    
    protected override void Awake () {
        base.Awake ();

        this.gameController = ServiceLocator.Instance.GameController;

        this.InitializeExternals ();
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.buttons = this.buttonRoot.GetComponentsInChildren<GameButton> (true, true);

        foreach (GameButton button in this.buttons) {
            button.InitializeExternals ();
        }
    }

    public void ManualUpdate () {
        if (! this.gameObject.activeInHierarchy) {
            return;
        }

        IList<Unit> units = this.gameController.CurrentGroup.Units;

        switch (units.Count) {
            case 0:
                this.SetVisible (false);
                break;

            case 1:
                this.SetNode (units[0].Data.RootMenuNode);
                break;

            default:
                GameButtonType[] buttons = this.MergeButtons (units);
                this.SetButtons (buttons);

                break;

        }

        this.SetCaption (string.Empty);
    }

    private GameButtonType[] MergeButtons (IList<Unit> units) {
        GameButtonType[] buttons = new GameButtonType[6];
        
        foreach (SpawnableSprite sprite in units) {
            int i = 0;
            ContextMenuNode node = sprite.Data.RootMenuNode;
            
            foreach (GameButtonType button in node.GetButtons ()) {
                if (buttons[i] == GameButtonType.Invalid || buttons[i] == button) {

                } else if (buttons[i] == GameButtonType.None) {
                    buttons[i] = button;
                } else {
                    buttons[i] = GameButtonType.Invalid;
                }

                i ++;
            }
            
            for (; i < buttons.Length; i ++) {
                buttons[i] = GameButtonType.Invalid;
            }
        }

        return buttons;
    }

    public void RefreshButtons () {
        this.SetNode (this.lastNode);
    }

    private void SetButtons (params GameButtonType[] buttons) {
        this.SetButtons (new List<GameButtonType> (buttons));
    }

    private void SetButtons (List<GameButtonType> buttons) {
        if (! this.buttonRoot.activeSelf) {
            this.buttonRoot.SetActive (true);
        }
        
        IList<Unit> units = this.gameController.CurrentGroup.Units;

        foreach (SpawnableSprite unit in units) {
            unit.FilterButtons (ref buttons);
        }
        
        int index = 0;
        
        foreach (GameButtonType button in buttons) {
            this.buttons[index].SetAction (button);
            index ++;
        }
        
        for (int i = index; i < this.buttons.Length; i++) {
            this.buttons[i].SetAction (GameButtonType.None);
        }
    }

    public void SetCaption (string text) {
        this.caption.text = text;
    }

    public void SetNode (ContextMenuNode node) {
        if (node == null) {
            return;
        }

        this.lastNode = node;

        this.SetButtons (node.GetButtons ());
    }

    public override void SetVisible (bool visible) {
        this.InitializeExternals ();

        this.buttonRoot.SetActive (visible);

        if (! visible) {
            foreach (GameButton button in this.buttons) {
                button.SetVisible (visible);
            }
        }
    }
}

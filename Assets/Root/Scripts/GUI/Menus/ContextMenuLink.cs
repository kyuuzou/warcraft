using System;
using UnityEngine;

[Serializable]
public class ContextMenuLink {

    [SerializeField]
    private GameButtonType button;
    public GameButtonType Button {
        get { return this.button; }
    }

    [SerializeField]
    private ContextMenuNode node;
    public ContextMenuNode Node {
        get { return this.node; }
    }
}

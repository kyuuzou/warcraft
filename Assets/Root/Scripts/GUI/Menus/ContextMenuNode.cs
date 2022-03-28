using System;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuNode : CustomScriptableObject {

    [SerializeField]
    private ContextMenuLink[] links;

    private Dictionary<GameButtonType, ContextMenuLink> linkByNode;

    public override void Initialize () {
        base.Initialize ();

        this.linkByNode = new Dictionary<GameButtonType, ContextMenuLink> ();

        foreach (ContextMenuLink link in this.links) {
            this.linkByNode[link.Button] = link;

            if (link.Node != null) {
                link.Node.Initialize ();
            }
        }
    }

    /// <summary>
    /// Gets the buttons.
    /// This method can't just return "this.linkByNode.Keys", because there's repeated buttons with the same key: 
    /// GameButtonType.None
    /// </summary>
    public List<GameButtonType> GetButtons () {
        List<GameButtonType> buttons = new List<GameButtonType> ();

        foreach (ContextMenuLink link in this.links) {
            buttons.Add (link.Button);
        }

        return buttons;
    }

    /// <summary>
    /// Returns the node this button links to.
    /// </summary>
    public ContextMenuNode GetLinkedNode (GameButtonType button) {
        foreach (ContextMenuLink link in this.links) {
            if (link.Button == button) {
                return link.Node;
            }
        }

        return null;
    }
}

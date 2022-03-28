using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class FactionStanding {

    [SerializeField]
    private FactionIdentifier identifier;
    public FactionIdentifier Identifier {
        get { return this.identifier; }
    }
    
    [SerializeField]
    private FactionRelationship relationship;
    public FactionRelationship Relationship {
        get { return this.relationship; }
    }
}

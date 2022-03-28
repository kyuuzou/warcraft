
public enum DecorationType {
    None,
    Door    = 3,
    Road    = 2,
    Rug     = 4,
    Wall    = 1,

    /// <summary>
    /// Unknown means the type wasn't set on the map editor. Here to differentiate from None, in the sense that None
    /// means the type wasn't set, while Unknown means the type wasn't set on purpose.
    /// </summary>
    Unknown               = 1000,
}

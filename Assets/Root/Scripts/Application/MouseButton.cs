
public enum MouseButton {
    None,
    Left,
    Right
}

public static class MouseButtonExtension {

    public static int GetIndex (this MouseButton button) {
        return ((int) button) - 1;
    }
}
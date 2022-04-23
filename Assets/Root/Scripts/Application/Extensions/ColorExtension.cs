using UnityEngine;

public static class ColorExtension {

    public static Color FromHex(string color) {
        color = color.TrimStart('#');

        float red = (ColorExtension.HexToInt(color[1]) + ColorExtension.HexToInt(color[0]) * 16f) / 255f;
        float green = (ColorExtension.HexToInt(color[3]) + ColorExtension.HexToInt(color[2]) * 16f) / 255f;
        float blue = (ColorExtension.HexToInt(color[5]) + ColorExtension.HexToInt(color[4]) * 16f) / 255f;

        return new Color { r = red, g = green, b = blue, a = 1 };
    }

    public static Color FromRGB(int r, int g, int b, int a = 255) {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }

    private static int HexToInt(char hexValue) {
        return int.Parse(hexValue.ToString(), System.Globalization.NumberStyles.HexNumber);
    }

    public static Color MoveTowards(this Color color, Color targetColor, float maximumStep) {
        for (int i = 0; i < 4; i++) {
            float difference = targetColor[i] - color[i];

            if (Mathf.Approximately(difference, 0.0f)) {
                continue;
            }

            float step = Mathf.Min(Mathf.Abs(difference), maximumStep);
            step *= Mathf.Sign(difference);

            color[i] += step;
        }

        return color;
    }

    public static string ToHex(this Color color, bool includeHash = false) {
        string red = Mathf.FloorToInt(color.r * 255).ToString("X2");
        string green = Mathf.FloorToInt(color.g * 255).ToString("X2");
        string blue = Mathf.FloorToInt(color.b * 255).ToString("X2");

        return (includeHash ? "#" : "") + red + green + blue;
    }
}

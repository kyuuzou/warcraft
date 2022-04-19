using System.Collections.Generic;
using UnityEngine;

public static class TextMeshExtension {

    private static readonly string[] separators = new string[] { " ", "\r\n", "\n" };

    public static void SetOpacity(this TextMesh mesh, float alpha) {
        Color color = mesh.color;
        color.a = alpha;
        mesh.color = color;
    }

    public static void WrapText(this TextMesh mesh, float maximumWidth) {
        string text = mesh.text;

        if (text == string.Empty) {
            return;
        }

        string[] words = text.Split(TextMeshExtension.separators, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> lines = new List<string>();

        if (words.Length == 0) {
            return;
        }

        string line = words[0];

        Renderer renderer = mesh.GetComponent<Renderer>();

        for (int i = 1; i < words.Length; i++) {
            string word = words[i];
            string tempLine = string.Concat(line, " ", word);

            mesh.text = tempLine;

            if (renderer.bounds.size.x > maximumWidth) {
                lines.Add(line);
                line = word;
            } else {
                line = tempLine;
            }
        }

        lines.Add(line);

        text = string.Join("\n", lines.ToArray());
        mesh.text = text;
    }
}

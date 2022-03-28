using System.Collections;
using System.IO;
using UnityEngine;

public static class StringExtension {

    public static int GetLineCount (this string text) {
        return text.Split ('\n').Length;
    }

    public static string RemoveFileExtension (this string text) {
        return Path.GetFileNameWithoutExtension (text);
    }
}

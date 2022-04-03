using System;
using System.Text;
using UnityEngine;

public enum PlayerPreference {
    None,
    MusicVolume = 6,
    SoundVolume = 11,
}

public static class PlayerPreferenceExtension {

    public static bool Exists(this PlayerPreference key) {
        return PlayerPrefs.HasKey(key.ToString());
    }

    public static bool GetBool(this PlayerPreference key) {
        return key.GetInt() == 1;
    }

    public static bool[] GetBoolArray(this PlayerPreference key) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            string[] stringArray = PlayerPrefs.GetString(keyString).Split("|"[0]);
            bool[] boolArray = new bool[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++) {
                boolArray[i] = Convert.ToBoolean(stringArray[i]);
            }

            return boolArray;
        }

        return new bool[0];
    }

    public static bool[] GetBoolArray(this PlayerPreference key, bool defaultValue, int defaultSize) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return GetBoolArray(key);
        }

        bool[] boolArray = new bool[defaultSize];

        for (int i = 0; i < defaultSize; i++) {
            boolArray[i] = defaultValue;
        }

        return boolArray;
    }

    public static float GetFloat(this PlayerPreference key) {
        return PlayerPrefs.GetFloat(key.ToString());
    }

    public static float[] GetFloatArray(this PlayerPreference key) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            string[] stringArray = PlayerPrefs.GetString(keyString).Split("|"[0]);
            float[] floatArray = new float[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++) {
                floatArray[i] = Convert.ToSingle(stringArray[i]);
            }

            return floatArray;
        }

        return new float[0];
    }

    public static float[] GetFloatArray(this PlayerPreference key, float defaultValue, int defaultSize) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return GetFloatArray(key);
        }

        float[] floatArray = new float[defaultSize];

        for (int i = 0; i < defaultSize; i++) {
            floatArray[i] = defaultValue;
        }

        return floatArray;
    }

    public static int GetInt(this PlayerPreference key) {
        return PlayerPrefs.GetInt(key.ToString());
    }

    public static int[] GetIntArray(this PlayerPreference key) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            string[] stringArray = PlayerPrefs.GetString(keyString).Split("|"[0]);
            int[] intArray = new int[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++) {
                intArray[i] = Convert.ToInt32(stringArray[i]);
            }

            return intArray;
        }

        return new int[0];
    }

    public static int[] GetIntArray(this PlayerPreference key, int defaultValue, int defaultSize) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return GetIntArray(key);
        }

        int[] intArray = new int[defaultSize];

        for (int i = 0; i < defaultSize; i++) {
            intArray[i] = defaultValue;
        }

        return intArray;
    }

    public static string GetString(this PlayerPreference key) {
        return PlayerPrefs.GetString(key.ToString());
    }

    public static string[] GetStringArray(this PlayerPreference key) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return PlayerPrefs.GetString(keyString).Split("\n"[0]);
        }

        return new string[0];
    }

    public static string[] GetStringArray(this PlayerPreference key, char separator) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return PlayerPrefs.GetString(keyString).Split(separator);
        }

        return new string[0];
    }

    public static string[] GetStringArray(this PlayerPreference key, string defaultValue, int defaultSize) {
        return GetStringArray(key, "\n"[0], defaultValue, defaultSize);
    }

    public static string[] GetStringArray(this PlayerPreference key, char separator, string defaultValue, int defaultSize) {
        string keyString = key.ToString();

        if (PlayerPrefs.HasKey(keyString)) {
            return PlayerPrefs.GetString(keyString).Split(separator);
        }

        string[] stringArray = new string[defaultSize];

        for (int i = 0; i < defaultSize; i++) {
            stringArray[i] = defaultValue;
        }

        return stringArray;
    }

    public static Vector3 GetVector3(this PlayerPreference key) {
        float[] floatArray = GetFloatArray(key);

        if (floatArray.Length < 3) {
            return Vector3.zero;
        }

        return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
    }

    public static bool IsSet(this PlayerPreference key) {
        return PlayerPrefs.HasKey(key.ToString());
    }

    public static void SetBool(this PlayerPreference key, bool value) {
        key.SetInt(value ? 1 : 0);
    }

    public static bool SetBoolArray(this PlayerPreference key, params bool[] boolArray) {
        string keyString = key.ToString();

        if (boolArray.Length == 0) {
            return false;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < boolArray.Length - 1; i++) {
            sb.Append(boolArray[i]).Append("|");
        }

        sb.Append(boolArray[boolArray.Length - 1]);

        try {
            PlayerPrefs.SetString(keyString, sb.ToString());
        } catch (Exception e) {
            Debug.Log(e);

            return false;
        }

        return true;
    }

    public static void SetFloat(this PlayerPreference key, float value) {
        PlayerPrefs.SetFloat(key.ToString(), value);
    }

    public static bool SetFloatArray(this PlayerPreference key, params float[] floatArray) {
        string keyString = key.ToString();

        if (floatArray.Length == 0) {
            return false;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < floatArray.Length - 1; i++) {
            sb.Append(floatArray[i]).Append("|");
        }

        sb.Append(floatArray[floatArray.Length - 1]);

        try {
            PlayerPrefs.SetString(keyString, sb.ToString());
        } catch (Exception e) {
            Debug.LogWarning(e);

            return false;
        }

        return true;
    }

    public static void SetInt(this PlayerPreference key, int value) {
        PlayerPrefs.SetInt(key.ToString(), value);
    }

    public static bool SetIntArray(this PlayerPreference key, params int[] intArray) {
        string keyString = key.ToString();

        if (intArray.Length == 0) {
            return false;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < intArray.Length - 1; i++) {
            sb.Append(intArray[i]).Append("|");
        }

        sb.Append(intArray[intArray.Length - 1]);

        try {
            PlayerPrefs.SetString(keyString, sb.ToString());
        } catch (Exception e) {
            Debug.Log(e);
            return false;
        }

        return true;
    }

    public static void SetString(this PlayerPreference key, string value) {
        PlayerPrefs.SetString(key.ToString(), value);
    }

    public static bool SetStringArray(this PlayerPreference key, params string[] stringArray) {
        if (!SetStringArray(key, "\n"[0], stringArray)) {
            return false;
        }

        return true;
    }

    public static bool SetStringArray(this PlayerPreference key, char separator, params string[] stringArray) {
        string keyString = key.ToString();

        if (stringArray.Length == 0) {
            return false;
        }

        try {
            PlayerPrefs.SetString(keyString, string.Join(separator.ToString(), stringArray));
        } catch (Exception e) {
            Debug.LogWarning(e);
            return false;
        }

        return true;
    }

    public static bool SetVector3(this PlayerPreference key, Vector3 vector) {
        return SetFloatArray(
            key,
            new float[3] { vector.x, vector.y, vector.z }
        );
    }
}

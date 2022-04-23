using System.Collections.Generic;
using UnityEngine;

public class FlagDictionary : Dictionary<string, int> {

    public void AddPattern(string pattern, int index) {
        List<string> keys = new List<string>();
        List<string> additions = new List<string>();

        char c = pattern[pattern.Length - 1];

        if (c == 'X') {
            keys.Add("0");
            keys.Add("1");
        } else {
            keys.Add(c.ToString());
        }

        for (int i = pattern.Length - 2; i >= 0; i--) {
            c = pattern[i];

            if (c == 'X') {
                for (int j = 0; j < keys.Count; j++) {
                    additions.Add(0 + keys[j]);
                    additions.Add(1 + keys[j]);
                }

                keys.Clear();
                keys.AddRange(additions);
                additions.Clear();
            } else {
                for (int j = 0; j < keys.Count; j++) {
                    keys[j] = c + keys[j];
                }
            }
        }

        foreach (string key in keys) {
            if (this.ContainsKey(key)) {
                string error = "Attempted to add repeated key: " + key + ", pattern: " + pattern;
                error += ", existing index: " + this[key] + ", new index: " + index;

                Debug.Log(error);
            } else {
                this.Add(key, index);
            }
        }
    }
}

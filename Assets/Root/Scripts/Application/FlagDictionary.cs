using System.Collections.Generic;
using UnityEngine;

public class FlagDictionary : Dictionary<string, int> {
    
    public FlagDictionary () : base () {
        
    }
    
    public FlagDictionary (int capacity) : base (capacity) {

    }
    
    public void AddPattern (string pattern, int index) {
        List<string> keys = new List<string> ();
        List<string> additions = new List<string> ();

        char c = pattern[pattern.Length - 1];
        
        if (c == 'X') {
            keys.Add ("0");
            keys.Add ("1");
        } else
            keys.Add (c.ToString ());
        
        for (int i = pattern.Length - 2; i >= 0; i --) {
            c = pattern[i];
            
            if (c == 'X') {
                for (int j = 0; j < keys.Count; j ++) {
                    additions.Add (0 + keys[j]);
                    additions.Add (1 + keys[j]);
                }
                
                keys.Clear ();
                keys.AddRange (additions);
                additions.Clear ();
            } else {
                for (int j = 0; j < keys.Count; j ++)
                    keys[j] = c + keys[j];
            }
        }
        
        foreach (string key in keys) {
            if (this.ContainsKey (key)) {
                string error = "Attempted to add repeated key: " + key + ", pattern: " + pattern;
                error += ", existing index: " + this[key] + ", new index: " + index;

                Debug.Log (error);
            } else
                this.Add (key, index);
        }
    }

    public void PrintMissingKeys () {
        for (int i = 0; i < 11111111; i ++) {
            string key = i.ToString ("D8");

            for (int j = 7; j >= 0; j --) {
                char c = key[j];

                if (c != '0' && c != '1') {
                    int aux = int.Parse (key);
                    aux += (int) (10 - char.GetNumericValue (c)) * ((int) Mathf.Pow (10, 7 - j));
                    key = aux.ToString ("D8");
                }
            }

            i = int.Parse (key);

            if (key[0] == '0' && key[4] == '0')
                continue;

            if (key[1] == '0' && key[2] == '1' && key[3] == '0')
                continue;

            if (key[5] == '0' && key[6] == '1' && key[7] == '0')
                continue;

            if (! this.ContainsKey (key))
                Debug.Log ("Missing key: " + key);
        }
    }
}

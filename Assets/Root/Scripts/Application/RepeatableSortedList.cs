using System.Collections.Generic;
using System.Text;

public class RepeatableSortedList<T> {

    private readonly List<KeyValuePair<float, T>> contents;
    private readonly List<T> values;

    public int Count {
        get {
            return this.contents.Count;
        }
    }

    public KeyValuePair<float, T> this[int index] {
        get {
            return this.contents[index];
        }
    }

    public RepeatableSortedList(int capacity = 0) {
        this.contents = new List<KeyValuePair<float, T>>(capacity);
        this.values = new List<T>(capacity);
    }

    public void Add(float key, T value) {
        KeyValuePair<float, T> pair = new KeyValuePair<float, T>(key, value);

        for (int i = 0; i < this.Count; i++) {
            KeyValuePair<float, T> nextPair = this.contents[i];

            if (nextPair.Key > pair.Key) {
                this.contents.Insert(i, pair);
                this.values.Add(value);
                return;
            }
        }

        this.contents.Add(pair);
        this.values.Add(value);
    }

    public void Clear() {
        this.contents.Clear();
        this.values.Clear();
    }

    public bool Contains(T value) {
        return this.values.Contains(value);
    }

    public int IndexOf(T value) {
        for (int i = 0; i < this.Count; i++) {
            KeyValuePair<float, T> pair = this.contents[i];

            if (pair.Value.Equals(value)) {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, float key, T value) {
        KeyValuePair<float, T> pair = new KeyValuePair<float, T>(key, value);

        this.contents.Insert(index, pair);
        this.values.Add(value);
    }

    public void Remove(T value) {
        this.values.Remove(value);

        for (int i = 0; i < this.Count; i++) {
            KeyValuePair<float, T> pair = this.contents[i];

            if (pair.Value.Equals(value)) {
                this.contents.RemoveAt(i);
            }
        }
    }

    public void RemoveAt(int index) {
        KeyValuePair<float, T> pair = this.contents[index];

        this.values.Remove(pair.Value);
        this.contents.RemoveAt(index);
    }

    public override string ToString() {
        StringBuilder output = new StringBuilder();

        foreach (KeyValuePair<float, T> pair in this.contents) {
            output.Append(pair.Key);
            output.Append(pair.Value);
        }

        return output.ToString();
    }
}

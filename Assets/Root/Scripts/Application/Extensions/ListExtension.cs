using System.Collections.Generic;

public static class ListExtension {

    public static void AddExclusive<T>(this List<T> list, T element) {
        if (!list.Contains(element)) {
            list.Add(element);
        }
    }

    public static void AddExclusiveRange<T>(this List<T> list, IEnumerable<T> elements) {
        foreach (T element in elements) {
            list.AddExclusive(element);
        }
    }

    public static void InsertExclusive<T>(this List<T> list, int index, T element) {
        if (!list.Contains(element)) {
            list.Insert(index, element);
        }
    }

    public static T GetRandom<T>(this List<T> list, bool remove = false) {
        int count = list.Count;

        if (count == 0) {
            return default(T);
        }

        int index = UnityEngine.Random.Range(0, count);
        T element = list[index];

        if (remove) {
            list.RemoveAt(index);
        }

        return element;
    }
}

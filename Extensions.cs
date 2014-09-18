using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Collections;

public static class Extensions
{
    public static int IncrementAndWrap(this IList t, int currentIndex)
    {
        int newIndex = currentIndex + 1;
        return (newIndex >= t.Count) ? 0 : newIndex;
    }

    public static int DecrementAndWrap(this IList t, int currentIndex)
    {
        int newIndex = currentIndex - 1;
        return (newIndex < 0) ? t.Count - 1 : newIndex;
    }

    public static T RandomInRange<T>(this IList<T> t)
    {
        if(t.Count == 0) {
            Debug.LogError("Range is zero!");
            return default(T);
        }
        return t[Random.Range(0, t.Count)];
    }

    // Handy utility function for iteration with index, from
    // http://stackoverflow.com/questions/521687/c-sharp-foreach-with-index
    public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
    {
        int i = 0;
        foreach(var e in ie) {
            action(e, i++);
        }
    }

    public static void Each<T>(this IEnumerable<T> ie, Action<T> action)
    {
        foreach(var e in ie) {
            action(e);
        }
    }


    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if(val.CompareTo(max) > 0) {
            return max;
        }
        if(val.CompareTo(min) < 0) {
            return min;
        }
        return val;
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Linq;

public static partial class EnumerableExtensions
{
    //From https://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        T[] elements = source.ToArray();
        // Note i > 0 to avoid final pointless iteration
        for(int i = elements.Length - 1; i > 0; i--) {
            // Swap element "i" with a random earlier element it (or itself)
            int swapIndex = Random.Range(0, i + 1);
            yield return elements[swapIndex];
            elements[swapIndex] = elements[i];
            // we don't actually perform the swap, we can forget about the
            // swapped element because we already returned it.
        }

        // there is one item remaining that was not returned - we return it now
        yield return elements[0];
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace COILib.Extensions;

public static class ArrayExtension {

    public static T[] UnShift<T>(this T[] array, params T[] toPush) {
        if (toPush == null || toPush.Length == 0) {
            return array;
        }

        int originalLength = array.Length;
        Array.Resize(ref array, originalLength + toPush.Length);
        Array.Copy(array, 0, array, toPush.Length, originalLength);
        Array.Copy(toPush, 0, array, 0, toPush.Length);
        return array;
    }

    public static T[] Push<T>(this T[] array, params T[] toPush) {
        if (toPush == null || toPush.Length == 0) {
            return array;
        }

        int originalLength = array.Length;
        Array.Resize(ref array, originalLength + toPush.Length);
        Array.Copy(toPush, 0, array, originalLength, toPush.Length);
        return array;
    }

    public static T[] PushUnique<T>(this T[] array, T toPush) {
        if (array.Contains(toPush)) {
            return array;
        }

        Array.Resize(ref array, array.Length + 1);
        array[array.Length - 1] = toPush;
        return array;
    }

    public static T[] Remove<T>(this T[] array, T itemToRemove) {
        int index = Array.IndexOf(array, itemToRemove);
        if (index < 0) {
            return array;
        }

        T[] newArray = new T[array.Length - 1];
        if (index > 0) {
            Array.Copy(array, 0, newArray, 0, index);
        }

        if (index < array.Length - 1) {
            Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
        }

        return newArray;
    }

    public static string ToPrint<T>(this IEnumerable<T> array) {
        return $"[{string.Join(", ", array)}]";
    }
}
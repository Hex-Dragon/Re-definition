using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Module {

    public static System.Random randomDefault = new();
    /// <summary>
    /// 将 List 随机打乱。
    /// </summary>
    public static List<T> Shuffle<T>(this List<T> list, System.Random random = null) {
        random ??= randomDefault;
        for (int i = 0; i < list.Count; i++) {
            int randomIndex = random.Next(list.Count);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
        return list;
    }
    /// <summary>
    /// 随机返回数组中的一个元素。
    /// </summary>
    public static T RandomOne<T>(this List<T> list, System.Random random = null) {
        random ??= randomDefault;
        return list[random.Next(list.Count)];
    }
    /// <summary>
    /// 可自我返回的 Sort 扩展方法。
    /// </summary>
    public static List<T> SortAndReturn<T>(this List<T> list) {
        list.Sort();
        return list;
    }

}
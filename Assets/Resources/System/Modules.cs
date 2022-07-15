using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Module {

    /// <summary>
    /// 将 List 随机打乱。
    /// </summary>
    public static List<T> Shuffle<T>(this List<T> list, Random random = null) {
        random ??= new Random();
        for (int i = 0; i < list.Count; i++) {
            int randomIndex = random.Next(list.Count);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
        return list;
    }
    /// <summary>
    /// 可自我返回的 Sort 扩展方法。
    /// </summary>
    public static List<T> SortAndReturn<T>(this List<T> list) {
        list.Sort();
        return list;
    }

}
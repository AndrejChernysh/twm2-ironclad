using System;
using System.Collections.Generic;
using System.Text;

namespace Ironclad.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> elements)
        {
            return new HashSet<T>(elements);
        }
        public static void Times(this int count, Action action)
        {
            for (int i = 0; i < count; i++)
            {
                action();
            }
        }
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                && Comparer<T>.Default.Compare(item, end) <= 0;
        }
        public static int PercentageOf(this int myInt, int total)
        {
            var myIntC = Convert.ToDouble(myInt);
            var totalC = Convert.ToDouble(total);
            return Convert.ToInt32(myIntC / totalC * 100.0);
        }
        public static IEnumerable<int> To(this int from, int to)
        {
            if (from < to)
            {
                while (from <= to)
                {
                    yield return from++;
                }
            }
            else
            {
                while (from >= to)
                {
                    yield return from--;
                }
            }
        }
    }
}

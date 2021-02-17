using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voicipher.Business.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IList<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<Func<Task<T>>> WhenTaskDone<T>(this IEnumerable<Func<Task<T>>> list, Action action)
        {
            return list.Select<Func<Task<T>>, Func<Task<T>>>(x => async () =>
            {
                var result = await x().ConfigureAwait(false);
                action();

                return result;
            });
        }
    }
}

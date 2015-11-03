using System.Collections.Generic;

namespace Binarysharp.MemoryManagement.Extensions
{
    public static class ListExtensions
    {
        #region Methods

        public static int IndexOf<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
        {
            var index = -1;

            foreach (var item in list)
            {
                index++;
                if (comparer.Equals(item, value))
                {
                    return index;
                }
            }

            return -1;
        }

        #endregion
    }
}
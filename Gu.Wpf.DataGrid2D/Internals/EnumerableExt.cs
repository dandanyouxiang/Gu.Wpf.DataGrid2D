﻿namespace Gu.Wpf.DataGrid2D
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EnumerableExt
    {
        internal static Type GetElementType(this IEnumerable enumerable)
        {
            var type = enumerable.GetType();
            if (type.HasElementType)
            {
                return type.GetElementType();
            }

            if (type.IsEnumerableOfT())
            {
                return type.GetEnumerableItemType();
            }

            return typeof(object);
        }

        internal static bool IsReadOnly(this IEnumerable<IEnumerable> source)
        {
            if (source.All(x => x is IList))
            {
                return false;
            }

            return source.Any(x => x.GetElementType().IsPrimitive);
        }

        internal static void SetElementAt(this IEnumerable source, int index, object value)
        {
            var list = source as IList;
            if (list != null)
            {
                list[index] = value;
                return;
            }

            var message = $"Only sources implementing {typeof(IList)} are supported for editing.";
            throw new NotSupportedException(message);
        }

        internal static int IndexOf(this IEnumerable source, object item)
        {
            var list = source as IList;
            if (list != null)
            {
                return list.IndexOf(item);
            }

            int index = 0;
            foreach (var element in source)
            {
                if (Equals(element, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        internal static T ElementAtOrDefault<T>(this IEnumerable source, int index)
        {
            return (T)(source.ElementAtOrDefault(index) ?? default(T));
        }

        internal static object ElementAtOrDefault(this IEnumerable source, int index)
        {
            if (source == null)
            {
                return null;
            }

            var list = source as IList;
            if (list != null)
            {
                if (index >= list.Count)
                {
                    return null;
                }

                return list[index];
            }

            var readOnlyList = source as IReadOnlyList<object>;
            if (readOnlyList != null)
            {
                if (index >= readOnlyList.Count)
                {
                    return null;
                }

                return readOnlyList[index];
            }

            var count = 0;
            foreach (var item in source)
            {
                if (count == index)
                {
                    return item;
                }

                count++;
            }

            return null;
        }
    }
}

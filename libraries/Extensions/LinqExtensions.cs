using System;
using System.Collections.Generic;
using System.Linq;

namespace nnDev.Components.Common
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> GetChildsRecurse<T>(
            this T source,
            Func<T, IEnumerable<T>> getChildsFunc)
        {
            return source.GetChildsRecurse(getChildsFunc, null);
        }

        public static IEnumerable<T> GetChildsRecurse<T>(
            this T source,
            Func<T, IEnumerable<T>> getChildsFunc, Func<T, bool> ignoreElement)
        {
            if (source == null)
                yield break;

            IEnumerable<T> enumerable = getChildsFunc(source);
            if (enumerable == null)
                yield break;

            foreach (var child in enumerable)
            {
                if (ignoreElement == null || !ignoreElement(child))
                    yield return child;

                IEnumerable<T> childs = child.GetChildsRecurse(getChildsFunc, ignoreElement);
                foreach (var childsRecurse in childs)
                {
                    if (ignoreElement == null || !ignoreElement(childsRecurse))
                        yield return childsRecurse;
                }
            }
        }

		public static IEnumerable<T> GetParentRecurse<T>(
			this T source,
			Func<T, T> getParentFunc) where T : class
		{
			if (source == null)
				yield break;

			T parent = getParentFunc(source);
			if (parent == null)
				yield break;

			yield return parent;

			IEnumerable<T> prevParents = parent.GetParentRecurse(getParentFunc);
			foreach (var prevParent in prevParents)
				yield return prevParent;
		}

        public static IEnumerable<T> Intersect<T>(this IEnumerable<IEnumerable<T>> enumeration)
        {
            // Check to see that enumeration is not null
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            IEnumerable<T> returnValue = null;

            foreach (var e in enumeration)
            {
                if (e == null) 
                    continue;

                if (returnValue == null) 
                    returnValue = e;
                else
                    returnValue = returnValue.Intersect(e);
            }

            return returnValue ?? new List<T>();
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<IEnumerable<T>> enumeration)
        {
            // Check to see that enumeration is not null
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            IEnumerable<T> returnValue = null;

            foreach (var e in enumeration)
            {
                if (returnValue == null)
                    returnValue = e;
                else
                    returnValue = returnValue.Union(e);
            }

            return returnValue ?? new List<T>();
        }

        public static IEnumerable<T> StartFrom<T>(this IEnumerable<T> enumeration, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            if (startIndex == 0)
                return enumeration;

            return enumeration.Skip(startIndex).Union(enumeration.Take(startIndex));
        }

        public static IEnumerable<T> StartFrom<T>(this IEnumerable<T> enumeration, T item)
        {
            int index = enumeration.IndexOf(item);
            if (index < 0)
                throw new ArgumentOutOfRangeException("item");

            return enumeration.StartFrom(index);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            return source.IndexOf(item, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            var list = source as IList<T>;
            if (list != null)
                return list.IndexOf(item);

            int i = 0;
            foreach (T x in source)
            {
                if (comparer.Equals(x, item))
                    return i;
                i++;
            }
            return -1;
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static bool Equals<T>(
            this T[] source,
            T[] another, Func<T, T, bool> compare)
        {
            if (source == another)
                return true;
            if (source == null || another == null)
                return false;
            if (source.Length != another.Length)
                return false;

            return !source
                .Where((t, i) => !compare(t, another[i]))
                .Any();
        }
    }
}
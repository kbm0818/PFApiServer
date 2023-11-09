using System;
using System.Runtime.CompilerServices;

namespace TableTester.Example.Tables
{
    public interface ITRange<T> : IComparable where T : IComparable
    {
    }

    public class TRange<T> : ITRange<T> where T : IComparable
    {
        public const int Length = 2;
        private readonly T[] values = new T[Length];

        public T this[int index] => values[index];

        public int CompareTo(object? obj)
        {
            if (obj is not T tobj) return 1;
            if (this[0].CompareTo(tobj) < 0) return -1;
            if (this[1].CompareTo(tobj) > 1) return 1;
            return 0;
        }
    }
}

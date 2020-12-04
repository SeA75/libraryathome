using System;
using System.Collections.Generic;

namespace BooksParser
{
    public static class ArrayExtension
    {
        public static T[] Resize<T>(this T[] Source, int NewSize)
        {
            Array.Resize(ref Source, Source.Length + 1);
            return Source;
        }
        public static T[] Append<T>(this T[] Source, T NewElement)
        {
            Array.Resize(ref Source, Source.Length + 1);
            Source[Source.Length - 1] = NewElement;
            return Source;
        }
        public static T[] Remove<T>(this T[] source, T value)
        {
            int arrayLength = source.Length - 1;
            T[] resizedArray = new T[arrayLength];

            for (int i = 0, y = 0; i < source.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(source[i], value))
                {
                    resizedArray[y] = source[i];
                    y++;
                }
            }

            return resizedArray;
        }
    }
}



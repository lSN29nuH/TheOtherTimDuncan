﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TOTD.Utility.EnumerableHelpers
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether a sequence is null or contains no elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>
        /// True if the source sequence is null or contains no elements; otherwise false
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Returns the specified number of elements starting at the beginning of the specified page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="page">The 1-based number of the page to skip to</param>
        /// <param name="pageSize">The number of elements in a page</param>
        /// <returns></returns>
        public static IEnumerable<T> TakePage<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns>Returns true if the source sequence contains any elements; returns false if otherwise or if the source is null</returns>
        public static bool NullSafeAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                return false;
            }

            return source.Any(predicate);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns>An IEnumerable{T} that contains elements from the input sequence that satisfy the condition; returns an empty IEnumerable{T} if the source is null</returns>
        public static IEnumerable<T> NullSafeWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            return source.Where(predicate);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns>An IEnumerable{TResult} whose elements are the result of invoking the transform fucntion on each element of {TSource}; returns an empty IEnumerable{TResult} if the source is null</returns>
        public static IEnumerable<TResult> NullSafeSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                return Enumerable.Empty<TResult>();
            }

            return source.Select(selector);
        }

        /// <summary>
        /// Returns the number of elements in a sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>The number of elements in the input sequence; returns 0 if the source is null</returns>
        public static int NullSafeCount<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return 0;
            }

            return source.Count();
        }

        public static void BatchForEach<TSource>(this IEnumerable<TSource> source, int batchSize, Action<IEnumerable<TSource>> action)
        {
            if(source.IsNullOrEmpty())
            {
                return;
            }

            int skip = 0;
            IEnumerable<TSource> batch = source.Skip(skip).Take(batchSize);
            while (batch.Any())
            {
                action(batch);
                skip += batchSize;
                batch = source.Skip(skip).Take(batchSize);
            }
        }
    }
}

//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Universe.CQRS.Models.Sort;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Dal.Base.Extensions
{
    /// <summary>
    ///     Расширения сортировки
    /// <author>Alex Envision</author>
    /// </summary>
    public static class OrderExtensions
    {
        /// <summary>
        /// The order by abstract magic.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="sorting">
        /// The sorting.
        /// </param>
        /// <param name="fillMappingDictionaryAction">
        /// The fill mapping dictionary action.
        /// </param>
        /// <param name="setDefaultSortFunc">
        /// The set default sort func.
        /// </param>
        /// <typeparam name="TSource">
        /// TSource.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static IQueryable<TSource> OrderByAbstractMagic<TSource>(
            this IQueryable<TSource> source,
            IEnumerable<ISortDescriptor> sorting,
            Action<Dictionary<string, Expression<Func<TSource, object>>>> fillMappingDictionaryAction,
            Func<IQueryable<TSource>, IQueryable<TSource>> setDefaultSortFunc)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (fillMappingDictionaryAction == null)
                throw new ArgumentNullException("fillMappingDictionaryAction");

            var mappingDictionary = new Dictionary<string, Expression<Func<TSource, object>>>();
            fillMappingDictionaryAction(mappingDictionary);

            return source.OrderByAbstractMagic(sorting, mappingDictionary, setDefaultSortFunc);
        }

        /// <summary>
        /// The order by abstract magic.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="sorting">
        /// The sorting.
        /// </param>
        /// <param name="mappingDictionary">
        /// The mapping dictionary.
        /// </param>
        /// <param name="setDefaultSortFunc">
        /// The set default sort func.
        /// </param>
        /// <typeparam name="TSource">
        /// TSource.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static IQueryable<TSource> OrderByAbstractMagic<TSource>(
            this IQueryable<TSource> source,
            IEnumerable<ISortDescriptor> sorting,
            Dictionary<string, Expression<Func<TSource, object>>> mappingDictionary,
            Func<IQueryable<TSource>, IQueryable<TSource>> setDefaultSortFunc)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var orderByMagic = false;
            if (sorting != null)
            {
                var flag = true;
                foreach (var sortDescriptor in sorting)
                {
                    flag = false;
                    var asc = string.Compare(sortDescriptor.SortType, "Asc", true, CultureInfo.CurrentCulture) == 0;

                    if (!mappingDictionary.ContainsKey(sortDescriptor.FieldName))
                        throw new NotSupportedException(
                            string.Format(CultureInfo.CurrentCulture, "Поле {0} для сортировки не поддерживается.", sortDescriptor.FieldName));

                    var exp = mappingDictionary[sortDescriptor.FieldName];
                    source = source.OrderByMagic(exp, asc, ref orderByMagic);
                }

                if (flag && setDefaultSortFunc != null)
                    source = setDefaultSortFunc(source);
            }
            else
            {
                if (setDefaultSortFunc != null)
                    source = setDefaultSortFunc(source);
            }

            return source;
        }

        /// <summary>
        /// The order by abstract magic.
        /// </summary>
        /// <param name="sorting">
        /// The sorting.
        /// </param>
        /// <param name="mappingDictionary">
        /// The mapping dictionary.
        /// </param>
        /// <param name="defaultSort">
        /// The default sort.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static string OrderByAbstractMagic(
            IEnumerable<ISortDescriptor> sorting,
            Dictionary<string, string> mappingDictionary,
            string defaultSort)
        {
            if (sorting != null)
            {
                var sb = new StringBuilder();
                foreach (var sortDescriptor in sorting)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    if (!mappingDictionary.ContainsKey(sortDescriptor.FieldName))
                        throw new NotSupportedException(
                            "Поле {0} для сортировки не поддерживается.".F(sortDescriptor.FieldName));

                    var exp = mappingDictionary[sortDescriptor.FieldName];

                    sb.Append(exp);
                    sb.Append(string.Compare(sortDescriptor.SortType, "Asc", true, CultureInfo.CurrentCulture) == 0 ? " asc" : " desc");
                }

                if (sb.Length == 0)
                    return defaultSort;

                return sb.ToString();
            }

            return defaultSort;
        }

        /// <summary>
        /// The order by abstract magic.
        /// </summary>
        /// <param name="sorting">
        /// The sorting.
        /// </param>
        /// <param name="mappingDictionary">
        /// The mapping dictionary.
        /// </param>
        /// <param name="defaultSort">
        /// The default sort.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string OrderByAbstractMagic(
            IEnumerable<SortDescriptor> sorting,
            Dictionary<string, string> mappingDictionary,
            string defaultSort)
        {
            return OrderByAbstractMagic(sorting != null ? sorting.Select(_ => (ISortDescriptor)_) : null, mappingDictionary, defaultSort);
        }

        /// <summary>
        /// The order by magic.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <param name="asc">
        /// The asc.
        /// </param>
        /// <param name="orderByMagic">
        /// The order By Magic.
        /// </param>
        /// <typeparam name="TSource">
        /// TSource.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IQueryable<TSource> OrderByMagic<TSource, TKey>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool asc,
            ref bool orderByMagic)
        {
            var k = keySelector as Expression<Func<TSource, object>>;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (k != null || typeof(TKey) == typeof(object))
            {
                var pt = k.GetPropertyType();
                if (pt == typeof(DateTimeOffset))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, DateTimeOffset>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(DateTimeOffset?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, DateTimeOffset?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(short))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, short>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(short?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, short?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(int))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, int>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(int?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, int?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(long))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, long>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(long?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, long?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(decimal))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, decimal>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(decimal?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, decimal?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(double))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, double>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(double?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, double?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(bool))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, bool>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(bool?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, bool?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(DateTime))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, DateTime>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(DateTime?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, DateTime?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(Guid))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, Guid>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(Guid?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, Guid?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(float))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, float>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                if (pt == typeof(float?))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, float?>>(k.GetMemberExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                // Добавлена поддержка кастомных экспрессий - k.GetCustomExpression()
                if (pt == typeof(string))
                    return source.OrderByMagicInternal(
                        Expression.Lambda<Func<TSource, string>>(k.GetMemberExpression() ?? k.GetCustomExpression(), keySelector.Parameters.Single()),
                        asc,
                        ref orderByMagic);

                throw new NotSupportedException("В сортировке нет типизации для типа {0}".F(pt.FullName));
            }

            return source.OrderByMagicInternal(keySelector, asc, ref orderByMagic);
        }

        /// <summary>
        /// The order by magic 1.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <param name="asc">
        /// The asc.
        /// </param>
        /// <param name="orderByMagic">
        /// The order by magic.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public static IQueryable<TSource> OrderByMagicInternal<TSource, TKey>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool asc,
            ref bool orderByMagic)
        {
            var orderedQueryable = source as IOrderedQueryable<TSource>;
            if (orderedQueryable != null && orderByMagic)
            {
                if (asc)
                    return orderedQueryable.ThenBy(keySelector);

                return orderedQueryable.ThenByDescending(keySelector);
            }

            orderByMagic = true;

            if (asc)
                return source.OrderBy(keySelector);

            return source.OrderByDescending(keySelector);
        }
    }
}
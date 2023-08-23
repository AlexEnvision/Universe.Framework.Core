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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Universe.CQRS.Dal.Base.FilterBuilders;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Filter;

namespace Universe.CQRS.Dal.Base.Extensions
{
    /// <summary>
    /// The queryable meta info extensions.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class QueryableMetaInfoExtensions
    {
        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="fieldTypeEnum">
        /// The field type enum.
        /// </param>
        /// <param name="sortable">
        /// The sortable.
        /// </param>
        /// <param name="filterable">
        /// The filterable.
        /// </param>
        /// <param name="filterTitle">
        /// The filter title.
        /// </param>
        /// <param name="isFilterHierarchy">
        /// The is filter hierarchy.
        /// </param>
        /// <param name="filterValues">
        /// The filter values.
        /// </param>
        /// <param name="alwaysSelect">
        /// The always select.
        /// </param>
        /// <param name="canBeVisible">
        /// The can be visible.
        /// </param>
        /// <param name="visibleDefault">
        /// The visible default.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="selectorForExtent"></param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryableMetaInfo{TSource}"/>.
        /// </returns>
        public static QueryableMetaInfo<TSource> AddField<TSource>(
            this QueryableMetaInfo<TSource> source,
            string fieldName,
            Expression<Func<TSource, object>> selector,
            string title,
            FieldTypes? fieldTypeEnum = null,
            bool sortable = true,
            bool filterable = true,
            string filterTitle = null,
            bool isFilterHierarchy = false,
            List<FilterValue> filterValues = null,
            bool alwaysSelect = false,
            bool canBeVisible = true,
            bool visibleDefault = false,
            string template = null,
            Expression<Func<TSource, object>> selectorForExtent = null)
        {
            var fieldMetaInfo = new QueryableFieldMetaInfo<TSource> {
                Name = fieldName,
                DbFieldSelector = selector,
                DbFieldSelectorForExtent = selectorForExtent,
                Title = title,
                FieldTypeEnum = fieldTypeEnum,
                Sortable = sortable,
                Filterable = filterable,
                FilterTitle = filterTitle,
                IsFilterHierarchy = isFilterHierarchy,
                FilterValues = filterValues,
                AlwaysSelect = alwaysSelect || visibleDefault,
                CanBeVisible = canBeVisible,
                VisibleDefault = visibleDefault,
                Template = template
            };

            source.FieldsMetaInfo.Add(fieldMetaInfo);
            return source;
        }

        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// oppo
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="fieldTypeEnum">
        /// The field type enum.
        /// </param>
        /// <param name="sortable">
        /// The sortable.
        /// </param>
        /// <param name="filterable">
        /// The filterable.
        /// </param>
        /// <param name="filterTitle">
        /// The filter title.
        /// </param>
        /// <param name="isFilterHierarchy">
        /// The is filter hierarchy.
        /// </param>
        /// <param name="filterValues">
        /// The filter values.
        /// </param>
        /// <param name="alwaysSelect">
        /// The always select.
        /// </param>
        /// <param name="canBeVisible">
        /// The can be visible.
        /// </param>
        /// <param name="visibleDefault">
        /// The visible default.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TDto">
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryableMetaInfo{TSource}"/>.
        /// </returns>
        public static QueryableMetaInfo<TSource> AddField<TSource, TDto>(
            this QueryableMetaInfo<TSource> source,
            Expression<Func<TDto, object>> fieldName,
            Expression<Func<TSource, object>> selector,
            string title,
            FieldTypes? fieldTypeEnum = null,
            bool sortable = true,
            bool filterable = true,
            string filterTitle = null,
            bool isFilterHierarchy = false,
            List<FilterValue> filterValues = null,
            bool alwaysSelect = false,
            bool canBeVisible = true,
            bool visibleDefault = false,
            string template = null)
        {
            return source.AddField(
                fieldName.GetPropertyNamePath(),
                selector,
                title,
                fieldTypeEnum,
                sortable,
                filterable,
                filterTitle,
                isFilterHierarchy,
                filterValues,
                alwaysSelect,
                canBeVisible,
                visibleDefault,
                template);
        }

        /// <summary>
        /// The create queryable meta info.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <typeparam name="TSource">
        /// TSource.
        /// </typeparam>
        /// <typeparam name="TDto">
        /// TDto.
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryableMetaInfo{TSource}"/>.
        /// </returns>
        public static QueryableMetaInfo<TSource> CreateQueryableMetaInfo<TSource, TDto>(
            this IQueryable<TSource> source,
            TDto dto,
            string entityName)
        {
            return new QueryableMetaInfo<TSource>(entityName);
        }

        /// <summary>
        /// The get property name path.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static string GetPropertyNamePath<T>(this Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var expression = selector.Body;

            var sb = new StringBuilder();

            while (expression != null)
            {
                var memberExpr = expression as MemberExpression;
                if (memberExpr == null)
                    if (expression is UnaryExpression unaryExpression)
                    {
                        expression = unaryExpression.Operand;
                        continue;
                    }

                if (memberExpr == null)
                    break;

                if (sb.Length == 0)
                    sb.Append(memberExpr.Member.Name);
                else
                    sb.Insert(0, memberExpr.Member.Name + ".");

                expression = memberExpr.Expression;
            }

            return sb.ToString();
        }

        /// <summary>
        /// The where by filter.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="metaInfo">
        /// The meta info.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public static IQueryable<TSource> WhereByFilter<TSource>(
            this IQueryable<TSource> source,
            QueryableMetaInfo<TSource> metaInfo,
            SearchFilterBase filter)
        {
            if (filter?.Rules == null)
                return source;

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (metaInfo == null)
                throw new ArgumentNullException(nameof(metaInfo));

            if (filter == null)
                return source;

            var sqlWhereBuilder = new QueryableWhereBuilder<TSource>(metaInfo, source, filter) {
                ThrowExceptionIfFilterEmpty = false
            };
            return sqlWhereBuilder.Build();
        }
    }
}
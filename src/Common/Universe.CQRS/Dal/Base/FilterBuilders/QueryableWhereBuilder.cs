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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Filter;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Dal.Base.FilterBuilders
{
    /// <summary>
    /// The sql where builder.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    public class QueryableWhereBuilder<TSource> : SqlWhereBuilderBase
    {
        /// <summary>
        /// The _sql template meta info.
        /// </summary>
        private readonly QueryableMetaInfo<TSource> _metaInfo;

        /// <summary>
        /// The _source.
        /// </summary>
        private readonly IQueryable<TSource> _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryableWhereBuilder{TSource}"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta info.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public QueryableWhereBuilder(QueryableMetaInfo<TSource> metaInfo, IQueryable<TSource> source, SearchFilterBase filters)
            : base(metaInfo, filters)
        {
            _metaInfo = metaInfo;
            _source = source ?? throw new ArgumentNullException(nameof(source));
            Filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<TSource> Build()
        {
            try
            {
                var predicate = BuildSqlBaseFilter(Filters);
                if (predicate != null)
                    return _source.Where(predicate);

                return _source;
            }
            catch (Exception ex)
            {
                throw new Exception("Составленный фильтр не корректен, необходимо исправить условие: {0}".F(ex.Message), ex);
            }
        }

        /// <summary>
        /// The build sql and.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<TSource, bool>> BuildSqlAnd(SearchFilterAnd filters)
        {
            return BuildSqlOperand(filters, Expression.And);
        }

        /// <summary>
        /// The build sql base filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<TSource, bool>> BuildSqlBaseFilter(SearchFilterBase filter)
        {
            if (filter is SearchFilterOr filterOr)
                return BuildSqlOr(filterOr);

            if (filter is SearchFilterAnd filterAnd)
                return BuildSqlAnd(filterAnd);

            throw new NotSupportedException(
                $@"Тип фильтра ""{filter.GetType().FullName}"" не поддерживается.");
        }

        /// <summary>
        /// The build sql base filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<TSource, bool>> BuildSqlBaseFilter(object filter)
        {
            if (filter is SearchFilterRule filterRule)
                return BuildSqlFieldFilter(filterRule);

            throw new NotSupportedException(
                $@"Тип фильтра ""{filter.GetType().FullName}"" не поддерживается.");
        }

        /// <summary>
        /// The build sql field filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected virtual Expression<Func<TSource, bool>> BuildSqlFieldFilter(SearchFilterRule filter)
        {
            if (filter.FieldName.IsNullOrEmpty())
                return null;

            if (filter.FilterTypeName.IsNullOrEmpty())
                return null;

            var filterFieldName = filter.FieldName;
            var fieldMetaInfo = (QueryableFieldMetaInfo<TSource>)_metaInfo.FieldsMetaInfo.FirstOrDefault(_ => _.Name == filterFieldName);
            if (fieldMetaInfo == null)
                throw new Exception(@"Поле фильтрации с именем ""{0}"" не найдено.".F(filterFieldName));

            return BuildSqlFieldFilterInt(filter, fieldMetaInfo);
        }

        /// <summary>
        /// The build sql field filter int.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="fieldMetaInfo">
        /// The field meta info.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected virtual Expression<Func<TSource, bool>> BuildSqlFieldFilterInt(
            SearchFilterRule filter,
            QueryableFieldMetaInfo<TSource> fieldMetaInfo)
        {
            if (filter.FilterType == FieldFilterTypes.IsNull)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.Equal(
                        fieldMetaInfo.DbFieldSelector.Body,
                        Expression.Constant(null, typeof(object))),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            if (filter.FilterType == FieldFilterTypes.IsNotNull)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.NotEqual(
                        fieldMetaInfo.DbFieldSelector.Body,
                        Expression.Constant(null, typeof(object))),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            var fieldType = fieldMetaInfo.DbFieldSelector.GetPropertyType();
            Debug.Assert(fieldType != null, "fieldType != null");

            if (filter.FilterType == FieldFilterTypes.Between)
            {
                var value12 = GetValuesForBetween(filter, fieldMetaInfo, fieldType);

                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.And(
                        Expression.GreaterThanOrEqual(
                            fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                            Expression.Constant(value12[0], fieldType)),
                        Expression.LessThanOrEqual(
                            fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                            Expression.Constant(value12[1], fieldType))),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());
            }

            if (filter.FilterType == FieldFilterTypes.In || filter.FilterType == FieldFilterTypes.NotIn)
            {
                var valueArr = GetValuesFromFilter(fieldType, filter);
                if (valueArr.Count == 0)
                {
                    var message =
                        $@"Для поля ""{fieldMetaInfo.FilterTitle}"" и оператора ""{
                                _metaInfo.GetFilterTypeTitle(filter)
                            }"" должно быть задано не менее одного значения фильтрации.";
                    throw new Exception(message);
                }

                for (var index = 0; index < valueArr.Count; index++)
                {
                    var iValue = valueArr[index];
                    var value = iValue;
                    if (value.GetType() != fieldType)
                        try
                        {
                            valueArr[index] = ChangeType(fieldType, value);
                        }
                        catch (Exception ex)
                        {
                            var message =
                                $@"Для поля ""{fieldMetaInfo.FilterTitle}"" и оператора ""{_metaInfo.GetFilterTypeTitle(filter)}"" тип {
                                        index
                                    }-го значение фильтра ""{(value != null ? value.GetType().FullName : "null")}"" несовпадает с типом поля ""{
                                        fieldType.FullName
                                    }"". И не поддается приведению: {ex.Message}";
                            throw new Exception(message, ex);
                        }
                }

                var p = fieldMetaInfo.DbFieldSelector.Parameters.Single();

                if (!valueArr.Any())
                    return e => false;

                if (filter.FilterType == FieldFilterTypes.In)
                {
                    var equals = valueArr.Select(
                        value =>
                            (Expression)Expression.Equal(
                                fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                                Expression.Constant(value, fieldType)));
                    var body = equals.Aggregate(Expression.Or);

                    return Expression.Lambda<Func<TSource, bool>>(body, p);
                }

                if (filter.FilterType == FieldFilterTypes.NotIn)
                {
                    var equals = valueArr.Select(
                        value =>
                            (Expression)Expression.NotEqual(
                                fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                                Expression.Constant(value, fieldType)));
                    var body = equals.Aggregate(Expression.And);

                    return Expression.Lambda<Func<TSource, bool>>(body, p);
                }
            }

            if (filter.FilterType == FieldFilterTypes.Like
                || filter.FilterType == FieldFilterTypes.NotLike)
            {
                var value = filter.ValueSelected;

                if (fieldMetaInfo.DataType != typeof(string))
                    throw new NotSupportedException("Like и NotLike в случае ObjectQuery<T> поддерживаются только для текстовых полей.");

                Expression expression = fieldMetaInfo.DbFieldSelector.GetMemberExpression() ?? fieldMetaInfo.DbFieldSelector.GetCustomExpression();
                if (expression == null)
                    throw new NotSupportedException();

                var containsMethodExp = Expression.Call(
                    expression,
                    typeof(string).GetMethod(
                        "Contains",
                        new[] {
                            typeof(string)
                        }) ?? throw new InvalidOperationException(),
                    Expression.Constant(value, typeof(string)));

                if (filter.FilterType == FieldFilterTypes.Like)
                    return Expression.Lambda<Func<TSource, bool>>(containsMethodExp, fieldMetaInfo.DbFieldSelector.Parameters.Single());

                if (filter.FilterType == FieldFilterTypes.NotLike)
                    return Expression.Lambda<Func<TSource, bool>>(
                        Expression.Not(containsMethodExp),
                        fieldMetaInfo.DbFieldSelector.Parameters.Single());
            }

            // NULL значение игнорируется, в случае пустоты в ValueSelected, 
            // это остается на случай потери значения
            if (filter.ValueSelected == null)
            {
                var message =
                    $@"Для поля ""{fieldMetaInfo.FilterTitle}"" и оператора ""{
                            _metaInfo.GetFilterTypeTitle(filter)
                        }"" должно быть указано значение фильтра.";
                throw new Exception(message);
            }

            // Однако, если задать строкой "null", 
            // это даст возможность создавать выражения такого вида: x => x != null, x => x == null
            var nullCondition = filter.ValueSelected == "null";
            var filterValue = !nullCondition ? GetValueFromFilter(fieldType, filter) : null;
            if (filterValue?.GetType() != fieldType && !nullCondition)
                try
                {
                    filterValue = ChangeType(fieldType, filterValue);
                }
                catch (Exception ex)
                {
                    var message =
                        $@"Для поля ""{fieldMetaInfo.FilterTitle}"" и оператора ""{_metaInfo.GetFilterTypeTitle(filter)}"" тип значение фильтра ""{
                                (filterValue != null ? filterValue.GetType().FullName : "null")
                            }"" несовпадает с типом поля ""{fieldType.FullName}"". И не поддается приведению: {ex.Message}";
                    throw new Exception(message, ex);
                }

            if (filterValue is DateTime)
                throw new Exception("filter.Value is DateTime - нужно передавать DateTimeOffset");

            // На данном фильтре внедрена фильтрация по выражению
            if (filter.FilterType == FieldFilterTypes.Eq)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.Equal(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression() ?? fieldMetaInfo.DbFieldSelector.GetCustomExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            // На данном фильтре внедрена фильтрация по выражению
            if (filter.FilterType == FieldFilterTypes.Neq)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.NotEqual(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression() ?? fieldMetaInfo.DbFieldSelector.GetCustomExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            // TODO -> проверить коррекность проверки выражения если пытаемся проверить на "null"
            if (filter.FilterType == FieldFilterTypes.Le)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.LessThan(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            // TODO -> проверить коррекность проверки выражения если пытаемся проверить на "null"
            if (filter.FilterType == FieldFilterTypes.Leq)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.LessThanOrEqual(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            // TODO -> проверить коррекность проверки выражения если пытаемся проверить на "null"
            if (filter.FilterType == FieldFilterTypes.Ge)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.GreaterThan(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            // TODO -> проверить коррекность проверки выражения если пытаемся проверить на "null"
            if (filter.FilterType == FieldFilterTypes.Geq)
                return Expression.Lambda<Func<TSource, bool>>(
                    Expression.LessThanOrEqual(
                        fieldMetaInfo.DbFieldSelector.GetMemberExpression(),
                        Expression.Constant(filterValue, fieldType)),
                    fieldMetaInfo.DbFieldSelector.Parameters.Single());

            throw new NotSupportedException(@"Тип ""{0}"" условного оператора не поддерживается.".F(filter.FilterType.ToString()));
        }

        /// <summary>
        /// The build sql operand.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <param name="andOr">
        /// The and Or.
        /// </param>
        /// <exception cref="InvalidDataException">
        /// </exception>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<TSource, bool>> BuildSqlOperand(SearchFilterBase filters, Func<Expression, Expression, Expression> andOr)
        {
            var predicates = new List<Expression<Func<TSource, bool>>>();
            if (filters.Rules != null)
                foreach (var filter in filters.Rules)
                {
                    var predicate = BuildSqlFieldFilter(filter);
                    if (ThrowExceptionIfFilterEmpty && predicate == null)
                        throw new InvalidDataException(
                            @"Построении sql where: переданный фильтр типа ""{0}"" некорректен".F(
                                filter.GetType().FullName));

                    if (predicate != null)
                        predicates.Add(predicate);
                }

            if (filters.Items != null)
                foreach (var filter in filters.Items)
                {
                    var predicate = BuildSqlBaseFilter(filter);
                    if (ThrowExceptionIfFilterEmpty && predicate == null)
                        throw new InvalidDataException(
                            @"Построении sql where: переданный фильтр типа ""{0}"" некорректен".F(
                                filter.GetType().FullName));

                    if (predicate != null)
                        predicates.Add(predicate);
                }

            if (predicates.Count > 0)
                return DataAccessExtensions.AggregatePredicates(predicates, andOr);

            return null;
        }

        /// <summary>
        /// The build sql or.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected Expression<Func<TSource, bool>> BuildSqlOr(SearchFilterOr filters)
        {
            return BuildSqlOperand(filters, Expression.Or);
        }
    }
}
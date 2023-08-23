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
using System.Reflection;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Base;
using Universe.CQRS.Models.Filter;
using Universe.CQRS.Models.Sort;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class DbQueryExtensions
    {
        private static bool CanIgnore(PropertyInfo propDto) =>
            propDto.PropertyType == typeof(List<>) ||
            propDto.PropertyType == typeof(IList<>);

        /// <summary>
        /// Создает фильтрацию по-умолчанию на основе типа
        /// </summary>
        /// <typeparam name="TDbEntity"></typeparam>
        public static IQueryable<TDbEntity> OrderByReflectionMagic<TDbEntity>(
            this IQueryable<TDbEntity> query,
            List<SortDescriptor> sortDescriptors,
            QueryableMetaInfo<TDbEntity> metaInfo)
            where TDbEntity : class
        {
            return sortDescriptors?.Count > 0
                ? query.OrderByReflectionMagicInternal(sortDescriptors, metaInfo)
                : query.OrderByReflectionMagicInternal(
                    new List<SortDescriptor> {
                        new SortDescriptor {
                            FieldName = "Id",
                            SortType = "asc"
                        }
                    },
                    metaInfo);
        }

        /// <summary>
        /// Создает фильтрацию по-умолчанию на основе типа
        /// </summary>
        /// <typeparam name="TDtoEntity"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        public static IQueryable<TDbEntity> OrderByReflectionMagic<TDbEntity, TDtoEntity>(
            this IQueryable<TDbEntity> query,
            List<SortDescriptor> sortDescriptors,
            QueryableMetaInfo<TDbEntity> metaInfo)
            where TDtoEntity : IEntityDto
            where TDbEntity : class
        {
            return sortDescriptors?.Count > 0
                ? query.OrderByReflectionMagicInternal<TDbEntity, TDtoEntity>(sortDescriptors, metaInfo)
                : query.OrderByReflectionMagicInternal<TDbEntity, TDtoEntity>(
                    new List<SortDescriptor> {
                        new SortDescriptor {
                            FieldName = "Id",
                            SortType = "asc"
                        }
                    },
                    metaInfo);
        }

        /// <summary>
        /// Создает фильтрацию по-умолчанию на основе типа
        /// </summary>
        /// <typeparam name="TDbEntity"></typeparam>
        private static IQueryable<TDbEntity> OrderByReflectionMagicInternal<TDbEntity>(
            this IQueryable<TDbEntity> query,
            List<SortDescriptor> sortDescriptors,
            QueryableMetaInfo<TDbEntity> metaInfo)
            where TDbEntity : class
        {
            var sortedQuery = query.OrderByAbstractMagic(
                sortDescriptors.ToSorting(),
                (Dictionary<string, Expression<Func<TDbEntity, object>>> map) => {
                    var fieldMap = metaInfo.MappinSortDictionary;
                    foreach (var field in fieldMap)
                    {
                        map.Add(field.Key, field.Value);
                    }
                },
                (IQueryable<TDbEntity> defaultSort) => defaultSort.OrderBy(Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntity>("Name")));

            return sortedQuery;
        }

        /// <summary>
        /// Создает фильтрацию по-умолчанию на основе типа
        /// </summary>
        /// <typeparam name="TDtoEntity"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        private static IQueryable<TDbEntity> OrderByReflectionMagicInternal<TDbEntity, TDtoEntity>(
            this IQueryable<TDbEntity> query,
            List<SortDescriptor> sortDescriptors,
            QueryableMetaInfo<TDbEntity> metaInfo)
            where TDtoEntity : IEntityDto
            where TDbEntity : class
        {
            var sortedQuery = query.OrderByAbstractMagic(
                sortDescriptors.ToSorting(),
                (Dictionary<string, Expression<Func<TDbEntity, object>>> map) => {
                    var fieldMap = metaInfo.MappinSortDictionary;
                    foreach (var field in fieldMap)
                    {
                        map.Add(field.Key, field.Value);
                    }
                },
                (IQueryable<TDbEntity> defaultSort) => defaultSort.OrderBy(Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntity>("Name")));

            return sortedQuery;
        }

        /// <summary>
        /// Создает метаинформацию на основе типа
        /// </summary>
        /// <typeparam name="TDtoEntity"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        public static QueryableMetaInfo<TDbEntity> CreateMetaInfo<TDbEntity, TDtoEntity>(
            this IQueryable<TDbEntity> query,
            Dictionary<string, Expression<Func<TDbEntity, object>>> fieldMap,
            Dictionary<string, Expression<Func<TDbEntity, object>>> autoMapfields)
            where TDtoEntity : IEntityDto
            where TDbEntity : class
        {
            QueryableMetaInfo<TDbEntity> metainfo = query.CreateQueryableMetaInfo(
                Activator.CreateInstance(typeof(TDtoEntity)),
                typeof(TDbEntity).Name);

            var properties = typeof(TDtoEntity)
                .GetProperties();

            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            //Регистрация кастомной метаинформации вне зависимости от полей входящей сущности,
            //а также регистрация ключей с маленькой буквы
            if (fieldMap != null && fieldMap.Count != 0)
                foreach (var kvp in fieldMap)
                {
                    var name = kvp.Key;
                    var field = kvp.Value;
                    metainfo.AddField(name, field, name);
                    metainfo.AddField(name.FirstLetterToLower(), field, name.FirstLetterToLower());
                }

            foreach (var kvp in propertiesDtoEntity)
            {
                var propDtoTypeList = kvp.Value;
                foreach (var propDto in propDtoTypeList)
                {
                    var name = kvp.Key;

                    if (CanIgnore(propDto))
                        continue;

                    // Игнорирование базовах классов при совпадении имен
                    if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TDtoEntity))
                        continue;

                    if (fieldMap != null &&
                        fieldMap.TryGetValue(name, out var field))
                        continue;

                    if (autoMapfields.TryGetValue(name, out var autoMapField))
                    {
                        metainfo.AddField(name, autoMapField, name);
                        metainfo.AddField(name.FirstLetterToLower(), autoMapField, name.FirstLetterToLower());
                        continue;
                    }

                    var newExpression = Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntity>(name);
                    if (newExpression != null)
                    {
                        metainfo.AddField(name, newExpression, name);
                        metainfo.AddField(name.FirstLetterToLower(), newExpression, name.FirstLetterToLower());
                    }
                }
            }

            metainfo.BuildMetaInfo();
            return metainfo;
        }

        /// <summary>
        /// Создает метаинформацию на основе типа
        /// </summary>
        /// <typeparam name="TDtoEntity"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        public static QueryableMetaInfo<TDbEntity> CreateMetaInfo<TDbEntity, TDtoEntity>(
            this IQueryable<TDbEntity> query,
            IFieldMapContainer container,
            Dictionary<string, Expression<Func<TDbEntity, object>>> autoMapfields)
            where TDtoEntity : IEntityDto 
            where TDbEntity : class
        {
            QueryableMetaInfo<TDbEntity> metainfo = query.CreateQueryableMetaInfo(
                Activator.CreateInstance(typeof(TDtoEntity)),
                typeof(TDbEntity).Name);

            var properties = typeof(TDtoEntity)
                .GetProperties();

            var metaContainer = container as FieldMapContainer<TDbEntity>;
            var fieldMap = metaContainer?.FieldMap;

            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            //Регистрация кастомной метаинформации вне зависимости от полей входящей сущности,
            //а также регистрация ключей с маленькой буквы
            if (fieldMap != null && fieldMap.Count != 0)
                foreach (var kvp in fieldMap)
                {
                    var name = kvp.Key;
                    var field = kvp.Value;
                    metainfo.AddField(name, field, name);
                    metainfo.AddField(name.FirstLetterToLower(), field, name.FirstLetterToLower());
                }

            foreach (var kvp in propertiesDtoEntity)
            {
                var propDtoTypeList = kvp.Value;
                foreach (var propDto in propDtoTypeList)
                {
                    var name = kvp.Key;

                    if (CanIgnore(propDto))
                        continue;

                    // Игнорирование базовах классов при совпадении имен
                    if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TDtoEntity))
                        continue;

                    if (fieldMap != null &&
                        fieldMap.TryGetValue(name, out var field))
                        continue;

                    if (autoMapfields.TryGetValue(name, out var autoMapField))
                    {
                        metainfo.AddField(name, autoMapField, name);
                        metainfo.AddField(name.FirstLetterToLower(), autoMapField, name.FirstLetterToLower());
                        continue;
                    }

                    var newExpression = Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntity>(name);
                    if (newExpression != null)
                    {
                        metainfo.AddField(name, newExpression, name);
                        metainfo.AddField(name.FirstLetterToLower(), newExpression, name.FirstLetterToLower());
                    }
                }
            }

            metainfo.BuildMetaInfo();
            return metainfo;
        }

        /// <summary>
        /// Создает метаинформацию на основе типа
        /// </summary>
        /// <typeparam name="TDbEntity"></typeparam>
        public static QueryableMetaInfo<TDbEntity> CreateDbRequestMetaInfo<TDbEntity>(
            this IQueryable<TDbEntity> query,
            Dictionary<string, Expression<Func<TDbEntity, object>>> fieldMap,
            bool disablePropsMiSearch = false)
            where TDbEntity : class
        {
            QueryableMetaInfo<TDbEntity> metainfo = query.CreateQueryableMetaInfo(
                Activator.CreateInstance(typeof(TDbEntity)),
                typeof(TDbEntity).Name);

            var properties = typeof(TDbEntity)
                .GetProperties();

            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            //Регистрация кастомной метаинформации вне зависимости от полей входящей сущности,
            //а также регистрация ключей с маленькой буквы
            if (fieldMap != null && fieldMap.Count != 0)
                foreach (var kvp in fieldMap)
                {
                    var name = kvp.Key;
                    var field = kvp.Value;
                    metainfo.AddField(name, field, name);
                    metainfo.AddField(name.FirstLetterToLower(), field, name.FirstLetterToLower());
                }

            if (!disablePropsMiSearch)
                foreach (var kvp in propertiesDtoEntity)
                {
                    var propDtoTypeList = kvp.Value;
                    foreach (var propDto in propDtoTypeList)
                    {
                        var name = kvp.Key;

                        if (CanIgnore(propDto))
                            continue;

                        // Игнорирование базовах классов при совпадении имен
                        if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TDbEntity))
                            continue;

                        if (fieldMap != null &&
                            fieldMap.TryGetValue(name, out var field))
                            continue;

                        var newExpression = Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntity>(name);
                        if (newExpression != null)
                        {
                            metainfo.AddField(name, newExpression, name);
                            metainfo.AddField(name.FirstLetterToLower(), newExpression, name.FirstLetterToLower());
                        }
                    }
                }

            metainfo.BuildMetaInfo();
            return metainfo;
        }

        /// <summary>
        /// Создает метаинформацию на основе типа
        /// </summary>
        /// <typeparam name="TDbEntityRequest"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        public static QueryableMetaInfo<TDbEntityRequest> CreateDbRequestMetaInfo<TDbEntity, TDbEntityRequest>(
            this IQueryable<TDbEntityRequest> query,
            Dictionary<string, Expression<Func<TDbEntityRequest, object>>> fieldMap,
            bool disablePropsMiSearch = false)
            where TDbEntityRequest : BaseDbEntityRequest
            where TDbEntity : class
        {
            QueryableMetaInfo<TDbEntityRequest> metainfo = query.CreateQueryableMetaInfo(
                Activator.CreateInstance(typeof(TDbEntityRequest)),
                typeof(TDbEntity).Name);

            var properties = typeof(TDbEntityRequest)
                .GetProperties();

            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            //Регистрация кастомной метаинформации вне зависимости от полей входящей сущности,
            //а также регистрация ключей с маленькой буквы
            if (fieldMap != null && fieldMap.Count != 0)
                foreach (var kvp in fieldMap)
                {
                    var name = kvp.Key;
                    var field = kvp.Value;
                    metainfo.AddField(name, field, name);
                    metainfo.AddField(name.FirstLetterToLower(), field, name.FirstLetterToLower());
                }

            if (!disablePropsMiSearch)
                foreach (var kvp in propertiesDtoEntity)
                {
                    var propDtoTypeList = kvp.Value;
                    foreach (var propDto in propDtoTypeList)
                    {
                        var name = kvp.Key;

                        if (CanIgnore(propDto))
                            continue;

                        // Игнорирование базовах классов при совпадении имен
                        if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TDbEntityRequest))
                            continue;

                        if (fieldMap != null &&
                            fieldMap.TryGetValue(name, out var field))
                            continue;

                        var newExpression = Dal.Base.Extensions.ExpressionExtensions.CreateExpressionDbeUniversal<TDbEntityRequest>(name);
                        if (newExpression != null)
                        {
                            metainfo.AddField(name, newExpression, name);
                            metainfo.AddField(name.FirstLetterToLower(), newExpression, name.FirstLetterToLower());
                        }
                    }
                }

            metainfo.BuildMetaInfo();
            return metainfo;
        }

        /// <summary>
        /// Создает дефолтную фильтрацию на основе анонимного типа
        /// </summary>
        /// <typeparam name="TDtoEntity"></typeparam>
        /// <typeparam name="TDbEntity"></typeparam>
        public static IQueryable<object> CreateAnonymQueryWithMetaInfo<TDbEntity, TDtoEntity>(
            this IQueryable<TDbEntity> query,
            IFieldMapContainer container,
            out QueryableMetaInfo<object> metainfo,
            Expression<Func<TDbEntity, object>> expression)
            where TDtoEntity : IEntityDto
            where TDbEntity : class
        {
            var anonymObjType = expression.Body.Type;

            var anonymTypesQuery = query.Select(expression);
            metainfo = anonymTypesQuery.CreateQueryableMetaInfo(Activator.CreateInstance(typeof(TDtoEntity)), typeof(TDbEntity).Name);

            var properties = anonymObjType
                .GetProperties();
            var propertiesDtoEntity = properties.GroupBy(g => g.Name).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var kvp in propertiesDtoEntity)
            {
                var propDtoTypeList = kvp.Value;
                foreach (var propDto in propDtoTypeList)
                {
                    var name = kvp.Key;

                    if (CanIgnore(propDto))
                        continue;

                    // Игнорирование базовах классов при совпадении имен
                    if (propDtoTypeList.Count > 1 && propDto.DeclaringType != typeof(TDtoEntity))
                        continue;

                    try
                    {
                        var newExpression = CreateExpression(anonymObjType, name);
                        //metainfo.AddField(name, x => GetTToProp(propDto.PropertyType, x, propDto), name);
                        metainfo.AddField(name, newExpression, name);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            metainfo.BuildMetaInfo();
            return anonymTypesQuery;
        }

        private static Expression<Func<object, object>> CreateExpression(Type entityType, string propertyName)
        {
            var param = Expression.Parameter(typeof(object), "e");
            Expression body = Expression.PropertyOrField(Expression.TypeAs(param, entityType), propertyName);
            var getterExpression = Expression.Lambda<Func<object, object>>(body, param);
            return getterExpression;
        }
    }
}
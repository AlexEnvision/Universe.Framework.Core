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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Base;
using Universe.CQRS.Models.Condition;
using Universe.CQRS.Models.Filter;

namespace Universe.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Применение фильтров (условий)
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="conditions">Условия</param>
        /// <param name="mi">Метаинформация</param>
        /// <param name="allowNoTrackingMode">Разрешение трекинга изменений</param>
        /// <returns>Запрос к БД с примененными фильтрами</returns>
        public static IQueryable<T> ApplyFiltersAtQuery<T>(
            this IQueryable<T> query,
            IList<ConditionConfiguration> conditions,
            QueryableMetaInfo<T> mi,
            bool allowNoTrackingMode)
            where T : class
        {
            var filters = ResolveSearchFilters(conditions);

            IQueryable<T> possiballyFilteredQuery = query;
            foreach (var filter in filters)
            {
                if (allowNoTrackingMode)
                    possiballyFilteredQuery = possiballyFilteredQuery.AsNoTracking().WhereByFilter(mi, filter);
                else
                    possiballyFilteredQuery = possiballyFilteredQuery.WhereByFilter(mi, filter);
            }

            if (possiballyFilteredQuery != null)
                query = possiballyFilteredQuery;
            return query;
        }

        /// <summary>
        /// Применение фильтров (условий)
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="conditions">Условия</param>
        /// <param name="mi">Метаинформация</param>
        /// <param name="allowNoTrackingMode">Разрешение трекинга изменений</param>
        /// <returns>Запрос к БД с примененными фильтрами</returns>
        public static IQueryable<T> ApplyFiltersAtQuery<T, TEntityDto>(
            this IQueryable<T> query,
            IList<ConditionConfiguration> conditions,
            QueryableMetaInfo<T> mi,            
            bool allowNoTrackingMode) 
            where T : class
        {
            var filters = ResolveSearchFilters(conditions);
            // todo: сейчас нельзя убирать: на вьюхе это должно быть, иначе отобразится 0 элементов, если первая выбранная страница содержит только удаленные элементы
            // убрать потом
            if (typeof(T).GetProperty("Deleted") != null)
                filters = filters.AddDefaultFilter<TEntityDto>();

            IQueryable<T> possiballyFilteredQuery = query;
            foreach (var filter in filters)
            {
                if (allowNoTrackingMode)
                    possiballyFilteredQuery = possiballyFilteredQuery.AsNoTracking().WhereByFilter(mi, filter);
                else
                    possiballyFilteredQuery = possiballyFilteredQuery.WhereByFilter(mi, filter);
            }

            if (possiballyFilteredQuery != null)
                query = possiballyFilteredQuery;
            return query;
        }

        public static List<SearchFilterBase> AddDefaultFilter<TEntityDto>(this List<SearchFilterBase> filters)
        {
            var entityType = typeof(TEntityDto);
            if (!entityType.IsSubclassOf(typeof(EntityDto)))
                return filters;

            filters = filters ?? new List<SearchFilterBase>();

            var filter = new SearchFilterAnd {
                Rules = new List<SearchFilterRule> {
                    new SearchFilterRule {
                        FieldName = "Deleted",
                        FilterTypeName = FieldFilterTypes.Eq.ToString(),
                        ValueSelected = "null"
                    }
                }
            };
            filters.Insert(0, filter);
            return filters;
        }

        public static SearchFilterBase ResolveSearchFilters(ConditionConfiguration c)
        {
            switch (c.Operator)
            {
                case "and":
                    return Mapper.Map<AndConfiguration, SearchFilterAnd>(c as AndConfiguration);
                case "or":
                    return Mapper.Map<OrConfiguration, SearchFilterOr>(c as OrConfiguration);
                case "eq":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<EqConfiguration, SearchFilterRule>(c as EqConfiguration)
                        }
                    };
                case "neq":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<NeqConfiguration, SearchFilterRule>(c as NeqConfiguration)
                        }
                    };
                case "in":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<InConfiguration, SearchFilterRule>(c as InConfiguration)
                        }
                    };
                case "contains":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<ContainsConfiguration, SearchFilterRule>(c as ContainsConfiguration)
                        }
                    };
                case "between":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<BetweenConfiguration, SearchFilterRule>(c as BetweenConfiguration)
                        }
                    };
                case "isNull":
                    return new SearchFilterAnd {
                        Rules = new List<SearchFilterRule> {
                            Mapper.Map<IsNullConfiguration, SearchFilterRule>(c as IsNullConfiguration)
                        }
                    };
                default:
                    return Mapper.Map<SearchFilterBase>(c);
            }
        }

        public static List<SearchFilterBase> ResolveSearchFilters(IList<ConditionConfiguration> conditions)
        {
            var filters = new List<SearchFilterBase>();
            if (conditions == null)
                return filters;

            filters.AddRange(conditions.Select(ResolveSearchFilters));
            return filters;
        }
    }
}
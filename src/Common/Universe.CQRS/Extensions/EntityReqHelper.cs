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
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Dal.FilterMetaInfo;
using Universe.CQRS.Models.Condition;
using Universe.CQRS.Models.Filter;
using Universe.CQRS.Models.Filter.Custom;
using Universe.CQRS.Models.Page;
using Universe.CQRS.Models.Req;
using Universe.CQRS.Models.Sort;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Extensions
{
    /// <summary>
    ///     Построитель запросов.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class EntityReqHelper
    {
        /// <summary>
        ///     Поиск элементов по вхождению указанный дискретный диапазон значений
        /// </summary>
        /// <param name="metainfo"></param>
        /// <param name="searchfieldName"></param>
        /// <param name="searchvalues"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetInReq(
            IFilterMetaInfo metainfo,
            string searchfieldName,
            List<string> searchvalues,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            if (searchvalues.Count == 0)
                return null;

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqIn(searchfieldName, searchvalues, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Поиск значений входящих в указанное множество значений
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="searchfieldName"></param>
        /// <param name="searchvalues"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetInReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string searchfieldName,
            List<string> searchvalues,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (searchvalues.Count == 0)
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntityDb> {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqIn(searchfieldName, searchvalues, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        // TODO -> при успешном использовании удалить, а во всех что начинаются с Get заменить  TEntityDb : Entity на TEntity : class
        /// <summary>
        ///     Поиск значений входящих в указанное множество значений
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="searchfieldName"></param>
        /// <param name="searchvalues"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq BuildInReq<TEntity>(
            Dictionary<string, Expression<Func<TEntity, object>>> fieldMap,
            string searchfieldName,
            List<string> searchvalues,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntity : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (searchvalues.Count == 0)
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntity>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqIn(searchfieldName, searchvalues, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Поиск значений не входящих в указанное множество значений
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="searchfieldName"></param>
        /// <param name="antaSearchValues"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetNeInReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string searchfieldName,
            List<string> antaSearchValues,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (antaSearchValues.Count == 0)
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntityDb> {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqNeIn(searchfieldName, antaSearchValues, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с несколькими шаблонами поиска и с частичным совпадением
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="searchfieldName"></param>
        /// <param name="patternValues"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetInTextsReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string searchfieldName,
            List<string> patternValues,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (patternValues.Count == 0)
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntityDb> {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqInContains(searchfieldName, patternValues, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с одним шаблоном поиска
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="patternValue">Значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="searchFieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetContainsReq(
            IFilterMetaInfo metainfo,
            string searchFieldName,
            string patternValue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqContains(searchFieldName, patternValue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с несколькими шаблонами поиска
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="searchFieldName"></param>
        /// <param name="patternValue"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetContainsReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string searchFieldName,
            string patternValue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (patternValue.IsNullOrWhiteSpace())
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqContains(searchFieldName, patternValue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с одним шаблоном поиска
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="eqvalue">Значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetEqReq(
            IFilterMetaInfo metainfo,
            string eqfieldName,
            string eqvalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqSingleEq(eqfieldName, eqvalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с одним шаблоном поиска
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="eqvalue">Значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetEqReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string eqfieldName,
            string eqvalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (eqvalue == null)
                throw new ArgumentNullException(nameof(eqvalue));

            var fieldMapContainer = new FieldMapContainer<TEntityDb> {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqSingleEq(eqfieldName, eqvalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с одним шаблоном поиска
        ///     Выдает те значения, что не равны шаблону
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="eqvalue">Значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetNeqReq(
            IFilterMetaInfo metainfo,
            string eqfieldName,
            string eqvalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqSingleEq(eqfieldName, eqvalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов по условию с одним шаблоном поиска.
        ///     Выдает те значения, что не равны шаблону
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="eqvalue">Значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetNeqReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string eqfieldName,
            string eqvalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (eqvalue == null)
                throw new ArgumentNullException(nameof(eqvalue));

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqSingleNeq(eqfieldName, eqvalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }
      
        /// <summary>
        ///     Строит запрос поиска элементов удовлетворяющих условию между указанными значениями
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="minbordervalue">Первое граничное значение для сравнения</param>
        /// <param name="maxbordervalue">Второе граничное значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetBetweenReq(
            IFilterMetaInfo metainfo,
            string eqfieldName,
            DateTimeOffset minbordervalue,
            DateTimeOffset maxbordervalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            if (minbordervalue == null)
                throw new ArgumentNullException(nameof(minbordervalue));

            if (maxbordervalue == null)
                throw new ArgumentNullException(nameof(maxbordervalue));

            return CreateGetEntitiesReqBetween(eqfieldName, minbordervalue, maxbordervalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов удовлетворяющих условию между указанными значениями
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="minbordervalue">Первое граничное значение для сравнения</param>
        /// <param name="maxbordervalue">Второе граничное значение для сравнения</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetBetweenReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string eqfieldName,
            DateTimeOffset minbordervalue,
            DateTimeOffset maxbordervalue,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (minbordervalue == null)
                throw new ArgumentNullException(nameof(minbordervalue));

            if (maxbordervalue == null)
                throw new ArgumentNullException(nameof(maxbordervalue));

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqBetween(eqfieldName, minbordervalue, maxbordervalue, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов с условием поиска равном null
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetIsNullReq(
            IFilterMetaInfo metainfo,
            string eqfieldName,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqIsNull(eqfieldName, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов с условием поиска равном null
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetIsNullReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string eqfieldName,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqIsNull(eqfieldName, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов с условием поиска не равном null
        /// </summary>
        /// <param name="metainfo">Метаинформация</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetIsNotNullReq(
            IFilterMetaInfo metainfo,
            string eqfieldName,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
        {
            if (metainfo == null)
                throw new ArgumentException(nameof(metainfo));

            var fieldMapContainer = metainfo.FieldMapContainer;

            return CreateGetEntitiesReqIsNotNull(eqfieldName, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов с условием поиска не равном null
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <param name="eqfieldName"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetIsNotNullReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            string eqfieldName,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqIsNotNull(eqfieldName, fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        /// <summary>
        ///     Строит запрос поиска элементов без условия поиска.
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap">Карта условий</param>
        /// <param name="countOnPage">Количество элементов на странице</param>
        /// <param name="pageIndex">Индекс страницы</param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetAnyReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            var fieldMapContainer = new FieldMapContainer<TEntityDb>
            {
                FieldMap = fieldMap
            };

            return CreateGetEntitiesReqAnything(fieldMapContainer, countOnPage, pageIndex, allItemsAsOnePage);
        }

        private static GetEntitiesReq CreateGetEntitiesReqIsNotNull(
            string eqfieldName,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new NeqConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = eqfieldName
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = "'null'"
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqIsNull(
            string eqfieldName,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new EqConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = eqfieldName
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = "'null'"
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqBetween(
            string eqfieldName,
            DateTimeOffset minbordervalue,
            DateTimeOffset maxbordervalue,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new BetweenConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = eqfieldName
                    }
                },
                RightOperand = new BetweenArgumentConfiguration
                {
                    Value = new DataTimePeriod {
                        Start = minbordervalue,
                        End = maxbordervalue
                    }
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqSingleNeq(
            string eqfieldName, 
            string eqvalue,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new NeqConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = eqfieldName
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = eqvalue
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqSingleEq(
            string eqfieldName,
            string singleValue,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new EqConfiguration {
                LeftOperand = new FieldArgumentConfiguration {
                    Field = new FieldConfiguration {
                        SpFieldName = eqfieldName
                    }
                },
                RightOperand = new ValueArgumentConfiguration {
                    Expression = singleValue
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqNeIn(
            string searchfieldName,
            List<string> values,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var operandsCertTypeFilters = new List<ConditionConfiguration>();

            foreach (var filterValue in values)
            {
                var filter = new NeqConfiguration {
                    LeftOperand = new FieldArgumentConfiguration {
                        Field = new FieldConfiguration {
                            SpFieldName = searchfieldName
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration {
                        Expression = filterValue
                    }
                };
                operandsCertTypeFilters.Add(filter);
            }

            var orFilter = new AndConfiguration {
                Operands = operandsCertTypeFilters
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    orFilter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqContains(
            string containFieldName,
            string containsValue,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var filter = new ContainsConfiguration()
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = containFieldName
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = containsValue
                }
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration>
                {
                    filter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqInContains(
            string searchfieldName,
            List<string> values,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var operandsCertTypeFilters = new List<ConditionConfiguration>();
            foreach (var filterValue in values)
            {
                var filter = new ContainsConfiguration {
                    LeftOperand = new FieldArgumentConfiguration {
                        Field = new FieldConfiguration {
                            SpFieldName = searchfieldName
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration {
                        Expression = filterValue
                    }
                };
                operandsCertTypeFilters.Add(filter);
            }

            var orFilter = new OrConfiguration {
                Operands = operandsCertTypeFilters
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    orFilter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqIn(
            string searchfieldName,
            List<string> values,
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var operandsCertTypeFilters = new List<ConditionConfiguration>();
            foreach (var filterValue in values)
            {
                var filter = new EqConfiguration {
                    LeftOperand = new FieldArgumentConfiguration {
                        Field = new FieldConfiguration {
                            SpFieldName = searchfieldName
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration {
                        Expression = filterValue
                    }
                };
                operandsCertTypeFilters.Add(filter);
            }

            var orFilter = new OrConfiguration {
                Operands = operandsCertTypeFilters
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    orFilter
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        private static GetEntitiesReq CreateGetEntitiesReqAnything(
            IFieldMapContainer fieldMapContainer,
            int countOnPage,
            int pageIndex,
            bool allItemsAsOnePage)
        {
            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq
            {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> (),
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        /// <summary>
        ///     Строит запрос с цепочкой вызовов - TODO: Сделать поддержку различных типов фильтров кроме Eq
        /// </summary>
        /// <typeparam name="TEntityDb"></typeparam>
        /// <param name="fieldMap"></param>
        /// <param name="condition"></param>
        /// <param name="countOnPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="allItemsAsOnePage"></param>
        /// <returns></returns>
        public static GetEntitiesReq GetConditionReq<TEntityDb>(
            Dictionary<string, Expression<Func<TEntityDb, object>>> fieldMap,
            ConditionConfiguration condition,
            int countOnPage = 100,
            int pageIndex = 1,
            bool allItemsAsOnePage = false)
            where TEntityDb : class 
        {
            if (fieldMap == null)
                throw new ArgumentException(nameof(fieldMap));

            if (condition == null)
                return null;

            var fieldMapContainer = new FieldMapContainer<TEntityDb> {
                FieldMap = fieldMap
            };

            var paging = !allItemsAsOnePage
                ? new Paging
                {
                    CountOnPage = countOnPage,
                    PageIndex = pageIndex
                }
                : null;

            return new GetEntitiesReq {
                FieldMapContainer = fieldMapContainer,
                Filters = new List<ConditionConfiguration> {
                    condition
                },
                Paging = paging,
                IsAllWithPaging = allItemsAsOnePage
            };
        }

        /// <summary>
        ///     Строит цепочку фильтров.
        ///     Пока поддерживает только Eq
        /// </summary>
        /// <param name="chainType">Тип цепочки</param>
        /// <param name="arguments">Пары поле-значение</param>
        /// <returns></returns>
        public static ConditionConfiguration CreateChain(ChainFilterTypes chainType, Dictionary<string, string> arguments)
        {
            var filters = new List<ConditionConfiguration>();
            foreach (var arg in arguments)
            {
                var filter = new EqConfiguration {
                    LeftOperand = new FieldArgumentConfiguration {
                        Field = new FieldConfiguration {
                            SpFieldName = arg.Key
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration {
                        Expression = arg.Value
                    }
                };
                filters.Add(filter);
            }

            if (chainType == ChainFilterTypes.Or)
            {
                var orConfig = new OrConfiguration {
                    Operands = filters
                };
                return orConfig;
            }

            if (chainType == ChainFilterTypes.And)
            {
                var andConfig = new AndConfiguration {
                    Operands = filters
                };
                return andConfig;
            }

            throw new ArgumentException("Выбранная цепочка не поддерживается!");
        }

        /// <summary>
        ///     Добавляет конфигурацию сортировки по одному или нескольким полям.
        /// </summary>
        /// <param name="req">Модель запроса сущностей.</param>
        /// <param name="sortConfigurations">Конфигурации сортировки.</param>
        /// <returns></returns>
        public static GetEntitiesReq Sorting(this GetEntitiesReq req, params SortConfiguration[] sortConfigurations)
        {
            req.Sorting = sortConfigurations.ToList();
            return req;
        }

        /// <summary>
        ///     Строит правило сортировки по конкретному полю
        /// </summary>
        /// <param name="fieldName">Поле, по которому будет производиться сортировка.</param>
        /// <param name="direction">Направление сортировки.</param>
        /// <returns></returns>
        public static SortConfiguration GetSortRule(string fieldName, SortDirection direction)
        {
            return new SortConfiguration {
                Field = fieldName,
                Direction = direction
            };
        }
    }
}
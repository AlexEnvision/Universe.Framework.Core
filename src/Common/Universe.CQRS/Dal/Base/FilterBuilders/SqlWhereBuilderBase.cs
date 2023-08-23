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
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Filter;

namespace Universe.CQRS.Dal.Base.FilterBuilders
{
    /// <summary>
    ///     The sql where builder base.
    /// <author>Alex Envision</author>
    /// </summary>
    public class SqlWhereBuilderBase
    {
        /// <summary>
        /// The _search filter.
        /// </summary>
        protected SearchFilterBase Filters;

        /// <summary>
        /// The _meta info.
        /// </summary>
        protected BaseMetaInfo MetaInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWhereBuilderBase"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta info.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public SqlWhereBuilderBase(BaseMetaInfo metaInfo, SearchFilterBase filters)
        {
            MetaInfo = metaInfo ?? throw new ArgumentNullException(nameof(metaInfo));
            Filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        /// <summary>
        /// Gets or sets a value indicating whether throw exception if filter empty.
        /// </summary>
        public bool ThrowExceptionIfFilterEmpty { get; set; } = true;

        protected static object ChangeType(Type fieldType, object value)
        {
            if (fieldType == typeof(bool?))
                return (bool?)Convert.ChangeType(value, typeof(bool));

            if (fieldType == typeof(Guid))
                return new Guid(value.ToString());

            if (fieldType.Name == typeof(Nullable<>).Name)
                return ConvertNullable(value, fieldType);

            return Convert.ChangeType(value, fieldType);
        }

        private static object ConvertNullable(object value, Type fieldType)
        {
            if (value == null)
                return null;

            var genericArgument = fieldType.GenericTypeArguments.FirstOrDefault();
            if (genericArgument != null)
            {
                var nullable = Convert.ChangeType(value, genericArgument);
                return nullable;
            }

            return null;
        }

        /// <summary>
        /// The get comparison operator by field filter type.
        /// </summary>
        /// <param name="fieldFilterType">
        /// The field filter type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        protected string GetComparisonOperatorByFieldFilterType(FieldFilterTypes fieldFilterType)
        {
            switch (fieldFilterType)
            {
                case FieldFilterTypes.Eq:
                    return "=";
                case FieldFilterTypes.Neq:
                    return "!=";
                case FieldFilterTypes.Leq:
                    return "<=";
                case FieldFilterTypes.Le:
                    return "<";
                case FieldFilterTypes.Geq:
                    return ">=";
                case FieldFilterTypes.Ge:
                    return ">";
                case FieldFilterTypes.Like:
                    return "like";
                case FieldFilterTypes.NotLike:
                    return "not like";
                case FieldFilterTypes.IsNull:
                    return "is null";
                case FieldFilterTypes.IsNotNull:
                    return "is not null";
                case FieldFilterTypes.Between:
                    return "between";
                case FieldFilterTypes.In:
                    return "in";
                case FieldFilterTypes.NotIn:
                    return "not in";
                default:
                    throw new NotSupportedException($"Не поддерживается {fieldFilterType}.");
            }
        }

        /// <summary>
        /// The get value from filter.
        /// </summary>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected object GetValueFromFilter(Type fieldType, SearchFilterRule filter)
        {
            if (fieldType == typeof(string))
                return filter.ValueSelected;

            return ChangeType(fieldType, filter.ValueSelected);
        }

        /// <summary>
        /// The get values for between.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="fieldMetaInfo">
        /// The field meta info.
        /// </param>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        protected List<object> GetValuesForBetween(SearchFilterRule filter, BaseFieldMetaInfo fieldMetaInfo, Type fieldType)
        {
            var valueArr = GetValuesFromFilter(fieldType, filter);
            if (valueArr.Count != 2)
            {
                var message =
                    $"Для поля {fieldMetaInfo.FilterTitle} и оператора {filter.FilterType} значение фильтра должно содержать два значения, а содержит {valueArr.Count}.";
                throw new Exception(message);
            }

            var value1 = valueArr[0];
            if (fieldType != null && value1.GetType() != fieldType)
                try
                {
                    value1 = ChangeType(fieldType, value1);
                }
                catch (Exception ex)
                {
                    var message =
                        $"Для поля {fieldMetaInfo.FilterTitle} и оператора {filter.FilterType} тип 1-го значение фильтра ({(value1 != null ? value1.GetType().FullName : "null")}) несовподает с типом поля ({fieldType.FullName}). И не поддаеться приведению: {ex.Message}";
                    throw new Exception(message, ex);
                }

            var value2 = valueArr[1];
            if (fieldType != null
                &&
                value2.GetType() != fieldType)
                try
                {
                    value2 = ChangeType(fieldType, value2);
                }
                catch (Exception ex)
                {
                    var message =
                        $"Для поля {fieldMetaInfo.FilterTitle} и оператора {filter.FilterType} тип 2-го значение фильтра ({(value2 != null ? value2.GetType().FullName : "null")}) несовподает с типом поля ({fieldType.FullName}). И не поддаеться приведению: {ex.Message}";
                    throw new Exception(message, ex);
                }

            valueArr[0] = value1;
            valueArr[1] = value2;
            return valueArr;
        }

        /// <summary>
        /// The get values from filter.
        /// </summary>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        protected List<object> GetValuesFromFilter(Type fieldType, SearchFilterRule filter)
        {
            var result = new List<object>();
            result.Add(filter.ValueObjectSource);
            result.Add(filter.ValueObject);

            return result;
        }
    }
}
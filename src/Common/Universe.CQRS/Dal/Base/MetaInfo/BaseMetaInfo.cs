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
using Universe.CQRS.Models.Filter;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Dal.Base.MetaInfo
{
    /// <summary>
    /// The base meta info.
    /// </summary>
    public class BaseMetaInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMetaInfo"/> class.
        /// </summary>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        protected BaseMetaInfo(string entityName)
        {
            EntityName = entityName;
            FieldsMetaInfo = new List<BaseFieldMetaInfo>();
        }

        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        public string EntityName { get; protected set; }

        /// <summary>
        /// Gets or sets the fields meta info.
        /// </summary>
        public List<BaseFieldMetaInfo> FieldsMetaInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the filter model.
        /// </summary>
        public SearchFilterModel FilterModel { get; protected set; }

        /// <summary>
        /// Gets or sets the grid view columns.
        /// </summary>
        public List<EntityColumn> GridViewColumns { get; protected set; }

        /// <summary>
        /// Gets or sets the grid view columns extent.
        /// </summary>
        public List<BaseFieldMetaInfo> GridViewColumnsExtent { get; protected set; }

        /// <summary>
        /// The build filter columns.
        /// </summary>
        /// <returns>
        /// The <see cref="List{SelectModel}"/>.
        /// </returns>
        public List<FilterField> BuildFilterColumns()
        {
            var result = new List<FilterField>();
            foreach (var fieldMetaInfo in FieldsMetaInfo)
            {
                if (!fieldMetaInfo.Filterable)
                    continue;

                var filterField = new FilterField {
                    FieldName = fieldMetaInfo.Name,
                    FieldTitle = fieldMetaInfo.FilterTitle,
                    FieldType = fieldMetaInfo.FieldType,
                    FilterTypes = new List<FieldFilterType>(),
                    FilterValues = fieldMetaInfo.FilterValues
                };

                if (fieldMetaInfo.FieldFilterTypes == null || fieldMetaInfo.FieldFilterTypes.Contains(FieldFilterTypes.Eq))
                    filterField.FilterTypes.AddRange(
                        new List<FieldFilterType> {
                            new FieldFilterType {
                                Title = "равно",
                                TypeName = FieldFilterTypes.Eq.ToString()
                            }
                        });

                if (fieldMetaInfo.FieldFilterTypes == null || fieldMetaInfo.FieldFilterTypes.Contains(FieldFilterTypes.Neq))
                    filterField.FilterTypes.AddRange(
                        new List<FieldFilterType> {
                            new FieldFilterType {
                                Title = "не равно",
                                TypeName = FieldFilterTypes.Neq.ToString()
                            }
                        });

                if ((fieldMetaInfo.FieldType == FieldTypes.DateTimeOffset.ToString()
                        || fieldMetaInfo.FieldType == FieldTypes.Int.ToString()
                        || fieldMetaInfo.FieldType == FieldTypes.Number.ToString())
                    && filterField.FilterValues == null)
                    filterField.FilterTypes.AddRange(
                        new List<FieldFilterType> {
                            new FieldFilterType {
                                Title = "меньше",
                                TypeName = FieldFilterTypes.Le.ToString()
                            },
                            new FieldFilterType {
                                Title = "меньше либо равно",
                                TypeName = FieldFilterTypes.Leq.ToString()
                            },
                            new FieldFilterType {
                                Title = "больше",
                                TypeName = FieldFilterTypes.Ge.ToString()
                            },
                            new FieldFilterType {
                                Title = "больше либо равно",
                                TypeName = FieldFilterTypes.Geq.ToString()
                            }
                        });

                filterField.FilterTypes.AddRange(
                    new List<FieldFilterType> {
                        new FieldFilterType {
                            Title = "не указанно",
                            TypeName = FieldFilterTypes.IsNull.ToString(),
                            NumberOfValues = 0
                        },
                        new FieldFilterType {
                            Title = "указанно",
                            TypeName = FieldFilterTypes.IsNotNull.ToString(),
                            NumberOfValues = 0
                        }
                    });

                if ((fieldMetaInfo.FieldType == FieldTypes.DateTimeOffset.ToString()
                        || fieldMetaInfo.FieldType == FieldTypes.Int.ToString()
                        || fieldMetaInfo.FieldType == FieldTypes.Number.ToString())
                    && filterField.FilterValues == null)
                    filterField.FilterTypes.AddRange(
                        new List<FieldFilterType> {
                            new FieldFilterType {
                                Title = "между",
                                TypeName = FieldFilterTypes.Between.ToString(),
                                NumberOfValues = 2
                            }
                        });

                if (fieldMetaInfo.FieldType == FieldTypes.Int.ToString()
                    || fieldMetaInfo.FieldType == FieldTypes.Number.ToString())
                    filterField.FilterTypes.AddRange(
                        new List<FieldFilterType> {
                            new FieldFilterType {
                                Title = "равно одному из",
                                TypeName = FieldFilterTypes.In.ToString(),
                                NumberOfValues = 3
                            },
                            new FieldFilterType {
                                Title = "не равно одному из",
                                TypeName = FieldFilterTypes.NotIn.ToString(),
                                NumberOfValues = 3
                            }
                        });

                result.Add(filterField);
            }

            return result;
        }

        /// <summary>
        /// The build grid view columns.
        /// </summary>
        /// <returns>
        /// The <see cref="List{EntityColumn}"/>.
        /// </returns>
        public List<EntityColumn> BuildGridViewColumns()
        {
            var result = new List<EntityColumn>();
            foreach (var fieldMetaInfo in FieldsMetaInfo)
            {
                if (!fieldMetaInfo.CanBeVisible)
                    continue;

                var entityColumn = new EntityColumn {
                    FieldName = fieldMetaInfo.Name,
                    Title = fieldMetaInfo.Title,
                    Directive = string.Empty,
                    SortType = string.Empty,
                    Visible = fieldMetaInfo.VisibleDefault,
                    Template = fieldMetaInfo.Template
                };
                result.Add(entityColumn);

                if (!entityColumn.Template.IsNullOrEmpty())
                    continue;

                if (fieldMetaInfo.DataType == typeof(DateTimeOffset) || fieldMetaInfo.DataType == typeof(DateTimeOffset?))
                    entityColumn.Template = $"{{{{item.{fieldMetaInfo.Name}.UtcDateTime | date : 'dd.MM.yyyy HH:mm'}}}}";
                else if (fieldMetaInfo.DataType == typeof(bool) || fieldMetaInfo.DataType == typeof(bool?))
                    entityColumn.Template = $"{{{{item.{fieldMetaInfo.Name}?'Да':'Нет'}}}}";
                else
                    entityColumn.Template = $"{{{{item.{fieldMetaInfo.Name}}}}}";
            }

            return result;
        }

        /// <summary>
        /// The get filter type title.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFilterTypeTitle(SearchFilterRule filter)
        {
            var f = FilterModel.Fields.FirstOrDefault(_ => _.FieldName == filter.FieldName);

            var ft = f?.FilterTypes.FirstOrDefault(_ => _.TypeName == filter.FilterTypeName);
            return ft == null ? string.Empty : ft.Title;
        }

        /// <summary>
        /// The build filter model.
        /// </summary>
        protected void BuildFilterModel()
        {
            FilterModel = new SearchFilterModel {
                EntityName = EntityName,
                Fields = BuildFilterColumns(),
                Filter = new SearchFilterAnd {
                    IsCollapsed = false,
                    Items = new List<SearchFilterBase>(),
                    Rules = new List<SearchFilterRule>()
                }
            };
        }

        /// <summary>
        /// The build grid view columns extent.
        /// </summary>
        protected void BuildGridViewColumnsExtent()
        {
            GridViewColumnsExtent = FieldsMetaInfo.Where(_ => _.Name.StartsWith("Extent.") && _.CanBeVisible).ToList();
        }
    }
}
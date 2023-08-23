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
using AutoMapper;
using Universe.CQRS.Dal.Mappings.Framework;
using Universe.CQRS.Models.Condition;
using Universe.CQRS.Models.Filter;

namespace Universe.CQRS.Dal.Mappings.FilterMappings.Base
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    internal abstract class SearchFilterBaseMapping<TFrom, TTo> : AutoMap<TFrom, TTo>
        where TFrom : ConditionConfiguration
        where TTo : SearchFilterBase
    {
        protected IList<SearchFilterRule> SearchFilterRulesResolver(ICollection<ConditionConfiguration> operands)
        {
            IList<SearchFilterRule> rules = new List<SearchFilterRule>();

            foreach (var c in operands)
            {
                switch (c.Operator)
                {
                    case "eq":
                        rules.Add(Mapper.Map<EqConfiguration, SearchFilterRule>(c as EqConfiguration));
                        break;
                    case "neq":
                        rules.Add(Mapper.Map<NeqConfiguration, SearchFilterRule>(c as NeqConfiguration));
                        break;
                    case "and":
                        /*var andConfig = c as AndConfiguration;
                        if (andConfig?.Operands != null)
                        {
                            var andrules = SearchFilterRulesResolver(andConfig.Operands);
                            foreach (var searchFilterRule in andrules)
                            {
                                rules.Add(searchFilterRule);
                            }
                        }*/

                        break;
                    case "or":
                        break;
                    case "in":
                        rules.Add(Mapper.Map<InConfiguration, SearchFilterRule>(c as InConfiguration));
                        break;
                    case "contains":
                        rules.Add(Mapper.Map<ContainsConfiguration, SearchFilterRule>(c as ContainsConfiguration));
                        break;
                    case "between":
                        rules.Add(Mapper.Map<BetweenConfiguration, SearchFilterRule>(c as BetweenConfiguration));
                        break;
                    default:
                        throw new ArgumentException("Неподдерживаемая конфигурация фильтров.");
                }
            }

            return rules;
        }

        protected IList<SearchFilterBase> SearchFilterItemsResolver(ICollection<ConditionConfiguration> operands)
        {
            IList<SearchFilterBase> items = new List<SearchFilterBase>();

            foreach (var c in operands)
            {
                switch (c.Operator)
                {
                    case "eq":
                        break;
                    case "neq":
                        break;
                    case "and":
                        items.Add(Mapper.Map<AndConfiguration, SearchFilterAnd>(c as AndConfiguration));
                        break;
                    case "or":
                        items.Add(Mapper.Map<OrConfiguration, SearchFilterOr>(c as OrConfiguration));
                        break;
                    case "in":
                        break;
                    case "contains":
                        break;
                    case "between":
                        break;
                    default:
                        throw new ArgumentException("Неподдерживаемая конфигурация фильтров.");
                }
            }

            return items;
        }
    }
}
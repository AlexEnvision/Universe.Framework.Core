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
using System.Linq;
using AutoMapper;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Models.Base;
using Universe.CQRS.Models.Sort;

namespace Universe.CQRS.Extensions
{
    public static class SortingExtensions
    {
        /// <summary>
        /// Применение сортировки
        /// <author>Alex Envision</author>
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="sorting">Конфигурации сортировки</param>
        /// <param name="mi">Метаинформация</param>
        /// <returns>Запрос к БД с примененными сортировками</returns>
        public static IQueryable<T> ApplySortingAtQuery<T, TEntityDto>(
            this IQueryable<T> query,
            IList<SortConfiguration> sorting,
            QueryableMetaInfo<T> mi)
            where TEntityDto : IEntityDto
            where T : class
        {
            var sortDescriptors = sorting?.Select(Mapper.Map<SortConfiguration, SortDescriptor>).ToList();
            var possiballySortedQuery = query.OrderByReflectionMagic<T, TEntityDto>(sortDescriptors, mi);
            query = possiballySortedQuery;
            return query;
        }

        /// <summary>
        /// Применение сортировки
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="sorting">Конфигурации сортировки</param>
        /// <param name="mi">Метаинформация</param>
        /// <returns>Запрос к БД с примененными сортировками</returns>
        public static IQueryable<T> ApplySortingAtQuery<T>(
            this IQueryable<T> query,
            IList<SortConfiguration> sorting,
            QueryableMetaInfo<T> mi)
            where T : class
        {
            var sortDescriptors = sorting?.Select(Mapper.Map<SortConfiguration, SortDescriptor>).ToList();
            var possiballySortedQuery = query.OrderByReflectionMagic<T>(sortDescriptors, mi);
            query = possiballySortedQuery;
            return query;
        }
    }
}
